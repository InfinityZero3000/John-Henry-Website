using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using JohnHenryFashionWeb.Data;
using JohnHenryFashionWeb.Models;
using JohnHenryFashionWeb.ViewModels;

namespace JohnHenryFashionWeb.Controllers
{
    [Authorize(Roles = UserRoles.Seller)]
    [Route("seller")]
    public class SellerController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public SellerController(
            ApplicationDbContext context, 
            UserManager<ApplicationUser> userManager,
            IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _userManager = userManager;
            _webHostEnvironment = webHostEnvironment;
        }

        [HttpGet("")]
        [HttpGet("dashboard")]
        public async Task<IActionResult> Dashboard()
        {
            var currentUser = await _userManager.GetUserAsync(User);
            var stats = await GetSellerDashboardStats(currentUser.Id);
            
            return View(stats);
        }

        private async Task<SellerDashboardViewModel> GetSellerDashboardStats(string sellerId)
        {
            // For now, we'll treat all products as belonging to sellers
            // In a real implementation, you'd have a SellerProducts relationship
            
            var myProductsCount = await _context.Products.CountAsync();
            var myOrdersCount = await _context.Orders.CountAsync();
            
            var myRevenue = await _context.Orders
                .Where(o => o.Status == "completed")
                .SumAsync(o => o.TotalAmount);

            var currentMonth = DateTime.UtcNow.Month;
            var currentYear = DateTime.UtcNow.Year;
            var monthlyRevenue = await _context.Orders
                .Where(o => o.CreatedAt.Month == currentMonth && 
                           o.CreatedAt.Year == currentYear &&
                           o.Status == "completed")
                .SumAsync(o => o.TotalAmount);

            var recentOrders = await _context.Orders
                .Include(o => o.User)
                .OrderByDescending(o => o.CreatedAt)
                .Take(5)
                .Select(o => new RecentOrder
                {
                    Id = o.Id,
                    OrderNumber = o.OrderNumber,
                    CustomerName = $"{o.User.FirstName} {o.User.LastName}".Trim(),
                    Total = o.TotalAmount,
                    Status = o.Status,
                    CreatedAt = o.CreatedAt
                })
                .ToListAsync();

            var topProducts = await _context.OrderItems
                .Include(oi => oi.Product)
                .Where(oi => oi.Order.Status == "completed")
                .GroupBy(oi => oi.ProductId)
                .Select(g => new TopSellingProduct
                {
                    ProductId = g.Key,
                    ProductName = g.First().Product.Name,
                    QuantitySold = g.Sum(oi => oi.Quantity),
                    Revenue = g.Sum(oi => oi.TotalPrice),
                    ImageUrl = g.First().Product.FeaturedImageUrl
                })
                .OrderByDescending(x => x.QuantitySold)
                .Take(5)
                .ToListAsync();

            return new SellerDashboardViewModel
            {
                MyProductsCount = myProductsCount,
                MyOrdersCount = myOrdersCount,
                MyRevenue = myRevenue,
                MonthlyRevenue = monthlyRevenue,
                MyRecentOrders = recentOrders,
                MyTopProducts = topProducts
            };
        }

        [HttpGet("products")]
        public async Task<IActionResult> Products(int page = 1, int pageSize = 10, string search = "", Guid? categoryId = null, string status = "")
        {
            // For now, show all products. In real implementation, filter by seller
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

            var totalCount = await query.CountAsync();
            var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);

            var products = await query
                .OrderByDescending(p => p.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(p => new ProductListItemViewModel
                {
                    Id = p.Id,
                    Name = p.Name,
                    SKU = p.SKU,
                    Price = p.Price,
                    SalePrice = p.SalePrice,
                    StockQuantity = p.StockQuantity,
                    Status = p.Status,
                    CategoryName = p.Category.Name,
                    BrandName = p.Brand != null ? p.Brand.Name : "",
                    FeaturedImageUrl = p.FeaturedImageUrl,
                    CreatedAt = p.CreatedAt,
                    IsFeatured = p.IsFeatured
                })
                .ToListAsync();

            var categories = await _context.Categories
                .Where(c => c.IsActive)
                .OrderBy(c => c.Name)
                .ToListAsync();

            var viewModel = new ProductListViewModel
            {
                Products = products,
                CurrentPage = page,
                TotalPages = totalPages,
                PageSize = pageSize,
                SearchTerm = search,
                CategoryId = categoryId,
                Status = status,
                Categories = categories
            };

            return View(viewModel);
        }

