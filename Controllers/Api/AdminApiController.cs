using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using JohnHenryFashionWeb.Data;
using JohnHenryFashionWeb.Models;
using JohnHenryFashionWeb.ViewModels;
using JohnHenryFashionWeb.Services;

namespace JohnHenryFashionWeb.Controllers.Api
{
    [ApiController]
    [Route("api/admin")]
    [Authorize(Roles = UserRoles.Admin)]
    public class AdminApiController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IAnalyticsService _analyticsService;
        private readonly IReportingService _reportingService;
        private readonly ILogger<AdminApiController> _logger;

        public AdminApiController(
            ApplicationDbContext context,
            IAnalyticsService analyticsService,
            IReportingService reportingService,
            ILogger<AdminApiController> logger)
        {
            _context = context;
            _analyticsService = analyticsService;
            _reportingService = reportingService;
            _logger = logger;
        }

        #region Dashboard APIs

        [HttpGet("dashboard/stats")]
        public async Task<IActionResult> GetDashboardStats()
        {
            try
            {
                var stats = new
                {
                    TotalProducts = await _context.Products.CountAsync(),
                    TotalOrders = await _context.Orders.CountAsync(),
                    TotalUsers = await _context.Users.CountAsync(),
                    TotalRevenue = await _context.Orders
                        .Where(o => o.Status == "completed")
                        .SumAsync(o => o.TotalAmount),
                    
                    TodayOrders = await _context.Orders
                        .Where(o => o.CreatedAt.Date == DateTime.UtcNow.Date)
                        .CountAsync(),
                    
                    TodayRevenue = await _context.Orders
                        .Where(o => o.CreatedAt.Date == DateTime.UtcNow.Date && o.Status == "completed")
                        .SumAsync(o => o.TotalAmount),
                    
                    PendingOrders = await _context.Orders
                        .Where(o => o.Status == "pending" || o.Status == "processing")
                        .CountAsync(),
                    
                    LowStockProducts = await _context.Products
                        .Where(p => p.StockQuantity <= 10 && p.IsActive)
                        .CountAsync()
                };

                return Ok(new { success = true, data = stats });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting dashboard stats");
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpGet("dashboard/recent-orders")]
        public async Task<IActionResult> GetRecentOrders(int limit = 10)
        {
            try
            {
                var orders = await _context.Orders
                    .Include(o => o.User)
                    .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                    .OrderByDescending(o => o.CreatedAt)
                    .Take(limit)
                    .Select(o => new
                    {
                        o.Id,
                        o.OrderNumber,
                        CustomerName = o.User.FullName,
                        CustomerEmail = o.User.Email,
                        o.TotalAmount,
                        o.Status,
                        o.CreatedAt,
                        ItemCount = o.OrderItems.Count
                    })
                    .ToListAsync();

                return Ok(new { success = true, data = orders });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting recent orders");
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        #endregion

        #region Product APIs

        [HttpGet("products")]
        public async Task<IActionResult> GetProducts(
            int page = 1, 
            int pageSize = 10, 
            string? search = null, 
            Guid? categoryId = null, 
            string? status = null)
        {
            try
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

                var totalProducts = await query.CountAsync();
                var products = await query
                    .OrderByDescending(p => p.CreatedAt)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .Select(p => new
                    {
                        p.Id,
                        p.Name,
                        p.SKU,
                        p.Price,
                        p.SalePrice,
                        p.StockQuantity,
                        p.Status,
                        CategoryName = p.Category.Name,
                        BrandName = p.Brand != null ? p.Brand.Name : null,
                        p.FeaturedImageUrl,
                        p.CreatedAt,
                        p.IsFeatured,
                        p.IsActive
                    })
                    .ToListAsync();

                return Ok(new
                {
                    success = true,
                    data = products,
                    pagination = new
                    {
                        currentPage = page,
                        pageSize = pageSize,
                        totalItems = totalProducts,
                        totalPages = (int)Math.Ceiling((double)totalProducts / pageSize)
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting products");
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpPost("products/{id}/toggle-status")]
        public async Task<IActionResult> ToggleProductStatus(Guid id)
        {
            try
            {
                var product = await _context.Products.FindAsync(id);
                if (product == null)
                {
                    return NotFound(new { success = false, message = "Sản phẩm không tồn tại" });
                }

                product.IsActive = !product.IsActive;
                product.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                return Ok(new 
                { 
                    success = true, 
                    message = product.IsActive ? "Đã kích hoạt sản phẩm" : "Đã vô hiệu hóa sản phẩm",
                    isActive = product.IsActive
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error toggling product status for {ProductId}", id);
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        #endregion

        #region Order APIs

        [HttpGet("orders")]
        public async Task<IActionResult> GetOrders(
            int page = 1,
            int pageSize = 20,
            string? search = null,
            string? status = null,
            DateTime? fromDate = null,
            DateTime? toDate = null)
        {
            try
            {
                var query = _context.Orders
                    .Include(o => o.User)
                    .Include(o => o.OrderItems)
                    .AsQueryable();

                if (!string.IsNullOrEmpty(search))
                {
                    query = query.Where(o => o.OrderNumber.Contains(search) ||
                                           (o.User != null && o.User.Email != null && o.User.Email.Contains(search)) ||
                                           (o.User != null && o.User.FirstName != null && o.User.FirstName.Contains(search)) ||
                                           (o.User != null && o.User.LastName != null && o.User.LastName.Contains(search)));
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
                    query = query.Where(o => o.CreatedAt <= toDate.Value);
                }

                var totalOrders = await query.CountAsync();
                var orders = await query
                    .OrderByDescending(o => o.CreatedAt)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .Select(o => new
                    {
                        o.Id,
                        o.OrderNumber,
                        CustomerName = o.User.FullName,
                        CustomerEmail = o.User.Email,
                        o.TotalAmount,
                        o.Status,
                        o.PaymentStatus,
                        o.CreatedAt,
                        ItemCount = o.OrderItems.Count
                    })
                    .ToListAsync();

                return Ok(new
                {
                    success = true,
                    data = orders,
                    pagination = new
                    {
                        currentPage = page,
                        pageSize = pageSize,
                        totalItems = totalOrders,
                        totalPages = (int)Math.Ceiling((double)totalOrders / pageSize)
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting orders");
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpPost("orders/{id}/status")]
        public async Task<IActionResult> UpdateOrderStatus(Guid id, [FromBody] UpdateOrderStatusRequest request)
        {
            try
            {
                var order = await _context.Orders.FindAsync(id);
                if (order == null)
                {
                    return NotFound(new { success = false, message = "Đơn hàng không tồn tại" });
                }

                order.Status = request.Status;
                order.UpdatedAt = DateTime.UtcNow;

                if (request.Status == "shipped" && !order.ShippedAt.HasValue)
                {
                    order.ShippedAt = DateTime.UtcNow;
                }
                else if (request.Status == "delivered" && !order.DeliveredAt.HasValue)
                {
                    order.DeliveredAt = DateTime.UtcNow;
                }

                await _context.SaveChangesAsync();

                return Ok(new 
                { 
                    success = true, 
                    message = "Đã cập nhật trạng thái đơn hàng",
                    status = order.Status
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating order status for {OrderId}", id);
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        #endregion

        #region Analytics APIs

        [HttpGet("analytics/realtime")]
        public async Task<IActionResult> GetRealTimeAnalytics()
        {
            try
            {
                var analytics = await _analyticsService.GetRealTimeAnalyticsAsync();
                return Ok(new { success = true, data = analytics });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting real-time analytics");
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpGet("analytics/sales")]
        public async Task<IActionResult> GetSalesAnalytics(
            [FromQuery] DateTime? fromDate = null,
            [FromQuery] DateTime? toDate = null)
        {
            try
            {
                var from = fromDate ?? DateTime.UtcNow.AddDays(-30);
                var to = toDate ?? DateTime.UtcNow;

                var analytics = await _analyticsService.GetSalesAnalyticsAsync(from, to);
                return Ok(new { success = true, data = analytics });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting sales analytics");
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        #endregion

        #region User APIs

        [HttpGet("users")]
        public async Task<IActionResult> GetUsers(
            int page = 1,
            int pageSize = 20,
            string? search = null,
            string? role = null)
        {
            try
            {
                var query = _context.Users.AsQueryable();

                if (!string.IsNullOrEmpty(search))
                {
                    query = query.Where(u => (u.Email != null && u.Email.Contains(search)) ||
                                           (u.FirstName != null && u.FirstName.Contains(search)) ||
                                           (u.LastName != null && u.LastName.Contains(search)));
                }

                var totalUsers = await query.CountAsync();
                var users = await query
                    .OrderByDescending(u => u.CreatedAt)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .Select(u => new
                    {
                        u.Id,
                        u.Email,
                        u.FirstName,
                        u.LastName,
                        FullName = u.FullName,
                        u.PhoneNumber,
                        u.IsActive,
                        u.EmailConfirmed,
                        u.CreatedAt,
                        u.LastLoginDate
                    })
                    .ToListAsync();

                return Ok(new
                {
                    success = true,
                    data = users,
                    pagination = new
                    {
                        currentPage = page,
                        pageSize = pageSize,
                        totalItems = totalUsers,
                        totalPages = (int)Math.Ceiling((double)totalUsers / pageSize)
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting users");
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpPost("users/{id}/toggle-status")]
        public async Task<IActionResult> ToggleUserStatus(string id)
        {
            try
            {
                var user = await _context.Users.FindAsync(id);
                if (user == null)
                {
                    return NotFound(new { success = false, message = "Người dùng không tồn tại" });
                }

                user.IsActive = !user.IsActive;
                await _context.SaveChangesAsync();

                return Ok(new 
                { 
                    success = true, 
                    message = user.IsActive ? "Đã kích hoạt người dùng" : "Đã vô hiệu hóa người dùng",
                    isActive = user.IsActive
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error toggling user status for {UserId}", id);
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        #endregion

        #region System APIs

        [HttpGet("system/health")]
        public async Task<IActionResult> GetSystemHealth()
        {
            try
            {
                var health = new
                {
                    Status = "Healthy",
                    Timestamp = DateTime.UtcNow,
                    Database = await CheckDatabaseHealthAsync(),
                    Memory = new
                    {
                        WorkingSet = Environment.WorkingSet,
                        GcMemory = GC.GetTotalMemory(false)
                    },
                    Uptime = DateTime.UtcNow - System.Diagnostics.Process.GetCurrentProcess().StartTime
                };

                return Ok(new { success = true, data = health });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting system health");
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        #endregion

        #region Helper Methods

        private async Task<object> CheckDatabaseHealthAsync()
        {
            try
            {
                var canConnect = await _context.Database.CanConnectAsync();
                return new { Status = canConnect ? "Connected" : "Disconnected" };
            }
            catch (Exception ex)
            {
                return new { Status = "Error", Message = ex.Message };
            }
        }

        #endregion
    }

    // Request DTOs
    public class UpdateOrderStatusRequest
    {
        public string Status { get; set; } = string.Empty;
        public string? Notes { get; set; }
    }
}