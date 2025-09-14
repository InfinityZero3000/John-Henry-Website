using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using JohnHenryFashionWeb.Data;
using JohnHenryFashionWeb.Models;

namespace JohnHenryFashionWeb.Controllers
{
    public partial class AdminController
    {
        #region Orders Management
        // GET: Admin/Orders
        [Route("orders")]
        public async Task<IActionResult> Orders(int page = 1, int pageSize = 20, string? search = null, string? status = null)
        {
            var query = _context.Orders
                .Include(o => o.User)
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                .AsQueryable();

            // Filter by search term
            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(o => o.OrderNumber.Contains(search) ||
                                        (o.User.FirstName != null && o.User.FirstName.Contains(search)) ||
                                        (o.User.LastName != null && o.User.LastName.Contains(search)) ||
                                        (o.User.Email != null && o.User.Email.Contains(search)));
            }

            // Filter by status
            if (!string.IsNullOrEmpty(status))
            {
                query = query.Where(o => o.Status == status);
            }

            var totalOrders = await query.CountAsync();
            var orders = await query
                .OrderByDescending(o => o.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            ViewBag.TotalOrders = totalOrders;
            ViewBag.CurrentPage = page;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalPages = (int)Math.Ceiling((double)totalOrders / pageSize);
            ViewBag.Search = search;
            ViewBag.Status = status;

            return View(orders);
        }

        // GET: Admin/OrderDetails/{id}
        [Route("order-details/{id}")]
        public async Task<IActionResult> OrderDetails(Guid id)
        {
            var order = await _context.Orders
                .Include(o => o.User)
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        // POST: Admin/UpdateOrderStatus
        [HttpPost]
        [Route("update-order-status")]
        public async Task<IActionResult> UpdateOrderStatus(Guid orderId, string status)
        {
            var order = await _context.Orders.FindAsync(orderId);
            if (order == null)
            {
                return Json(new { success = false, message = "Không tìm thấy đơn hàng" });
            }

            order.Status = status;
            order.UpdatedAt = DateTime.UtcNow;

            if (status == "shipped")
            {
                order.ShippedAt = DateTime.UtcNow;
            }
            else if (status == "delivered")
            {
                order.DeliveredAt = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync();

            return Json(new { success = true, message = "Cập nhật trạng thái đơn hàng thành công" });
        }
        #endregion
    }
}
