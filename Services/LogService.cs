using System.Text.RegularExpressions;

namespace JohnHenryFashionWeb.Services
{
    public interface ILogService
    {
        Task<List<LogEntry>> GetLogsAsync(DateTime? fromDate = null, DateTime? toDate = null, string? level = null, int maxCount = 500);
        Task<LogEntry?> GetLogByIdAsync(string logId);
        Task<List<string>> GetLogFilesAsync();
        Task<string> GetLogFileContentAsync(string fileName);
        Task<Dictionary<string, int>> GetLogStatisticsAsync();
    }

    public class LogService : ILogService
    {
        private readonly string _logDirectory;
        private readonly ILogger<LogService> _logger;
        private static readonly Dictionary<string, List<LogEntry>> _cachedLogs = new();
        private static DateTime _lastCacheUpdate = DateTime.MinValue;

        public LogService(ILogger<LogService> logger)
        {
            _logger = logger;
            _logDirectory = Path.Combine(Directory.GetCurrentDirectory(), "logs");
        }

        public async Task<List<LogEntry>> GetLogsAsync(
            DateTime? fromDate = null, 
            DateTime? toDate = null, 
            string? level = null,
            int maxCount = 500)
        {
            var logs = new List<LogEntry>();
            
            if (!Directory.Exists(_logDirectory))
            {
                _logger.LogWarning("Log directory not found: {Directory}", _logDirectory);
                return logs;
            }

            // Get log files sorted by date (newest first)
            var files = Directory.GetFiles(_logDirectory, "john-henry-*.txt")
                .OrderByDescending(f => f)
                .Take(7) // Only last 7 days for performance
                .ToList();

            foreach (var file in files)
            {
                try
                {
                    var fileName = Path.GetFileName(file);
                    var fileDate = ExtractDateFromFileName(fileName);

                    // Skip files outside date range
                    if (fromDate.HasValue && fileDate < fromDate.Value.Date)
                        continue;
                    if (toDate.HasValue && fileDate > toDate.Value.Date)
                        continue;

                    var lines = await File.ReadAllLinesAsync(file);
                    foreach (var line in lines)
                    {
                        var logEntry = ParseLogLine(line, fileDate);
                        if (logEntry != null)
                        {
                            // Filter by timestamp
                            if (fromDate.HasValue && logEntry.Timestamp < fromDate.Value)
                                continue;
                            if (toDate.HasValue && logEntry.Timestamp > toDate.Value)
                                continue;
                            
                            // Filter by level
                            if (!string.IsNullOrEmpty(level) && 
                                !logEntry.Level.Equals(level, StringComparison.OrdinalIgnoreCase))
                                continue;

                            logs.Add(logEntry);

                            // Stop if we have enough logs
                            if (logs.Count >= maxCount * 2) // Get extra for accurate filtering
                                break;
                        }
                    }

                    if (logs.Count >= maxCount * 2)
                        break;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error reading log file: {File}", file);
                }
            }

            return logs.OrderByDescending(l => l.Timestamp).Take(maxCount).ToList();
        }

        public async Task<LogEntry?> GetLogByIdAsync(string logId)
        {
            var logs = await GetLogsAsync(maxCount: 1000);
            return logs.FirstOrDefault(l => l.Id == logId);
        }

        public Task<List<string>> GetLogFilesAsync()
        {
            if (!Directory.Exists(_logDirectory))
                return Task.FromResult(new List<string>());

            var files = Directory.GetFiles(_logDirectory, "john-henry-*.txt")
                .Select(f => new FileInfo(f))
                .OrderByDescending(f => f.LastWriteTime)
                .Select(f => f.Name)
                .ToList();

            return Task.FromResult(files);
        }

        public async Task<string> GetLogFileContentAsync(string fileName)
        {
            // Security: validate filename
            if (!fileName.StartsWith("john-henry-") || !fileName.EndsWith(".txt"))
                throw new ArgumentException("Invalid log file name");

            var filePath = Path.Combine(_logDirectory, fileName);

            if (!File.Exists(filePath))
                throw new FileNotFoundException("Log file not found");

            return await File.ReadAllTextAsync(filePath);
        }

