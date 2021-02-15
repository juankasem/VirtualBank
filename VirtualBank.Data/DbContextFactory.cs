using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace VirtualBank.Data
{
    public static class DbContextFactory
    {
       public static IServiceCollection AddDatabaseContext(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<VirtualBankDbContext>(options => options.UseSqlServer(configuration.GetConnectionString("VirtualBankDbConnection")));

            return services;
        }
    }
}
