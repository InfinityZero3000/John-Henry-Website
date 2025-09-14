using JohnHenryFashionWeb.Models;

namespace JohnHenryFashionWeb.ViewModels
{
    // Main Dashboard ViewModel
    public class DashboardViewModel
    {
        public DashboardSummary Summary { get; set; } = new();
        public DashboardSummary DashboardSummary { get; set; } = new();
        public List<ChartData> SalesChartData { get; set; } = new();
        public List<ChartData> TopProductsData { get; set; } = new();
        public List<TimeSeriesData> RevenueTimeSeriesData { get; set; } = new();
        public List<CategoryPerformanceData> CategoryPerformance { get; set; } = new();
        public RealTimeAnalyticsData RealTimeData { get; set; } = new();
        public PerformanceMetrics PerformanceMetrics { get; set; } = new();
        public List<GeographicData> GeographicData { get; set; } = new();
        public DeviceAnalytics DeviceAnalytics { get; set; } = new();
        
        // Enhanced dashboard data
        public List<Order> RecentOrders { get; set; } = new();
        public List<ApplicationUser> RecentUsers { get; set; } = new();
        public List<Product> LowStockProducts { get; set; } = new();
        public DashboardStats? TodayStats { get; set; }
        public WeeklyComparisonStats? WeeklyComparison { get; set; }
        public List<SystemAlert> SystemAlerts { get; set; } = new();
        public QuickAction[] QuickActions { get; set; } = Array.Empty<QuickAction>();
        
        // Filter options
        public AnalyticsFilter CurrentFilter { get; set; } = new();
        public List<DateRange> AvailableDateRanges { get; set; } = new();
        public List<Product> AvailableProducts { get; set; } = new();
        public List<Category> AvailableCategories { get; set; } = new();
        
        // UI State
        public string SelectedTab { get; set; } = "overview";
        public bool IsLoading { get; set; } = false;
        public string? ErrorMessage { get; set; }
        public DateTime LastUpdated { get; set; } = DateTime.UtcNow;
    }

    // Analytics Overview ViewModel
    public class AnalyticsViewModel
    {
        public UserAnalyticsData UserAnalytics { get; set; } = new();
        public SalesAnalyticsData SalesAnalytics { get; set; } = new();
        public ProductAnalyticsData ProductAnalytics { get; set; } = new();
        public MarketingAnalyticsData MarketingAnalytics { get; set; } = new();
        public ConversionAnalyticsData ConversionAnalytics { get; set; } = new();
        
        // Time-based data
        public List<MultiSeriesData> ComparisonData { get; set; } = new();
        public List<FunnelAnalysis> FunnelAnalyses { get; set; } = new();
        
        // Filter and date range
        public AnalyticsFilter Filter { get; set; } = new();
        public DateRange DateRange { get; set; } = DateRange.Last30Days;
        public string ComparisonPeriod { get; set; } = "previous"; // previous, same_last_year
        
        // Export options
        public List<ExportFormat> AvailableFormats { get; set; } = new();
        public bool CanExport { get; set; } = true;
        
        // UI Configuration
        public AnalyticsViewConfiguration ViewConfig { get; set; } = new();
    }

    // Reporting ViewModel
    public class ReportViewModel
    {
        public List<SalesReport> AvailableReports { get; set; } = new();
        public List<ReportTemplate> ScheduledReports { get; set; } = new();
        public ReportTemplate? CurrentTemplate { get; set; }
        public SalesReport? CurrentReport { get; set; }
        
        // Report generation
        public ReportGenerationRequest GenerationRequest { get; set; } = new();
        public List<ReportType> AvailableReportTypes { get; set; } = new();
        public List<ExportFormat> AvailableFormats { get; set; } = new();
        
        // Filters
        public AnalyticsFilter Filter { get; set; } = new();
        public List<Category> AvailableCategories { get; set; } = new();
        public List<Product> AvailableProducts { get; set; } = new();
        
        // Scheduling
        public ReportScheduleSettings ScheduleSettings { get; set; } = new();
        public List<string> AvailableFrequencies { get; set; } = new() { "Daily", "Weekly", "Monthly", "Quarterly", "Yearly" };
        
