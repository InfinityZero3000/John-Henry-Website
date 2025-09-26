using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using JohnHenryFashionWeb.Models;
using JohnHenryFashionWeb.Data;
using JohnHenryFashionWeb.ViewModels;

namespace JohnHenryFashionWeb.Controllers
{
    public class UserDashboardController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;
        private readonly ILogger<UserDashboardController> _logger;

        public UserDashboardController(
            UserManager<ApplicationUser> userManager,
            ApplicationDbContext context,
            ILogger<UserDashboardController> logger)
        {
            _userManager = userManager;
            _context = context;
            _logger = logger;
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
                Avatar = user.Avatar
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
    }
}