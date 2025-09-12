using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using JohnHenryFashionWeb.Data;
using JohnHenryFashionWeb.Models;
using System.Security.Claims;

namespace JohnHenryFashionWeb.Controllers
{
    [Authorize]
    public class CartController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public CartController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Cart
        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Login", "Account");
            }

            var cartItems = await _context.ShoppingCartItems
                .Include(c => c.Product)
                .ThenInclude(p => p.Category)
                .Where(c => c.UserId == userId)
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync();

            ViewBag.CartTotal = cartItems.Sum(c => c.Price * c.Quantity);
            ViewBag.CartCount = cartItems.Sum(c => c.Quantity);

            return View(cartItems);
        }

        // POST: Cart/UpdateQuantity
        [HttpPost]
        public async Task<IActionResult> UpdateQuantity(Guid cartItemId, int quantity)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                {
                    return Json(new { success = false, message = "User not authenticated" });
                }

                var cartItem = await _context.ShoppingCartItems
                    .Include(c => c.Product)
                    .FirstOrDefaultAsync(c => c.Id == cartItemId && c.UserId == userId);

                if (cartItem == null)
                {
                    return Json(new { success = false, message = "Cart item not found" });
                }

                if (quantity <= 0)
                {
                    return Json(new { success = false, message = "Quantity must be greater than 0" });
                }

                // Check stock availability
                if (cartItem.Product.StockQuantity < quantity)
                {
                    return Json(new { 
                        success = false, 
                        message = $"Only {cartItem.Product.StockQuantity} items available in stock" 
                    });
                }

                cartItem.Quantity = quantity;
                cartItem.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                // Calculate new totals
                var cartItems = await _context.ShoppingCartItems
                    .Where(c => c.UserId == userId)
                    .ToListAsync();

                var cartTotal = cartItems.Sum(c => c.Price * c.Quantity);
                var cartCount = cartItems.Sum(c => c.Quantity);
                var itemTotal = cartItem.Price * cartItem.Quantity;

                return Json(new { 
                    success = true, 
                    message = "Quantity updated",
                    cartTotal = cartTotal,
                    cartCount = cartCount,
                    itemTotal = itemTotal
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "An error occurred while updating quantity" });
            }
        }

        // POST: Cart/RemoveItem
        [HttpPost]
        public async Task<IActionResult> RemoveItem(Guid cartItemId)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                {
                    return Json(new { success = false, message = "User not authenticated" });
                }

                var cartItem = await _context.ShoppingCartItems
                    .FirstOrDefaultAsync(c => c.Id == cartItemId && c.UserId == userId);

                if (cartItem == null)
                {
                    return Json(new { success = false, message = "Cart item not found" });
                }

                _context.ShoppingCartItems.Remove(cartItem);
                await _context.SaveChangesAsync();

                // Calculate new totals
                var cartItems = await _context.ShoppingCartItems
                    .Where(c => c.UserId == userId)
                    .ToListAsync();

                var cartTotal = cartItems.Sum(c => c.Price * c.Quantity);
                var cartCount = cartItems.Sum(c => c.Quantity);

                return Json(new { 
                    success = true, 
                    message = "Item removed from cart",
                    cartTotal = cartTotal,
                    cartCount = cartCount
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "An error occurred while removing item" });
            }
        }

        // POST: Cart/ClearCart
        [HttpPost]
        public async Task<IActionResult> ClearCart()
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                {
                    return Json(new { success = false, message = "User not authenticated" });
                }

                var cartItems = await _context.ShoppingCartItems
                    .Where(c => c.UserId == userId)
                    .ToListAsync();

                _context.ShoppingCartItems.RemoveRange(cartItems);
                await _context.SaveChangesAsync();

                return Json(new { 
                    success = true, 
                    message = "Cart cleared successfully",
                    cartTotal = 0,
                    cartCount = 0
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "An error occurred while clearing cart" });
            }
        }

        // GET: Cart/Checkout
        public async Task<IActionResult> Checkout()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Login", "Account");
            }

            var cartItems = await _context.ShoppingCartItems
                .Include(c => c.Product)
                .Where(c => c.UserId == userId)
                .ToListAsync();

            if (!cartItems.Any())
            {
                TempData["Message"] = "Your cart is empty";
                return RedirectToAction("Index");
            }

            ViewBag.CartTotal = cartItems.Sum(c => c.Price * c.Quantity);
            ViewBag.CartCount = cartItems.Sum(c => c.Quantity);
            ViewBag.ShippingFee = 30000; // 30k VND shipping fee
            ViewBag.GrandTotal = ViewBag.CartTotal + ViewBag.ShippingFee;

            return View(cartItems);
        }

        // GET: Cart/GetCartCount
        [HttpGet]
        public async Task<IActionResult> GetCartCount()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Json(new { cartCount = 0 });
            }

            var cartCount = await _context.ShoppingCartItems
                .Where(c => c.UserId == userId)
                .SumAsync(c => c.Quantity);

            return Json(new { cartCount = cartCount });
        }
    }
}
