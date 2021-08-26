using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using VirtualBank.Core.Entities;

namespace VirtualBank.Api
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();

            using var scope = host.Services.CreateScope();

            var services = scope.ServiceProvider;
            var loggerFactory = services.GetRequiredService<ILoggerProvider>();
            var logger = loggerFactory.CreateLogger("app");

            try
            {
                var userManager = services.GetRequiredService<UserManager<AppUser>>();
                var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

                await Core.Seeds.DefaultRoles.SeedAsync(roleManager);
                await Core.Seeds.DefaultUsers.SeedBasicUser(userManager);
                await Core.Seeds.DefaultUsers.SeedSuperAdminUser(userManager, roleManager);

                logger.LogInformation("data seeded");
                logger.LogInformation("Application started");

                host.Run();
            }
            catch (System.Exception ex)
            {
                logger.LogWarning(ex, "An error occured while seeding data");
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                    webBuilder.UseUrls("http://*:8000");
                });
    }
}