        // UI State
        public bool IsGenerating { get; set; } = false;
        public string? GenerationProgress { get; set; }
        public string? ErrorMessage { get; set; }
        public string? SuccessMessage { get; set; }
    }

    // Chart Data ViewModels
    public class ChartDataViewModel
    {
        public string ChartType { get; set; } = ""; // line, bar, pie, doughnut, area
        public string Title { get; set; } = "";
        public string SubTitle { get; set; } = "";
        public List<ChartSeries> Series { get; set; } = new();
        public List<string> Labels { get; set; } = new();
        public ChartOptions Options { get; set; } = new();
        public Dictionary<string, object> CustomData { get; set; } = new();
        
        // Responsive settings
        public bool IsResponsive { get; set; } = true;
        public int? Height { get; set; }
        public int? Width { get; set; }
        
        // Interaction
        public bool EnableTooltips { get; set; } = true;
        public bool EnableLegend { get; set; } = true;
        public bool EnableAnimation { get; set; } = true;
        public bool EnableZoom { get; set; } = false;
        
        // Data refresh
        public bool AutoRefresh { get; set; } = false;
        public int RefreshInterval { get; set; } = 30000; // milliseconds
        public DateTime LastUpdated { get; set; } = DateTime.UtcNow;
    }

    // Real-time Dashboard ViewModel
    public class RealTimeDashboardViewModel
    {
        public RealTimeAnalyticsData RealTimeData { get; set; } = new();
        public List<ActiveUserData> LiveVisitors { get; set; } = new();
        public List<ConversionData> RecentConversions { get; set; } = new();
        public List<PageActivityData> TopActivePages { get; set; } = new();
        
        // Live metrics
        public LiveMetrics LiveMetrics { get; set; } = new();
        public List<LiveEvent> RecentEvents { get; set; } = new();
        public LivePerformanceData Performance { get; set; } = new();
        
        // Configuration
        public int UpdateInterval { get; set; } = 10000; // 10 seconds
        public bool AutoUpdate { get; set; } = true;
        public List<string> EnabledMetrics { get; set; } = new();
        
        // Alerts
        public List<RealTimeAlert> ActiveAlerts { get; set; } = new();
        public AlertSettings AlertSettings { get; set; } = new();
    }

    // Product Analytics ViewModel
    public class ProductAnalyticsViewModel
    {
        public List<ProductAnalyticsData> ProductPerformance { get; set; } = new();
        public ProductAnalyticsData? SelectedProduct { get; set; }
        public List<CategoryPerformanceData> CategoryComparison { get; set; } = new();
        
        // Product-specific charts
        public List<TimeSeriesData> ProductViewsTrend { get; set; } = new();
        public List<TimeSeriesData> ProductSalesTrend { get; set; } = new();
        public List<ChartData> ProductConversionFunnel { get; set; } = new();
        
        // Filters
        public ProductAnalyticsFilter Filter { get; set; } = new();
        public List<Category> AvailableCategories { get; set; } = new();
        public List<Brand> AvailableBrands { get; set; } = new();
        
        // Sorting and pagination
        public ProductAnalyticsSortOptions SortOptions { get; set; } = new();
        public PaginationInfo Pagination { get; set; } = new();
        
        // Recommendations
        public List<ProductRecommendation> Recommendations { get; set; } = new();
        public ProductOptimizationSuggestions OptimizationSuggestions { get; set; } = new();
    }

    // Customer Analytics ViewModel
    public class CustomerAnalyticsViewModel
    {
        public CustomerSegmentAnalysis SegmentAnalysis { get; set; } = new();
        public List<CustomerLifetimeValue> CustomerLTV { get; set; } = new();
        public List<CustomerBehaviorData> BehaviorAnalysis { get; set; } = new();
        
        // Customer charts
        public List<ChartData> CustomerSegmentChart { get; set; } = new();
        public List<TimeSeriesData> CustomerAcquisitionTrend { get; set; } = new();
        public List<ChartData> CustomerRetentionChart { get; set; } = new();
        
