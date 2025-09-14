using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using JohnHenryFashionWeb.Data;
using JohnHenryFashionWeb.Models;
using System.Security.Claims;

namespace JohnHenryFashionWeb.Controllers
{
    public class CouponController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CouponController(ApplicationDbContext context)
        {
            _context = context;
        }

        // POST: Coupon/Apply
        [HttpPost]
        public async Task<IActionResult> Apply(string couponCode)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(couponCode))
                {
                    return Json(new { success = false, message = "Vui lòng nhập mã giảm giá" });
                }

                var coupon = await _context.Coupons
                    .FirstOrDefaultAsync(c => c.Code.ToUpper() == couponCode.ToUpper() && 
                                             c.IsActive &&
                                             (c.StartDate == null || c.StartDate <= DateTime.UtcNow) &&
                                             (c.EndDate == null || c.EndDate >= DateTime.UtcNow));

                if (coupon == null)
                {
                    return Json(new { success = false, message = "Mã giảm giá không hợp lệ hoặc đã hết hạn" });
                }

                // Check usage limit
                if (coupon.UsageLimit.HasValue && coupon.UsageCount >= coupon.UsageLimit.Value)
                {
                    return Json(new { success = false, message = "Mã giảm giá đã được sử dụng hết" });
                }

                return Json(new 
                { 
                    success = true, 
                    message = "Áp dụng mã giảm giá thành công!",
                    coupon = new 
                    {
                        id = coupon.Id,
                        code = coupon.Code,
                        type = coupon.Type,
                        value = coupon.Value,
                        minOrderAmount = coupon.MinOrderAmount
                    }
                });
            }
            catch
            {
                return Json(new { success = false, message = "Có lỗi xảy ra khi áp dụng mã giảm giá" });
            }
        }

        // POST: Coupon/Calculate
        [HttpPost]
        public async Task<IActionResult> Calculate(Guid couponId, decimal subtotal)
        {
            try
            {
                var coupon = await _context.Coupons.FindAsync(couponId);
                if (coupon == null)
                {
                    return Json(new { success = false, message = "Mã giảm giá không tồn tại" });
                }

                // Check minimum amount
                if (coupon.MinOrderAmount.HasValue && subtotal < coupon.MinOrderAmount.Value)
                {
                    return Json(new 
                    { 
                        success = false, 
                        message = $"Đơn hàng tối thiểu {coupon.MinOrderAmount.Value:C} để sử dụng mã này" 
                    });
                }

                decimal discountAmount = 0;

                if (coupon.Type == "percentage")
                {
                    discountAmount = subtotal * (coupon.Value / 100);
                }
                else // fixed amount
                {
                    discountAmount = coupon.Value;
                    
                    // Discount cannot exceed subtotal
                    if (discountAmount > subtotal)
                    {
                        discountAmount = subtotal;
                    }
                }

                var finalTotal = subtotal - discountAmount;

                return Json(new 
                { 
                    success = true,
                    discountAmount = discountAmount,
                    finalTotal = finalTotal,
                    savings = discountAmount
                });
            }
            catch
            {
                return Json(new { success = false, message = "Có lỗi xảy ra khi tính toán giảm giá" });
            }
        }

        // GET: Coupon/Available
        public async Task<IActionResult> Available()
        {
            var currentDate = DateTime.UtcNow;
            
            var coupons = await _context.Coupons
                .Where(c => c.IsActive && 
                           (c.StartDate == null || c.StartDate <= currentDate) &&
                           (c.EndDate == null || c.EndDate >= currentDate))
                .Select(c => new 
                {
                    c.Id,
                    c.Code,
                    c.Name,
                    c.Description,
                    c.Type,
                    c.Value,
                    c.MinOrderAmount,
                    c.EndDate
                })
                .ToListAsync();

            return Json(new { success = true, coupons = coupons });
        }

        // Admin Management Actions

        // GET: Coupon/Manage
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Manage(int page = 1, string? search = null)
        {
            const int pageSize = 20;
            
            var query = _context.Coupons.AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(c => c.Code.Contains(search) || 
                                        c.Name.Contains(search) ||
                                        (c.Description != null && c.Description.Contains(search)));
                ViewBag.Search = search;
            }

            var totalCoupons = await query.CountAsync();
            var coupons = await query
                .OrderByDescending(c => c.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = (int)Math.Ceiling((double)totalCoupons / pageSize);

            return View(coupons);
        }

        // GET: Coupon/Create
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Coupon/Create
        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Coupon coupon)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    // Check if code already exists
                    var existingCoupon = await _context.Coupons
                        .FirstOrDefaultAsync(c => c.Code.ToUpper() == coupon.Code.ToUpper());
                    
                    if (existingCoupon != null)
                    {
                        ModelState.AddModelError("Code", "Mã giảm giá này đã tồn tại");
                        return View(coupon);
                    }

                    coupon.Id = Guid.NewGuid();
                    coupon.Code = coupon.Code.ToUpper();
                    coupon.CreatedAt = DateTime.UtcNow;
                    coupon.UpdatedAt = DateTime.UtcNow;

                    _context.Coupons.Add(coupon);
                    await _context.SaveChangesAsync();

                    TempData["SuccessMessage"] = "Tạo mã giảm giá thành công!";
                    return RedirectToAction(nameof(Manage));
                }
            }
            catch
            {
                ModelState.AddModelError("", "Có lỗi xảy ra khi tạo mã giảm giá");
            }

            return View(coupon);
        }

        // GET: Coupon/Edit/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(Guid id)
        {
            var coupon = await _context.Coupons.FindAsync(id);
            if (coupon == null)
            {
                return NotFound();
            }

            return View(coupon);
        }

        // POST: Coupon/Edit/5
        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, Coupon coupon)
        {
            if (id != coupon.Id)
            {
                return NotFound();
            }

            try
            {
                if (ModelState.IsValid)
                {
                    // Check if code already exists (excluding current coupon)
                    var existingCoupon = await _context.Coupons
                        .FirstOrDefaultAsync(c => c.Code.ToUpper() == coupon.Code.ToUpper() && c.Id != id);
                    
                    if (existingCoupon != null)
                    {
                        ModelState.AddModelError("Code", "Mã giảm giá này đã tồn tại");
                        return View(coupon);
                    }

                    var existingEntity = await _context.Coupons.FindAsync(id);
                    if (existingEntity == null)
                    {
                        return NotFound();
                    }

                    // Update properties
                    existingEntity.Code = coupon.Code.ToUpper();
                    existingEntity.Name = coupon.Name;
                    existingEntity.Description = coupon.Description;
                    existingEntity.Type = coupon.Type;
                    existingEntity.Value = coupon.Value;
                    existingEntity.MinOrderAmount = coupon.MinOrderAmount;
                    existingEntity.StartDate = coupon.StartDate;
                    existingEntity.EndDate = coupon.EndDate;
                    existingEntity.UsageLimit = coupon.UsageLimit;
                    existingEntity.IsActive = coupon.IsActive;
                    existingEntity.UpdatedAt = DateTime.UtcNow;

                    await _context.SaveChangesAsync();

                    TempData["SuccessMessage"] = "Cập nhật mã giảm giá thành công!";
                    return RedirectToAction(nameof(Manage));
                }
            }
            catch
            {
                ModelState.AddModelError("", "Có lỗi xảy ra khi cập nhật mã giảm giá");
            }

            return View(coupon);
        }

        // POST: Coupon/Delete/5
        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                var coupon = await _context.Coupons.FindAsync(id);
                if (coupon == null)
                {
                    return Json(new { success = false, message = "Mã giảm giá không tồn tại" });
                }

                // Check if coupon is being used in any orders
                var isUsed = await _context.Orders.AnyAsync(o => o.CouponCode == coupon.Code);
                if (isUsed)
                {
                    // Don't delete, just deactivate
                    coupon.IsActive = false;
                    coupon.UpdatedAt = DateTime.UtcNow;
                    await _context.SaveChangesAsync();
                    
                    return Json(new { success = true, message = "Mã giảm giá đã được vô hiệu hóa" });
                }
                else
                {
                    _context.Coupons.Remove(coupon);
                    await _context.SaveChangesAsync();
                    
                    return Json(new { success = true, message = "Xóa mã giảm giá thành công" });
                }
            }
            catch
            {
                return Json(new { success = false, message = "Có lỗi xảy ra khi xóa mã giảm giá" });
            }
        }

        // POST: Coupon/ToggleStatus/5
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ToggleStatus(Guid id)
        {
            try
            {
                var coupon = await _context.Coupons.FindAsync(id);
                if (coupon == null)
                {
                    return Json(new { success = false, message = "Mã giảm giá không tồn tại" });
                }

                coupon.IsActive = !coupon.IsActive;
                coupon.UpdatedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();

                return Json(new 
                { 
                    success = true, 
                    message = coupon.IsActive ? "Kích hoạt thành công" : "Vô hiệu hóa thành công",
                    isActive = coupon.IsActive
                });
            }
            catch
            {
                return Json(new { success = false, message = "Có lỗi xảy ra" });
            }
        }

        // GET: Coupon/Details/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Details(Guid id)
        {
            var coupon = await _context.Coupons.FindAsync(id);
            if (coupon == null)
            {
                return NotFound();
            }

            // Get usage statistics
            var totalUses = await _context.Orders.CountAsync(o => o.CouponCode == coupon.Code);
            var totalSavings = await _context.Orders
                .Where(o => o.CouponCode == coupon.Code)
                .SumAsync(o => o.DiscountAmount);

            ViewBag.TotalUses = totalUses;
            ViewBag.TotalSavings = totalSavings;

            return View(coupon);
        }
    }
}
