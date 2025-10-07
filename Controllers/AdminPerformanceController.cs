using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using JohnHenryFashionWeb.Services;

namespace JohnHenryFashionWeb.Controllers
{
    [Authorize(Roles = "Admin")]
    [Route("admin/performance")]
    public class AdminPerformanceController : Controller
    {
        private readonly IPerformanceMonitorService _performanceService;
        private readonly ICacheService _cacheService;
        private readonly IOptimizedDataService _optimizedDataService;
        private readonly ILogger<AdminPerformanceController> _logger;

        public AdminPerformanceController(
            IPerformanceMonitorService performanceService,
            ICacheService cacheService,
            IOptimizedDataService optimizedDataService,
            ILogger<AdminPerformanceController> logger)
        {
            _performanceService = performanceService;
            _cacheService = cacheService;
            _optimizedDataService = optimizedDataService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var metrics = await _performanceService.GetCurrentMetricsAsync();
            var health = await _performanceService.GetSystemHealthAsync();
            var cacheSize = await _cacheService.GetCacheSizeAsync();
            var siteStats = await _optimizedDataService.GetSiteStatisticsAsync();

            var viewModel = new PerformanceDashboardViewModel
            {
                CurrentMetrics = metrics,
                SystemHealth = health,
                CacheEntryCount = cacheSize,
                SiteStatistics = siteStats
            };

            return View(viewModel);
        }

        [HttpGet("metrics")]
        public async Task<IActionResult> GetMetrics()
        {
            var metrics = await _performanceService.GetCurrentMetricsAsync();
            return Json(metrics);
        }

        [HttpGet("health")]
        public async Task<IActionResult> GetHealth()
        {
            var health = await _performanceService.GetSystemHealthAsync();
            return Json(health);
        }

        [HttpGet("database-report")]
        public async Task<IActionResult> DatabaseReport(DateTime? from = null, DateTime? to = null)
        {
            from ??= DateTime.UtcNow.AddDays(-7);
            to ??= DateTime.UtcNow;

            var report = await _performanceService.GetDatabasePerformanceReportAsync(from.Value, to.Value);
            return View(report);
        }

        [HttpGet("page-report")]
        public async Task<IActionResult> PageReport(DateTime? from = null, DateTime? to = null)
        {
            from ??= DateTime.UtcNow.AddDays(-7);
            to ??= DateTime.UtcNow;

            var report = await _performanceService.GetPagePerformanceReportAsync(from.Value, to.Value);
            return View(report);
        }

        [HttpPost("clear-cache")]
        public async Task<IActionResult> ClearCache()
        {
            try
            {
                await _cacheService.ClearAllAsync();
                _logger.LogInformation("Cache cleared by admin user");
                
                return Json(new { success = true, message = "Cache cleared successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to clear cache");
                return Json(new { success = false, message = "Failed to clear cache" });
            }
        }

        [HttpPost("clear-cache-pattern")]
        public async Task<IActionResult> ClearCachePattern([FromForm] string pattern)
        {
            try
            {
                await _cacheService.RemoveByPatternAsync(pattern);
                _logger.LogInformation("Cache pattern {Pattern} cleared by admin user", pattern);
                
                return Json(new { success = true, message = $"Cache pattern '{pattern}' cleared successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to clear cache pattern {Pattern}", pattern);
                return Json(new { success = false, message = "Failed to clear cache pattern" });
            }
        }

        [HttpGet("cache-keys")]
        public async Task<IActionResult> GetCacheKeys()
        {
            try
            {
                var keys = await _cacheService.GetAllKeysAsync();
                return Json(keys);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get cache keys");
                return Json(new { error = "Failed to get cache keys" });
            }
        }

        [HttpGet("api/performance-chart-data")]
        public async Task<IActionResult> GetPerformanceChartData(string type = "memory", int hours = 24)
        {
            try
            {
                // This would typically come from a time-series database
                // For now, we'll generate sample data
                await Task.CompletedTask;
                var data = GenerateSampleChartData(type, hours);
                return Json(data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get performance chart data");
                return Json(new { error = "Failed to get performance chart data" });
            }
        }

        private object GenerateSampleChartData(string type, int hours)
        {
            var now = DateTime.UtcNow;
            var data = new List<object>();

            for (int i = hours; i >= 0; i--)
            {
                var timestamp = now.AddHours(-i);
                var value = type switch
                {
                    "memory" => Random.Shared.Next(200, 800),
                    "cpu" => Random.Shared.Next(10, 90),
                    "requests" => Random.Shared.Next(50, 200),
                    "response_time" => Random.Shared.Next(100, 2000),
                    _ => Random.Shared.Next(0, 100)
                };

                data.Add(new { timestamp, value });
            }

            return data;
        }
    }

    public class PerformanceDashboardViewModel
    {
        public MonitorPerformanceMetrics CurrentMetrics { get; set; } = new();
        public SystemHealthStatus SystemHealth { get; set; } = new();
        public long CacheEntryCount { get; set; }
        public SiteStatistics SiteStatistics { get; set; } = new();
    }
}