        // Cohort analysis
        public CohortAnalysisData CohortAnalysis { get; set; } = new();
        public List<RetentionMetrics> RetentionMetrics { get; set; } = new();
        
        // Filters
        public CustomerAnalyticsFilter Filter { get; set; } = new();
        public DateRange DateRange { get; set; } = DateRange.Last30Days;
        
        // Customer insights
        public List<CustomerInsight> Insights { get; set; } = new();
        public CustomerHealthScore OverallHealthScore { get; set; } = new();
    }

    // Supporting ViewModels and Classes

    public class ExportFormat
    {
        public string Id { get; set; } = "";
        public string Name { get; set; } = "";
        public string Extension { get; set; } = "";
        public string MimeType { get; set; } = "";
        public bool IsAvailable { get; set; } = true;
    }

    public class ReportType
    {
        public string Id { get; set; } = "";
        public string Name { get; set; } = "";
        public string Description { get; set; } = "";
        public List<string> RequiredParameters { get; set; } = new();
        public List<string> OptionalParameters { get; set; } = new();
    }

    public class ReportGenerationRequest
    {
        public string ReportType { get; set; } = "";
        public string Format { get; set; } = "excel";
        public DateTime StartDate { get; set; } = DateTime.UtcNow.AddDays(-30);
        public DateTime EndDate { get; set; } = DateTime.UtcNow;
        public Dictionary<string, object> Parameters { get; set; } = new();
        public bool IncludeCharts { get; set; } = true;
        public bool IncludeSummary { get; set; } = true;
        public string? EmailTo { get; set; }
    }

    public class ReportScheduleSettings
    {
        public string Name { get; set; } = "";
        public string Description { get; set; } = "";
        public string ReportType { get; set; } = "";
        public string Format { get; set; } = "excel";
        public string Frequency { get; set; } = "weekly";
        public string? EmailTo { get; set; }
        public Dictionary<string, object> Parameters { get; set; } = new();
        public bool IsActive { get; set; } = true;
        public DateTime StartDate { get; set; } = DateTime.UtcNow;
        public DateTime? EndDate { get; set; }
    }

    public class AnalyticsViewConfiguration
    {
        public List<string> VisibleCharts { get; set; } = new();
        public Dictionary<string, ChartConfiguration> ChartConfigs { get; set; } = new();
        public bool ShowComparison { get; set; } = true;
        public bool ShowRealTime { get; set; } = true;
        public string DefaultTimeRange { get; set; } = "last30days";
        public string Theme { get; set; } = "light";
    }

    public class ChartConfiguration
    {
        public string Type { get; set; } = "";
        public bool Visible { get; set; } = true;
        public int Order { get; set; } = 0;
        public Dictionary<string, object> Options { get; set; } = new();
    }

    public class ChartSeries
    {
        public string Name { get; set; } = "";
        public List<decimal> Data { get; set; } = new();
        public string Color { get; set; } = "";
        public string Type { get; set; } = ""; // For mixed charts
        public bool Visible { get; set; } = true;
    }

    public class ChartOptions
    {
        public bool Responsive { get; set; } = true;
        public bool MaintainAspectRatio { get; set; } = false;
        public ChartLegend Legend { get; set; } = new();
        public ChartTooltip Tooltip { get; set; } = new();
        public Dictionary<string, object> Scales { get; set; } = new();
        public Dictionary<string, object> Plugins { get; set; } = new();
    }

    public class ChartLegend
    {
        public bool Display { get; set; } = true;
        public string Position { get; set; } = "top";
        public Dictionary<string, object> Labels { get; set; } = new();
    }

    public class ChartTooltip
    {
        public bool Enabled { get; set; } = true;
        public string Mode { get; set; } = "nearest";
        public bool Intersect { get; set; } = false;
    }

