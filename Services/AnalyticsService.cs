using Microsoft.EntityFrameworkCore;
using JohnHenryFashionWeb.Data;
using JohnHenryFashionWeb.Models;
using System.Text.Json;
using Microsoft.AspNetCore.Http;

namespace JohnHenryFashionWeb.Services
{
    public interface IAnalyticsService
    {
        // User Analytics
        Task TrackPageViewAsync(string userId, string page, string userAgent, string ipAddress, string? referrer = null);
        Task TrackUserSessionAsync(string userId, string sessionId, DateTime startTime, string userAgent, string ipAddress);
        Task EndUserSessionAsync(string sessionId, DateTime endTime);
        Task<UserAnalyticsData> GetUserAnalyticsAsync(DateTime from, DateTime to);
        Task<List<PageViewData>> GetPopularPagesAsync(DateTime from, DateTime to, int limit = 10);

        // Sales Analytics
        Task TrackSaleAsync(Guid orderId, decimal amount, string paymentMethod, List<Guid> productIds);
        Task TrackConversionAsync(string userId, string conversionType, decimal value, string? source = null);
        Task<SalesAnalyticsData> GetSalesAnalyticsAsync(DateTime from, DateTime to);
        Task<List<ProductPerformanceData>> GetProductPerformanceAsync(DateTime from, DateTime to);
        Task<ConversionAnalyticsData> GetConversionAnalyticsAsync(DateTime from, DateTime to);

        // Product Analytics
        Task TrackProductViewAsync(Guid productId, string userId, string? source = null);
        Task TrackProductAddToCartAsync(Guid productId, string userId, int quantity);
        Task TrackProductPurchaseAsync(Guid productId, string userId, int quantity, decimal price);
        Task<List<ProductAnalyticsData>> GetProductAnalyticsAsync(DateTime from, DateTime to);

        // Marketing Analytics
        Task TrackCampaignClickAsync(string campaignId, string source, string userId);
        Task TrackEmailOpenAsync(string emailId, string userId);
        Task TrackEmailClickAsync(string emailId, string userId, string linkUrl);
        Task<MarketingAnalyticsData> GetMarketingAnalyticsAsync(DateTime from, DateTime to);

        // Custom Events
        Task TrackCustomEventAsync(string eventName, string userId, Dictionary<string, object> properties);
        Task<List<CustomEventData>> GetCustomEventsAsync(string eventName, DateTime from, DateTime to);

        // Real-time Analytics
        Task<RealTimeAnalyticsData> GetRealTimeAnalyticsAsync();
        Task<List<ActiveUserData>> GetActiveUsersAsync(int minutes = 30);

        // Export Functions
        Task<byte[]> ExportAnalyticsDataAsync(DateTime from, DateTime to, string format = "excel");
        Task<string> GenerateAnalyticsReportAsync(DateTime from, DateTime to, string reportType);
    }

    public class AnalyticsService : IAnalyticsService
    {
        private readonly ApplicationDbContext _context;
        private readonly ICacheService _cacheService;
        private readonly ILogger<AnalyticsService> _logger;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AnalyticsService(
            ApplicationDbContext context,
            ICacheService cacheService,
            ILogger<AnalyticsService> logger,
            IConfiguration configuration,
            IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _cacheService = cacheService;
            _logger = logger;
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
        }

        // User Analytics Implementation
        public async Task TrackPageViewAsync(string userId, string page, string userAgent, string ipAddress, string? referrer = null)
        {
            try
            {
                var pageView = new PageView
                {
                    Id = Guid.NewGuid(),
                    UserId = userId,
                    Page = page,
                    UserAgent = userAgent,
                    IpAddress = ipAddress,
                    Referrer = referrer,
                    ViewedAt = DateTime.UtcNow,
                    SessionId = GetCurrentSessionId()
                };

                _context.PageViews.Add(pageView);
                await _context.SaveChangesAsync();

                // Update real-time cache
                await UpdateRealTimeCacheAsync("page_views", pageView);

                _logger.LogInformation("Page view tracked: {Page} by user {UserId}", page, userId ?? "anonymous");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error tracking page view for page {Page}", page);
            }
        }

