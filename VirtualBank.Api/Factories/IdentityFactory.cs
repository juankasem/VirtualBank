using System;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using VirtualBank.Core.Entities;
using VirtualBank.Data;

namespace VirtualBank.Api.Factories
{
    public static class IdentityFactory
    {
        public static IServiceCollection AddIdentity(this IServiceCollection services)
        {

            services.AddIdentity<AppUser, IdentityRole>()
                       .AddEntityFrameworkStores<VirtualBankDbContext>()
                       .AddDefaultTokenProviders();

            return services;
        }

        public static IServiceCollection ConfigureIdentityOptions(this IServiceCollection services)
        {
            services.Configure<IdentityOptions>(options =>
            {
                //No duplicated emails
                options.User.RequireUniqueEmail = true;

                //Lockout settings
                options.Lockout.MaxFailedAccessAttempts = 5;
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(30);
            });

            return services;
        }

        public static IServiceCollection ConfigurePassword(this IServiceCollection services)
        {
            services.Configure<IdentityOptions>(options =>
            {
                options.Password.RequiredLength = 6;
                options.Password.RequireDigit = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireUppercase = false;
            });

            return services;
        }
    }
}
