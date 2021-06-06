using System;
using System.Threading.Tasks;

namespace VirtualBank.Core.Interfaces
{
    public interface IResponseCacheService
    {
        Task CacheResponseAsync(string cacheKey, object response, TimeSpan timeToLive);

        Task<string> GetCachedResponseAsync(string cachedKey);
    }
}
