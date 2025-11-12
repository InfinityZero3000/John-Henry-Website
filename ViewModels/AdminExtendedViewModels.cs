using JohnHenryFashionWeb.Models;

namespace JohnHenryFashionWeb.ViewModels
{
    #region Banner Management ViewModels

    public class BannerOrderModel
    {
        public Guid Id { get; set; }
        public int SortOrder { get; set; }
    }

    public class BannerReorderModel
    {
        public string Position { get; set; } = string.Empty;
        public string? TargetPage { get; set; }
        public int NewSortOrder { get; set; }
        public int OldSortOrder { get; set; }
    }

    public class BannerAssignmentModel
    {
        public string Position { get; set; } = string.Empty;
        public string? TargetPage { get; set; }
    }

    #endregion

    #region Enhanced User Management ViewModels

    public class EnhancedUserViewModel
    {
        public string Id { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime? LastLoginDate { get; set; }
        public string LastLoginDisplay => LastLoginDate?.ToString("dd/MM/yyyy HH:mm") ?? "Chưa đăng nhập";
        public bool IsActive { get; set; }
        public bool EmailConfirmed { get; set; }
        public string Role { get; set; } = string.Empty;
        public List<string> Roles { get; set; } = new();
        public int TotalOrders { get; set; }
        public int OrderCount => TotalOrders;
        public decimal TotalSpent { get; set; }
        public string AvatarUrl { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public DateTime? LastActivity { get; set; }
    }

    public class UserStatistics
    {
        public int TotalUsers { get; set; }
        public int ActiveUsers { get; set; }
        public int NewUsersThisMonth { get; set; }
        public int OnlineUsers { get; set; }
    }

    #endregion

    #region Performance & Security ViewModels

    public class SystemPerformanceViewModel
    {
        public double CpuUsage { get; set; }
        public double MemoryUsage { get; set; }
        public double DiskUsage { get; set; }
        public int ActiveConnections { get; set; }
        public double ResponseTime { get; set; }
        public int ErrorRate { get; set; }
    }

    public class SecurityOverviewViewModel
    {
        public int TotalThreats { get; set; }
        public int ActiveSessions { get; set; }
        public int FailedLogins { get; set; }
        public int SuspiciousActivities { get; set; }
        public List<SecurityEvent> RecentEvents { get; set; } = new();
    }

    public class SecurityEvent
    {
        public int Id { get; set; }
        public DateTime Timestamp { get; set; }
        public string Type { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Severity { get; set; } = string.Empty;
        public string IpAddress { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;
    }

    public class LoginHistoryEntry
    {
        public int Id { get; set; }
        public DateTime LoginTime { get; set; }
        public string IpAddress { get; set; } = string.Empty;
        public string UserAgent { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public bool Success { get; set; }
        public string UserId { get; set; } = string.Empty;
    }

    public class SystemSettingsViewModel
    {
        public Dictionary<string, string> Settings { get; set; } = new();
        public List<string> Categories { get; set; } = new();
        public bool HasUnsavedChanges { get; set; }
        
        // General Settings
        public string SiteName { get; set; } = "John Henry Fashion";
        public string? SiteDescription { get; set; }
        public string? AdminEmail { get; set; }
        public string? SupportEmail { get; set; }
        
        // E-commerce Settings
        public string Currency { get; set; } = "VND";
        public decimal TaxRate { get; set; }
        public decimal ShippingFee { get; set; }
        public decimal FreeShippingThreshold { get; set; }
        
        // Security Settings
        public bool EnableTwoFactorAuth { get; set; }
        public int PasswordExpirationDays { get; set; }
        public int MaxLoginAttempts { get; set; }
        public int SessionTimeoutMinutes { get; set; }
        
        // Email Settings
        public string? SmtpServer { get; set; }
        public int SmtpPort { get; set; }
        public string? SmtpUsername { get; set; }
        public string? SmtpPassword { get; set; }
        
        // Notification Settings
        public bool EnableEmailNotifications { get; set; }
        public bool EnableSmsNotifications { get; set; }
        public bool EnablePushNotifications { get; set; }
        
        // Maintenance Settings
        public bool MaintenanceMode { get; set; }
        public string? MaintenanceMessage { get; set; }
        
        // Analytics Settings
        public string? GoogleAnalyticsId { get; set; }
        public string? FacebookPixelId { get; set; }
        public bool EnableAnalytics { get; set; }
        
        // Payment Settings
        public bool EnableCashOnDelivery { get; set; }
        public bool EnableBankTransfer { get; set; }
        public bool EnablePayPal { get; set; }
        public bool EnableStripe { get; set; }
        
        // Inventory Settings
        public int LowStockThreshold { get; set; }
        public bool AutoReduceStock { get; set; }
        public bool AllowBackorders { get; set; }
        
        // SEO Settings
        public string? DefaultMetaTitle { get; set; }
        public string? DefaultMetaDescription { get; set; }
        public string? DefaultMetaKeywords { get; set; }
    }

    #endregion

    #region System Logs ViewModels

    public class SystemLogEntry
    {
        public int Id { get; set; }
        public DateTime Timestamp { get; set; }
        public string Level { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public string Source { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;
        public string IpAddress { get; set; } = string.Empty;
        public string Details { get; set; } = string.Empty;
    }

    public class SystemLogsViewModel
    {
        public List<SystemLogEntry> Logs { get; set; } = new();
        public string LogLevel { get; set; } = "All";
        public DateTime StartDate { get; set; } = DateTime.Today.AddDays(-7);
        public DateTime EndDate { get; set; } = DateTime.Today;
        public string SearchTerm { get; set; } = string.Empty;
        public int PageSize { get; set; } = 50;
        public int CurrentPage { get; set; } = 1;
        public int TotalLogs { get; set; }
        
        // Additional properties needed for the controller
        public int TotalPages => (int)Math.Ceiling((double)TotalLogs / PageSize);
        public string SelectedLevel { get; set; } = "All";
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public List<string> LogLevels { get; set; } = new() { "All", "Error", "Warning", "Information", "Debug", "Trace" };
    }

    #endregion

    #region System Management ViewModels

    public class MaintenanceViewModel
    {
        public bool IsMaintenanceMode { get; set; }
        public string MaintenanceMessage { get; set; } = string.Empty;
        public DateTime? ScheduledMaintenanceStart { get; set; }
        public DateTime? ScheduledMaintenanceEnd { get; set; }
        public List<string> AllowedIpAddresses { get; set; } = new();
        public bool AllowAdminAccess { get; set; } = true;
    }

    public class BackupInfo
    {
        public Guid Id { get; set; }
        public string FileName { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; }
        public DateTime CreatedAt { get; set; }
        public long FileSize { get; set; }
        public long Size { get; set; }
        public string Description { get; set; } = string.Empty;
        public string Type { get; set; } = "Full";
    }

    public class BackupRestoreViewModel
    {
        public List<BackupInfo> Backups { get; set; } = new();
        public List<BackupInfo> BackupHistory { get; set; } = new();
        public string NewBackupDescription { get; set; } = string.Empty;
        public bool AutoBackupEnabled { get; set; }
        public int BackupRetentionDays { get; set; } = 30;
        public int RetentionDays { get; set; } = 30;
        public string BackupFrequency { get; set; } = "Daily";
    }

    public class SystemLog
    {
        public Guid Id { get; set; }
        public DateTime Timestamp { get; set; }
        public string Level { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public string Source { get; set; } = string.Empty;
        public string? UserId { get; set; }
        public string? IpAddress { get; set; }
        public string? Details { get; set; }
    }

    public class ServerInfo
    {
        public string ServerName { get; set; } = string.Empty;
        public string MachineName { get; set; } = string.Empty;
        public string OperatingSystem { get; set; } = string.Empty;
        public string Framework { get; set; } = string.Empty;
        public DateTime StartTime { get; set; }
        public TimeSpan Uptime { get; set; }
        public double CpuUsage { get; set; }
        public long MemoryUsed { get; set; }
        public long MemoryTotal { get; set; }
        public long WorkingSet { get; set; }
        public long TotalPhysicalMemory { get; set; }
        public long AvailablePhysicalMemory { get; set; }
        public int ProcessorCount { get; set; }
        public long DiskUsed { get; set; }
        public long DiskTotal { get; set; }
    }

    public class DatabaseInfo
    {
        public string DatabaseName { get; set; } = string.Empty;
        public string Version { get; set; } = string.Empty;
        public long DatabaseSize { get; set; }
        public int TableCount { get; set; }
        public int RecordCount { get; set; }
        public DateTime LastBackup { get; set; }
        public string Status { get; set; } = string.Empty;
    }

    public class ApplicationInfo
    {
        public string Version { get; set; } = string.Empty;
        public string Environment { get; set; } = string.Empty;
        public DateTime BuildDate { get; set; }
        public TimeSpan Uptime { get; set; }
        public int ActiveUsers { get; set; }
        public int TotalUsers { get; set; }
        public int TotalProducts { get; set; }
        public int TotalOrders { get; set; }
    }

    public class SystemInfoViewModel
    {
        public ServerInfo Server { get; set; } = new();
        public DatabaseInfo Database { get; set; } = new();
        public ApplicationInfo Application { get; set; } = new();
        public List<string> RecentLogs { get; set; } = new();
        public Dictionary<string, object> ConfigSettings { get; set; } = new();
    }

    #endregion

    #region Analytics ViewModels

    public class SalesAnalytics
    {
        public double TotalRevenue { get; set; }
        public double RevenueGrowth { get; set; }
        public int TotalOrders { get; set; }
        public double OrderGrowth { get; set; }
        public double AverageOrderValue { get; set; }
        public double ConversionRate { get; set; }
    }

    public class UserAnalytics
    {
        public int TotalUsers { get; set; }
        public int NewUsers { get; set; }
        public double UserGrowth { get; set; }
        public double RetentionRate { get; set; }
        public double ChurnRate { get; set; }
        public int ActiveUsers { get; set; }
    }

    public class ProductAnalytics
    {
        public int TotalProducts { get; set; }
        public int OutOfStock { get; set; }
        public int LowStock { get; set; }
        public double StockTurnover { get; set; }
        public int NewProducts { get; set; }
        public double AverageRating { get; set; }
    }

    public class MarketingAnalytics
    {
        public int TotalCampaigns { get; set; }
        public double CampaignROI { get; set; }
        public int TotalImpressions { get; set; }
        public double CTR { get; set; }
        public double CPC { get; set; }
        public double ConversionRate { get; set; }
    }

    public class AnalyticsChart
    {
        public string Id { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public object Data { get; set; } = new { };
        public object Options { get; set; } = new { };
        public string Description { get; set; } = string.Empty;
        public bool IsRealTime { get; set; }
    }

    public class KPI
    {
        public string Id { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Value { get; set; } = string.Empty;
        public string Change { get; set; } = string.Empty;
        public string ChangeType { get; set; } = string.Empty;
        public string Icon { get; set; } = string.Empty;
        public string Color { get; set; } = string.Empty;
    }

    public class AnalyticsFilter
    {
        public DateTime StartDate { get; set; } = DateTime.Today.AddDays(-30);
        public DateTime EndDate { get; set; } = DateTime.Today;
        public string Granularity { get; set; } = "day";
        public List<string> Categories { get; set; } = new();
        public List<string> Products { get; set; } = new();
        public List<string> Channels { get; set; } = new();
    }

    public class AnalyticsExport
    {
        public string Id { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Format { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public bool IsAvailable { get; set; } = true;
    }

    #endregion

    #region API Models

    public class AnalyticsRequest
    {
        public string DataType { get; set; } = string.Empty;
        public string DateRange { get; set; } = "today";
        public string Metric { get; set; } = string.Empty;
        public List<string> Filters { get; set; } = new();
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }

    public class AnalyticsResponse
    {
        public bool Success { get; set; }
        public object Data { get; set; } = new();
        public string Error { get; set; } = string.Empty;
        public DateTime LastUpdated { get; set; }
        public int TotalRecords { get; set; }
        public Dictionary<string, object> Metadata { get; set; } = new();
    }

    public class ExportRequest
    {
        public string Format { get; set; } = "excel";
        public string ReportName { get; set; } = string.Empty;
        public string DateRange { get; set; } = "today";
        public List<string> Metrics { get; set; } = new();
        public Dictionary<string, object> Filters { get; set; } = new();
        public bool IncludeCharts { get; set; } = true;
        public string Template { get; set; } = "standard";
    }

    public class ScheduleReportRequest
    {
        public string ReportType { get; set; } = string.Empty;
        public string Schedule { get; set; } = string.Empty;
        public List<string> Recipients { get; set; } = new();
        public string Format { get; set; } = "pdf";
        public string DateRange { get; set; } = "last_month";
        public Dictionary<string, object> Parameters { get; set; } = new();
        public string TimeZone { get; set; } = "Asia/Ho_Chi_Minh";
    }

    #endregion
}