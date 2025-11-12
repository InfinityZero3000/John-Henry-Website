using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using JohnHenryFashionWeb.Models;
using JohnHenryFashionWeb.Data;
using JohnHenryFashionWeb.ViewModels;
using JohnHenryFashionWeb.Helpers;

namespace JohnHenryFashionWeb.Controllers
{
    public class UserDashboardController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;
        private readonly ILogger<UserDashboardController> _logger;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public UserDashboardController(
            UserManager<ApplicationUser> userManager,
            ApplicationDbContext context,
            ILogger<UserDashboardController> logger,
            IWebHostEnvironment webHostEnvironment)
        {
            _userManager = userManager;
            _context = context;
            _logger = logger;
            _webHostEnvironment = webHostEnvironment;
        }

        [AllowAnonymous]
        public IActionResult ProfileOrLogin()
        {
            if (User?.Identity?.IsAuthenticated == true)
            {
                return RedirectToAction(nameof(Index));
            }
            else
            {
                return RedirectToAction("Login", "Account");
            }
        }

        [Authorize]
        public async Task<IActionResult> Index()
        {
            var userId = _userManager.GetUserId(User);
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }

            // Check if user has admin or seller role - redirect them to proper dashboard
            var roles = await _userManager.GetRolesAsync(user);
            if (roles.Contains("Admin"))
            {
                _logger.LogInformation($"Redirecting admin user {user.Email} to admin dashboard from UserDashboard");
                return RedirectToAction("Dashboard", "Admin");
            }
            if (roles.Contains("Seller"))
            {
                _logger.LogInformation($"Redirecting seller user {user.Email} to seller dashboard from UserDashboard");
                return RedirectToAction("Dashboard", "Seller");
            }