    public class LiveMetrics
    {
        public int ActiveUsers { get; set; }
        public int PageViewsPerMinute { get; set; }
        public int OrdersPerHour { get; set; }
        public decimal RevenuePerHour { get; set; }
        public double CurrentConversionRate { get; set; }
        public TimeSpan AverageSessionDuration { get; set; }
        public List<TopPage> TopPages { get; set; } = new();
        public List<TopReferrer> TopReferrers { get; set; } = new();
    }

    public class TopPage
    {
        public string Path { get; set; } = "";
        public int ActiveUsers { get; set; }
        public int ViewsLastHour { get; set; }
    }

    public class TopReferrer
    {
        public string Source { get; set; } = "";
        public int Visitors { get; set; }
        public string Type { get; set; } = ""; // organic, direct, social, etc.
    }

    public class LiveEvent
    {
        public string Type { get; set; } = "";
        public string Description { get; set; } = "";
        public DateTime Timestamp { get; set; }
        public string? UserId { get; set; }
        public string? SessionId { get; set; }
        public Dictionary<string, object> Data { get; set; } = new();
    }

    public class LivePerformanceData
    {
        public TimeSpan AverageResponseTime { get; set; }
        public double ErrorRate { get; set; }
        public int RequestsPerMinute { get; set; }
        public List<ServerMetric> ServerMetrics { get; set; } = new();
    }

    public class ServerMetric
    {
        public string Name { get; set; } = "";
        public decimal Value { get; set; }
        public string Unit { get; set; } = "";
        public string Status { get; set; } = "normal"; // normal, warning, critical
    }

    public class RealTimeAlert
    {
        public string Id { get; set; } = "";
        public string Type { get; set; } = ""; // performance, traffic, error, business
        public string Severity { get; set; } = ""; // low, medium, high, critical
        public string Title { get; set; } = "";
        public string Message { get; set; } = "";
        public DateTime CreatedAt { get; set; }
        public bool IsAcknowledged { get; set; }
        public string? ActionUrl { get; set; }
    }

    public class AlertSettings
    {
        public bool EnableAlerts { get; set; } = true;
        public Dictionary<string, AlertThreshold> Thresholds { get; set; } = new();
        public List<string> NotificationChannels { get; set; } = new();
        public bool PlaySounds { get; set; } = true;
        public bool ShowPopups { get; set; } = true;
    }

    public class AlertThreshold
    {
        public decimal WarningValue { get; set; }
        public decimal CriticalValue { get; set; }
        public string ComparisonOperator { get; set; } = "greater_than"; // greater_than, less_than, equals
        public bool IsEnabled { get; set; } = true;
    }

    // Product Analytics Support Classes
    public class ProductAnalyticsFilter
    {
        public List<Guid> CategoryIds { get; set; } = new();
        public List<Guid> BrandIds { get; set; } = new();
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public bool? IsActive { get; set; }
        public DateTime StartDate { get; set; } = DateTime.UtcNow.AddDays(-30);
        public DateTime EndDate { get; set; } = DateTime.UtcNow;
        public int? MinViews { get; set; }
        public int? MinSales { get; set; }
        public string? SearchTerm { get; set; }
    }

    public class ProductAnalyticsSortOptions
    {
        public string SortBy { get; set; } = "revenue"; // revenue, views, sales, conversion, name
        public string SortOrder { get; set; } = "desc"; // asc, desc
        public List<SortOption> AvailableOptions { get; set; } = new();
    }

    public class SortOption
    {
        public string Value { get; set; } = "";
        public string Label { get; set; } = "";
        public bool IsNumeric { get; set; } = false;
    }

    public class PaginationInfo
    {
        public int CurrentPage { get; set; } = 1;
        public int PageSize { get; set; } = 25;
        public int TotalItems { get; set; }
        public int TotalPages => (int)Math.Ceiling((double)TotalItems / PageSize);
        public bool HasPrevious => CurrentPage > 1;
        public bool HasNext => CurrentPage < TotalPages;
        public List<int> AvailablePageSizes { get; set; } = new() { 10, 25, 50, 100 };
    }