        [HttpGet("orders")]
        public async Task<IActionResult> Orders(int page = 1, int pageSize = 10, string search = "", string status = "", DateTime? fromDate = null, DateTime? toDate = null)
        {
            var query = _context.Orders
                .Include(o => o.User)
                .AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(o => o.OrderNumber.Contains(search) || 
                                        o.User.FirstName.Contains(search) || 
                                        o.User.LastName.Contains(search) ||
                                        o.User.Email.Contains(search));
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
                query = query.Where(o => o.CreatedAt <= toDate.Value.AddDays(1));
            }

            var totalCount = await query.CountAsync();
            var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);

            var orders = await query
                .OrderByDescending(o => o.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(o => new OrderListItemViewModel
                {
                    Id = o.Id,
                    OrderNumber = o.OrderNumber,
                    CustomerName = $"{o.User.FirstName} {o.User.LastName}".Trim(),
                    CustomerEmail = o.User.Email,
                    Total = o.TotalAmount,
                    Status = o.Status,
                    PaymentStatus = o.PaymentStatus,
                    CreatedAt = o.CreatedAt,
                    ItemCount = o.OrderItems.Count()
                })
                .ToListAsync();

            var viewModel = new OrderListViewModel
            {
                Orders = orders,
                CurrentPage = page,
                TotalPages = totalPages,
                PageSize = pageSize,
                SearchTerm = search,
                Status = status,
                FromDate = fromDate,
                ToDate = toDate
            };

            return View(viewModel);
        }

        [HttpGet("orders/{id}")]
        public async Task<IActionResult> OrderDetail(Guid id)
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

            var viewModel = new OrderDetailViewModel
            {
                Id = order.Id,
                OrderNumber = order.OrderNumber,
                CustomerName = $"{order.User.FirstName} {order.User.LastName}".Trim(),
                CustomerEmail = order.User.Email,
                CustomerPhone = order.User.PhoneNumber ?? "",
                ShippingAddress = order.ShippingAddress,
                BillingAddress = order.BillingAddress,
                Subtotal = order.TotalAmount - order.ShippingFee,
                ShippingCost = order.ShippingFee,
                Tax = order.Tax,
                Discount = order.DiscountAmount,
                Total = order.TotalAmount,
                Status = order.Status,
                PaymentStatus = order.PaymentStatus,
                PaymentMethod = order.PaymentMethod,
                CreatedAt = order.CreatedAt,
                Notes = order.Notes,
                Items = order.OrderItems.Select(oi => new OrderItemViewModel
                {
                    ProductId = oi.ProductId,
                    ProductName = oi.Product.Name,
                    ProductSKU = oi.Product.SKU,
                    ProductImage = oi.Product.FeaturedImageUrl,
                    Quantity = oi.Quantity,
                    Price = oi.UnitPrice,
                    Total = oi.TotalPrice
                }).ToList()
            };

            return View(viewModel);
        }

        [HttpPost("orders/{id}/update-status")]
        public async Task<IActionResult> UpdateOrderStatus(Guid id, string status)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order == null)
            {
                return NotFound();
            }

            order.Status = status;
            order.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            TempData["Success"] = "Trạng thái đơn hàng đã được cập nhật!";
            return RedirectToAction(nameof(OrderDetail), new { id });
        }

        // Copy the same product management methods from AdminController
        // but with seller-specific permissions if needed

        [HttpGet("products/create")]
        public async Task<IActionResult> CreateProduct()
        {
            var viewModel = new ProductCreateEditViewModel
            {
                Categories = await _context.Categories.Where(c => c.IsActive).OrderBy(c => c.Name).ToListAsync(),
                Brands = await _context.Brands.Where(b => b.IsActive).OrderBy(b => b.Name).ToListAsync()
            };

            return View(viewModel);
        }

        [HttpPost("products/create")]
        public async Task<IActionResult> CreateProduct(ProductCreateEditViewModel model)
        {
            if (ModelState.IsValid)
            {
                var product = new Product
                {
                    Id = Guid.NewGuid(),
                    Name = model.Name,
                    SKU = model.SKU,
                    Price = model.Price,
                    SalePrice = model.SalePrice,
                    CategoryId = model.CategoryId,
                    BrandId = model.BrandId,
                    ShortDescription = model.ShortDescription,
                    Description = model.Description,
                    StockQuantity = model.StockQuantity,
                    ManageStock = model.ManageStock,
                    InStock = model.InStock,
                    IsFeatured = model.IsFeatured,
                    IsActive = model.IsActive,
                    Size = model.Size,
                    Color = model.Color,
                    Material = model.Material,
                    Weight = model.Weight,
                    Dimensions = model.Dimensions,
                    Status = model.Status,
                    Slug = GenerateSlug(model.Name),
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                // Handle featured image upload
                if (model.FeaturedImage != null)
                {
                    product.FeaturedImageUrl = await SaveUploadedFile(model.FeaturedImage, "products");
                }

                // Handle gallery images upload
                if (model.GalleryImages != null && model.GalleryImages.Count > 0)
                {
                    var galleryUrls = new List<string>();
                    foreach (var image in model.GalleryImages)
                    {
                        var imageUrl = await SaveUploadedFile(image, "products");
                        galleryUrls.Add(imageUrl);
                    }
                    product.GalleryImages = galleryUrls.ToArray();
                }

                _context.Products.Add(product);
                await _context.SaveChangesAsync();

                TempData["Success"] = "Sản phẩm đã được tạo thành công!";
                return RedirectToAction(nameof(Products));
            }

            // Reload dropdown data if validation fails
            model.Categories = await _context.Categories.Where(c => c.IsActive).OrderBy(c => c.Name).ToListAsync();
            model.Brands = await _context.Brands.Where(b => b.IsActive).OrderBy(b => b.Name).ToListAsync();

            return View(model);
        }

        #region Helper Methods
        private string GenerateSlug(string text)
        {
            return text.ToLower()
                      .Replace("á", "a").Replace("à", "a").Replace("ả", "a").Replace("ã", "a").Replace("ạ", "a")
                      .Replace("ă", "a").Replace("ắ", "a").Replace("ằ", "a").Replace("ẳ", "a").Replace("ẵ", "a").Replace("ặ", "a")
                      .Replace("â", "a").Replace("ấ", "a").Replace("ầ", "a").Replace("ẩ", "a").Replace("ẫ", "a").Replace("ậ", "a")
                      .Replace("é", "e").Replace("è", "e").Replace("ẻ", "e").Replace("ẽ", "e").Replace("ẹ", "e")
                      .Replace("ê", "e").Replace("ế", "e").Replace("ề", "e").Replace("ể", "e").Replace("ễ", "e").Replace("ệ", "e")
                      .Replace("í", "i").Replace("ì", "i").Replace("ỉ", "i").Replace("ĩ", "i").Replace("ị", "i")
                      .Replace("ó", "o").Replace("ò", "o").Replace("ỏ", "o").Replace("õ", "o").Replace("ọ", "o")
                      .Replace("ô", "o").Replace("ố", "o").Replace("ồ", "o").Replace("ổ", "o").Replace("ỗ", "o").Replace("ộ", "o")
                      .Replace("ơ", "o").Replace("ớ", "o").Replace("ờ", "o").Replace("ở", "o").Replace("ỡ", "o").Replace("ợ", "o")
                      .Replace("ú", "u").Replace("ù", "u").Replace("ủ", "u").Replace("ũ", "u").Replace("ụ", "u")
                      .Replace("ư", "u").Replace("ứ", "u").Replace("ừ", "u").Replace("ử", "u").Replace("ữ", "u").Replace("ự", "u")
                      .Replace("ý", "y").Replace("ỳ", "y").Replace("ỷ", "y").Replace("ỹ", "y").Replace("ỵ", "y")
                      .Replace("đ", "d")
                      .Replace(" ", "-")
                      .Replace("--", "-")
                      .Trim('-');
        }

        private async Task<string> SaveUploadedFile(IFormFile file, string folder)
        {
            if (file == null || file.Length == 0)
                return string.Empty;

            var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images", folder);
            Directory.CreateDirectory(uploadsFolder);

            var uniqueFileName = Guid.NewGuid().ToString() + "_" + file.FileName;
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(fileStream);
            }

            return $"/images/{folder}/{uniqueFileName}";
        }
        #endregion
    }
}
