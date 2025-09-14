namespace JohnHenryFashionWeb.Models
{
    // Analytics Data Transfer Objects
    public class UserAnalyticsData
    {
        public int TotalSessions { get; set; }
        public int UniqueSessions { get; set; }
        public int RegisteredUserSessions { get; set; }
        public int AnonymousSessions { get; set; }
        public int TotalPageViews { get; set; }
        public int UniquePageViews { get; set; }
        public double AverageSessionDuration { get; set; }
        public double BounceRate { get; set; }
        public List<PageData> TopPages { get; set; } = new();
        public List<HourlyData> HourlyData { get; set; } = new();
        public List<DailyData> DailyData { get; set; } = new();
    }

    public class SalesAnalyticsData
    {
        public decimal TotalRevenue { get; set; }
        public int TotalOrders { get; set; }
        public int CompletedOrders { get; set; }
        public int PendingOrders { get; set; }
        public int CancelledOrders { get; set; }
        public decimal AverageOrderValue { get; set; }
        public double ConversionRate { get; set; }
        public List<PaymentMethodData> PaymentMethodBreakdown { get; set; } = new();
        public List<DailyRevenueData> DailyRevenue { get; set; } = new();
        public List<ProductSalesData> TopSellingProducts { get; set; } = new();
        public List<CategoryPerformanceData> CategoryPerformance { get; set; } = new();
    }

    public class ProductAnalyticsData
    {
        public Guid ProductId { get; set; }
        public string ProductName { get; set; } = "";
        public string CategoryName { get; set; } = "";
        public int Views { get; set; }
        public int Sales { get; set; }
        public decimal Revenue { get; set; }
        public double ConversionRate { get; set; }
        public int CartAdditions { get; set; }
        public int WishlistAdditions { get; set; }
    }

    public class MarketingAnalyticsData
    {
        public List<CampaignData> CampaignPerformance { get; set; } = new();
        public List<EmailData> EmailPerformance { get; set; } = new();
        public List<ReferrerData> TopReferrers { get; set; } = new();
        public List<SourceData> SourceBreakdown { get; set; } = new();
    }

    public class ConversionAnalyticsData
    {
        public int TotalConversions { get; set; }
        public decimal TotalValue { get; set; }
        public List<ConversionTypeData> ConversionsByType { get; set; } = new();
        public List<DailyConversionData> DailyConversions { get; set; } = new();
    }

    public class RealTimeAnalyticsData
    {
        public int ActiveUsers { get; set; }
        public int PageViewsLastHour { get; set; }
        public int OrdersLastHour { get; set; }
        public decimal TodayRevenue { get; set; }
        public List<PageActivityData> TopActivePages { get; set; } = new();
        public List<ConversionData> RecentConversions { get; set; } = new();
        public List<ActiveUserData> LiveVisitors { get; set; } = new();
    }

    // Supporting Data Classes
    public class PageData
    {
        public string Page { get; set; } = "";
        public int Views { get; set; }
    }

    public class PageViewData
    {
        public string Page { get; set; } = "";
        public int Views { get; set; }
        public int UniqueViews { get; set; }
    }

    public class HourlyData
    {
        public int Hour { get; set; }
        public int Sessions { get; set; }
        public int PageViews { get; set; }
    }

    public class DailyData
    {
        public DateTime Date { get; set; }
        public int Sessions { get; set; }
        public int PageViews { get; set; }
        public int UniqueUsers { get; set; }
    }

    public class PaymentMethodData
    {
        public string PaymentMethod { get; set; } = "";
        public int Count { get; set; }
        public decimal Revenue { get; set; }
    }

    public class DailyRevenueData
    {
        public DateTime Date { get; set; }
        public decimal Revenue { get; set; }
        public int Orders { get; set; }
    }

    public class ProductSalesData
    {
        public Guid ProductId { get; set; }
        public string ProductName { get; set; } = "";
        public int Quantity { get; set; }
        public decimal Revenue { get; set; }
    }

    public class ProductPerformanceData
    {
        public Guid ProductId { get; set; }
        public string ProductName { get; set; } = "";
        public int Sales { get; set; }
        public decimal Revenue { get; set; }
        public int Orders { get; set; }
    }

    public class CategoryPerformanceData
    {
        public Guid CategoryId { get; set; }
        public string CategoryName { get; set; } = "";
        public decimal Revenue { get; set; }
        public int Quantity { get; set; }
        public int Orders { get; set; }
    }

    public class CampaignData
    {
        public string CampaignId { get; set; } = "";
        public string Source { get; set; } = "";
        public int Clicks { get; set; }
        public int Conversions { get; set; }
        public decimal Revenue { get; set; }
    }

    public class EmailData
    {
        public string EmailId { get; set; } = "";
        public int Opens { get; set; }
        public int UniqueOpens { get; set; }
        public int Clicks { get; set; }
        public int UniqueClicks { get; set; }
    }

    public class ReferrerData
    {
        public string Referrer { get; set; } = "";
        public int Visits { get; set; }
        public int UniqueVisitors { get; set; }
    }

    public class SourceData
    {
        public string Source { get; set; } = "";
        public int Events { get; set; }
        public int UniqueUsers { get; set; }
    }

    public class ConversionTypeData
    {
        public string Type { get; set; } = "";
        public int Count { get; set; }
        public decimal Value { get; set; }
    }

    public class DailyConversionData
    {
        public DateTime Date { get; set; }
        public int Conversions { get; set; }
        public decimal Value { get; set; }
    }

    public class ActiveUserData
    {
        public string UserId { get; set; } = "";
        public string SessionId { get; set; } = "";
        public DateTime LastActivity { get; set; }
        public int PageViews { get; set; }
        public string? CurrentPage { get; set; }
        public string? Location { get; set; }
        public string? DeviceType { get; set; }
    }

    public class PageActivityData
    {
        public string Page { get; set; } = "";
        public int ActiveUsers { get; set; }
        public int Views { get; set; }
    }

    public class ConversionData
    {
        public string ConversionType { get; set; } = "";
        public decimal Value { get; set; }
        public DateTime ConvertedAt { get; set; }
        public Guid? OrderId { get; set; }
        public string? ProductName { get; set; }
    }

    public class CustomEventData
    {
        public string EventName { get; set; } = "";
        public string? UserId { get; set; }
        public string? Data { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    // Chart Data Models
    public class ChartData
    {
        public string Label { get; set; } = "";
        public decimal Value { get; set; }
        public string? Color { get; set; }
        public Dictionary<string, object> AdditionalData { get; set; } = new();
    }

    public class TimeSeriesData
    {
        public DateTime Date { get; set; }
        public decimal Value { get; set; }
        public string? Label { get; set; }
    }

    public class MultiSeriesData
    {
        public DateTime Date { get; set; }
        public Dictionary<string, decimal> Values { get; set; } = new();
    }

    // Dashboard Summary Data
    public class DashboardSummary
    {
        public decimal TodayRevenue { get; set; }
        public decimal YesterdayRevenue { get; set; }
        public decimal RevenueGrowth { get; set; }
        
        public int TodayOrders { get; set; }
        public int YesterdayOrders { get; set; }
        public decimal OrdersGrowth { get; set; }
        
        public int TodayVisitors { get; set; }
        public int YesterdayVisitors { get; set; }
        public decimal VisitorsGrowth { get; set; }
        
        public double ConversionRate { get; set; }
        public double PreviousConversionRate { get; set; }
        public decimal ConversionGrowth { get; set; }
        
        public decimal AverageOrderValue { get; set; }
        public decimal PreviousAverageOrderValue { get; set; }
        public decimal AOVGrowth { get; set; }
        
        public int ActiveUsers { get; set; }
        public List<RecentActivity> RecentActivities { get; set; } = new();
        public List<TopProduct> TopProducts { get; set; } = new();
        public List<Alert> Alerts { get; set; } = new();
    }

    public class RecentActivity
    {
        public string Type { get; set; } = ""; // order, user_registration, review, etc.
        public string Description { get; set; } = "";
        public DateTime Timestamp { get; set; }
        public string? UserId { get; set; }
        public string? UserName { get; set; }
        public string? ActionUrl { get; set; }
        public decimal? Value { get; set; }
    }

    public class TopProduct
    {
        public Guid ProductId { get; set; }
        public string ProductName { get; set; } = "";
        public string? ImageUrl { get; set; }
        public int Sales { get; set; }
        public decimal Revenue { get; set; }
        public int Views { get; set; }
        public double ConversionRate { get; set; }
    }

    public class Alert
    {
        public string Type { get; set; } = ""; // warning, info, error, success
        public string Title { get; set; } = "";
        public string Message { get; set; } = "";
        public DateTime CreatedAt { get; set; }
        public bool IsRead { get; set; }
        public string? ActionUrl { get; set; }
        public string? ActionText { get; set; }
    }

    // Filter and Date Range Models
    public class AnalyticsFilter
    {
        public DateTime StartDate { get; set; } = DateTime.UtcNow.AddDays(-30);
        public DateTime EndDate { get; set; } = DateTime.UtcNow;
        public string? Source { get; set; }
        public string? Medium { get; set; }
        public string? Campaign { get; set; }
        public List<Guid>? ProductIds { get; set; }
        public List<Guid>? CategoryIds { get; set; }
        public List<string>? UserIds { get; set; }
        public string? DeviceType { get; set; }
        public string? Country { get; set; }
        public string? City { get; set; }
    }

    public class DateRange
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Label { get; set; } = "";
        
        public static DateRange Today => new()
        {
            StartDate = DateTime.UtcNow.Date,
            EndDate = DateTime.UtcNow,
            Label = "Today"
        };
        
        public static DateRange Yesterday => new()
        {
            StartDate = DateTime.UtcNow.AddDays(-1).Date,
            EndDate = DateTime.UtcNow.AddDays(-1).Date.AddDays(1).AddSeconds(-1),
            Label = "Yesterday"
        };
        
        public static DateRange Last7Days => new()
        {
            StartDate = DateTime.UtcNow.AddDays(-7).Date,
            EndDate = DateTime.UtcNow,
            Label = "Last 7 Days"
        };
        
        public static DateRange Last30Days => new()
        {
            StartDate = DateTime.UtcNow.AddDays(-30).Date,
            EndDate = DateTime.UtcNow,
            Label = "Last 30 Days"
        };
        
        public static DateRange ThisMonth => new()
        {
            StartDate = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1),
            EndDate = DateTime.UtcNow,
            Label = "This Month"
        };
        
        public static DateRange LastMonth
        {
            get
            {
                var firstDayOfLastMonth = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1).AddMonths(-1);
                return new DateRange
                {
                    StartDate = firstDayOfLastMonth,
                    EndDate = firstDayOfLastMonth.AddMonths(1).AddSeconds(-1),
                    Label = "Last Month"
                };
            }
        }
    }

    // Performance Metrics
    public class PerformanceMetrics
    {
        public TimeSpan AveragePageLoadTime { get; set; }
        public TimeSpan AverageApiResponseTime { get; set; }
        public double ErrorRate { get; set; }
        public int TotalRequests { get; set; }
        public int SuccessfulRequests { get; set; }
        public int FailedRequests { get; set; }
        public Dictionary<string, int> StatusCodeBreakdown { get; set; } = new();
        public List<SlowEndpoint> SlowestEndpoints { get; set; } = new();
    }

    public class SlowEndpoint
    {
        public string Endpoint { get; set; } = "";
        public TimeSpan AverageResponseTime { get; set; }
        public int RequestCount { get; set; }
        public double ErrorRate { get; set; }
    }

    // Geographic Analytics
    public class GeographicData
    {
        public string Country { get; set; } = "";
        public string? CountryCode { get; set; }
        public string? City { get; set; }
        public int Visitors { get; set; }
        public int Sessions { get; set; }
        public decimal Revenue { get; set; }
        public double ConversionRate { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }

    // Device and Browser Analytics
    public class DeviceAnalytics
    {
        public Dictionary<string, int> DeviceTypes { get; set; } = new(); // desktop, mobile, tablet
        public Dictionary<string, int> Browsers { get; set; } = new();
        public Dictionary<string, int> OperatingSystems { get; set; } = new();
        public Dictionary<string, int> ScreenResolutions { get; set; } = new();
    }

    // Funnel Analysis
    public class FunnelStep
    {
        public string StepName { get; set; } = "";
        public int Users { get; set; }
        public double ConversionRate { get; set; }
        public double DropoffRate { get; set; }
    }

    public class FunnelAnalysis
    {
        public string FunnelName { get; set; } = "";
        public List<FunnelStep> Steps { get; set; } = new();
        public double OverallConversionRate { get; set; }
        public int TotalUsers { get; set; }
        public int ConvertedUsers { get; set; }
    }
}