    public class ProductRecommendation
    {
        public string Type { get; set; } = ""; // price_optimization, promotion, inventory, etc.
        public string Title { get; set; } = "";
        public string Description { get; set; } = "";
        public string Priority { get; set; } = "medium"; // low, medium, high
        public Guid ProductId { get; set; }
        public string ProductName { get; set; } = "";
        public Dictionary<string, object> Data { get; set; } = new();
        public string? ActionUrl { get; set; }
    }

    public class ProductOptimizationSuggestions
    {
        public List<PricingOptimization> PricingOptimizations { get; set; } = new();
        public List<InventoryOptimization> InventoryOptimizations { get; set; } = new();
        public List<MarketingOptimization> MarketingOptimizations { get; set; } = new();
        public List<ContentOptimization> ContentOptimizations { get; set; } = new();
    }

    public class PricingOptimization
    {
        public Guid ProductId { get; set; }
        public string ProductName { get; set; } = "";
        public decimal CurrentPrice { get; set; }
        public decimal SuggestedPrice { get; set; }
        public decimal PotentialRevenueIncrease { get; set; }
        public string Reasoning { get; set; } = "";
        public double Confidence { get; set; }
    }

    public class InventoryOptimization
    {
        public Guid ProductId { get; set; }
        public string ProductName { get; set; } = "";
        public int CurrentStock { get; set; }
        public int SuggestedStock { get; set; }
        public string Action { get; set; } = ""; // reorder, reduce, discontinue
        public string Reasoning { get; set; } = "";
        public DateTime? SuggestedActionDate { get; set; }
    }

    public class MarketingOptimization
    {
        public Guid ProductId { get; set; }
        public string ProductName { get; set; } = "";
        public string Channel { get; set; } = ""; // email, social, ads, etc.
        public string Action { get; set; } = ""; // promote, feature, discount
        public string Reasoning { get; set; } = "";
        public decimal EstimatedROI { get; set; }
    }

    public class ContentOptimization
    {
        public Guid ProductId { get; set; }
        public string ProductName { get; set; } = "";
        public string Area { get; set; } = ""; // title, description, images, seo
        public string Suggestion { get; set; } = "";
        public string Priority { get; set; } = "medium";
        public string ExpectedImpact { get; set; } = "";
    }

    // Customer Analytics Support Classes
    public class CustomerSegmentAnalysis
    {
        public List<CustomerSegment> Segments { get; set; } = new();
        public CustomerSegmentComparison Comparison { get; set; } = new();
        public List<SegmentTrend> Trends { get; set; } = new();
    }

    public class CustomerSegment
    {
        public string Id { get; set; } = "";
        public string Name { get; set; } = "";
        public string Description { get; set; } = "";
        public int CustomerCount { get; set; }
        public decimal TotalRevenue { get; set; }
        public decimal AverageOrderValue { get; set; }
        public double AverageOrderFrequency { get; set; }
        public decimal LifetimeValue { get; set; }
        public List<string> Characteristics { get; set; } = new();
    }

    public class CustomerSegmentComparison
    {
        public List<SegmentMetric> Metrics { get; set; } = new();
        public string TopPerformingSegment { get; set; } = "";
        public string FastestGrowingSegment { get; set; } = "";
        public List<SegmentInsight> Insights { get; set; } = new();
    }

    public class SegmentMetric
    {
        public string MetricName { get; set; } = "";
        public Dictionary<string, decimal> SegmentValues { get; set; } = new();
        public string BestSegment { get; set; } = "";
        public string WorstSegment { get; set; } = "";
    }

    public class SegmentTrend
    {
        public string SegmentId { get; set; } = "";
        public string SegmentName { get; set; } = "";
        public List<TimeSeriesData> CustomerCountTrend { get; set; } = new();
        public List<TimeSeriesData> RevenueTrend { get; set; } = new();
        public decimal GrowthRate { get; set; }
    }

    public class SegmentInsight
    {
        public string Type { get; set; } = "";
        public string Title { get; set; } = "";
        public string Description { get; set; } = "";
        public string SegmentId { get; set; } = "";
        public string Priority { get; set; } = "medium";
        public List<string> Recommendations { get; set; } = new();
    }

