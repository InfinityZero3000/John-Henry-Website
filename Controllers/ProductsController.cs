using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using JohnHenryFashionWeb.Data;
using JohnHenryFashionWeb.Models;
using System.Security.Claims;

namespace JohnHenryFashionWeb.Controllers
{
    public class ProductsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<ProductsController> _logger;

        public ProductsController(ApplicationDbContext context, ILogger<ProductsController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // POST: Products/AddToCart
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> AddToCart([FromForm] string productId, [FromForm] int quantity = 1, [FromForm] string? size = null, [FromForm] string? color = null)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                {
                    return Json(new { success = false, message = "Bạn cần đăng nhập để thêm sản phẩm vào giỏ hàng" });
                }

                // Find product by SKU
                var product = await _context.Products
                    .FirstOrDefaultAsync(p => p.SKU == productId && p.IsActive);

                if (product == null)
                {
                    _logger.LogWarning("Product not found with SKU: {ProductId}", productId);
                    return Json(new { success = false, message = "Không tìm thấy sản phẩm" });
                }

                // Check stock availability
                if (product.StockQuantity < quantity)
                {
                    return Json(new { 
                        success = false, 
                        message = $"Chỉ còn {product.StockQuantity} sản phẩm trong kho" 
                    });
                }

                // Check if item already exists in cart
                var existingCartItem = await _context.ShoppingCartItems
                    .FirstOrDefaultAsync(c => c.UserId == userId && c.ProductId == product.Id);

                if (existingCartItem != null)
                {
                    // Update quantity
                    var newQuantity = existingCartItem.Quantity + quantity;
                    
                    if (product.StockQuantity < newQuantity)
                    {
                        return Json(new { 
                            success = false, 
                            message = $"Chỉ còn {product.StockQuantity} sản phẩm trong kho" 
                        });
                    }

                    existingCartItem.Quantity = newQuantity;
                    existingCartItem.UpdatedAt = DateTime.UtcNow;
                    
                    _logger.LogInformation("Updated cart item quantity for user {UserId}, product {ProductId}, new quantity: {Quantity}", 
                        userId, product.Id, newQuantity);
                }
                else
                {
                    // Create new cart item
                    var cartItem = new ShoppingCartItem
                    {
                        Id = Guid.NewGuid(),
                        UserId = userId,
                        ProductId = product.Id,
                        Quantity = quantity,
                        Price = product.SalePrice ?? product.Price,
                        Size = size,
                        Color = color,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    };

                    _context.ShoppingCartItems.Add(cartItem);
                    
                    _logger.LogInformation("Added new cart item for user {UserId}, product {ProductId}, quantity: {Quantity}", 
                        userId, product.Id, quantity);
                }

                await _context.SaveChangesAsync();

                // Calculate cart totals
                var cartItems = await _context.ShoppingCartItems
                    .Where(c => c.UserId == userId)
                    .ToListAsync();

                var cartCount = cartItems.Sum(c => c.Quantity);
                var cartTotal = cartItems.Sum(c => c.Price * c.Quantity);

                return Json(new 
                { 
                    success = true, 
                    message = "Đã thêm sản phẩm vào giỏ hàng",
                    cartCount = cartCount,
                    cartTotal = cartTotal,
                    productName = product.Name
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding product to cart. ProductId: {ProductId}, UserId: {UserId}", 
                    productId, User.FindFirstValue(ClaimTypes.NameIdentifier));
                return Json(new { success = false, message = "Có lỗi xảy ra khi thêm sản phẩm vào giỏ hàng" });
            }
        }
    }
}
