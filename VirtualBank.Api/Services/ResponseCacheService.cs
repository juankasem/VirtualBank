using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using VirtualBank.Core.Interfaces;

namespace VirtualBank.Api.Services
{
    public class ResponseCacheService : IResponseCacheService
    {
        private readonly IDistributedCache _distributedCache;

        public ResponseCacheService(IDistributedCache distributedCache)
        {
            _distributedCache = distributedCache;
        }

        public async Task CacheResponseAsync(string cacheKey, object response, TimeSpan timeToLive)
        {
            if (response == null)
            {
                return;
            }

            var serializedResponse = JsonConvert.SerializeObject(response);

            await _distributedCache.SetStringAsync(cacheKey, serializedResponse, new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = timeToLive
            });
        }

        public async Task<string> GetCachedResponseAsync(string cachedKey)
        {
            var cachedResponse = await _distributedCache.GetStringAsync(cachedKey);
            return string.IsNullOrEmpty(cachedResponse) ? null : cachedResponse;
        }
    }
}
