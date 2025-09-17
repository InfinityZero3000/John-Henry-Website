using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using JohnHenryFashionWeb.Data;
using JohnHenryFashionWeb.Models;
using JohnHenryFashionWeb.ViewModels;
using JohnHenryFashionWeb.Services;
using System.Security.Claims;

namespace JohnHenryFashionWeb.Controllers
{
    public class ProductsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SeoService _seoService;

        public ProductsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, SeoService seoService)
        {
            _context = context;
            _userManager = userManager;
            _seoService = seoService;
        }

        // GET: Products
        public async Task<IActionResult> Index(ProductSearchFilterViewModel model)
        {
            // Initialize the model if it's null
            model ??= new ProductSearchFilterViewModel();

            // Load dropdown data
            await LoadFilterData(model);

            // Build the query
            var query = _context.Products
                .Include(p => p.Category)
                .Include(p => p.Brand)
                .Where(p => p.IsActive);

            // Apply search filters
            query = ApplySearchFilters(query, model);

            // Get total count before pagination
            var totalCount = await query.CountAsync();

            // Apply sorting
            query = ApplySorting(query, model.SortBy);

            // Apply pagination
            var products = await query
                .Skip((model.Page - 1) * model.PageSize)
                .Take(model.PageSize)
                .Select(p => new ProductSearchItemViewModel
                {
                    Id = p.Id,
                    Name = p.Name,
                    ShortDescription = p.ShortDescription,
                    Price = p.Price,
                    SalePrice = p.SalePrice,
                    ImageUrl = p.FeaturedImageUrl,
                    CategoryName = p.Category.Name,
                    BrandName = p.Brand != null ? p.Brand.Name : null,
                    IsFeatured = p.IsFeatured,
                    InStock = p.InStock,
                    StockQuantity = p.StockQuantity,
                    CreatedAt = p.CreatedAt,
                    Size = p.Size,
                    Color = p.Color,
                    Material = p.Material,
                    AverageRating = _context.ProductReviews
                        .Where(r => r.ProductId == p.Id && r.IsApproved)
                        .Average(r => (double?)r.Rating) ?? 0,
                    ReviewCount = _context.ProductReviews
                        .Count(r => r.ProductId == p.Id && r.IsApproved)
                })
                .ToListAsync();

            // Set up the results
            model.Results = new PagedProductListViewModel
            {
                Products = products,
                TotalCount = totalCount,
                PageNumber = model.Page,
                PageSize = model.PageSize
            };

            return View(model);
        }

        // GET: Products/Details/5
        public async Task<IActionResult> Details(Guid id)
        {
            var product = await _context.Products
                .Include(p => p.Category)
                .Include(p => p.Brand)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (product == null)
            {
                return NotFound();
            }

            // Check if user is authenticated and get wishlist status
            bool isInWishlist = false;
            bool canReview = false;
            if (User.Identity?.IsAuthenticated == true)
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (!string.IsNullOrEmpty(userId))
                {
                    isInWishlist = await _context.Wishlists
                        .AnyAsync(w => w.UserId == userId && w.ProductId == product.Id);

                    // Check if user can leave a review
                    var hasPurchased = await _context.OrderItems
                        .Include(oi => oi.Order)
                        .AnyAsync(oi => oi.ProductId == product.Id && 
                                       oi.Order.UserId == userId && 
                                       oi.Order.Status == "delivered");

                    var hasReviewed = await _context.ProductReviews
                        .AnyAsync(r => r.ProductId == product.Id && r.UserId == userId);

                    canReview = hasPurchased && !hasReviewed;
                }
            }

            // Get recent reviews (limit to 5)
            var recentReviews = await _context.ProductReviews
                .Include(r => r.User)
                .Where(r => r.ProductId == product.Id && r.IsApproved)
                .OrderByDescending(r => r.CreatedAt)
                .Take(5)
                .ToListAsync();

            // Calculate rating statistics
            var ratingStats = await _context.ProductReviews
                .Where(r => r.ProductId == product.Id && r.IsApproved)
                .GroupBy(r => r.Rating)
                .Select(g => new { Rating = g.Key, Count = g.Count() })
                .ToListAsync();

            var totalReviews = ratingStats.Sum(r => r.Count);
            var averageRating = totalReviews > 0 ? 
                ratingStats.Sum(r => r.Rating * r.Count) / (double)totalReviews : 0;

            // Generate SEO data
            var productSeoData = new ProductSeoData
            {
                Name = product.Name,
                Description = product.Description ?? product.ShortDescription ?? "",
                Price = product.Price,
                Currency = "VND",
                Availability = product.StockQuantity > 0 ? "InStock" : "OutOfStock",
                Brand = product.Brand?.Name ?? "John Henry Fashion",
                Category = product.Category?.Name ?? "Fashion"
            };

            var productUrl = Url.Action("Details", "Products", new { id = product.Id }, Request.Scheme) ?? "";
            var productJsonLd = _seoService.GenerateProductJsonLd(productSeoData, productUrl);
            
            var metaTitle = $"{product.Name} - John Henry Fashion";
            var metaDescription = _seoService.OptimizeMetaDescription(product.Description ?? product.ShortDescription ?? "", 160);
            
            // Generate breadcrumbs
            var breadcrumbs = new List<BreadcrumbItem>
            {
                new BreadcrumbItem { Name = "Trang chủ", Url = Url.Action("Index", "Home") ?? "/" },
                new BreadcrumbItem { Name = "Sản phẩm", Url = Url.Action("Index", "Products") ?? "/products" },
                new BreadcrumbItem { Name = product.Category?.Name ?? "Danh mục", Url = Url.Action("Index", "Products", new { categoryId = product.CategoryId }) ?? "/products" },
                new BreadcrumbItem { Name = product.Name, Url = "" }
            };

            var breadcrumbJsonLd = _seoService.GenerateBreadcrumbJsonLd(breadcrumbs);

            ViewBag.IsInWishlist = isInWishlist;
            ViewBag.IsAuthenticated = User.Identity?.IsAuthenticated == true;
            ViewBag.CanReview = canReview;
            ViewBag.RecentReviews = recentReviews;
            ViewBag.TotalReviews = totalReviews;
            ViewBag.AverageRating = averageRating;
            ViewBag.RatingStats = ratingStats;
            ViewBag.ProductJsonLd = productJsonLd;
            ViewBag.MetaTitle = metaTitle;
            ViewBag.MetaDescription = metaDescription;
            ViewBag.BreadcrumbJsonLd = breadcrumbJsonLd;
            ViewBag.Breadcrumbs = breadcrumbs;

            return View(product);
        }

        // POST: Products/AddToWishlist
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> AddToWishlist(Guid productId)
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
                var existingWishlistItem = await _context.Wishlists
                    .FirstOrDefaultAsync(w => w.UserId == userId && w.ProductId == productId);

                if (existingWishlistItem != null)
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

                return Json(new { success = true, message = "Product added to wishlist" });
            }
            catch (Exception)
            {
                return Json(new { success = false, message = "An error occurred while adding to wishlist" });
            }
        }

        // POST: Products/RemoveFromWishlist
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> RemoveFromWishlist(Guid productId)
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

                return Json(new { success = true, message = "Product removed from wishlist" });
            }
            catch (Exception)
            {
                return Json(new { success = false, message = "An error occurred while removing from wishlist" });
            }
        }

        // POST: Products/AddToCart
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> AddToCart(Guid productId, int quantity = 1, string? size = null, string? color = null)
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
                if (product.StockQuantity < quantity)
                {
                    return Json(new { success = false, message = "Insufficient stock available" });
                }

                // Check if item already exists in cart with same specifications
                var existingCartItem = await _context.ShoppingCartItems
                    .FirstOrDefaultAsync(c => c.UserId == userId && 
                                            c.ProductId == productId && 
                                            c.Size == size && 
                                            c.Color == color);

                if (existingCartItem != null)
                {
                    // Update quantity
                    var newQuantity = existingCartItem.Quantity + quantity;
                    if (product.StockQuantity < newQuantity)
                    {
                        return Json(new { success = false, message = "Insufficient stock for requested quantity" });
                    }

                    existingCartItem.Quantity = newQuantity;
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
                        Quantity = quantity,
                        Size = size,
                        Color = color,
                        Price = product.Price,
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
            catch (Exception)
            {
                return Json(new { success = false, message = "An error occurred while adding to cart" });
            }
        }

        // GET: Products/IsInWishlist
        [HttpGet]
        public async Task<IActionResult> IsInWishlist(Guid productId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Json(new { isInWishlist = false });
            }

            var isInWishlist = await _context.Wishlists
                .AnyAsync(w => w.UserId == userId && w.ProductId == productId);

            return Json(new { isInWishlist = isInWishlist });
        }

        // GET: Products/GetCartCount
        [HttpGet]
        [Authorize]
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

        #region Helper Methods

        private async Task LoadFilterData(ProductSearchFilterViewModel model)
        {
            // Load categories
            model.Categories = await _context.Categories
                .Where(c => c.IsActive)
                .OrderBy(c => c.Name)
                .ToListAsync();

            // Load brands
            model.Brands = await _context.Brands
                .Where(b => b.IsActive)
                .OrderBy(b => b.Name)
                .ToListAsync();

            // Load available sizes, colors, materials
            var products = _context.Products.Where(p => p.IsActive);

            model.AvailableSizes = await products
                .Where(p => !string.IsNullOrEmpty(p.Size))
                .Select(p => p.Size!)
                .Distinct()
                .OrderBy(s => s)
                .ToListAsync();

            model.AvailableColors = await products
                .Where(p => !string.IsNullOrEmpty(p.Color))
                .Select(p => p.Color!)
                .Distinct()
                .OrderBy(c => c)
                .ToListAsync();

            model.AvailableMaterials = await products
                .Where(p => !string.IsNullOrEmpty(p.Material))
                .Select(p => p.Material!)
                .Distinct()
                .OrderBy(m => m)
                .ToListAsync();

            // Get price range
            if (await products.AnyAsync())
            {
                model.MinPriceRange = await products.MinAsync(p => p.Price);
                model.MaxPriceRange = await products.MaxAsync(p => p.Price);
            }
        }

        private IQueryable<Product> ApplySearchFilters(IQueryable<Product> query, ProductSearchFilterViewModel model)
        {
            // Search term
            if (!string.IsNullOrWhiteSpace(model.SearchTerm))
            {
                var searchTerm = model.SearchTerm.ToLower();
                query = query.Where(p => 
                    p.Name.ToLower().Contains(searchTerm) ||
                    p.ShortDescription!.ToLower().Contains(searchTerm) ||
                    p.Description!.ToLower().Contains(searchTerm) ||
                    p.SKU.ToLower().Contains(searchTerm));
            }

            // Category filter
            if (model.CategoryId.HasValue)
            {
                query = query.Where(p => p.CategoryId == model.CategoryId.Value);
            }

            // Brand filter
            if (model.BrandId.HasValue)
            {
                query = query.Where(p => p.BrandId == model.BrandId.Value);
            }

            // Price range filter
            if (model.MinPrice.HasValue)
            {
                query = query.Where(p => p.Price >= model.MinPrice.Value);
            }

            if (model.MaxPrice.HasValue)
            {
                query = query.Where(p => p.Price <= model.MaxPrice.Value);
            }

            // Size filter
            if (!string.IsNullOrWhiteSpace(model.Size))
            {
                query = query.Where(p => p.Size == model.Size);
            }

            // Color filter
            if (!string.IsNullOrWhiteSpace(model.Color))
            {
                query = query.Where(p => p.Color == model.Color);
            }

            // Material filter
            if (!string.IsNullOrWhiteSpace(model.Material))
            {
                query = query.Where(p => p.Material == model.Material);
            }

            // Featured filter
            if (model.IsFeatured.HasValue)
            {
                query = query.Where(p => p.IsFeatured == model.IsFeatured.Value);
            }

            // In stock filter
            if (model.InStock.HasValue)
            {
                if (model.InStock.Value)
                {
                    query = query.Where(p => p.InStock && p.StockQuantity > 0);
                }
                else
                {
                    query = query.Where(p => !p.InStock || p.StockQuantity <= 0);
                }
            }

            return query;
        }

        private IQueryable<Product> ApplySorting(IQueryable<Product> query, string sortBy)
        {
            return sortBy?.ToLower() switch
            {
                "name" => query.OrderBy(p => p.Name),
                "price_asc" => query.OrderBy(p => p.Price),
                "price_desc" => query.OrderByDescending(p => p.Price),
                "newest" => query.OrderByDescending(p => p.CreatedAt),
                "featured" => query.OrderByDescending(p => p.IsFeatured).ThenBy(p => p.Name),
                "popularity" => query.OrderByDescending(p => p.ViewCount).ThenBy(p => p.Name),
                _ => query.OrderBy(p => p.Name)
            };
        }

        #endregion

        #region AJAX Actions

        [HttpGet]
        public async Task<IActionResult> QuickSearch(string? query, int limit = 5)
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                return Json(new { results = new List<object>() });
            }

            var searchTerm = query.ToLower();
            var products = await _context.Products
                .Include(p => p.Category)
                .Where(p => p.IsActive && 
                           (p.Name.ToLower().Contains(searchTerm) || 
                            p.SKU.ToLower().Contains(searchTerm)))
                .Take(limit)
                .Select(p => new
                {
                    id = p.Id,
                    name = p.Name,
                    price = p.Price,
                    salePrice = p.SalePrice,
                    imageUrl = p.FeaturedImageUrl,
                    categoryName = p.Category.Name,
                    priceFormatted = (p.SalePrice ?? p.Price).ToString("N0") + " VNĐ"
                })
                .ToListAsync();

            return Json(new { results = products });
        }

        [HttpGet]
        public async Task<IActionResult> GetFilterOptions(string type)
        {
            switch (type.ToLower())
            {
                case "categories":
                    var categories = await _context.Categories
                        .Where(c => c.IsActive)
                        .Select(c => new { id = c.Id, name = c.Name })
                        .OrderBy(c => c.name)
                        .ToListAsync();
                    return Json(categories);

                case "brands":
                    var brands = await _context.Brands
                        .Where(b => b.IsActive)
                        .Select(b => new { id = b.Id, name = b.Name })
                        .OrderBy(b => b.name)
                        .ToListAsync();
                    return Json(brands);

                case "sizes":
                    var sizes = await _context.Products
                        .Where(p => p.IsActive && !string.IsNullOrEmpty(p.Size))
                        .Select(p => p.Size!)
                        .Distinct()
                        .OrderBy(s => s)
                        .ToListAsync();
                    return Json(sizes);

                case "colors":
                    var colors = await _context.Products
                        .Where(p => p.IsActive && !string.IsNullOrEmpty(p.Color))
                        .Select(p => p.Color!)
                        .Distinct()
                        .OrderBy(c => c)
                        .ToListAsync();
                    return Json(colors);

                case "materials":
                    var materials = await _context.Products
                        .Where(p => p.IsActive && !string.IsNullOrEmpty(p.Material))
                        .Select(p => p.Material!)
                        .Distinct()
                        .OrderBy(m => m)
                        .ToListAsync();
                    return Json(materials);

                default:
                    return Json(new List<object>());
            }
        }

        #endregion
    }
}