        public async Task TrackUserSessionAsync(string userId, string sessionId, DateTime startTime, string userAgent, string ipAddress)
        {
            try
            {
                var session = new UserSession
                {
                    Id = Guid.NewGuid(),
                    SessionId = sessionId,
                    UserId = userId,
                    StartTime = startTime,
                    UserAgent = userAgent,
                    IpAddress = ipAddress,
                    IsActive = true
                };

                _context.UserSessions.Add(session);
                await _context.SaveChangesAsync();

                _logger.LogInformation("User session started: {SessionId} for user {UserId}", sessionId, userId ?? "anonymous");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error tracking user session {SessionId}", sessionId);
            }
        }

        public async Task EndUserSessionAsync(string sessionId, DateTime endTime)
        {
            try
            {
                var session = await _context.UserSessions
                    .FirstOrDefaultAsync(s => s.SessionId == sessionId && s.IsActive);

                if (session != null)
                {
                    session.EndTime = endTime;
                    session.IsActive = false;
                    session.Duration = (int)(endTime - session.StartTime).TotalMinutes;

                    await _context.SaveChangesAsync();

                    _logger.LogInformation("User session ended: {SessionId}, Duration: {Duration} minutes", sessionId, session.Duration);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error ending user session {SessionId}", sessionId);
            }
        }

        public async Task<UserAnalyticsData> GetUserAnalyticsAsync(DateTime from, DateTime to)
        {
            try
            {
                var cacheKey = $"user_analytics_{from:yyyyMMdd}_{to:yyyyMMdd}";
                var cached = await _cacheService.GetAsync<UserAnalyticsData>(cacheKey);
                
                if (cached != null)
                    return cached;

                var sessions = await _context.UserSessions
                    .Where(s => s.StartTime >= from && s.StartTime <= to)
                    .ToListAsync();

                var pageViews = await _context.PageViews
                    .Where(p => p.ViewedAt >= from && p.ViewedAt <= to)
                    .ToListAsync();

                var data = new UserAnalyticsData
                {
                    TotalSessions = sessions.Count,
                    UniqueSessions = sessions.Select(s => s.SessionId).Distinct().Count(),
                    RegisteredUserSessions = sessions.Count(s => !string.IsNullOrEmpty(s.UserId)),
                    AnonymousSessions = sessions.Count(s => string.IsNullOrEmpty(s.UserId)),
                    TotalPageViews = pageViews.Count,
                    UniquePageViews = pageViews.GroupBy(p => new { p.UserId, p.Page }).Count(),
                    AverageSessionDuration = sessions.Where(s => s.Duration.HasValue).Any() 
                        ? sessions.Where(s => s.Duration.HasValue).Average(s => s.Duration ?? 0) 
                        : 0,
                    BounceRate = CalculateBounceRate(sessions, pageViews),
                    TopPages = pageViews.GroupBy(p => p.Page)
                        .OrderByDescending(g => g.Count())
                        .Take(10)
                        .Select(g => new PageData { Page = g.Key, Views = g.Count() })
                        .ToList(),
                    HourlyData = GetHourlyUserData(sessions, pageViews),
                    DailyData = GetDailyUserData(sessions, pageViews, from, to)
                };

                await _cacheService.SetAsync(cacheKey, data, TimeSpan.FromHours(1));
                return data;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting user analytics from {From} to {To}", from, to);
                return new UserAnalyticsData();
            }
        }

        // Sales Analytics Implementation
        public async Task TrackSaleAsync(Guid orderId, decimal amount, string paymentMethod, List<Guid> productIds)
        {
            try
            {
                var conversionEvent = new ConversionEvent
                {
                    Id = Guid.NewGuid(),
                    OrderId = orderId,
                    ConversionType = "purchase",
                    Value = amount,
                    PaymentMethod = paymentMethod,
                    ProductIds = JsonSerializer.Serialize(productIds),
                    ConvertedAt = DateTime.UtcNow,
                    SessionId = GetCurrentSessionId()
                };

                _context.ConversionEvents.Add(conversionEvent);
                await _context.SaveChangesAsync();

                // Track individual product sales
                foreach (var productId in productIds)
                {
                    await TrackProductPurchaseEventAsync(productId, orderId, amount / productIds.Count);
                }

                await UpdateRealTimeCacheAsync("sales", conversionEvent);

                _logger.LogInformation("Sale tracked: Order {OrderId}, Amount {Amount}", orderId, amount);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error tracking sale for order {OrderId}", orderId);
            }
        }

        public async Task<SalesAnalyticsData> GetSalesAnalyticsAsync(DateTime from, DateTime to)
        {
            try
            {
                var cacheKey = $"sales_analytics_{from:yyyyMMdd}_{to:yyyyMMdd}";
                var cached = await _cacheService.GetAsync<SalesAnalyticsData>(cacheKey);
                
                if (cached != null)
                    return cached;

                var orders = await _context.Orders
                    .Include(o => o.OrderItems)
                    .Where(o => o.CreatedAt >= from && o.CreatedAt <= to)
                    .ToListAsync();

                var conversionEvents = await _context.ConversionEvents
                    .Where(c => c.ConvertedAt >= from && c.ConvertedAt <= to && c.ConversionType == "purchase")
                    .ToListAsync();

                var data = new SalesAnalyticsData
                {
                    TotalRevenue = orders.Where(o => o.PaymentStatus == "paid").Sum(o => o.TotalAmount),
                    TotalOrders = orders.Count,
                    CompletedOrders = orders.Count(o => o.PaymentStatus == "paid"),
                    PendingOrders = orders.Count(o => o.PaymentStatus == "pending"),
                    CancelledOrders = orders.Count(o => o.Status == "cancelled"),
                    AverageOrderValue = orders.Where(o => o.PaymentStatus == "paid").Average(o => o.TotalAmount),
                    ConversionRate = CalculateConversionRate(from, to),
                    PaymentMethodBreakdown = GetPaymentMethodBreakdown(orders),
                    DailyRevenue = GetDailyRevenue(orders, from, to),
                    TopSellingProducts = await GetTopSellingProductsAsync(from, to),
                    CategoryPerformance = await GetCategoryPerformanceAsync(from, to)
                };

                await _cacheService.SetAsync(cacheKey, data, TimeSpan.FromHours(1));
                return data;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting sales analytics from {From} to {To}", from, to);
                return new SalesAnalyticsData();
            }
        }

        // Product Analytics Implementation
        public async Task TrackProductViewAsync(Guid productId, string userId, string? source = null)
        {
            try
            {
                var analyticsData = new AnalyticsData
                {
                    Id = Guid.NewGuid(),
                    EventType = "product_view",
                    EntityId = productId.ToString(),
                    UserId = userId,
                    SessionId = GetCurrentSessionId(),
                    Source = source,
                    CreatedAt = DateTime.UtcNow,
                    Data = JsonSerializer.Serialize(new { productId, source })
                };

                _context.AnalyticsData.Add(analyticsData);
                await _context.SaveChangesAsync();

                // Update product view count cache
                var cacheKey = $"product_views_{productId}_{DateTime.UtcNow:yyyyMMdd}";
                await _cacheService.IncrementAsync(cacheKey, TimeSpan.FromDays(1));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error tracking product view for product {ProductId}", productId);
            }
        }

        public async Task<List<ProductAnalyticsData>> GetProductAnalyticsAsync(DateTime from, DateTime to)
        {
            try
            {
                var productViews = await _context.AnalyticsData
                    .Where(a => a.EventType == "product_view" && a.CreatedAt >= from && a.CreatedAt <= to)
                    .GroupBy(a => a.EntityId)
                    .Select(g => new { ProductId = g.Key, Views = g.Count() })
                    .ToListAsync();

                var productSales = await _context.OrderItems
                    .Include(oi => oi.Order)
                    .Where(oi => oi.Order.CreatedAt >= from && oi.Order.CreatedAt <= to && oi.Order.PaymentStatus == "paid")
                    .GroupBy(oi => oi.ProductId)
                    .Select(g => new 
                    { 
                        ProductId = g.Key, 
                        Sales = g.Sum(oi => oi.Quantity),
                        Revenue = g.Sum(oi => oi.TotalPrice)
                    })
                    .ToListAsync();

                var products = await _context.Products
                    .Include(p => p.Category)
                    .ToListAsync();

                var result = products.Select(p => new ProductAnalyticsData
                {
                    ProductId = p.Id,
                    ProductName = p.Name,
                    CategoryName = p.Category?.Name ?? "Uncategorized",
                    Views = productViews.FirstOrDefault(pv => pv.ProductId == p.Id.ToString())?.Views ?? 0,
                    Sales = productSales.FirstOrDefault(ps => ps.ProductId == p.Id)?.Sales ?? 0,
                    Revenue = productSales.FirstOrDefault(ps => ps.ProductId == p.Id)?.Revenue ?? 0,
                    ConversionRate = CalculateProductConversionRate(p.Id, from, to)
                }).OrderByDescending(p => p.Revenue).ToList();

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting product analytics from {From} to {To}", from, to);
                return new List<ProductAnalyticsData>();
            }
        }

        // Marketing Analytics Implementation
        public async Task TrackCampaignClickAsync(string campaignId, string source, string userId)
        {
            try
            {
                var analyticsData = new AnalyticsData
                {
                    Id = Guid.NewGuid(),
                    EventType = "campaign_click",
                    EntityId = campaignId,
                    UserId = userId,
                    SessionId = GetCurrentSessionId(),
                    Source = source,
                    CreatedAt = DateTime.UtcNow,
                    Data = JsonSerializer.Serialize(new { campaignId, source })
                };

                _context.AnalyticsData.Add(analyticsData);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error tracking campaign click for campaign {CampaignId}", campaignId);
            }
        }

        public async Task<MarketingAnalyticsData> GetMarketingAnalyticsAsync(DateTime from, DateTime to)
        {
            try
            {
                var campaignClicks = await _context.AnalyticsData
                    .Where(a => a.EventType == "campaign_click" && a.CreatedAt >= from && a.CreatedAt <= to)
                    .GroupBy(a => new { a.EntityId, a.Source })
                    .Select(g => new CampaignData 
                    { 
                        CampaignId = g.Key.EntityId, 
                        Source = g.Key.Source, 
                        Clicks = g.Count() 
                    })
                    .ToListAsync();

                var emailOpens = await _context.AnalyticsData
                    .Where(a => a.EventType == "email_open" && a.CreatedAt >= from && a.CreatedAt <= to)
                    .GroupBy(a => a.EntityId)
                    .Select(g => new EmailData 
                    { 
                        EmailId = g.Key, 
                        Opens = g.Count(),
                        UniqueOpens = g.Select(x => x.UserId).Distinct().Count()
                    })
                    .ToListAsync();

                return new MarketingAnalyticsData
                {
                    CampaignPerformance = campaignClicks,
                    EmailPerformance = emailOpens,
                    TopReferrers = await GetTopReferrersAsync(from, to),
                    SourceBreakdown = await GetSourceBreakdownAsync(from, to)
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting marketing analytics from {From} to {To}", from, to);
                return new MarketingAnalyticsData();
            }
        }

        // Real-time Analytics Implementation
        public async Task<RealTimeAnalyticsData> GetRealTimeAnalyticsAsync()
        {
            try
            {
                var now = DateTime.UtcNow;
                var last24Hours = now.AddHours(-24);
                var lastHour = now.AddHours(-1);

                var activeUsers = await GetActiveUsersAsync(30);
                var recentPageViews = await _context.PageViews
                    .Where(p => p.ViewedAt >= lastHour)
                    .CountAsync();

                var recentOrders = await _context.Orders
                    .Where(o => o.CreatedAt >= lastHour)
                    .CountAsync();

                var todayRevenue = await _context.Orders
                    .Where(o => o.CreatedAt >= now.Date && o.PaymentStatus == "paid")
                    .SumAsync(o => o.TotalAmount);

                return new RealTimeAnalyticsData
                {
                    ActiveUsers = activeUsers.Count,
                    PageViewsLastHour = recentPageViews,
                    OrdersLastHour = recentOrders,
                    TodayRevenue = todayRevenue,
                    TopActivePages = await GetTopActivePagesAsync(),
                    RecentConversions = await GetRecentConversionsAsync(),
                    LiveVisitors = activeUsers
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting real-time analytics");
                return new RealTimeAnalyticsData();
            }
        }

        // Helper Methods
        private string GetCurrentSessionId()
        {
            return _httpContextAccessor.HttpContext?.Session?.Id ?? Guid.NewGuid().ToString();
        }

        private async Task UpdateRealTimeCacheAsync(string eventType, object eventData)
        {
            var cacheKey = $"realtime_{eventType}";
            var events = await _cacheService.GetAsync<List<object>>(cacheKey) ?? new List<object>();
            
            events.Add(eventData);
            
            // Keep only last 100 events
            if (events.Count > 100)
            {
                events = events.TakeLast(100).ToList();
            }
            
            await _cacheService.SetAsync(cacheKey, events, TimeSpan.FromHours(1));
        }

        private double CalculateBounceRate(List<UserSession> sessions, List<PageView> pageViews)
        {
            if (!sessions.Any()) return 0;

            var sessionPageViews = pageViews.GroupBy(p => p.SessionId)
                .ToDictionary(g => g.Key, g => g.Count());

            var bouncedSessions = sessions.Count(s => sessionPageViews.GetValueOrDefault(s.SessionId, 0) <= 1);
            
            return (double)bouncedSessions / sessions.Count * 100;
        }

        private double CalculateConversionRate(DateTime from, DateTime to)
        {
            var sessions = _context.UserSessions
                .Count(s => s.StartTime >= from && s.StartTime <= to);

            var conversions = _context.ConversionEvents
                .Count(c => c.ConvertedAt >= from && c.ConvertedAt <= to && c.ConversionType == "purchase");

            return sessions > 0 ? (double)conversions / sessions * 100 : 0;
        }

        private double CalculateProductConversionRate(Guid productId, DateTime from, DateTime to)
        {
            var views = _context.AnalyticsData
                .Count(a => a.EventType == "product_view" && 
                           a.EntityId == productId.ToString() && 
                           a.CreatedAt >= from && a.CreatedAt <= to);

            var purchases = _context.OrderItems
                .Include(oi => oi.Order)
                .Count(oi => oi.ProductId == productId && 
                            oi.Order.CreatedAt >= from && oi.Order.CreatedAt <= to &&
                            oi.Order.PaymentStatus == "paid");

            return views > 0 ? (double)purchases / views * 100 : 0;
        }

        private async Task TrackProductPurchaseEventAsync(Guid productId, Guid orderId, decimal value)
        {
            var analyticsData = new AnalyticsData
            {
                Id = Guid.NewGuid(),
                EventType = "product_purchase",
                EntityId = productId.ToString(),
                SessionId = GetCurrentSessionId(),
                CreatedAt = DateTime.UtcNow,
                Data = JsonSerializer.Serialize(new { productId, orderId, value })
            };

            _context.AnalyticsData.Add(analyticsData);
        }

        private List<DailyData> GetDailyUserData(List<UserSession> sessions, List<PageView> pageViews, DateTime from, DateTime to)
        {
            var result = new List<DailyData>();
            
            for (var date = from.Date; date <= to.Date; date = date.AddDays(1))
            {
                var dayEnd = date.AddDays(1);
                
                result.Add(new DailyData
                {
                    Date = date,
                    Sessions = sessions.Count(s => s.StartTime >= date && s.StartTime < dayEnd),
                    PageViews = pageViews.Count(p => p.ViewedAt >= date && p.ViewedAt < dayEnd),
                    UniqueUsers = sessions.Where(s => s.StartTime >= date && s.StartTime < dayEnd)
                                        .Select(s => s.UserId)
                                        .Where(u => !string.IsNullOrEmpty(u))
                                        .Distinct()
                                        .Count()
                });
            }
            
            return result;
        }

        private List<HourlyData> GetHourlyUserData(List<UserSession> sessions, List<PageView> pageViews)
        {
            return Enumerable.Range(0, 24).Select(hour => new HourlyData
            {
                Hour = hour,
                Sessions = sessions.Count(s => s.StartTime.Hour == hour),
                PageViews = pageViews.Count(p => p.ViewedAt.Hour == hour)
            }).ToList();
        }

        private List<PaymentMethodData> GetPaymentMethodBreakdown(List<Order> orders)
        {
            return orders.Where(o => o.PaymentStatus == "paid")
                        .GroupBy(o => o.PaymentMethod)
                        .Select(g => new PaymentMethodData
                        {
                            PaymentMethod = g.Key,
                            Count = g.Count(),
                            Revenue = g.Sum(o => o.TotalAmount)
                        })
                        .OrderByDescending(p => p.Revenue)
                        .ToList();
        }

        private List<DailyRevenueData> GetDailyRevenue(List<Order> orders, DateTime from, DateTime to)
        {
            var result = new List<DailyRevenueData>();
            
            for (var date = from.Date; date <= to.Date; date = date.AddDays(1))
            {
                var dayEnd = date.AddDays(1);
                var dayOrders = orders.Where(o => o.CreatedAt >= date && o.CreatedAt < dayEnd && o.PaymentStatus == "paid");
                
                result.Add(new DailyRevenueData
                {
                    Date = date,
                    Revenue = dayOrders.Sum(o => o.TotalAmount),
                    Orders = dayOrders.Count()
                });
            }
            
            return result;
        }

        private async Task<List<ProductSalesData>> GetTopSellingProductsAsync(DateTime from, DateTime to)
        {
            return await _context.OrderItems
                .Include(oi => oi.Order)
                .Include(oi => oi.Product)
                .Where(oi => oi.Order.CreatedAt >= from && oi.Order.CreatedAt <= to && oi.Order.PaymentStatus == "paid")
                .GroupBy(oi => new { oi.ProductId, oi.Product.Name })
                .Select(g => new ProductSalesData
                {
                    ProductId = g.Key.ProductId,
                    ProductName = g.Key.Name,
                    Quantity = g.Sum(oi => oi.Quantity),
                    Revenue = g.Sum(oi => oi.TotalPrice)
                })
                .OrderByDescending(p => p.Revenue)
                .Take(10)
                .ToListAsync();
        }

        private async Task<List<CategoryPerformanceData>> GetCategoryPerformanceAsync(DateTime from, DateTime to)
        {
            return await _context.OrderItems
                .Include(oi => oi.Order)
                .Include(oi => oi.Product)
                .ThenInclude(p => p.Category)
                .Where(oi => oi.Order.CreatedAt >= from && oi.Order.CreatedAt <= to && oi.Order.PaymentStatus == "paid")
                .GroupBy(oi => new { oi.Product.CategoryId, oi.Product.Category.Name })
                .Select(g => new CategoryPerformanceData
                {
                    CategoryId = g.Key.CategoryId,
                    CategoryName = g.Key.Name ?? "Uncategorized",
                    Revenue = g.Sum(oi => oi.TotalPrice),
                    Quantity = g.Sum(oi => oi.Quantity),
                    Orders = g.Select(oi => oi.OrderId).Distinct().Count()
                })
                .OrderByDescending(c => c.Revenue)
                .ToListAsync();
        }

        private async Task<List<ReferrerData>> GetTopReferrersAsync(DateTime from, DateTime to)
        {
            return await _context.PageViews
                .Where(p => p.ViewedAt >= from && p.ViewedAt <= to && !string.IsNullOrEmpty(p.Referrer))
                .GroupBy(p => p.Referrer)
                .Select(g => new ReferrerData
                {
                    Referrer = g.Key,
                    Visits = g.Count(),
                    UniqueVisitors = g.Select(p => p.UserId).Distinct().Count()
                })
                .OrderByDescending(r => r.Visits)
                .Take(10)
                .ToListAsync();
        }

        private async Task<List<SourceData>> GetSourceBreakdownAsync(DateTime from, DateTime to)
        {
            return await _context.AnalyticsData
                .Where(a => a.CreatedAt >= from && a.CreatedAt <= to && !string.IsNullOrEmpty(a.Source))
                .GroupBy(a => a.Source)
                .Select(g => new SourceData
                {
                    Source = g.Key,
                    Events = g.Count(),
                    UniqueUsers = g.Select(a => a.UserId).Distinct().Count()
                })
                .OrderByDescending(s => s.Events)
                .ToListAsync();
        }

        public async Task<List<ActiveUserData>> GetActiveUsersAsync(int minutes = 30)
        {
            var cutoff = DateTime.UtcNow.AddMinutes(-minutes);
            
            return await _context.PageViews
                .Where(p => p.ViewedAt >= cutoff)
                .GroupBy(p => new { p.UserId, p.SessionId })
                .Select(g => new ActiveUserData
                {
                    UserId = g.Key.UserId ?? "anonymous",
                    SessionId = g.Key.SessionId,
                    LastActivity = g.Max(p => p.ViewedAt),
                    PageViews = g.Count()
                })
                .ToListAsync();
        }

        private async Task<List<PageActivityData>> GetTopActivePagesAsync()
        {
            var cutoff = DateTime.UtcNow.AddMinutes(-30);
            
            return await _context.PageViews
                .Where(p => p.ViewedAt >= cutoff)
                .GroupBy(p => p.Page)
                .Select(g => new PageActivityData
                {
                    Page = g.Key,
                    ActiveUsers = g.Select(p => p.UserId).Distinct().Count(),
                    Views = g.Count()
                })
                .OrderByDescending(p => p.ActiveUsers)
                .Take(10)
                .ToListAsync();
        }

        private async Task<List<ConversionData>> GetRecentConversionsAsync()
        {
            return await _context.ConversionEvents
                .Where(c => c.ConvertedAt >= DateTime.UtcNow.AddHours(-1))
                .OrderByDescending(c => c.ConvertedAt)
                .Take(10)
                .Select(c => new ConversionData
                {
                    ConversionType = c.ConversionType,
                    Value = c.Value,
                    ConvertedAt = c.ConvertedAt,
                    OrderId = c.OrderId
                })
                .ToListAsync();
        }

        // Additional interface implementations
        public async Task TrackConversionAsync(string userId, string conversionType, decimal value, string? source = null)
        {
            var conversionEvent = new ConversionEvent
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                ConversionType = conversionType,
                Value = value,
                Source = source,
                ConvertedAt = DateTime.UtcNow,
                SessionId = GetCurrentSessionId()
            };

            _context.ConversionEvents.Add(conversionEvent);
            await _context.SaveChangesAsync();
        }

        public async Task<ConversionAnalyticsData> GetConversionAnalyticsAsync(DateTime from, DateTime to)
        {
            var conversions = await _context.ConversionEvents
                .Where(c => c.ConvertedAt >= from && c.ConvertedAt <= to)
                .ToListAsync();

            return new ConversionAnalyticsData
            {
                TotalConversions = conversions.Count,
                TotalValue = conversions.Sum(c => c.Value),
                ConversionsByType = conversions.GroupBy(c => c.ConversionType)
                    .Select(g => new ConversionTypeData
                    {
                        Type = g.Key,
                        Count = g.Count(),
                        Value = g.Sum(c => c.Value)
                    }).ToList(),
                DailyConversions = GetDailyConversions(conversions, from, to)
            };
        }

        public async Task<List<PageViewData>> GetPopularPagesAsync(DateTime from, DateTime to, int limit = 10)
        {
            return await _context.PageViews
                .Where(p => p.ViewedAt >= from && p.ViewedAt <= to)
                .GroupBy(p => p.Page)
                .Select(g => new PageViewData
                {
                    Page = g.Key,
                    Views = g.Count(),
                    UniqueViews = g.Select(p => p.UserId).Distinct().Count()
                })
                .OrderByDescending(p => p.Views)
                .Take(limit)
                .ToListAsync();
        }

        public async Task<List<ProductPerformanceData>> GetProductPerformanceAsync(DateTime from, DateTime to)
        {
            return await _context.OrderItems
                .Include(oi => oi.Order)
                .Include(oi => oi.Product)
                .Where(oi => oi.Order.CreatedAt >= from && oi.Order.CreatedAt <= to && oi.Order.PaymentStatus == "paid")
                .GroupBy(oi => new { oi.ProductId, oi.Product.Name })
                .Select(g => new ProductPerformanceData
                {
                    ProductId = g.Key.ProductId,
                    ProductName = g.Key.Name,
                    Sales = g.Sum(oi => oi.Quantity),
                    Revenue = g.Sum(oi => oi.TotalPrice),
                    Orders = g.Select(oi => oi.OrderId).Distinct().Count()
                })
                .OrderByDescending(p => p.Revenue)
                .ToListAsync();
        }

        public async Task TrackProductAddToCartAsync(Guid productId, string userId, int quantity)
        {
            var analyticsData = new AnalyticsData
            {
                Id = Guid.NewGuid(),
                EventType = "add_to_cart",
                EntityId = productId.ToString(),
                UserId = userId,
                SessionId = GetCurrentSessionId(),
                CreatedAt = DateTime.UtcNow,
                Data = JsonSerializer.Serialize(new { productId, quantity })
            };

            _context.AnalyticsData.Add(analyticsData);
            await _context.SaveChangesAsync();
        }

        public async Task TrackProductPurchaseAsync(Guid productId, string userId, int quantity, decimal price)
        {
            var analyticsData = new AnalyticsData
            {
                Id = Guid.NewGuid(),
                EventType = "product_purchase",
                EntityId = productId.ToString(),
                UserId = userId,
                SessionId = GetCurrentSessionId(),
                CreatedAt = DateTime.UtcNow,
                Data = JsonSerializer.Serialize(new { productId, quantity, price })
            };

            _context.AnalyticsData.Add(analyticsData);
            await _context.SaveChangesAsync();
        }

        public async Task TrackEmailOpenAsync(string emailId, string userId)
        {
            var analyticsData = new AnalyticsData
            {
                Id = Guid.NewGuid(),
                EventType = "email_open",
                EntityId = emailId,
                UserId = userId,
                SessionId = GetCurrentSessionId(),
                CreatedAt = DateTime.UtcNow,
                Data = JsonSerializer.Serialize(new { emailId })
            };

            _context.AnalyticsData.Add(analyticsData);
            await _context.SaveChangesAsync();
        }

        public async Task TrackEmailClickAsync(string emailId, string userId, string linkUrl)
        {
            var analyticsData = new AnalyticsData
            {
                Id = Guid.NewGuid(),
                EventType = "email_click",
                EntityId = emailId,
                UserId = userId,
                SessionId = GetCurrentSessionId(),
                CreatedAt = DateTime.UtcNow,
                Data = JsonSerializer.Serialize(new { emailId, linkUrl })
            };

            _context.AnalyticsData.Add(analyticsData);
            await _context.SaveChangesAsync();
        }

        public async Task TrackCustomEventAsync(string eventName, string userId, Dictionary<string, object> properties)
        {
            var analyticsData = new AnalyticsData
            {
                Id = Guid.NewGuid(),
                EventType = eventName,
                UserId = userId,
                SessionId = GetCurrentSessionId(),
                CreatedAt = DateTime.UtcNow,
                Data = JsonSerializer.Serialize(properties)
            };

            _context.AnalyticsData.Add(analyticsData);
            await _context.SaveChangesAsync();
        }

        public async Task<List<CustomEventData>> GetCustomEventsAsync(string eventName, DateTime from, DateTime to)
        {
            return await _context.AnalyticsData
                .Where(a => a.EventType == eventName && a.CreatedAt >= from && a.CreatedAt <= to)
                .Select(a => new CustomEventData
                {
                    EventName = a.EventType,
                    UserId = a.UserId,
                    Data = a.Data,
                    CreatedAt = a.CreatedAt
                })
                .ToListAsync();
        }

        public async Task<byte[]> ExportAnalyticsDataAsync(DateTime from, DateTime to, string format = "excel")
        {
            // Implementation for exporting analytics data
            // This would use a library like EPPlus for Excel or similar
            throw new NotImplementedException("Export functionality to be implemented");
        }

        public async Task<string> GenerateAnalyticsReportAsync(DateTime from, DateTime to, string reportType)
        {
            // Implementation for generating comprehensive analytics reports
            // This would create formatted reports in HTML, PDF, etc.
            throw new NotImplementedException("Report generation to be implemented");
        }

        private List<DailyConversionData> GetDailyConversions(List<ConversionEvent> conversions, DateTime from, DateTime to)
        {
            var result = new List<DailyConversionData>();
            
            for (var date = from.Date; date <= to.Date; date = date.AddDays(1))
            {
                var dayEnd = date.AddDays(1);
                var dayConversions = conversions.Where(c => c.ConvertedAt >= date && c.ConvertedAt < dayEnd);
                
                result.Add(new DailyConversionData
                {
                    Date = date,
                    Conversions = dayConversions.Count(),
                    Value = dayConversions.Sum(c => c.Value)
                });
            }
            
            return result;
        }
    }
}
