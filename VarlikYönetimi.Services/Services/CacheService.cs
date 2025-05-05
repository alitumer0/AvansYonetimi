using Microsoft.Extensions.Caching.Memory;
using System;
using System.Threading.Tasks;
using VarlikYönetimi.Services.Services.Interfaces;

namespace VarlikYönetimi.Services.Services
{
    public class CacheService : ICacheService
    {
        private readonly IMemoryCache _cache;
        private readonly MemoryCacheEntryOptions _defaultOptions;

        public CacheService(IMemoryCache cache)
        {
            _cache = cache;
            _defaultOptions = new MemoryCacheEntryOptions()
                .SetSlidingExpiration(TimeSpan.FromMinutes(10))
                .SetAbsoluteExpiration(TimeSpan.FromHours(1));
        }

        public async Task<T> GetOrCreateAsync<T>(string key, Func<Task<T>> factory, TimeSpan? slidingExpiration = null)
        {
            if (_cache.TryGetValue(key, out T cachedValue))
            {
                return cachedValue;
            }

            var value = await factory();
            var options = slidingExpiration.HasValue
                ? new MemoryCacheEntryOptions()
                    .SetSlidingExpiration(slidingExpiration.Value)
                    .SetAbsoluteExpiration(slidingExpiration.Value * 2)
                : _defaultOptions;

            _cache.Set(key, value, options);
            return value;
        }

        public void Remove(string key)
        {
            _cache.Remove(key);
        }

        public void Clear()
        {
            if (_cache is MemoryCache memoryCache)
            {
                memoryCache.Compact(1.0);
            }
        }
    }
} 