    public class CustomerLifetimeValue
    {
        public string UserId { get; set; } = "";
        public string CustomerName { get; set; } = "";
        public string Email { get; set; } = "";
        public decimal LifetimeValue { get; set; }
        public decimal PredictedLifetimeValue { get; set; }
        public int TotalOrders { get; set; }
        public DateTime FirstOrderDate { get; set; }
        public DateTime? LastOrderDate { get; set; }
        public double AverageOrderFrequency { get; set; }
        public decimal AverageOrderValue { get; set; }
        public string Segment { get; set; } = "";
        public double ChurnProbability { get; set; }
    }

    public class CustomerBehaviorData
    {
        public string UserId { get; set; } = "";
        public List<BehaviorPattern> Patterns { get; set; } = new();
        public List<PreferenceData> Preferences { get; set; } = new();
        public CustomerJourney Journey { get; set; } = new();
        public EngagementMetrics Engagement { get; set; } = new();
    }

    public class BehaviorPattern
    {
        public string PatternType { get; set; } = ""; // browsing, purchasing, seasonal, etc.
        public string Description { get; set; } = "";
        public double Frequency { get; set; }
        public decimal Value { get; set; }
        public List<string> AssociatedProducts { get; set; } = new();
        public List<string> Triggers { get; set; } = new();
    }

    public class PreferenceData
    {
        public string Category { get; set; } = "";
        public List<string> PreferredBrands { get; set; } = new();
        public List<string> PreferredCategories { get; set; } = new();
        public decimal PreferredPriceRange { get; set; }
        public List<string> PreferredChannels { get; set; } = new();
        public double ConfidenceScore { get; set; }
    }

    public class CustomerJourney
    {
        public List<JourneyStage> Stages { get; set; } = new();
        public List<Touchpoint> Touchpoints { get; set; } = new();
        public TimeSpan AverageJourneyDuration { get; set; }
        public string CurrentStage { get; set; } = "";
        public double ConversionProbability { get; set; }
    }

    public class JourneyStage
    {
        public string Id { get; set; } = "";
        public string Name { get; set; } = "";
        public DateTime EnteredAt { get; set; }
        public DateTime? ExitedAt { get; set; }
        public TimeSpan? Duration { get; set; }
        public List<string> Actions { get; set; } = new();
        public bool IsCompleted { get; set; }
    }

    public class Touchpoint
    {
        public string Channel { get; set; } = "";
        public string Action { get; set; } = "";
        public DateTime Timestamp { get; set; }
        public string? Content { get; set; }
        public decimal? Value { get; set; }
        public Dictionary<string, object> Metadata { get; set; } = new();
    }

    public class EngagementMetrics
    {
        public double EmailEngagementRate { get; set; }
        public double WebsiteEngagementRate { get; set; }
        public double SocialEngagementRate { get; set; }
        public TimeSpan AverageSessionDuration { get; set; }
        public int PageViewsPerSession { get; set; }
        public double BounceRate { get; set; }
        public DateTime LastActivityDate { get; set; }
        public int DaysSinceLastActivity { get; set; }
    }

    public class CohortAnalysisData
    {
        public List<CohortGroup> CohortGroups { get; set; } = new();
        public CohortMetrics OverallMetrics { get; set; } = new();
        public List<RetentionHeatmap> RetentionHeatmaps { get; set; } = new();
    }

    public class CohortGroup
    {
        public string Id { get; set; } = "";
        public string Name { get; set; } = "";
        public DateTime PeriodStart { get; set; }
        public DateTime PeriodEnd { get; set; }
        public int InitialCustomers { get; set; }
        public List<RetentionPeriod> RetentionPeriods { get; set; } = new();
        public decimal TotalRevenue { get; set; }
        public decimal RevenuePerCustomer { get; set; }
    }

    public class RetentionPeriod
    {
        public int Period { get; set; } // 0, 1, 2, 3... (months/weeks after initial)
        public int ActiveCustomers { get; set; }
        public double RetentionRate { get; set; }
        public decimal Revenue { get; set; }
        public decimal RevenuePerCustomer { get; set; }
    }

