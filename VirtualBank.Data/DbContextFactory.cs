using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace VirtualBank.Data
{
    public static class DbContextFactory
    {
       public static IServiceCollection AddDatabaseContext(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<VirtualBankDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("VirtualBankDbConnection")));

            return services;
        }

        public static async Task<IServiceProvider> ConfigureDatabseContext(this IServiceProvider serviceProvider)
        {
            var databaseContext = serviceProvider.GetService<VirtualBankDbContext>();

            await databaseContext.Database.MigrateAsync();

            return serviceProvider;
        }
    }
}
