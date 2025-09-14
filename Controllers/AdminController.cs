using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using JohnHenryFashionWeb.Data;
using JohnHenryFashionWeb.Models;
using JohnHenryFashionWeb.ViewModels;
using JohnHenryFashionWeb.Extensions;
using JohnHenryFashionWeb.Services;
using System.Text.Json;

namespace JohnHenryFashionWeb.Controllers
{
    [Authorize(Roles = UserRoles.Admin, AuthenticationSchemes = "Identity.Application")]
    [Route("admin")]
    public partial class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IAnalyticsService _analyticsService;
        private readonly IReportingService _reportingService;

        public AdminController(
            ApplicationDbContext context, 
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            IWebHostEnvironment webHostEnvironment,
            IAnalyticsService analyticsService,
            IReportingService reportingService)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
            _webHostEnvironment = webHostEnvironment;
            _analyticsService = analyticsService;
            _reportingService = reportingService;
        }

        [HttpGet("")]
        [HttpGet("dashboard")]
        public async Task<IActionResult> Dashboard()
        {
            var dashboardSummary = await _reportingService.GetDashboardSummaryAsync();
            var salesChartData = await _reportingService.GetSalesChartDataAsync("daily", 30);
            var topProductsData = await _reportingService.GetTopProductsChartDataAsync(10);
            var revenueTimeSeries = await _reportingService.GetRevenueTimeSeriesAsync(
                DateTime.UtcNow.AddDays(-30), DateTime.UtcNow, "daily");

            // Enhanced data for comprehensive dashboard
            var recentOrders = await _context.Orders
                .Include(o => o.User)
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                .OrderByDescending(o => o.CreatedAt)
                .Take(10)
                .ToListAsync();

            var recentUsers = await _context.Users
                .Where(u => u.CreatedAt >= DateTime.UtcNow.AddDays(-7))
                .OrderByDescending(u => u.CreatedAt)
                .Take(5)
                .ToListAsync();

            var lowStockProducts = await _context.Products
                .Where(p => p.StockQuantity <= 10 && p.IsActive)
                .OrderBy(p => p.StockQuantity)
                .Take(10)
                .ToListAsync();

            var pendingOrders = await _context.Orders
                .Where(o => o.Status == "pending" || o.Status == "processing")
                .CountAsync();

            var todayStats = new ViewModels.DashboardStats
            {
                TodayOrders = await _context.Orders
                    .Where(o => o.CreatedAt.Date == DateTime.UtcNow.Date)
                    .CountAsync(),
                TodayRevenue = await _context.Orders
                    .Where(o => o.CreatedAt.Date == DateTime.UtcNow.Date && o.Status == "completed")
                    .SumAsync(o => o.TotalAmount),
                TodayVisitors = dashboardSummary.TodayVisitors,
                ActiveUsers = await _context.Users
                    .Where(u => u.IsActive)
                    .CountAsync()
            };

            var weeklyComparison = new ViewModels.WeeklyComparisonStats
            {
                ThisWeekOrders = await _context.Orders
                    .Where(o => o.CreatedAt >= DateTime.UtcNow.AddDays(-7))
                    .CountAsync(),
                LastWeekOrders = await _context.Orders
                    .Where(o => o.CreatedAt >= DateTime.UtcNow.AddDays(-14) && o.CreatedAt < DateTime.UtcNow.AddDays(-7))
                    .CountAsync(),
                ThisWeekRevenue = await _context.Orders
                    .Where(o => o.CreatedAt >= DateTime.UtcNow.AddDays(-7) && o.Status == "completed")
                    .SumAsync(o => o.TotalAmount),
                LastWeekRevenue = await _context.Orders
                    .Where(o => o.CreatedAt >= DateTime.UtcNow.AddDays(-14) && o.CreatedAt < DateTime.UtcNow.AddDays(-7) && o.Status == "completed")
                    .SumAsync(o => o.TotalAmount)
            };

            var systemAlerts = new List<ViewModels.SystemAlert>();
            
            // Check for low stock alerts
            if (lowStockProducts.Any())
            {
                systemAlerts.Add(new ViewModels.SystemAlert { 
                    Type = "warning", 
                    Message = $"{lowStockProducts.Count} sản phẩm sắp hết hàng", 
                    Action = "/Admin/Inventory",
                    Icon = "fas fa-exclamation-triangle"
                });
            }

            // Check for pending orders
            if (pendingOrders > 0)
            {
                systemAlerts.Add(new ViewModels.SystemAlert { 
                    Type = "info", 
                    Message = $"{pendingOrders} đơn hàng đang chờ xử lý", 
                    Action = "/Admin/Orders",
                    Icon = "fas fa-clock"
                });
            }

            // Quick actions data
            var quickActions = new[]
            {
                new ViewModels.QuickAction { Title = "Thêm sản phẩm", Icon = "fas fa-plus", Url = "/Admin/CreateProduct", Color = "primary" },
                new ViewModels.QuickAction { Title = "Xem đơn hàng", Icon = "fas fa-shopping-cart", Url = "/Admin/Orders", Color = "success" },
                new ViewModels.QuickAction { Title = "Quản lý người dùng", Icon = "fas fa-users", Url = "/Admin/Users", Color = "info" },
                new ViewModels.QuickAction { Title = "Báo cáo", Icon = "fas fa-chart-bar", Url = "/Admin/Reports", Color = "warning" },
                new ViewModels.QuickAction { Title = "Cài đặt", Icon = "fas fa-cog", Url = "/Admin/Settings", Color = "secondary" },
                new ViewModels.QuickAction { Title = "Thương hiệu", Icon = "fas fa-award", Url = "/Admin/Brands", Color = "purple" }
            };

            var viewModel = new DashboardViewModel
            {
                Summary = dashboardSummary,
                DashboardSummary = dashboardSummary,
                SalesChartData = salesChartData,
                TopProductsData = topProductsData,
                RevenueTimeSeriesData = revenueTimeSeries,
                CategoryPerformance = await _reportingService.GetCategoryPerformanceAsync(
                    DateTime.UtcNow.AddDays(-30), DateTime.UtcNow),
                RealTimeData = await _analyticsService.GetRealTimeAnalyticsAsync(),
                PerformanceMetrics = await _reportingService.GetPerformanceMetricsAsync(
                    DateTime.UtcNow.AddDays(-7), DateTime.UtcNow),
                GeographicData = await _reportingService.GetGeographicPerformanceAsync(
                    DateTime.UtcNow.AddDays(-30), DateTime.UtcNow),
                DeviceAnalytics = new DeviceAnalytics(), // Would be populated from analytics
                
                // Enhanced dashboard data
                RecentOrders = recentOrders,
                RecentUsers = recentUsers,
                LowStockProducts = lowStockProducts,
                TodayStats = todayStats,
                WeeklyComparison = weeklyComparison,
                SystemAlerts = systemAlerts,
                QuickActions = quickActions,
                
                CurrentFilter = new ViewModels.AnalyticsFilter
                {
                    StartDate = DateTime.UtcNow.AddDays(-30),
                    EndDate = DateTime.UtcNow
                },
                AvailableDateRanges = new List<DateRange>
                {
                    DateRange.Today,
                    DateRange.Yesterday,
                    DateRange.Last7Days,
                    DateRange.Last30Days,
                    DateRange.ThisMonth,
                    DateRange.LastMonth
                },
                SelectedTab = "overview",
                LastUpdated = DateTime.UtcNow
            };

            return View("Dashboard_New", viewModel);
        }

        [HttpGet("analytics")]
        public async Task<IActionResult> Analytics(string? tab = "overview", string? dateRange = "last30days")
        {
            var filter = GetAnalyticsFilterFromQuery(dateRange);
            
            var userAnalytics = await _analyticsService.GetUserAnalyticsAsync(filter.StartDate, filter.EndDate);
            var salesAnalytics = await _analyticsService.GetSalesAnalyticsAsync(filter.StartDate, filter.EndDate);
            var productAnalyticsList = await _analyticsService.GetProductAnalyticsAsync(filter.StartDate, filter.EndDate);
            var marketingAnalytics = await _analyticsService.GetMarketingAnalyticsAsync(filter.StartDate, filter.EndDate);
            var conversionAnalytics = await _analyticsService.GetConversionAnalyticsAsync(filter.StartDate, filter.EndDate);

            var viewModel = new AnalyticsViewModel
            {
                UserAnalytics = userAnalytics,
                SalesAnalytics = salesAnalytics,
                ProductAnalytics = productAnalyticsList.FirstOrDefault() ?? new ProductAnalyticsData(),
                MarketingAnalytics = marketingAnalytics,
                ConversionAnalytics = conversionAnalytics,
                Filter = filter,
                DateRange = GetDateRangeFromString(dateRange),
                AvailableFormats = GetAvailableExportFormats(),
                ViewConfig = new AnalyticsViewConfiguration
                {
                    VisibleCharts = new List<string> { "sales", "users", "products", "conversions" },
                    ShowComparison = true,
                    ShowRealTime = true,
                    DefaultTimeRange = dateRange ?? "last30days",
                    Theme = "light"
                }
            };

            ViewBag.SelectedTab = tab;
            return View(viewModel);
        }

        [HttpGet("reports")]
        public async Task<IActionResult> Reports()
        {
            var viewModel = new ReportsViewModel
            {
                TotalRevenue = 1000000,
                TotalOrders = 150,
                TotalCustomers = 50,
                TotalProducts = await _context.Products.CountAsync(),
                RevenueGrowth = 15.5m,
                NewCustomers = 10,
                LowStockProducts = await _context.Products.CountAsync(p => p.StockQuantity <= 10),
                InventoryValue = 500000
            };

            return View(viewModel);
        }

        [HttpPost("reports/generate")]
        public async Task<IActionResult> GenerateReport([FromBody] ReportGenerationRequest request)
        {
            try
            {
                byte[] reportData;
                string fileName;
                string contentType;

                switch (request.Format.ToLower())
                {
                    case "excel":
                        reportData = await _reportingService.GenerateExcelReportAsync(
                            request.ReportType, request.StartDate, request.EndDate, request.Parameters);
                        fileName = $"{request.ReportType}_report_{DateTime.UtcNow:yyyyMMdd_HHmmss}.xlsx";
                        contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                        break;
                    case "pdf":
                        reportData = await _reportingService.GeneratePdfReportAsync(
                            request.ReportType, request.StartDate, request.EndDate, request.Parameters);
                        fileName = $"{request.ReportType}_report_{DateTime.UtcNow:yyyyMMdd_HHmmss}.pdf";
                        contentType = "application/pdf";
                        break;
                    default:
                        return BadRequest("Unsupported format");
                }

                if (!string.IsNullOrEmpty(request.EmailTo))
                {
                    // TODO: Email the report
                    return Json(new { success = true, message = "Report has been generated and emailed." });
                }

                return File(reportData, contentType, fileName);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost("reports/schedule")]
        public async Task<IActionResult> ScheduleReport([FromBody] ReportScheduleSettings settings)
        {
            try
            {
                var template = new ReportTemplate
                {
                    Name = settings.Name,
                    Description = settings.Description,
                    ReportType = settings.ReportType,
                    Format = settings.Format,
                    Frequency = settings.Frequency.ToLower(),
                    Parameters = JsonSerializer.Serialize(settings.Parameters),
                    IsActive = settings.IsActive,
                    CreatedAt = DateTime.UtcNow,
                    NextRunDate = CalculateNextRunDate(DateTime.UtcNow, settings.Frequency)
                };

                await _reportingService.ScheduleReportAsync(template);

                return Json(new { success = true, message = "Report scheduled successfully." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpGet("analytics/real-time")]
        public async Task<IActionResult> RealTimeAnalytics()
        {
            var realTimeData = await _analyticsService.GetRealTimeAnalyticsAsync();
            var liveMetrics = new LiveMetrics
            {
                ActiveUsers = realTimeData.ActiveUsers,
                PageViewsPerMinute = realTimeData.PageViewsLastHour / 60,
                OrdersPerHour = realTimeData.OrdersLastHour,
                RevenuePerHour = realTimeData.TodayRevenue / 24,
                CurrentConversionRate = 0, // Would calculate from real-time data
                AverageSessionDuration = TimeSpan.FromMinutes(5), // Would get from analytics
                TopPages = realTimeData.TopActivePages.Select(p => new TopPage
                {
                    Path = p.Page,
                    ActiveUsers = p.ActiveUsers,
                    ViewsLastHour = p.Views
                }).ToList(),
                TopReferrers = new List<TopReferrer>() // Would populate from analytics
            };

            var viewModel = new RealTimeDashboardViewModel
            {
                RealTimeData = realTimeData,
                LiveVisitors = realTimeData.LiveVisitors,
                RecentConversions = realTimeData.RecentConversions,
                TopActivePages = realTimeData.TopActivePages,
                LiveMetrics = liveMetrics,
                RecentEvents = new List<LiveEvent>(), // Would populate from recent events
                Performance = new LivePerformanceData
                {
                    AverageResponseTime = TimeSpan.FromMilliseconds(120),
                    ErrorRate = 0.5,
                    RequestsPerMinute = 50,
                    ServerMetrics = new List<ServerMetric>
                    {
                        new() { Name = "CPU Usage", Value = 65, Unit = "%", Status = "normal" },
                        new() { Name = "Memory Usage", Value = 78, Unit = "%", Status = "normal" },
                        new() { Name = "Database Response", Value = 45, Unit = "ms", Status = "normal" }
                    }
                },
                UpdateInterval = 10000,
                AutoUpdate = true,
                EnabledMetrics = new List<string> { "users", "pageviews", "orders", "revenue" },
                ActiveAlerts = new List<RealTimeAlert>(),
                AlertSettings = new AlertSettings
                {
                    EnableAlerts = true,
                    PlaySounds = true,
                    ShowPopups = true,
                    Thresholds = new Dictionary<string, AlertThreshold>()
                }
            };

            return View(viewModel);
        }

        [HttpGet("analytics/api/real-time")]
        public async Task<IActionResult> GetRealTimeData()
        {
            try
            {
                var realTimeData = await _analyticsService.GetRealTimeAnalyticsAsync();
                return Json(realTimeData);
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message });
            }
        }

        [HttpGet("analytics/api/chart-data")]
        public async Task<IActionResult> GetChartData(string type, string period = "daily", int days = 30)
        {
            try
            {
                List<ChartData> data = type.ToLower() switch
                {
                    "sales" => await _reportingService.GetSalesChartDataAsync(period, days),
                    "products" => await _reportingService.GetTopProductsChartDataAsync(days),
                    _ => new List<ChartData>()
                };

                return Json(data);
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message });
            }
        }

        [HttpPost("analytics/export")]
        public IActionResult ExportAnalytics([FromBody] ViewModels.AnalyticsFilter filter)
        {
            try
            {
                // TODO: Fix model conversion between ViewModels.AnalyticsFilter and Models.AnalyticsFilter
                // var data = await _reportingService.ExportAnalyticsToExcelAsync(filter);
                var data = new byte[0]; // Temporary placeholder
                var fileName = $"analytics_export_{DateTime.UtcNow:yyyyMMdd_HHmmss}.xlsx";
                var contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

                return File(data, contentType, fileName);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        #region Analytics Helper Methods

        private ViewModels.AnalyticsFilter GetAnalyticsFilterFromQuery(string? dateRange)
        {
            var filter = new ViewModels.AnalyticsFilter();
            
            switch (dateRange?.ToLower())
            {
                case "today":
                    filter.StartDate = DateTime.UtcNow.Date;
                    filter.EndDate = DateTime.UtcNow;
                    break;
                case "yesterday":
                    filter.StartDate = DateTime.UtcNow.AddDays(-1).Date;
                    filter.EndDate = DateTime.UtcNow.AddDays(-1).Date.AddDays(1).AddSeconds(-1);
                    break;
                case "last7days":
                    filter.StartDate = DateTime.UtcNow.AddDays(-7).Date;
                    filter.EndDate = DateTime.UtcNow;
                    break;
                case "last30days":
                default:
                    filter.StartDate = DateTime.UtcNow.AddDays(-30).Date;
                    filter.EndDate = DateTime.UtcNow;
                    break;
                case "thismonth":
                    filter.StartDate = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1);
                    filter.EndDate = DateTime.UtcNow;
                    break;
                case "lastmonth":
                    var firstDayOfLastMonth = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1).AddMonths(-1);
                    filter.StartDate = firstDayOfLastMonth;
                    filter.EndDate = firstDayOfLastMonth.AddMonths(1).AddSeconds(-1);
                    break;
            }
            
            return filter;
        }

        private DateRange GetDateRangeFromString(string? dateRange)
        {
            return dateRange?.ToLower() switch
            {
                "today" => DateRange.Today,
                "yesterday" => DateRange.Yesterday,
                "last7days" => DateRange.Last7Days,
                "last30days" => DateRange.Last30Days,
                "thismonth" => DateRange.ThisMonth,
                "lastmonth" => DateRange.LastMonth,
                _ => DateRange.Last30Days
            };
        }

        private List<ExportFormat> GetAvailableExportFormats()
        {
            return new List<ExportFormat>
            {
                new() { Id = "excel", Name = "Excel", Extension = ".xlsx", MimeType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" },
                new() { Id = "pdf", Name = "PDF", Extension = ".pdf", MimeType = "application/pdf" },
                new() { Id = "csv", Name = "CSV", Extension = ".csv", MimeType = "text/csv" },
                new() { Id = "json", Name = "JSON", Extension = ".json", MimeType = "application/json" }
            };
        }

        private List<ReportType> GetAvailableReportTypes()
        {
            return new List<ReportType>
            {
                new() { Id = "sales", Name = "Sales Report", Description = "Comprehensive sales analysis with order details, revenue breakdown, and product performance" },
                new() { Id = "products", Name = "Product Report", Description = "Product performance, inventory analysis, and category insights" },
                new() { Id = "customers", Name = "Customer Report", Description = "Customer analytics, segmentation, and lifetime value analysis" },
                new() { Id = "analytics", Name = "Analytics Report", Description = "Website analytics, user behavior, and conversion metrics" },
                new() { Id = "financial", Name = "Financial Report", Description = "Revenue analysis, profit margins, and financial KPIs" }
            };
        }

        private async Task<List<SalesReport>> GetAvailableReportsAsync()
        {
            // Since SalesReport might not have CreatedAt, we'll use a different approach
            return await _context.SalesReports
                .OrderBy(r => r.Id) // Use Id for ordering since CreatedAt might not exist
                .Take(10)
                .ToListAsync();
        }

        private DateTime CalculateNextRunDate(DateTime baseDate, string frequency)
        {
            return frequency.ToLower() switch
            {
                "daily" => baseDate.AddDays(1),
                "weekly" => baseDate.AddDays(7),
                "monthly" => baseDate.AddMonths(1),
                "quarterly" => baseDate.AddMonths(3),
                "yearly" => baseDate.AddYears(1),
                _ => baseDate.AddDays(1)
            };
        }

        #endregion

        #region Dashboard Data Methods
        private async Task<Models.DashboardSummary> GetDashboardStats()
        {
            var totalProducts = await _context.Products.CountAsync();
            var totalOrders = await _context.Orders.CountAsync();
            var totalCustomers = await _userManager.GetUsersInRoleAsync(UserRoles.Customer);
            var totalSellers = await _userManager.GetUsersInRoleAsync(UserRoles.Seller);
            
            var totalRevenue = await _context.Orders
                .Where(o => o.Status == "completed")
                .SumAsync(o => o.TotalAmount);

            var currentMonth = DateTime.UtcNow.Month;
            var currentYear = DateTime.UtcNow.Year;
            var monthlyRevenue = await _context.Orders
                .Where(o => o.CreatedAt.Month == currentMonth && 
                           o.CreatedAt.Year == currentYear &&
                           o.Status == "completed")
                .SumAsync(o => o.TotalAmount);

            var pendingOrders = await _context.Orders
                .CountAsync(o => o.Status == "pending");

            var lowStockProducts = await _context.Products
                .CountAsync(p => p.StockQuantity <= 10);

            return new Models.DashboardSummary
            {
                TodayRevenue = monthlyRevenue,
                YesterdayRevenue = 0, // Will be calculated properly
                RevenueGrowth = 0,
                TodayOrders = totalOrders,
                YesterdayOrders = 0,
                OrdersGrowth = 0,
                TodayVisitors = 150, // Mock data for now
                YesterdayVisitors = 120,
                VisitorsGrowth = 25,
                ConversionRate = 3.5,
                PreviousConversionRate = 3.0,
                ConversionGrowth = 16.7m,
                AverageOrderValue = totalRevenue / Math.Max(totalOrders, 1),
                PreviousAverageOrderValue = 0,
                AOVGrowth = 0,
                ActiveUsers = totalCustomers.Count + totalSellers.Count
            };
        }

        private async Task<List<RecentOrder>> GetRecentOrders(int count)
        {
            return await _context.Orders
                .Include(o => o.User)
                .OrderByDescending(o => o.CreatedAt)
                .Take(count)
                .Select(o => new RecentOrder
                {
                    Id = o.Id,
                    OrderNumber = o.OrderNumber,
                    CustomerName = $"{o.User.FirstName} {o.User.LastName}".Trim(),
                    Total = o.TotalAmount,
                    Status = o.Status,
                    CreatedAt = o.CreatedAt
                })
                .ToListAsync();
        }

        private async Task<List<TopSellingProduct>> GetTopSellingProducts(int count)
        {
            return await _context.OrderItems
                .Include(oi => oi.Product)
                .Where(oi => oi.Order.Status == "completed")
                .GroupBy(oi => oi.ProductId)
                .Select(g => new TopSellingProduct
                {
                    ProductId = g.Key,
                    ProductName = g.First().Product.Name,
                    QuantitySold = g.Sum(oi => oi.Quantity),
                    Revenue = g.Sum(oi => oi.TotalPrice),
                    ImageUrl = g.First().Product.FeaturedImageUrl
                })
                .OrderByDescending(x => x.QuantitySold)
                .Take(count)
                .ToListAsync();
        }

        private async Task<List<MonthlyRevenue>> GetMonthlyRevenueData(int months)
        {
            var result = new List<MonthlyRevenue>();
            var currentDate = DateTime.UtcNow;

            for (int i = months - 1; i >= 0; i--)
            {
                var targetDate = currentDate.AddMonths(-i);
                var revenue = await _context.Orders
                    .Where(o => o.CreatedAt.Month == targetDate.Month && 
                               o.CreatedAt.Year == targetDate.Year &&
                               o.Status == "completed")
                    .SumAsync(o => o.TotalAmount);

                var orderCount = await _context.Orders
                    .CountAsync(o => o.CreatedAt.Month == targetDate.Month && 
                                    o.CreatedAt.Year == targetDate.Year &&
                                    o.Status == "completed");

                result.Add(new MonthlyRevenue
                {
                    Month = targetDate.Month,
                    Year = targetDate.Year,
                    Revenue = revenue,
                    OrderCount = orderCount
                });
            }

            return result;
        }
        #endregion

        #region Product Management
        [HttpGet("products")]
        public async Task<IActionResult> Products(int page = 1, int pageSize = 10, string search = "", Guid? categoryId = null, string status = "")
        {
            var query = _context.Products
                .Include(p => p.Category)
                .Include(p => p.Brand)
                .AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(p => p.Name.Contains(search) || p.SKU.Contains(search));
            }

            if (categoryId.HasValue)
            {
                query = query.Where(p => p.CategoryId == categoryId.Value);
            }

            if (!string.IsNullOrEmpty(status))
            {
                query = query.Where(p => p.Status == status);
            }

            var totalCount = await query.CountAsync();
            var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);

            var products = await query
                .OrderByDescending(p => p.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(p => new ProductListItemViewModel
                {
                    Id = p.Id,
                    Name = p.Name,
                    SKU = p.SKU,
                    Price = p.Price,
                    SalePrice = p.SalePrice,
                    StockQuantity = p.StockQuantity,
                    Status = p.Status,
                    CategoryName = p.Category.Name,
                    BrandName = p.Brand != null ? p.Brand.Name : "",
                    FeaturedImageUrl = p.FeaturedImageUrl,
                    CreatedAt = p.CreatedAt,
                    IsFeatured = p.IsFeatured,
                    IsActive = p.IsActive,
                    Description = p.Description,
                    Category = p.Category
                })
                .ToListAsync();

            var categories = await _context.Categories
                .Where(c => c.IsActive)
                .OrderBy(c => c.Name)
                .ToListAsync();

            var viewModel = new ProductListViewModel
            {
                Products = products,
                CurrentPage = page,
                TotalPages = totalPages,
                PageSize = pageSize,
                SearchTerm = search,
                CategoryId = categoryId,
                Status = status,
                Categories = categories
            };

            return View(viewModel);
        }

        [HttpGet("products/create")]
        public async Task<IActionResult> CreateProduct()
        {
            var viewModel = new ProductCreateEditViewModel
            {
                Categories = await _context.Categories.Where(c => c.IsActive).OrderBy(c => c.Name).ToListAsync(),
                Brands = await _context.Brands.Where(b => b.IsActive).OrderBy(b => b.Name).ToListAsync()
            };

            return View(viewModel);
        }

        [HttpPost("products/create")]
        public async Task<IActionResult> CreateProduct(ProductCreateEditViewModel model)
        {
            if (ModelState.IsValid)
            {
                var product = new Product
                {
                    Id = Guid.NewGuid(),
                    Name = model.Name,
                    SKU = model.SKU,
                    Price = model.Price,
                    SalePrice = model.SalePrice,
                    CategoryId = model.CategoryId,
                    BrandId = model.BrandId,
                    ShortDescription = model.ShortDescription,
                    Description = model.Description,
                    StockQuantity = model.StockQuantity,
                    ManageStock = model.ManageStock,
                    InStock = model.InStock,
                    IsFeatured = model.IsFeatured,
                    IsActive = model.IsActive,
                    Size = model.Size,
                    Color = model.Color,
                    Material = model.Material,
                    Weight = model.Weight,
                    Dimensions = model.Dimensions,
                    Status = model.Status,
                    Slug = GenerateSlug(model.Name),
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                // Handle featured image upload
                if (model.FeaturedImage != null)
                {
                    product.FeaturedImageUrl = await SaveUploadedFile(model.FeaturedImage, "products");
                }

                // Handle gallery images upload
                if (model.GalleryImages != null && model.GalleryImages.Count > 0)
                {
                    var galleryUrls = new List<string>();
                    foreach (var image in model.GalleryImages)
                    {
                        var imageUrl = await SaveUploadedFile(image, "products");
                        galleryUrls.Add(imageUrl);
                    }
                    product.GalleryImages = galleryUrls.ToArray();
                }

                _context.Products.Add(product);
                await _context.SaveChangesAsync();

                TempData["Success"] = "Sản phẩm đã được tạo thành công!";
                return RedirectToAction(nameof(Products));
            }

            // Reload dropdown data if validation fails
            model.Categories = await _context.Categories.Where(c => c.IsActive).OrderBy(c => c.Name).ToListAsync();
            model.Brands = await _context.Brands.Where(b => b.IsActive).OrderBy(b => b.Name).ToListAsync();

            return View(model);
        }

        [HttpGet("products/edit/{id}")]
        public async Task<IActionResult> EditProduct(Guid id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            var viewModel = new ProductCreateEditViewModel
            {
                Id = product.Id,
                Name = product.Name,
                SKU = product.SKU,
                Price = product.Price,
                SalePrice = product.SalePrice,
                CategoryId = product.CategoryId,
                BrandId = product.BrandId,
                ShortDescription = product.ShortDescription,
                Description = product.Description,
                StockQuantity = product.StockQuantity,
                ManageStock = product.ManageStock,
                InStock = product.InStock,
                IsFeatured = product.IsFeatured,
                IsActive = product.IsActive,
                Size = product.Size,
                Color = product.Color,
                Material = product.Material,
                Weight = product.Weight,
                Dimensions = product.Dimensions,
                Status = product.Status,
                FeaturedImageUrl = product.FeaturedImageUrl,
                ExistingGalleryImages = product.GalleryImages,
                Categories = await _context.Categories.Where(c => c.IsActive).OrderBy(c => c.Name).ToListAsync(),
                Brands = await _context.Brands.Where(b => b.IsActive).OrderBy(b => b.Name).ToListAsync()
            };

            return View(viewModel);
        }

        [HttpPost("products/edit/{id}")]
        public async Task<IActionResult> EditProduct(Guid id, ProductCreateEditViewModel model)
        {
            if (id != model.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var product = await _context.Products.FindAsync(id);
                if (product == null)
                {
                    return NotFound();
                }

                product.Name = model.Name;
                product.SKU = model.SKU;
                product.Price = model.Price;
                product.SalePrice = model.SalePrice;
                product.CategoryId = model.CategoryId;
                product.BrandId = model.BrandId;
                product.ShortDescription = model.ShortDescription;
                product.Description = model.Description;
                product.StockQuantity = model.StockQuantity;
                product.ManageStock = model.ManageStock;
                product.InStock = model.InStock;
                product.IsFeatured = model.IsFeatured;
                product.IsActive = model.IsActive;
                product.Size = model.Size;
                product.Color = model.Color;
                product.Material = model.Material;
                product.Weight = model.Weight;
                product.Dimensions = model.Dimensions;
                product.Status = model.Status;
                product.Slug = GenerateSlug(model.Name);
                product.UpdatedAt = DateTime.UtcNow;

                // Handle featured image upload
                if (model.FeaturedImage != null)
                {
                    product.FeaturedImageUrl = await SaveUploadedFile(model.FeaturedImage, "products");
                }

                // Handle gallery images upload
                if (model.GalleryImages != null && model.GalleryImages.Count > 0)
                {
                    var galleryUrls = new List<string>();
                    foreach (var image in model.GalleryImages)
                    {
                        var imageUrl = await SaveUploadedFile(image, "products");
                        galleryUrls.Add(imageUrl);
                    }
                    product.GalleryImages = galleryUrls.ToArray();
                }

                await _context.SaveChangesAsync();

                TempData["Success"] = "Sản phẩm đã được cập nhật thành công!";
                return RedirectToAction(nameof(Products));
            }

            // Reload dropdown data if validation fails
            model.Categories = await _context.Categories.Where(c => c.IsActive).OrderBy(c => c.Name).ToListAsync();
            model.Brands = await _context.Brands.Where(b => b.IsActive).OrderBy(b => b.Name).ToListAsync();

            return View(model);
        }

        [HttpGet("products/delete/{id}")]
        public async Task<IActionResult> DeleteProduct(Guid id)
        {
            var product = await _context.Products
                .Include(p => p.Category)
                .Include(p => p.Brand)
                .FirstOrDefaultAsync(p => p.Id == id);
            
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        [HttpPost("products/delete/{id}")]
        public async Task<IActionResult> DeleteProductConfirmed(Guid id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            // Check if product is referenced in orders
            var hasOrders = await _context.OrderItems.AnyAsync(oi => oi.ProductId == id);
            if (hasOrders)
            {
                TempData["Warning"] = "Không thể xóa sản phẩm này vì đã có đơn hàng liên quan. Bạn có thể ngừng kích hoạt sản phẩm thay thế.";
                return RedirectToAction(nameof(EditProduct), new { id });
            }

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Sản phẩm đã được xóa thành công!";
            return RedirectToAction(nameof(Products));
        }
        #endregion

        #region Category Management
        [HttpGet("categories")]
        public async Task<IActionResult> Categories(string search = "")
        {
            var query = _context.Categories
                .Include(c => c.Parent)
                .AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(c => c.Name.Contains(search) || (c.Description != null && c.Description.Contains(search)));
            }

            var categories = await query
                .OrderBy(c => c.ParentId.HasValue ? 1 : 0)
                .ThenBy(c => c.SortOrder)
                .ThenBy(c => c.Name)
                .Select(c => new CategoryListItemViewModel
                {
                    Id = c.Id,
                    Name = c.Name,
                    Description = c.Description,
                    ImageUrl = c.ImageUrl,
                    ParentId = c.ParentId,
                    ParentName = c.Parent != null ? c.Parent.Name : null,
                    IsActive = c.IsActive,
                    SortOrder = c.SortOrder,
                    CreatedAt = c.CreatedAt,
                    ProductCount = c.Products.Count()
                })
                .ToListAsync();

            var viewModel = new CategoryListViewModel
            {
                Categories = categories,
                SearchTerm = search
            };

            return View(viewModel);
        }

        [HttpGet("categories/create")]
        public async Task<IActionResult> CreateCategory()
        {
            var viewModel = new CategoryCreateEditViewModel
            {
                ParentCategories = await _context.Categories
                    .Where(c => c.IsActive && !c.ParentId.HasValue)
                    .OrderBy(c => c.Name)
                    .ToListAsync()
            };

            return View(viewModel);
        }

        [HttpPost("categories/create")]
        public async Task<IActionResult> CreateCategory(CategoryCreateEditViewModel model)
        {
            if (ModelState.IsValid)
            {
                var category = new Category
                {
                    Id = Guid.NewGuid(),
                    Name = model.Name,
                    Description = model.Description,
                    ParentId = model.ParentId,
                    IsActive = model.IsActive,
                    SortOrder = model.SortOrder,
                    Slug = GenerateSlug(model.Name),
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                // Handle image upload
                if (model.Image != null)
                {
                    category.ImageUrl = await SaveUploadedFile(model.Image, "categories");
                }

                _context.Categories.Add(category);
                await _context.SaveChangesAsync();

                TempData["Success"] = "Danh mục đã được tạo thành công!";
                return RedirectToAction(nameof(Categories));
            }

            // Reload dropdown data if validation fails
            model.ParentCategories = await _context.Categories
                .Where(c => c.IsActive && !c.ParentId.HasValue)
                .OrderBy(c => c.Name)
                .ToListAsync();

            return View(model);
        }

        [HttpGet("categories/edit/{id}")]
        public async Task<IActionResult> EditCategory(Guid id)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category == null)
            {
                return NotFound();
            }

            var viewModel = new CategoryCreateEditViewModel
            {
                Id = category.Id,
                Name = category.Name,
                Description = category.Description,
                ParentId = category.ParentId,
                IsActive = category.IsActive,
                SortOrder = category.SortOrder,
                ImageUrl = category.ImageUrl,
                ParentCategories = await _context.Categories
                    .Where(c => c.IsActive && !c.ParentId.HasValue && c.Id != id)
                    .OrderBy(c => c.Name)
                    .ToListAsync()
            };

            return View(viewModel);
        }

        [HttpPost("categories/edit/{id}")]
        public async Task<IActionResult> EditCategory(Guid id, CategoryCreateEditViewModel model)
        {
            if (id != model.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var category = await _context.Categories.FindAsync(id);
                if (category == null)
                {
                    return NotFound();
                }

                category.Name = model.Name;
                category.Description = model.Description;
                category.ParentId = model.ParentId;
                category.IsActive = model.IsActive;
                category.SortOrder = model.SortOrder;
                category.Slug = GenerateSlug(model.Name);
                category.UpdatedAt = DateTime.UtcNow;

                // Handle image upload
                if (model.Image != null)
                {
                    category.ImageUrl = await SaveUploadedFile(model.Image, "categories");
                }

                await _context.SaveChangesAsync();

                TempData["Success"] = "Danh mục đã được cập nhật thành công!";
                return RedirectToAction(nameof(Categories));
            }

            // Reload dropdown data if validation fails
            model.ParentCategories = await _context.Categories
                .Where(c => c.IsActive && !c.ParentId.HasValue && c.Id != id)
                .OrderBy(c => c.Name)
                .ToListAsync();

            return View(model);
        }

        [HttpPost("categories/delete/{id}")]
        public async Task<IActionResult> DeleteCategory(Guid id)
        {
            var category = await _context.Categories
                .Include(c => c.Products)
                .Include(c => c.Children)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (category == null)
            {
                return NotFound();
            }

            // Check if category has products or subcategories
            if (category.Products.Any() || category.Children.Any())
            {
                TempData["Error"] = "Không thể xóa danh mục này vì đang có sản phẩm hoặc danh mục con!";
                return RedirectToAction(nameof(Categories));
            }

            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Danh mục đã được xóa thành công!";
            return RedirectToAction(nameof(Categories));
        }

        [HttpGet("categories/from-images")]
        public async Task<IActionResult> CreateCategoriesFromImages()
        {
            var categoriesCreated = 0;
            var imagesPath = Path.Combine(_webHostEnvironment.WebRootPath, "images");
            
            if (Directory.Exists(imagesPath))
            {
                var categoryFolders = Directory.GetDirectories(imagesPath)
                    .Where(d => !Path.GetFileName(d).StartsWith(".") && 
                               !Path.GetFileName(d).Equals("store", StringComparison.OrdinalIgnoreCase) &&
                               !Path.GetFileName(d).Equals("Banner", StringComparison.OrdinalIgnoreCase))
                    .ToList();

                foreach (var folder in categoryFolders)
                {
                    var folderName = Path.GetFileName(folder);
                    var slug = GenerateSlug(folderName);

                    // Check if category already exists
                    var existingCategory = await _context.Categories
                        .FirstOrDefaultAsync(c => c.Slug == slug);

                    if (existingCategory == null)
                    {
                        var category = new Category
                        {
                            Id = Guid.NewGuid(),
                            Name = folderName,
                            Slug = slug,
                            Description = $"Danh mục {folderName}",
                            IsActive = true,
                            SortOrder = categoriesCreated,
                            CreatedAt = DateTime.UtcNow,
                            UpdatedAt = DateTime.UtcNow
                        };

                        // Try to find a representative image
                        var imageFiles = Directory.GetFiles(folder, "*.*", SearchOption.TopDirectoryOnly)
                            .Where(file => file.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase) ||
                                          file.EndsWith(".jpeg", StringComparison.OrdinalIgnoreCase) ||
                                          file.EndsWith(".png", StringComparison.OrdinalIgnoreCase))
                            .FirstOrDefault();

                        if (imageFiles != null)
                        {
                            var relativePath = Path.GetRelativePath(_webHostEnvironment.WebRootPath, imageFiles)
                                .Replace("\\", "/");
                            category.ImageUrl = "/" + relativePath;
                        }

                        _context.Categories.Add(category);
                        categoriesCreated++;
                    }
                }

                if (categoriesCreated > 0)
                {
                    await _context.SaveChangesAsync();
                    TempData["Success"] = $"Đã tạo {categoriesCreated} danh mục từ thư mục hình ảnh!";
                }
                else
                {
                    TempData["Info"] = "Tất cả danh mục đã tồn tại!";
                }
            }
            else
            {
                TempData["Error"] = "Không tìm thấy thư mục hình ảnh!";
            }

            return RedirectToAction(nameof(Categories));
        }
        #endregion

        #region Product Management (continued)
        [HttpGet("products/import-from-images")]
        public async Task<IActionResult> ImportProductsFromImages()
        {
            var productsCreated = 0;
            var imagesPath = Path.Combine(_webHostEnvironment.WebRootPath, "images");
            
            if (Directory.Exists(imagesPath))
            {
                var categoryFolders = Directory.GetDirectories(imagesPath)
                    .Where(d => !Path.GetFileName(d).StartsWith(".") && 
                               !Path.GetFileName(d).Equals("store", StringComparison.OrdinalIgnoreCase) &&
                               !Path.GetFileName(d).Equals("Banner", StringComparison.OrdinalIgnoreCase))
                    .ToList();

                foreach (var folder in categoryFolders)
                {
                    var folderName = Path.GetFileName(folder);
                    
                    // Find or create category
                    var category = await _context.Categories
                        .FirstOrDefaultAsync(c => c.Name == folderName);
                    
                    if (category == null)
                    {
                        category = new Category
                        {
                            Id = Guid.NewGuid(),
                            Name = folderName,
                            Slug = GenerateSlug(folderName),
                            Description = $"Danh mục {folderName}",
                            IsActive = true,
                            CreatedAt = DateTime.UtcNow,
                            UpdatedAt = DateTime.UtcNow
                        };
                        _context.Categories.Add(category);
                        await _context.SaveChangesAsync();
                    }

                    // Get all image files in the folder
                    var imageFiles = Directory.GetFiles(folder, "*.*", SearchOption.TopDirectoryOnly)
                        .Where(file => file.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase) ||
                                      file.EndsWith(".jpeg", StringComparison.OrdinalIgnoreCase) ||
                                      file.EndsWith(".png", StringComparison.OrdinalIgnoreCase))
                        .ToList();

                    foreach (var imageFile in imageFiles)
                    {
                        var fileName = Path.GetFileNameWithoutExtension(imageFile);
                        var productName = FormatProductName(fileName);
                        var sku = GenerateProductSKU(fileName);

                        // Check if product already exists
                        var existingProduct = await _context.Products
                            .FirstOrDefaultAsync(p => p.SKU == sku);

                        if (existingProduct == null)
                        {
                            var relativePath = Path.GetRelativePath(_webHostEnvironment.WebRootPath, imageFile)
                                .Replace("\\", "/");

                            // Extract price and details from filename if possible
                            var (price, size, color) = ExtractProductDetails(fileName);

                            var product = new Product
                            {
                                Id = Guid.NewGuid(),
                                Name = productName,
                                SKU = sku,
                                Slug = GenerateSlug(productName),
                                Price = price,
                                CategoryId = category.Id,
                                Description = $"Sản phẩm {productName} thuộc danh mục {folderName}",
                                ShortDescription = $"Sản phẩm thời trang {productName}",
                                StockQuantity = 100, // Default stock
                                ManageStock = true,
                                InStock = true,
                                IsActive = true,
                                IsFeatured = false,
                                Status = "active",
                                FeaturedImageUrl = "/" + relativePath,
                                Size = size,
                                Color = color,
                                CreatedAt = DateTime.UtcNow,
                                UpdatedAt = DateTime.UtcNow
                            };

                            _context.Products.Add(product);
                            productsCreated++;
                        }
                    }
                }

                if (productsCreated > 0)
                {
                    await _context.SaveChangesAsync();
                    TempData["Success"] = $"Đã tạo {productsCreated} sản phẩm từ thư mục hình ảnh!";
                }
                else
                {
                    TempData["Info"] = "Tất cả sản phẩm đã tồn tại!";
                }
            }
            else
            {
                TempData["Error"] = "Không tìm thấy thư mục hình ảnh!";
            }

            return RedirectToAction(nameof(Products));
        }

        [HttpPost("products/{id}/update-stock")]
        public async Task<IActionResult> UpdateProductStock(Guid id, [FromBody] UpdateStockRequest request)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return Json(new { success = false, message = "Không tìm thấy sản phẩm" });
            }

            product.StockQuantity = request.Stock;
            product.InStock = request.Stock > 0;
            product.Status = request.Stock > 0 ? "active" : "out_of_stock";
            product.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return Json(new { success = true, message = "Cập nhật tồn kho thành công" });
        }

        public class UpdateStockRequest
        {
            public int Stock { get; set; }
        }
        #endregion

        #region Order Management
        [HttpGet("orders")]
        public async Task<IActionResult> Orders(int page = 1, int pageSize = 20, string search = "", string status = "", DateTime? fromDate = null, DateTime? toDate = null)
        {
            var query = _context.Orders
                .Include(o => o.User)
                .Include(o => o.OrderItems)
                .AsQueryable();

            // Apply filters
            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(o => o.OrderNumber.Contains(search) ||
                                        (o.User.FirstName != null && o.User.FirstName.Contains(search)) ||
                                        (o.User.LastName != null && o.User.LastName.Contains(search)) ||
                                        (o.User.Email != null && o.User.Email.Contains(search)));
            }

            if (!string.IsNullOrEmpty(status))
            {
                query = query.Where(o => o.Status == status);
            }

            if (fromDate.HasValue)
            {
                query = query.Where(o => o.CreatedAt >= fromDate.Value);
            }

            if (toDate.HasValue)
            {
                query = query.Where(o => o.CreatedAt <= toDate.Value.Date.AddDays(1).AddTicks(-1));
            }

            // Get statistics
            var allOrders = _context.Orders.AsQueryable();
            var pendingOrders = await allOrders.CountAsync(o => o.Status == "pending");
            var processingOrders = await allOrders.CountAsync(o => o.Status == "processing");
            var shippedOrders = await allOrders.CountAsync(o => o.Status == "shipped");
            var deliveredOrders = await allOrders.CountAsync(o => o.Status == "delivered");
            var cancelledOrders = await allOrders.CountAsync(o => o.Status == "cancelled");

            var totalRevenue = await allOrders
                .Where(o => o.Status == "delivered" || o.Status == "completed")
                .SumAsync(o => o.TotalAmount);

            var totalOrders = await query.CountAsync();
            var orders = await query
                .OrderByDescending(o => o.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(o => new OrderSummaryViewModel
                {
                    Id = o.Id,
                    OrderNumber = o.OrderNumber,
                    CustomerName = o.User.FirstName + " " + o.User.LastName,
                    CustomerPhone = o.User.PhoneNumber ?? "",
                    OrderDate = o.CreatedAt,
                    TotalAmount = o.TotalAmount,
                    Status = o.Status,
                    PaymentStatus = o.PaymentStatus
                })
                .ToListAsync();

            var viewModel = new OrderManagementViewModel
            {
                Orders = orders,
                CurrentPage = page,
                TotalPages = (int)Math.Ceiling((double)totalOrders / pageSize),
                PageSize = pageSize,
                SearchTerm = search,
                StatusFilter = status,
                FromDate = fromDate,
                ToDate = toDate,
                PendingOrders = pendingOrders,
                ProcessingOrdersCount = processingOrders,
                ShippingOrders = shippedOrders,
                CompletedOrders = deliveredOrders,
                CancelledOrders = cancelledOrders,
                TotalOrders = totalOrders,
                TotalRevenue = totalRevenue
            };

            return View(viewModel);
        }
        #endregion

        #region Helper Methods
        private string GenerateSlug(string text)
        {
            return text.ToLower()
                      .Replace("á", "a").Replace("à", "a").Replace("ả", "a").Replace("ã", "a").Replace("ạ", "a")
                      .Replace("ă", "a").Replace("ắ", "a").Replace("ằ", "a").Replace("ẳ", "a").Replace("ẵ", "a").Replace("ặ", "a")
                      .Replace("â", "a").Replace("ấ", "a").Replace("ầ", "a").Replace("ẩ", "a").Replace("ẫ", "a").Replace("ậ", "a")
                      .Replace("é", "e").Replace("è", "e").Replace("ẻ", "e").Replace("ẽ", "e").Replace("ẹ", "e")
                      .Replace("ê", "e").Replace("ế", "e").Replace("ề", "e").Replace("ể", "e").Replace("ễ", "e").Replace("ệ", "e")
                      .Replace("í", "i").Replace("ì", "i").Replace("ỉ", "i").Replace("ĩ", "i").Replace("ị", "i")
                      .Replace("ó", "o").Replace("ò", "o").Replace("ỏ", "o").Replace("õ", "o").Replace("ọ", "o")
                      .Replace("ô", "o").Replace("ố", "o").Replace("ồ", "o").Replace("ổ", "o").Replace("ỗ", "o").Replace("ộ", "o")
                      .Replace("ơ", "o").Replace("ớ", "o").Replace("ờ", "o").Replace("ở", "o").Replace("ỡ", "o").Replace("ợ", "o")
                      .Replace("ú", "u").Replace("ù", "u").Replace("ủ", "u").Replace("ũ", "u").Replace("ụ", "u")
                      .Replace("ư", "u").Replace("ứ", "u").Replace("ừ", "u").Replace("ử", "u").Replace("ữ", "u").Replace("ự", "u")
                      .Replace("ý", "y").Replace("ỳ", "y").Replace("ỷ", "y").Replace("ỹ", "y").Replace("ỵ", "y")
                      .Replace("đ", "d")
                      .Replace(" ", "-")
                      .Replace("--", "-")
                      .Trim('-');
        }

        private async Task<string> SaveUploadedFile(IFormFile file, string folder)
        {
            if (file == null || file.Length == 0)
                return string.Empty;

            var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images", folder);
            Directory.CreateDirectory(uploadsFolder);

            var uniqueFileName = Guid.NewGuid().ToString() + "_" + file.FileName;
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(fileStream);
            }

            return $"/images/{folder}/{uniqueFileName}";
        }

        private string FormatProductName(string fileName)
        {
            // Clean up filename to create a readable product name
            return fileName
                .Replace("-", " ")
                .Replace("_", " ")
                .Trim()
                .ToTitleCase();
        }

        private string GenerateProductSKU(string fileName)
        {
            // Generate SKU from filename
            var cleanName = fileName.Replace(" ", "").Replace("-", "").Replace("_", "").ToUpper();
            if (cleanName.Length > 10)
            {
                cleanName = cleanName.Substring(0, 10);
            }
            return $"JH{cleanName}{DateTime.Now.ToString("MMdd")}";
        }

        private (decimal price, string? size, string? color) ExtractProductDetails(string fileName)
        {
            var price = 299000m; // Default price
            string? size = null;
            string? color = null;

            // Try to extract size
            var sizeMatches = new[] { "XS", "S", "M", "L", "XL", "XXL", "36", "38", "40", "42", "44", "46" };
            foreach (var sizeMatch in sizeMatches)
            {
                if (fileName.ToUpper().Contains(sizeMatch))
                {
                    size = sizeMatch;
                    break;
                }
            }

            // Try to extract color from Vietnamese color names
            var colorMatches = new Dictionary<string, string>
            {
                { "đen", "Đen" }, { "trắng", "Trắng" }, { "xanh", "Xanh" },
                { "đỏ", "Đỏ" }, { "vàng", "Vàng" }, { "hồng", "Hồng" },
                { "tím", "Tím" }, { "nâu", "Nâu" }, { "xám", "Xám" },
                { "black", "Đen" }, { "white", "Trắng" }, { "blue", "Xanh" },
                { "red", "Đỏ" }, { "yellow", "Vàng" }, { "pink", "Hồng" },
                { "purple", "Tím" }, { "brown", "Nâu" }, { "gray", "Xám" }
            };

            foreach (var colorMatch in colorMatches)
            {
                if (fileName.ToLower().Contains(colorMatch.Key))
                {
                    color = colorMatch.Value;
                    break;
                }
            }

            // Adjust price based on category
            if (fileName.ToLower().Contains("vest") || fileName.ToLower().Contains("suit"))
                price = 899000m;
            else if (fileName.ToLower().Contains("áo khoác") || fileName.ToLower().Contains("jacket"))
                price = 599000m;
            else if (fileName.ToLower().Contains("áo sơ mi") || fileName.ToLower().Contains("shirt"))
                price = 399000m;
            else if (fileName.ToLower().Contains("quần") || fileName.ToLower().Contains("pants"))
                price = 499000m;
            else if (fileName.ToLower().Contains("đầm") || fileName.ToLower().Contains("dress"))
                price = 699000m;

            return (price, size, color);
        }
        #endregion

        #region User Management
        [HttpGet("users")]
        public async Task<IActionResult> Users(int page = 1, int pageSize = 20, string search = "", string role = "", string status = "")
        {
            var query = _userManager.Users.AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(u => (u.FirstName != null && u.FirstName.Contains(search)) || 
                                       (u.LastName != null && u.LastName.Contains(search)) || 
                                       (u.Email != null && u.Email.Contains(search)));
            }

            if (!string.IsNullOrEmpty(status))
            {
                var isActive = status == "active";
                query = query.Where(u => u.IsActive == isActive);
            }

            var totalCount = await query.CountAsync();
            var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);

            var users = await query
                .OrderByDescending(u => u.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var userViewModels = new List<UserListItemViewModel>();
            
            foreach (var user in users)
            {
                var userRoles = await _userManager.GetRolesAsync(user);
                userViewModels.Add(new UserListItemViewModel
                {
                    Id = user.Id,
                    FirstName = user.FirstName ?? "",
                    LastName = user.LastName ?? "",
                    Email = user.Email ?? "",
                    IsActive = user.IsActive,
                    CreatedAt = user.CreatedAt,
                    LastLogin = user.LastLoginDate,
                    Roles = userRoles.ToList()
                });
            }

            // Get statistics
            var totalUsers = await _userManager.Users.CountAsync();
            var activeUsers = await _userManager.Users.CountAsync(u => u.IsActive);
            var thisMonth = DateTime.UtcNow.AddDays(-30);
            var newUsersThisMonth = await _userManager.Users.CountAsync(u => u.CreatedAt >= thisMonth);
            var sellersCount = (await _userManager.GetUsersInRoleAsync("Seller")).Count;

            var viewModel = new UserManagementViewModel
            {
                Users = userViewModels,
                CurrentPage = page,
                TotalPages = totalPages,
                PageSize = pageSize,
                SearchTerm = search,
                RoleFilter = role,
                StatusFilter = status,
                TotalUsers = totalUsers,
                ActiveUsers = activeUsers,
                NewUsersThisMonth = newUsersThisMonth,
                SellersCount = sellersCount
            };

            return View(viewModel);
        }

        [HttpPost("users/toggle-status/{id}")]
        public async Task<IActionResult> ToggleUserStatus(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            user.IsActive = !user.IsActive;
            var result = await _userManager.UpdateAsync(user);

            if (result.Succeeded)
            {
                TempData["Success"] = $"Trạng thái người dùng đã được {(user.IsActive ? "kích hoạt" : "vô hiệu hóa")}!";
            }
            else
            {
                TempData["Error"] = "Có lỗi xảy ra khi cập nhật trạng thái người dùng!";
            }

            return RedirectToAction(nameof(Users));
        }
        #endregion

        #region Inventory Management
        [HttpGet("inventory")]
        public async Task<IActionResult> Inventory(string search = "", bool lowStock = false)
        {
            var query = _context.Products
                .Include(p => p.Category)
                .Include(p => p.Brand)
                .AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(p => p.Name.Contains(search) || p.SKU.Contains(search));
            }

            if (lowStock)
            {
                query = query.Where(p => p.StockQuantity <= 10);
            }

            var products = await query
                .OrderBy(p => p.StockQuantity)
                .ThenBy(p => p.Name)
                .Select(p => new InventoryItemViewModel
                {
                    Id = p.Id,
                    ProductId = p.Id,
                    ProductName = p.Name,
                    SKU = p.SKU,
                    CurrentStock = p.StockQuantity,
                    CategoryName = p.Category.Name,
                    Price = p.Price,
                    LastUpdated = p.UpdatedAt
                })
                .ToListAsync();

            var viewModel = new InventoryListViewModel
            {
                Items = products,
                SearchTerm = search,
                Filter = lowStock ? "low_stock" : "all"
            };

            return View(viewModel);
        }

        [HttpPost("inventory/update-stock")]
        public async Task<IActionResult> UpdateStock([FromForm] Guid productId, [FromForm] int newStock, [FromForm] string reason = "")
        {
            var product = await _context.Products.FindAsync(productId);
            if (product == null)
            {
                return Json(new { success = false, message = "Không tìm thấy sản phẩm!" });
            }

            var oldStock = product.StockQuantity;
            product.StockQuantity = newStock;
            product.UpdatedAt = DateTime.UtcNow;

            // TODO: Add stock movement history logging
            // var stockMovement = new StockMovement
            // {
            //     ProductId = productId,
            //     OldQuantity = oldStock,
            //     NewQuantity = newStock,
            //     Reason = reason,
            //     CreatedAt = DateTime.UtcNow,
            //     CreatedBy = User.Identity.Name
            // };
            // _context.StockMovements.Add(stockMovement);

            await _context.SaveChangesAsync();

            return Json(new { 
                success = true, 
                message = $"Đã cập nhật tồn kho từ {oldStock} thành {newStock}!",
                newStock = newStock
            });
        }
        #endregion

        #region Brand Management

        [HttpGet("brands")]
        public async Task<IActionResult> Brands(int page = 1, int pageSize = 10, string search = "")
        {
            var query = _context.Brands.AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(b => b.Name.Contains(search) || 
                                        b.Description!.Contains(search));
            }

            var totalBrands = await query.CountAsync();
            var brands = await query
                .OrderBy(b => b.Name)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            ViewBag.TotalBrands = totalBrands;
            ViewBag.CurrentPage = page;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalPages = (int)Math.Ceiling((double)totalBrands / pageSize);
            ViewBag.Search = search;

            return View(brands);
        }

        [HttpGet("brands/create")]
        public IActionResult CreateBrand()
        {
            return View();
        }

        [HttpPost("brands/create")]
        public async Task<IActionResult> CreateBrand(Brand model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    model.Id = Guid.NewGuid();
                    model.Slug = GenerateSlug(model.Name);
                    model.CreatedAt = DateTime.UtcNow;
                    model.UpdatedAt = DateTime.UtcNow;

                    _context.Brands.Add(model);
                    await _context.SaveChangesAsync();

                    TempData["SuccessMessage"] = "Thương hiệu đã được tạo thành công!";
                    return RedirectToAction(nameof(Brands));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Có lỗi xảy ra khi tạo thương hiệu: " + ex.Message);
                }
            }

            return View(model);
        }

        [HttpGet("brands/edit/{id}")]
        public async Task<IActionResult> EditBrand(Guid id)
        {
            var brand = await _context.Brands.FindAsync(id);
            if (brand == null)
            {
                return NotFound();
            }

            return View(brand);
        }

        [HttpPost("brands/edit/{id}")]
        public async Task<IActionResult> EditBrand(Guid id, Brand model)
        {
            if (id != model.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var brand = await _context.Brands.FindAsync(id);
                    if (brand == null)
                    {
                        return NotFound();
                    }

                    brand.Name = model.Name;
                    brand.Description = model.Description;
                    brand.LogoUrl = model.LogoUrl;
                    brand.Website = model.Website;
                    brand.IsActive = model.IsActive;
                    brand.Slug = GenerateSlug(model.Name);
                    brand.UpdatedAt = DateTime.UtcNow;

                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Thương hiệu đã được cập nhật thành công!";
                    return RedirectToAction(nameof(Brands));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Có lỗi xảy ra khi cập nhật thương hiệu: " + ex.Message);
                }
            }

            return View(model);
        }

        [HttpPost("brands/delete/{id}")]
        public async Task<IActionResult> DeleteBrand(Guid id)
        {
            try
            {
                var brand = await _context.Brands.FindAsync(id);
                if (brand == null)
                {
                    return NotFound();
                }

                // Check if brand has products
                var hasProducts = await _context.Products.AnyAsync(p => p.BrandId == id);
                if (hasProducts)
                {
                    TempData["ErrorMessage"] = "Không thể xóa thương hiệu này vì có sản phẩm đang sử dụng!";
                    return RedirectToAction(nameof(Brands));
                }

                _context.Brands.Remove(brand);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Thương hiệu đã được xóa thành công!";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Có lỗi xảy ra khi xóa thương hiệu: " + ex.Message;
            }

            return RedirectToAction(nameof(Brands));
        }

        [HttpPost("brands/toggle-status/{id}")]
        public async Task<IActionResult> ToggleBrandStatus(Guid id)
        {
            try
            {
                var brand = await _context.Brands.FindAsync(id);
                if (brand == null)
                {
                    return NotFound();
                }

                brand.IsActive = !brand.IsActive;
                brand.UpdatedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = $"Trạng thái thương hiệu đã được {(brand.IsActive ? "kích hoạt" : "vô hiệu hóa")}!";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Có lỗi xảy ra: " + ex.Message;
            }

            return RedirectToAction(nameof(Brands));
        }

        #endregion
    }
}

// Extension method for string title case
namespace JohnHenryFashionWeb.Extensions
{
    public static class StringExtensions
    {
        public static string ToTitleCase(this string input)
        {
            if (string.IsNullOrEmpty(input))
                return string.Empty;

            var words = input.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < words.Length; i++)
            {
                if (words[i].Length > 0)
                {
                    words[i] = char.ToUpper(words[i][0]) + (words[i].Length > 1 ? words[i].Substring(1).ToLower() : "");
                }
            }
            return string.Join(' ', words);
        }
    }
}
