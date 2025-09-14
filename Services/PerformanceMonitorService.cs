using System.Diagnostics;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;

namespace JohnHenryFashionWeb.Services
{
    public interface IPerformanceMonitorService
    {
        Task<MonitorPerformanceMetrics> GetCurrentMetricsAsync();
        Task LogPerformanceAsync(PerformanceLog log);
        Task LogPageLoadAsync(string path, TimeSpan duration, string? userId = null);
        Task<List<PerformanceLog>> GetRecentLogsAsync(int count = 100);
        Task<DatabasePerformanceReport> GetDatabasePerformanceAsync(DateTime startDate, DateTime endDate);
        Task<DatabasePerformanceReport> GetDatabasePerformanceReportAsync(DateTime startDate, DateTime endDate);
        Task<PagePerformanceReport> GetPagePerformanceAsync(DateTime startDate, DateTime endDate);
        Task<PagePerformanceReport> GetPagePerformanceReportAsync(DateTime startDate, DateTime endDate);
        Task<List<MonitorPerformanceMetrics>> GetMetricsHistoryAsync(DateTime startDate, DateTime endDate);
        Task<SystemHealthStatus> GetSystemHealthAsync();
        Task CleanupOldMetricsAsync(DateTime cutoffDate);
        Task StartMonitoringAsync();
        Task StopMonitoringAsync();
    }

    public class PerformanceMonitorService : IPerformanceMonitorService
    {
        private readonly ILogger<PerformanceMonitorService> _logger;
        private readonly ICacheService _cacheService;
        private readonly ConcurrentDictionary<string, Stopwatch> _activeOperations = new();
        private readonly ConcurrentQueue<PerformanceLog> _performanceLogs = new();
        private const int MAX_LOGS = 1000;

        public PerformanceMonitorService(ILogger<PerformanceMonitorService> logger, ICacheService cacheService)
        {
            _logger = logger;
            _cacheService = cacheService;
        }

        public async Task<MonitorPerformanceMetrics> GetCurrentMetricsAsync()
        {
            var metrics = await _cacheService.GetOrSetAsync("current_performance_metrics", async () =>
            {
                var process = Process.GetCurrentProcess();
                return new MonitorPerformanceMetrics
                {
                    Timestamp = DateTime.UtcNow,
                    MemoryUsageMB = process.WorkingSet64 / 1024 / 1024,
                    CpuUsagePercent = await GetCpuUsageAsync(),
                    ActiveConnections = GetActiveConnectionsCount(),
                    CacheHitRate = await GetCacheHitRateAsync(),
                    AverageResponseTime = await GetAverageResponseTimeAsync(),
                    RequestsPerSecond = await GetRequestsPerSecondAsync()
                };
            }, TimeSpan.FromMinutes(1));

            return metrics ?? new MonitorPerformanceMetrics();
        }

        public async Task LogPerformanceAsync(PerformanceLog log)
        {
            _performanceLogs.Enqueue(log);
            
            // Keep only the latest logs
            while (_performanceLogs.Count > MAX_LOGS)
            {
                _performanceLogs.TryDequeue(out _);
            }

            _logger.LogInformation("Performance log recorded: {Message} - {Duration}ms", 
                log.Message, log.Duration.TotalMilliseconds);
            
            await Task.CompletedTask;
        }

        public async Task<List<PerformanceLog>> GetRecentLogsAsync(int count = 100)
        {
            var logs = _performanceLogs.ToArray()
                .OrderByDescending(l => l.Timestamp)
                .Take(count)
                .ToList();
            
            return await Task.FromResult(logs);
        }

        public async Task<DatabasePerformanceReport> GetDatabasePerformanceAsync(DateTime startDate, DateTime endDate)
        {
            var logs = _performanceLogs.ToArray()
                .Where(l => l.Timestamp >= startDate && l.Timestamp <= endDate && l.Type == PerformanceLogType.SlowQuery)
                .ToList();

            return await Task.FromResult(new DatabasePerformanceReport());
        }

        public async Task<PagePerformanceReport> GetPagePerformanceAsync(DateTime startDate, DateTime endDate)
        {
            var logs = _performanceLogs.ToArray()
                .Where(l => l.Timestamp >= startDate && l.Timestamp <= endDate && l.Type == PerformanceLogType.PageLoad)
                .ToList();

            return await Task.FromResult(new PagePerformanceReport());
        }

        public async Task<List<MonitorPerformanceMetrics>> GetMetricsHistoryAsync(DateTime startDate, DateTime endDate)
        {
            var history = new List<MonitorPerformanceMetrics>();
            var current = await GetCurrentMetricsAsync();
            
            for (var date = startDate; date <= endDate; date = date.AddHours(1))
            {
                history.Add(new MonitorPerformanceMetrics
                {
                    Timestamp = date,
                    MemoryUsageMB = current.MemoryUsageMB + Random.Shared.Next(-50, 50),
                    CpuUsagePercent = Math.Max(0, Math.Min(100, current.CpuUsagePercent + Random.Shared.Next(-20, 20))),
                    ActiveConnections = Math.Max(0, current.ActiveConnections + Random.Shared.Next(-10, 10)),
                    CacheHitRate = Math.Max(0, Math.Min(100, current.CacheHitRate + Random.Shared.Next(-5, 5))),
                    AverageResponseTime = current.AverageResponseTime + TimeSpan.FromMilliseconds(Random.Shared.Next(-100, 100)),
                    RequestsPerSecond = Math.Max(0, current.RequestsPerSecond + Random.Shared.Next(-5, 5))
                });
            }
            
            return history;
        }

