using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using System.Text.Json;

namespace JohnHenryFashionWeb.Services
{
    public interface ICacheService
    {
        Task<T?> GetAsync<T>(string key) where T : class;
        Task SetAsync<T>(string key, T value, TimeSpan? expiry = null) where T : class;
        Task RemoveAsync(string key);
        Task RemoveByPatternAsync(string pattern);
        T? Get<T>(string key) where T : class;
        void Set<T>(string key, T value, TimeSpan? expiry = null) where T : class;
        void Remove(string key);
    }

    public class CacheService : ICacheService
    {
        private readonly IDistributedCache _distributedCache;
        private readonly IMemoryCache _memoryCache;
        private readonly ILogger<CacheService> _logger;
        private readonly JsonSerializerOptions _jsonOptions;

        public CacheService(
            IDistributedCache distributedCache,
            IMemoryCache memoryCache,
            ILogger<CacheService> logger)
        {
            _distributedCache = distributedCache;
            _memoryCache = memoryCache;
            _logger = logger;
            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = false
            };
        }

        public async Task<T?> GetAsync<T>(string key) where T : class
        {
            try
            {
                var cached = await _distributedCache.GetStringAsync(key);
                if (cached != null)
                {
                    return JsonSerializer.Deserialize<T>(cached, _jsonOptions);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting cached value for key: {Key}", key);
            }
            return null;
        }

        public async Task SetAsync<T>(string key, T value, TimeSpan? expiry = null) where T : class
        {
            try
            {
                var serialized = JsonSerializer.Serialize(value, _jsonOptions);
                var options = new DistributedCacheEntryOptions();
                
                if (expiry.HasValue)
                {
                    options.SetAbsoluteExpiration(expiry.Value);
                }
                else
                {
                    options.SetAbsoluteExpiration(TimeSpan.FromMinutes(30)); // Default 30 minutes
                }

                await _distributedCache.SetStringAsync(key, serialized, options);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error setting cached value for key: {Key}", key);
            }
        }

        public async Task RemoveAsync(string key)
        {
            try
            {
                await _distributedCache.RemoveAsync(key);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error removing cached value for key: {Key}", key);
            }
        }

        public async Task RemoveByPatternAsync(string pattern)
        {
            try
            {
                // Note: This is a simplified implementation
                // For production, consider using Redis-specific pattern removal
                _logger.LogWarning("Pattern-based cache removal not fully implemented for distributed cache");
                await Task.CompletedTask;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error removing cached values by pattern: {Pattern}", pattern);
            }
        }

        public T? Get<T>(string key) where T : class
        {
            try
            {
                return _memoryCache.Get<T>(key);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting memory cached value for key: {Key}", key);
                return null;
            }
        }

        public void Set<T>(string key, T value, TimeSpan? expiry = null) where T : class
        {
            try
            {
                var options = new MemoryCacheEntryOptions();
                if (expiry.HasValue)
                {
                    options.SetAbsoluteExpiration(expiry.Value);
                }
                else
                {
                    options.SetAbsoluteExpiration(TimeSpan.FromMinutes(15)); // Default 15 minutes
                }

                _memoryCache.Set(key, value, options);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error setting memory cached value for key: {Key}", key);
            }
        }

        public void Remove(string key)
        {
            try
            {
                _memoryCache.Remove(key);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error removing memory cached value for key: {Key}", key);
            }
        }
    }

    public static class CacheKeys
    {
        public const string Products = "products";
        public const string Categories = "categories";
        public const string FeaturedProducts = "featured_products";
        public const string PopularProducts = "popular_products";
        public const string RecentPosts = "recent_posts";
        public const string SiteStatistics = "site_statistics";
        
        public static string ProductById(Guid id) => $"product_{id}";
        public static string CategoryById(Guid id) => $"category_{id}";
        public static string ProductsByCategory(Guid categoryId) => $"products_category_{categoryId}";
        public static string UserOrders(string userId) => $"user_orders_{userId}";
        public static string UserCart(string userId) => $"user_cart_{userId}";
        public static string UserWishlist(string userId) => $"user_wishlist_{userId}";
    }
}
