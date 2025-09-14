using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using System.Text.Json;
using System.Collections.Concurrent;

namespace JohnHenryFashionWeb.Services
{
    public interface ICacheService
    {
        Task<T?> GetAsync<T>(string key) where T : class;
        Task SetAsync<T>(string key, T value, TimeSpan? expiry = null) where T : class;
        Task RemoveAsync(string key);
        Task RemoveByPatternAsync(string pattern);
        Task<T> GetOrSetAsync<T>(string key, Func<Task<T>> getItem, TimeSpan? expiry = null) where T : class;
        T? Get<T>(string key) where T : class;
        void Set<T>(string key, T value, TimeSpan? expiry = null) where T : class;
        void Remove(string key);
        T GetOrSet<T>(string key, Func<T> getItem, TimeSpan? expiry = null) where T : class;
        Task ClearAllAsync();
        Task<bool> ExistsAsync(string key);
        Task RefreshAsync(string key);
        Task<long> GetCacheSizeAsync();
        Task<IEnumerable<string>> GetAllKeysAsync();
        Task<long> IncrementAsync(string key, TimeSpan? expiry = null);
    }

    public class CacheService : ICacheService
    {
        private readonly IDistributedCache _distributedCache;
        private readonly IMemoryCache _memoryCache;
        private readonly ILogger<CacheService> _logger;
        private readonly JsonSerializerOptions _jsonOptions;
        private readonly ConcurrentDictionary<string, byte> _keys = new();
        private readonly SemaphoreSlim _semaphore = new(1, 1);

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
                WriteIndented = false,
                DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
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
                _keys.TryAdd(key, 0); // Track the key
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error setting cached value for key: {Key}", key);
            }
        }

        public async Task<T> GetOrSetAsync<T>(string key, Func<Task<T>> getItem, TimeSpan? expiry = null) where T : class
        {
            var cachedValue = await GetAsync<T>(key);
            if (cachedValue != null)
            {
                return cachedValue;
            }

            await _semaphore.WaitAsync();
            try
            {
                // Double-check pattern
                cachedValue = await GetAsync<T>(key);
                if (cachedValue != null)
                {
                    return cachedValue;
                }

                var value = await getItem();
                if (value != null)
                {
                    await SetAsync(key, value, expiry);
                    return value;
                }
                
                // This should not happen as we have where T : class constraint
                throw new InvalidOperationException($"Unable to get or create value for cache key: {key}");
            }
            finally
            {
                _semaphore.Release();
            }
        }

        public async Task RemoveAsync(string key)
        {
            try
            {
                await _distributedCache.RemoveAsync(key);
                _keys.TryRemove(key, out _);
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
                var keysToRemove = _keys.Keys.Where(k => k.Contains(pattern)).ToList();
                var tasks = keysToRemove.Select(RemoveAsync);
                await Task.WhenAll(tasks);
                
                _logger.LogInformation("Removed {Count} cache entries matching pattern: {Pattern}", 
                    keysToRemove.Count, pattern);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error removing cached values by pattern: {Pattern}", pattern);
            }
        }

        public async Task<bool> ExistsAsync(string key)
        {
            try
            {
                var value = await _distributedCache.GetStringAsync(key);
                return !string.IsNullOrEmpty(value);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking if cache key exists: {Key}", key);
                return false;
            }
        }

        public async Task RefreshAsync(string key)
        {
            try
            {
                await _distributedCache.RefreshAsync(key);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error refreshing cache key: {Key}", key);
            }
        }

        public async Task ClearAllAsync()
        {
            try
            {
                var tasks = _keys.Keys.Select(RemoveAsync);
                await Task.WhenAll(tasks);
                _keys.Clear();
                _logger.LogInformation("Cleared all cache entries");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error clearing all cache entries");
            }
        }

        public async Task<long> GetCacheSizeAsync()
        {
            try
            {
                return await Task.FromResult(_keys.Count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting cache size");
                return 0;
            }
        }

        public async Task<IEnumerable<string>> GetAllKeysAsync()
        {
            try
            {
                return await Task.FromResult(_keys.Keys.AsEnumerable());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all cache keys");
                return Enumerable.Empty<string>();
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
                _keys.TryAdd($"mem_{key}", 0); // Track memory cache keys
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error setting memory cached value for key: {Key}", key);
            }
        }

        public T GetOrSet<T>(string key, Func<T> getItem, TimeSpan? expiry = null) where T : class
        {
            var cachedValue = Get<T>(key);
            if (cachedValue != null)
            {
                return cachedValue;
            }

            var value = getItem();
            if (value != null)
            {
                Set(key, value, expiry);
                return value;
            }
            
            // This should not happen as we have where T : class constraint
            throw new InvalidOperationException($"Unable to get or create value for cache key: {key}");
        }

        public void Remove(string key)
        {
            try
            {
                _memoryCache.Remove(key);
                _keys.TryRemove($"mem_{key}", out _);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error removing memory cached value for key: {Key}", key);
            }
        }

        public async Task<long> IncrementAsync(string key, TimeSpan? expiry = null)
        {
            try
            {
                var cached = await _distributedCache.GetStringAsync(key);
                var current = !string.IsNullOrEmpty(cached) ? long.Parse(cached) : 0;
                var newValue = current + 1;
                
                var options = new DistributedCacheEntryOptions();
                if (expiry.HasValue)
                {
                    options.SetAbsoluteExpiration(expiry.Value);
                }
                else
                {
                    options.SetAbsoluteExpiration(TimeSpan.FromMinutes(30));
                }
                
                await _distributedCache.SetStringAsync(key, newValue.ToString(), options);
                _keys.TryAdd(key, 0);
                return newValue;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error incrementing cached value for key: {Key}", key);
                return 0;
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
        public const string Brands = "brands";
        public const string ProductSearch = "product_search";
        public const string CategoryCounts = "category_counts";
        public const string PerformanceMetrics = "performance_metrics";
        
        public static string ProductById(Guid id) => $"product_{id}";
        public static string CategoryById(Guid id) => $"category_{id}";
        public static string BrandById(Guid id) => $"brand_{id}";
        public static string ProductsByCategory(Guid categoryId) => $"products_category_{categoryId}";
        public static string ProductsByBrand(Guid brandId) => $"products_brand_{brandId}";
        public static string UserOrders(string userId) => $"user_orders_{userId}";
        public static string UserCart(string userId) => $"user_cart_{userId}";
        public static string UserWishlist(string userId) => $"user_wishlist_{userId}";
        public static string UserProfile(string userId) => $"user_profile_{userId}";
        public static string ProductReviews(Guid productId) => $"product_reviews_{productId}";
        public static string RelatedProducts(Guid productId) => $"related_products_{productId}";
        public static string SearchResults(string query) => $"search_results_{query.GetHashCode()}";
        public static string BlogPost(Guid postId) => $"blog_post_{postId}";
        public static string BlogPostsByCategory(Guid categoryId) => $"blog_posts_category_{categoryId}";
    }
}
