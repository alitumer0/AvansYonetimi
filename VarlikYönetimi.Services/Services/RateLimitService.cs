using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using VarlikYönetimi.Services.Interfaces;

namespace VarlikYönetimi.Services.Services
{
    public class RateLimitService : IRateLimitService
    {
        private readonly IMemoryCache _cache;
        private readonly ILogger<RateLimitService> _logger;
        private const int MaxRequestsPerMinute = 100;
        private const int TimeWindowInMinutes = 1;

        public RateLimitService(IMemoryCache cache, ILogger<RateLimitService> logger)
        {
            _cache = cache;
            _logger = logger;
        }

        public Task<bool> IsRequestAllowed(string clientId, string endpoint)
        {
            var cacheKey = $"rate_limit_{clientId}_{endpoint}";
            var requestCount = _cache.GetOrCreate(cacheKey, entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(TimeWindowInMinutes);
                return 0;
            });

            if (requestCount >= MaxRequestsPerMinute)
            {
                _logger.LogWarning($"Rate limit exceeded for client {clientId} on endpoint {endpoint}");
                return Task.FromResult(false);
            }

            _cache.Set(cacheKey, requestCount + 1, TimeSpan.FromMinutes(TimeWindowInMinutes));
            return Task.FromResult(true);
        }

        public Task<int> GetRemainingRequests(string clientId, string endpoint)
        {
            var cacheKey = $"rate_limit_{clientId}_{endpoint}";
            var requestCount = _cache.GetOrCreate(cacheKey, entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(TimeWindowInMinutes);
                return 0;
            });

            var remainingRequests = Math.Max(0, MaxRequestsPerMinute - requestCount);
            return Task.FromResult(remainingRequests);
        }

        public Task ResetRateLimit(string clientId, string endpoint)
        {
            var cacheKey = $"rate_limit_{clientId}_{endpoint}";
            _cache.Remove(cacheKey);
            _logger.LogInformation($"Rate limit reset for client {clientId} on endpoint {endpoint}");
            return Task.CompletedTask;
        }
    }
} 