            // For regular customers, show simple profile management
            _logger.LogInformation($"Showing customer profile for user {user.Email}");
            return RedirectToAction(nameof(Profile));
        }

        [HttpGet]
        public async Task<IActionResult> Profile()
        {
            var userId = _userManager.GetUserId(User);
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var profileModel = new UserProfileViewModel
            {
                Id = user.Id,
                Email = user.Email ?? "",
                FirstName = user.FirstName ?? "",
                LastName = user.LastName ?? "",
                PhoneNumber = user.PhoneNumber ?? "",
                Gender = user.Gender ?? "",
                DateOfBirth = user.DateOfBirth,
                Avatar = AvatarHelper.GetAvatarOrDefault(user.Avatar, _webHostEnvironment)
            };

            return View(profileModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Profile(UserProfileViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var userId = _userManager.GetUserId(User);
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }

            // Update user information
            user.FirstName = model.FirstName;
            user.LastName = model.LastName;
            user.PhoneNumber = model.PhoneNumber;
            user.Gender = model.Gender;
            user.DateOfBirth = model.DateOfBirth;
            user.UpdatedAt = DateTime.UtcNow;

            var result = await _userManager.UpdateAsync(user);
            if (result.Succeeded)
            {
                TempData["SuccessMessage"] = "Thông tin hồ sơ đã được cập nhật thành công!";
                return RedirectToAction(nameof(Profile));
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Orders()
        {
            var userId = _userManager.GetUserId(User);
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var orders = await _context.Orders
                .Where(o => o.UserId == userId)
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                .OrderByDescending(o => o.CreatedAt)
                .ToListAsync();

            return View(orders);
        }

        [HttpGet]
        public async Task<IActionResult> OrderDetail(Guid id)
        {
            var userId = _userManager.GetUserId(User);
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var order = await _context.Orders
                .Where(o => o.Id == id && o.UserId == userId)
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                .FirstOrDefaultAsync();

            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        [HttpGet]
        public async Task<IActionResult> Notifications(int page = 1)
        {
            var userId = _userManager.GetUserId(User);
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            const int pageSize = 20;
            var totalNotifications = await _context.Notifications
                .Where(n => n.UserId == userId)
                .CountAsync();

            var notifications = await _context.Notifications
                .Where(n => n.UserId == userId)
                .OrderByDescending(n => n.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var viewModel = new UserNotificationsViewModel
            {
                Notifications = notifications,
                CurrentPage = page,
                PageSize = pageSize,
                TotalNotifications = totalNotifications,
                TotalPages = (int)Math.Ceiling(totalNotifications / (double)pageSize)
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MarkNotificationAsRead([FromBody] int notificationId)
        {
            var userId = _userManager.GetUserId(User);
            if (userId == null)
            {
                return Json(new { success = false, message = "Unauthorized" });
            }

            var notification = await _context.Notifications
                .Where(n => n.Id == notificationId && n.UserId == userId)
                .FirstOrDefaultAsync();

            if (notification == null)
            {
                return Json(new { success = false, message = "Notification not found" });
            }

            notification.IsRead = true;
            notification.ReadAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return Json(new { success = true });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MarkAllNotificationsAsRead()
        {
            var userId = _userManager.GetUserId(User);
            if (userId == null)
            {
                return Json(new { success = false, message = "Unauthorized" });
            }

            var unreadNotifications = await _context.Notifications
                .Where(n => n.UserId == userId && !n.IsRead)
                .ToListAsync();

            foreach (var notification in unreadNotifications)
            {
                notification.IsRead = true;
                notification.ReadAt = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync();

            return Json(new { success = true });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateAvatar(IFormFile avatarFile)
        {
            try
            {
                var userId = _userManager.GetUserId(User);
                if (userId == null)
                {
                    TempData["Error"] = "Vui lòng đăng nhập để cập nhật ảnh đại diện.";
                    return RedirectToAction("Login", "Account");
                }

                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    TempData["Error"] = "Không tìm thấy người dùng.";
                    return RedirectToAction("Login", "Account");
                }

                // Validate file
                if (avatarFile == null || avatarFile.Length == 0)
                {
                    TempData["Error"] = "Vui lòng chọn một file ảnh.";
                    return RedirectToAction(nameof(Profile));
                }

                // Check file size (max 5MB)
                if (avatarFile.Length > 5 * 1024 * 1024)
                {
                    TempData["Error"] = "Kích thước file không được vượt quá 5MB.";
                    return RedirectToAction(nameof(Profile));
                }

                // Check file extension
                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
                var fileExtension = Path.GetExtension(avatarFile.FileName).ToLowerInvariant();
                if (!allowedExtensions.Contains(fileExtension))
                {
                    TempData["Error"] = "Chỉ chấp nhận file ảnh định dạng JPG, PNG, GIF hoặc WEBP.";
                    return RedirectToAction(nameof(Profile));
                }

                // Create avatars directory if not exists
                var avatarsPath = Path.Combine(_webHostEnvironment.WebRootPath, "images", "avatars");
                if (!Directory.Exists(avatarsPath))
                {
                    Directory.CreateDirectory(avatarsPath);
                }

                // Delete old avatar if exists
                if (!string.IsNullOrEmpty(user.Avatar))
                {
                    var oldAvatarPath = Path.Combine(_webHostEnvironment.WebRootPath, user.Avatar.TrimStart('/'));
                    if (System.IO.File.Exists(oldAvatarPath))
                    {
                        try
                        {
                            System.IO.File.Delete(oldAvatarPath);
                            _logger.LogInformation($"Deleted old avatar: {oldAvatarPath}");
                        }
                        catch (Exception ex)
                        {
                            _logger.LogWarning($"Failed to delete old avatar: {ex.Message}");
                        }
                    }
                }

                // Generate unique filename
                var uniqueFileName = $"{userId}_{Guid.NewGuid()}{fileExtension}";
                var filePath = Path.Combine(avatarsPath, uniqueFileName);

                // Save file
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await avatarFile.CopyToAsync(fileStream);
                }

                // Update user avatar path
                user.Avatar = $"/images/avatars/{uniqueFileName}";
                user.UpdatedAt = DateTime.UtcNow;

                var result = await _userManager.UpdateAsync(user);
                if (result.Succeeded)
                {
                    _logger.LogInformation($"User {user.Email} updated avatar successfully: {user.Avatar}");
                    TempData["Success"] = "Cập nhật ảnh đại diện thành công!";
                }
                else
                {
                    // Delete uploaded file if update failed
                    if (System.IO.File.Exists(filePath))
                    {
                        System.IO.File.Delete(filePath);
                    }
                    TempData["Error"] = "Có lỗi xảy ra khi cập nhật ảnh đại diện. Vui lòng thử lại.";
                }

                return RedirectToAction(nameof(Profile));
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error updating avatar: {ex.Message}");
                TempData["Error"] = "Có lỗi xảy ra khi cập nhật ảnh đại diện. Vui lòng thử lại.";
                return RedirectToAction(nameof(Profile));
            }
        }
    }
}