        public async Task<Dictionary<string, int>> GetLogStatisticsAsync()
        {
            var logs = await GetLogsAsync(
                fromDate: DateTime.Today.AddDays(-1), 
                maxCount: 10000
            );

            return new Dictionary<string, int>
            {
                ["Total"] = logs.Count,
                ["Info"] = logs.Count(l => l.Level == "INF"),
                ["Warning"] = logs.Count(l => l.Level == "WRN"),
                ["Error"] = logs.Count(l => l.Level == "ERR"),
                ["Debug"] = logs.Count(l => l.Level == "DBG")
            };
        }

        private LogEntry? ParseLogLine(string line, DateTime fileDate)
        {
            try
            {
                // Serilog format: 2025-10-22 08:25:27.910 +07:00 [INF] Message
                var pattern = @"^(\d{4}-\d{2}-\d{2}\s+\d{2}:\d{2}:\d{2}\.\d{3})\s+[+-]\d{2}:\d{2}\s+\[(\w+)\]\s+(.+)$";
                var match = Regex.Match(line, pattern);

                if (!match.Success)
                    return null;

                var timestampStr = match.Groups[1].Value;
                var level = match.Groups[2].Value;
                var message = match.Groups[3].Value;

                // Parse timestamp
                if (!DateTime.TryParseExact(timestampStr, "yyyy-MM-dd HH:mm:ss.fff", 
                    System.Globalization.CultureInfo.InvariantCulture, 
                    System.Globalization.DateTimeStyles.None, 
                    out var timestamp))
                {
                    timestamp = fileDate; // Fallback to file date
                }

                // Generate consistent ID from timestamp and message hash
                var id = $"{timestamp:yyyyMMddHHmmssfff}_{message.GetHashCode():X8}";

                // Extract additional info from message
                var userEmail = ExtractEmail(message);
                var ipAddress = ExtractIpAddress(message);
                var requestUrl = ExtractUrl(message);

                return new LogEntry
                {
                    Id = id,
                    Timestamp = timestamp,
                    Level = level,
                    Message = message,
                    Source = "Application",
                    UserEmail = userEmail,
                    IpAddress = ipAddress,
                    RequestUrl = requestUrl
                };
            }
            catch (Exception ex)
            {
                _logger.LogTrace(ex, "Failed to parse log line: {Line}", line);
                return null;
            }
        }

        private DateTime ExtractDateFromFileName(string fileName)
        {
            // Extract date from filename: john-henry-20251022.txt
            var pattern = @"john-henry-(\d{8})\.txt";
            var match = Regex.Match(fileName, pattern);

            if (match.Success && DateTime.TryParseExact(
                match.Groups[1].Value, 
                "yyyyMMdd", 
                System.Globalization.CultureInfo.InvariantCulture,
                System.Globalization.DateTimeStyles.None,
                out var date))
            {
                return date;
            }

            return DateTime.Today;
        }

        private string? ExtractEmail(string message)
        {
            var emailPattern = @"\b[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Z|a-z]{2,}\b";
            var match = Regex.Match(message, emailPattern);
            return match.Success ? match.Value : null;
        }

        private string? ExtractIpAddress(string message)
        {
            var ipPattern = @"\b(?:\d{1,3}\.){3}\d{1,3}\b";
            var match = Regex.Match(message, ipPattern);
            return match.Success ? match.Value : null;
        }

        private string? ExtractUrl(string message)
        {
            var urlPattern = @"(https?://[^\s]+|/[^\s]*)";
            var match = Regex.Match(message, urlPattern);
            return match.Success ? match.Value : null;
        }
    }

    public class LogEntry
    {
        public string Id { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; }
        public string Level { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public string Source { get; set; } = string.Empty;
        public string? Exception { get; set; }
        public string? UserEmail { get; set; }
        public string? IpAddress { get; set; }
        public string? RequestUrl { get; set; }
    }
}
