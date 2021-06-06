using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using VirtualBank.Api.Cache;
using VirtualBank.Api.Services;
using VirtualBank.Core.Interfaces;

namespace VirtualBank.Api.Installers
{
    public static class RedisCacheInstaller
    {
        public static IServiceCollection ConfigureRedisCaheSettings(this IServiceCollection services, IConfiguration configuration )
        {
            var redisCacheSettings = new RedisCacheSettings();
            configuration.GetSection(nameof(RedisCacheSettings)).Bind(redisCacheSettings);
            services.AddSingleton(redisCacheSettings);

            if (!redisCacheSettings.Enabled)
                return services;


            services.AddStackExchangeRedisCache(options => options.Configuration = redisCacheSettings.ConnectionString);
            services.AddSingleton<IResponseCacheService, ResponseCacheService>();

            return services;
        }
    }
}
