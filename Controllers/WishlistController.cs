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
    public class WishlistController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public WishlistController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Wishlist
        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Login", "Account");
            }

            var wishlistItems = await _context.Wishlists
                .Include(w => w.Product)
                .ThenInclude(p => p.Category)
                .Where(w => w.UserId == userId)
                .OrderByDescending(w => w.CreatedAt)
                .ToListAsync();

            ViewBag.WishlistCount = wishlistItems.Count;

            return View(wishlistItems);
        }

        // POST: Wishlist/Add
        [HttpPost]
        public async Task<IActionResult> Add(Guid productId)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                {
                    return Json(new { success = false, message = "User not authenticated" });
                }

                // Check if product exists
                var product = await _context.Products.FindAsync(productId);
                if (product == null)
                {
                    return Json(new { success = false, message = "Product not found" });
                }

                // Check if already in wishlist
                var existingItem = await _context.Wishlists
                    .FirstOrDefaultAsync(w => w.UserId == userId && w.ProductId == productId);

                if (existingItem != null)
                {
                    return Json(new { success = false, message = "Product already in wishlist" });
                }

                // Add to wishlist
                var wishlistItem = new Wishlist
                {
                    Id = Guid.NewGuid(),
                    UserId = userId,
                    ProductId = productId,
                    CreatedAt = DateTime.UtcNow
                };

                _context.Wishlists.Add(wishlistItem);
                await _context.SaveChangesAsync();

                // Get updated count
                var wishlistCount = await _context.Wishlists
                    .CountAsync(w => w.UserId == userId);

                return Json(new { 
                    success = true, 
                    message = "Product added to wishlist",
                    wishlistCount = wishlistCount
                });
            }
            catch (Exception ex)
            {
                // Log the error for debugging
                Console.WriteLine($"Error adding to wishlist: {ex.Message}");
                return Json(new { success = false, message = "An error occurred while adding to wishlist" });
            }
        }

        // POST: Wishlist/Remove
        [HttpPost]
        public async Task<IActionResult> Remove(Guid productId)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                {
                    return Json(new { success = false, message = "User not authenticated" });
                }

                var wishlistItem = await _context.Wishlists
                    .FirstOrDefaultAsync(w => w.UserId == userId && w.ProductId == productId);

                if (wishlistItem == null)
                {
                    return Json(new { success = false, message = "Product not in wishlist" });
                }

                _context.Wishlists.Remove(wishlistItem);
                await _context.SaveChangesAsync();

                // Get updated count
                var wishlistCount = await _context.Wishlists
                    .CountAsync(w => w.UserId == userId);

                return Json(new { 
                    success = true, 
                    message = "Product removed from wishlist",
                    wishlistCount = wishlistCount
                });
            }
            catch (Exception ex)
            {
                // Log the error for debugging
                Console.WriteLine($"Error removing from wishlist: {ex.Message}");
                return Json(new { success = false, message = "An error occurred while removing from wishlist" });
            }
        }

        // POST: Wishlist/Clear
        [HttpPost]
        public async Task<IActionResult> Clear()
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                {
                    return Json(new { success = false, message = "User not authenticated" });
                }

                var wishlistItems = await _context.Wishlists
                    .Where(w => w.UserId == userId)
                    .ToListAsync();

                _context.Wishlists.RemoveRange(wishlistItems);
                await _context.SaveChangesAsync();

                return Json(new { 
                    success = true, 
                    message = "Wishlist cleared",
                    wishlistCount = 0
                });
            }
            catch (Exception ex)
            {
                // Log the error for debugging
                Console.WriteLine($"Error clearing wishlist: {ex.Message}");
                return Json(new { success = false, message = "An error occurred while clearing wishlist" });
            }
        }

        // POST: Wishlist/AddToCart
        [HttpPost]
        public async Task<IActionResult> AddToCart(Guid productId)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                {
                    return Json(new { success = false, message = "User not authenticated" });
                }

                // Check if product exists and is available
                var product = await _context.Products.FindAsync(productId);
                if (product == null || !product.IsActive)
                {
                    return Json(new { success = false, message = "Product not available" });
                }

                // Check stock availability
                if (product.StockQuantity < 1)
                {
                    return Json(new { success = false, message = "Product out of stock" });
                }

                // Check if item already exists in cart
                var existingCartItem = await _context.ShoppingCartItems
                    .FirstOrDefaultAsync(c => c.UserId == userId && c.ProductId == productId);

                if (existingCartItem != null)
                {
                    // Update quantity
                    existingCartItem.Quantity += 1;
                    existingCartItem.UpdatedAt = DateTime.UtcNow;
                }
                else
                {
                    // Add new cart item
                    var cartItem = new ShoppingCartItem
                    {
                        Id = Guid.NewGuid(),
                        UserId = userId,
                        ProductId = productId,
                        Quantity = 1,
                        Price = product.SalePrice ?? product.Price,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    };

                    _context.ShoppingCartItems.Add(cartItem);
                }

                await _context.SaveChangesAsync();

                // Get cart count for response
                var cartCount = await _context.ShoppingCartItems
                    .Where(c => c.UserId == userId)
                    .SumAsync(c => c.Quantity);

                return Json(new { 
                    success = true, 
                    message = "Product added to cart",
                    cartCount = cartCount
                });
            }
            catch (Exception ex)
            {
                // Log the error for debugging
                Console.WriteLine($"Error adding to cart from wishlist: {ex.Message}");
                return Json(new { success = false, message = "An error occurred while adding to cart" });
            }
        }

        // GET: Wishlist/GetCount
        [HttpGet]
        public async Task<IActionResult> GetCount()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Json(new { wishlistCount = 0 });
            }

            var wishlistCount = await _context.Wishlists
                .CountAsync(w => w.UserId == userId);

            return Json(new { wishlistCount = wishlistCount });
        }
    }
}