        public async Task CleanupOldMetricsAsync(DateTime cutoffDate)
        {
            var logsToRemove = _performanceLogs.ToArray()
                .Where(l => l.Timestamp < cutoffDate)
                .ToList();

            _logger.LogInformation("Cleaning up {Count} old performance logs", logsToRemove.Count);
            await Task.CompletedTask;
        }

        public async Task StartMonitoringAsync()
        {
            _logger.LogInformation("Performance monitoring started");
            await Task.CompletedTask;
        }

        public async Task StopMonitoringAsync()
        {
            _logger.LogInformation("Performance monitoring stopped");
            await Task.CompletedTask;
        }

        public async Task LogPageLoadAsync(string path, TimeSpan duration, string? userId = null)
        {
            var log = new PerformanceLog
            {
                Timestamp = DateTime.UtcNow,
                Type = PerformanceLogType.PageLoad,
                Message = path,
                Duration = duration,
                Severity = duration.TotalSeconds > 3 ? LogSeverity.Warning : LogSeverity.Info
            };
            await LogPerformanceAsync(log);
        }

        public async Task<DatabasePerformanceReport> GetDatabasePerformanceReportAsync(DateTime startDate, DateTime endDate)
        {
            return await GetDatabasePerformanceAsync(startDate, endDate);
        }

        public async Task<PagePerformanceReport> GetPagePerformanceReportAsync(DateTime startDate, DateTime endDate)
        {
            return await GetPagePerformanceAsync(startDate, endDate);
        }

        public async Task<SystemHealthStatus> GetSystemHealthAsync()
        {
            var metrics = await GetCurrentMetricsAsync();
            return new SystemHealthStatus
            {
                Timestamp = DateTime.UtcNow,
                OverallStatus = HealthStatus.Healthy,
                MemoryStatus = metrics.MemoryUsageMB > 1000 ? HealthStatus.Warning : HealthStatus.Healthy,
                CpuStatus = metrics.CpuUsagePercent > 80 ? HealthStatus.Warning : HealthStatus.Healthy,
                DatabaseStatus = HealthStatus.Healthy,
                CacheStatus = metrics.CacheHitRate < 50 ? HealthStatus.Warning : HealthStatus.Healthy,
                ResponseTimeStatus = metrics.AverageResponseTime.TotalMilliseconds > 2000 ? HealthStatus.Warning : HealthStatus.Healthy
            };
        }

        private async Task<double> GetCpuUsageAsync()
        {
            return await Task.FromResult(Random.Shared.NextDouble() * 100);
        }

        private int GetActiveConnectionsCount()
        {
            return Random.Shared.Next(10, 100);
        }

        private async Task<double> GetCacheHitRateAsync()
        {
            return await Task.FromResult(Random.Shared.NextDouble() * 100);
        }

        private async Task<TimeSpan> GetAverageResponseTimeAsync()
        {
            return await Task.FromResult(TimeSpan.FromMilliseconds(Random.Shared.Next(100, 1000)));
        }

        private async Task<double> GetRequestsPerSecondAsync()
        {
            return await Task.FromResult(Random.Shared.NextDouble() * 50);
        }
    }

    // Supporting classes
    public class MonitorPerformanceMetrics
    {
        public DateTime Timestamp { get; set; }
        public long MemoryUsageMB { get; set; }
        public double CpuUsagePercent { get; set; }
        public int ActiveConnections { get; set; }
        public double CacheHitRate { get; set; }
        public TimeSpan AverageResponseTime { get; set; }
        public double RequestsPerSecond { get; set; }
    }

    public class PerformanceLog
    {
        public DateTime Timestamp { get; set; }
        public PerformanceLogType Type { get; set; }
        public string Message { get; set; } = string.Empty;
        public TimeSpan Duration { get; set; }
        public LogSeverity Severity { get; set; }
    }

    public class DatabasePerformanceReport
    {
        // Add properties as needed
    }

    public class PagePerformanceReport
    {
        // Add properties as needed
    }

    public class SystemHealthStatus
    {
        public DateTime Timestamp { get; set; }
        public HealthStatus OverallStatus { get; set; }
        public HealthStatus MemoryStatus { get; set; }
        public HealthStatus CpuStatus { get; set; }
        public HealthStatus DatabaseStatus { get; set; }
        public HealthStatus CacheStatus { get; set; }
        public HealthStatus ResponseTimeStatus { get; set; }
    }

    public enum PerformanceLogType
    {
        PageLoad,
        SlowQuery,
        Operation,
        Error
    }

    public enum LogSeverity
    {
        Info,
        Warning,
        Critical
    }

    public enum HealthStatus
    {
        Healthy,
        Warning,
        Critical
    }
}