    public class CohortMetrics
    {
        public double AverageRetentionRate { get; set; }
        public string BestPerformingCohort { get; set; } = "";
        public string WorstPerformingCohort { get; set; } = "";
        public List<RetentionTrend> Trends { get; set; } = new();
    }

    public class RetentionTrend
    {
        public int Period { get; set; }
        public double AverageRetentionRate { get; set; }
        public double ChangeFromPrevious { get; set; }
        public string Trend { get; set; } = ""; // improving, declining, stable
    }

    public class RetentionHeatmap
    {
        public string CohortId { get; set; } = "";
        public string CohortName { get; set; } = "";
        public List<List<double>> Data { get; set; } = new(); // 2D array for heatmap
        public List<string> XLabels { get; set; } = new(); // Period labels
        public List<string> YLabels { get; set; } = new(); // Cohort labels
    }

    public class RetentionMetrics
    {
        public string Period { get; set; } = ""; // Month 1, Month 2, etc.
        public double RetentionRate { get; set; }
        public double ChurnRate { get; set; }
        public int CustomersRetained { get; set; }
        public int CustomersLost { get; set; }
        public decimal RevenueRetained { get; set; }
        public decimal RevenueLost { get; set; }
    }

    public class CustomerAnalyticsFilter
    {
        public DateTime StartDate { get; set; } = DateTime.UtcNow.AddDays(-90);
        public DateTime EndDate { get; set; } = DateTime.UtcNow;
        public List<string> Segments { get; set; } = new();
        public decimal? MinLifetimeValue { get; set; }
        public decimal? MaxLifetimeValue { get; set; }
        public int? MinOrders { get; set; }
        public int? MaxOrders { get; set; }
        public string? CohortPeriod { get; set; } // monthly, weekly
        public bool IncludeChurned { get; set; } = true;
        public List<string> AcquisitionChannels { get; set; } = new();
    }

    public class CustomerInsight
    {
        public string Type { get; set; } = ""; // behavior, value, risk, opportunity
        public string Title { get; set; } = "";
        public string Description { get; set; } = "";
        public string Priority { get; set; } = "medium";
        public List<string> AffectedCustomers { get; set; } = new();
        public decimal PotentialImpact { get; set; }
        public List<string> RecommendedActions { get; set; } = new();
        public DateTime GeneratedAt { get; set; }
        public double ConfidenceScore { get; set; }
    }

    public class CustomerHealthScore
    {
        public double OverallScore { get; set; } // 0-100
        public string HealthStatus { get; set; } = ""; // healthy, at_risk, critical
        public List<HealthFactor> Factors { get; set; } = new();
        public List<HealthTrend> Trends { get; set; } = new();
        public List<string> RiskIndicators { get; set; } = new();
        public List<string> OpportunityIndicators { get; set; } = new();
    }

    public class HealthFactor
    {
        public string Name { get; set; } = "";
        public double Score { get; set; }
        public double Weight { get; set; }
        public string Status { get; set; } = "";
        public string Description { get; set; } = "";
    }

    // Dashboard Support Classes
    public class DashboardStats
    {
        public decimal TodayRevenue { get; set; }
        public int TodayOrders { get; set; }
        public int TodayVisitors { get; set; }
        public int ActiveUsers { get; set; }
    }

    public class WeeklyComparisonStats
    {
        public decimal ThisWeekRevenue { get; set; }
        public decimal LastWeekRevenue { get; set; }
        public int ThisWeekOrders { get; set; }
        public int LastWeekOrders { get; set; }
    }

    public class SystemAlert
    {
        public string Type { get; set; } = "";
        public string Message { get; set; } = "";
        public string Action { get; set; } = "";
        public string Icon { get; set; } = "";
    }

    public class QuickAction
    {
        public string Title { get; set; } = "";
        public string Icon { get; set; } = "";
        public string Url { get; set; } = "";
        public string Color { get; set; } = "";
    }

    public class HealthTrend
    {
        public DateTime Date { get; set; }
        public double Score { get; set; }
        public string Status { get; set; } = "";
    }
}
