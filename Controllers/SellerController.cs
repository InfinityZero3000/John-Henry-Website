using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using JohnHenryFashionWeb.Data;
using JohnHenryFashionWeb.Models;
using JohnHenryFashionWeb.ViewModels;

namespace JohnHenryFashionWeb.Controllers
{
    [Authorize(Roles = UserRoles.Seller, AuthenticationSchemes = "Identity.Application")]
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
            if (currentUser == null)
            {
                return RedirectToAction("Login", "Account");
            }
            
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

        [HttpGet("products/edit/{id}")]
        public async Task<IActionResult> EditProduct(Guid id)
        {
            var product = await _context.Products
                .Include(p => p.Category)
                .Include(p => p.Brand)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (product == null)
            {
                TempData["Error"] = "Không tìm thấy sản phẩm!";
                return RedirectToAction(nameof(Products));
            }

            var viewModel = new ProductCreateEditViewModel
            {
                Id = product.Id,
                Name = product.Name,
                SKU = product.SKU,
                Price = product.Price,
                SalePrice = product.SalePrice,
                CategoryId = product.CategoryId,
                BrandId = product.BrandId,
                ShortDescription = product.ShortDescription,
                Description = product.Description,
                StockQuantity = product.StockQuantity,
                ManageStock = product.ManageStock,
                InStock = product.InStock,
                IsFeatured = product.IsFeatured,
                IsActive = product.IsActive,
                Size = product.Size,
                Color = product.Color,
                Material = product.Material,
                Weight = product.Weight,
                Dimensions = product.Dimensions,
                Status = product.Status,
                FeaturedImageUrl = product.FeaturedImageUrl,
                ExistingGalleryImages = product.GalleryImages?.ToArray() ?? new string[0],
                Categories = await _context.Categories.Where(c => c.IsActive).OrderBy(c => c.Name).ToListAsync(),
                Brands = await _context.Brands.Where(b => b.IsActive).OrderBy(b => b.Name).ToListAsync()
            };

            return View(viewModel);
        }

        [HttpPost("products/edit/{id}")]
        public async Task<IActionResult> EditProduct(Guid id, ProductCreateEditViewModel model)
        {
            if (id != model.Id)
            {
                TempData["Error"] = "ID sản phẩm không khớp!";
                return RedirectToAction(nameof(Products));
            }

            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                TempData["Error"] = "Không tìm thấy sản phẩm!";
                return RedirectToAction(nameof(Products));
            }

            if (ModelState.IsValid)
            {
                product.Name = model.Name;
                product.SKU = model.SKU;
                product.Price = model.Price;
                product.SalePrice = model.SalePrice;
                product.CategoryId = model.CategoryId;
                product.BrandId = model.BrandId;
                product.ShortDescription = model.ShortDescription;
                product.Description = model.Description;
                product.StockQuantity = model.StockQuantity;
                product.ManageStock = model.ManageStock;
                product.InStock = model.InStock;
                product.IsFeatured = model.IsFeatured;
                product.IsActive = model.IsActive;
                product.Size = model.Size;
                product.Color = model.Color;
                product.Material = model.Material;
                product.Weight = model.Weight;
                product.Dimensions = model.Dimensions;
                product.Status = model.Status;
                product.Slug = GenerateSlug(model.Name);
                product.UpdatedAt = DateTime.UtcNow;

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

                _context.Update(product);
                await _context.SaveChangesAsync();

                TempData["Success"] = "Sản phẩm đã được cập nhật thành công!";
                return RedirectToAction(nameof(Products));
            }

            // Reload dropdown data if validation fails
            model.Categories = await _context.Categories.Where(c => c.IsActive).OrderBy(c => c.Name).ToListAsync();
            model.Brands = await _context.Brands.Where(b => b.IsActive).OrderBy(b => b.Name).ToListAsync();

            return View(model);
        }

        [HttpGet("inventory")]
        public async Task<IActionResult> Inventory(string search = "", bool lowStock = false)
        {
            var query = _context.Products
                .Include(p => p.Category)
                .Include(p => p.Brand)
                .AsQueryable();

            // TODO: Filter by seller when seller-product relationship is implemented
            // query = query.Where(p => p.SellerId == currentSellerId);

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(p => p.Name.Contains(search) || p.SKU.Contains(search));
            }

            if (lowStock)
            {
                query = query.Where(p => p.StockQuantity <= 10);
            }

            var products = await query
                .OrderBy(p => p.StockQuantity)
                .ThenBy(p => p.Name)
                .Select(p => new InventoryItemViewModel
                {
                    Id = p.Id,
                    ProductId = p.Id,
                    ProductName = p.Name,
                    SKU = p.SKU,
                    CurrentStock = p.StockQuantity,
                    CategoryName = p.Category.Name,
                    Price = p.Price,
                    LastUpdated = p.UpdatedAt
                })
                .ToListAsync();

            var viewModel = new InventoryListViewModel
            {
                Items = products,
                SearchTerm = search,
                Filter = lowStock ? "low_stock" : "all"
            };

            return View(viewModel);
        }

        [HttpPost("inventory/update-stock")]
        public async Task<IActionResult> UpdateStock([FromForm] Guid productId, [FromForm] int newStock, [FromForm] string reason = "")
        {
            var product = await _context.Products.FindAsync(productId);
            if (product == null)
            {
                return Json(new { success = false, message = "Không tìm thấy sản phẩm!" });
            }

            // TODO: Check if product belongs to current seller

            var oldStock = product.StockQuantity;
            product.StockQuantity = newStock;
            product.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return Json(new { 
                success = true, 
                message = $"Đã cập nhật tồn kho từ {oldStock} thành {newStock}!",
                newStock = newStock
            });
        }

        [HttpGet("sales")]
        public async Task<IActionResult> Sales(DateTime? fromDate = null, DateTime? toDate = null)
        {
            var startDate = fromDate ?? DateTime.UtcNow.AddMonths(-1);
            var endDate = toDate ?? DateTime.UtcNow;

            // TODO: Filter by seller when relationship is implemented
            var salesData = await _context.Orders
                .Where(o => o.CreatedAt >= startDate && o.CreatedAt <= endDate && o.Status == "completed")
                .GroupBy(o => o.CreatedAt.Date)
                .Select(g => new
                {
                    Date = g.Key,
                    Revenue = g.Sum(o => o.TotalAmount),
                    Orders = g.Count()
                })
                .OrderBy(x => x.Date)
                .ToListAsync();

            var viewModel = new SellerSalesViewModel
            {
                FromDate = startDate,
                ToDate = endDate,
                TotalRevenue = salesData.Sum(s => s.Revenue),
                TotalOrders = salesData.Sum(s => s.Orders),
                SalesData = salesData.Select(s => new DailySales
                {
                    Date = s.Date,
                    Revenue = s.Revenue,
                    OrderCount = s.Orders
                }).ToList()
            };

            return View(viewModel);
        }

        [HttpGet("analytics")]
        public async Task<IActionResult> Analytics()
        {
            // TODO: Implement seller-specific analytics
            var viewModel = new SellerAnalyticsViewModel
            {
                TopProducts = await _context.Products
                    .OrderByDescending(p => p.StockQuantity) // This should be orders count
                    .Take(10)
                    .Select(p => new TopSellingProduct
                    {
                        ProductName = p.Name,
                        QuantitySold = p.StockQuantity,
                        Revenue = p.Price * p.StockQuantity
                    })
                    .ToListAsync()
            };

            return View(viewModel);
        }

        [HttpGet("profile")]
        public async Task<IActionResult> Profile()
        {
            var currentUser = await _userManager.GetUserAsync(User);
            
            var viewModel = new SellerProfileViewModel
            {
                CompanyName = currentUser.CompanyName,
                BusinessLicense = currentUser.BusinessLicense,
                TaxCode = currentUser.TaxCode,
                Address = currentUser.Address,
                Phone = currentUser.Phone,
                Email = currentUser.Email,
                FirstName = currentUser.FirstName,
                LastName = currentUser.LastName,
                IsApproved = currentUser.IsApproved,
                ApprovedAt = currentUser.ApprovedAt,
                Notes = currentUser.Notes,
                CreatedAt = currentUser.CreatedAt,
                UpdatedAt = currentUser.UpdatedAt,
                IsActive = currentUser.IsActive
            };

            return View(viewModel);
        }

        [HttpPost("profile")]
        public async Task<IActionResult> Profile(SellerProfileViewModel model)
        {
            if (ModelState.IsValid)
            {
                var currentUser = await _userManager.GetUserAsync(User);
                
                currentUser.CompanyName = model.CompanyName;
                currentUser.BusinessLicense = model.BusinessLicense;
                currentUser.TaxCode = model.TaxCode;
                currentUser.Address = model.Address;
                currentUser.Phone = model.Phone;
                currentUser.FirstName = model.FirstName;
                currentUser.LastName = model.LastName;

                var result = await _userManager.UpdateAsync(currentUser);
                
                if (result.Succeeded)
                {
                    TempData["Success"] = "Hồ sơ đã được cập nhật thành công!";
                }
                else
                {
                    TempData["Error"] = "Có lỗi xảy ra khi cập nhật hồ sơ!";
                }
            }

            return View(model);
        }

        [HttpGet("settings")]
        public IActionResult Settings()
        {
            var viewModel = new SellerSettingsViewModel
            {
                // Load current settings
            };

            return View(viewModel);
        }

        [HttpPost("settings")]
        public IActionResult Settings(SellerSettingsViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Save settings
                TempData["Success"] = "Cài đặt đã được lưu thành công!";
            }

            return View(model);
        }

        // MARK: - Quản lý Coupons/Discounts
        [HttpGet("coupons")]
        public async Task<IActionResult> Coupons(int page = 1, int pageSize = 10, string search = "", string status = "")
        {
            var currentUser = await _userManager.GetUserAsync(User);
            
            var query = _context.Coupons.AsQueryable();
            
            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(c => c.Code.Contains(search) || c.Description.Contains(search));
            }
            
            if (!string.IsNullOrEmpty(status))
            {
                query = query.Where(c => c.IsActive == (status == "active"));
            }
            
            var totalCount = await query.CountAsync();
            var coupons = await query
                .OrderByDescending(c => c.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
            
            var model = new SellerCouponsViewModel
            {
                Coupons = coupons,
                CurrentPage = page,
                TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize),
                PageSize = pageSize,
                Search = search,
                Status = status,
                TotalCount = totalCount
            };
            
            return View(model);
        }

        [HttpGet("coupons/create")]
        public IActionResult CreateCoupon()
        {
            return View(new CouponCreateEditViewModel());
        }

        [HttpPost("coupons/create")]
        public async Task<IActionResult> CreateCoupon(CouponCreateEditViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var currentUser = await _userManager.GetUserAsync(User);
            
            var coupon = new Coupon
            {
                Id = Guid.NewGuid(),
                Code = model.Code.ToUpper(),
                Description = model.Description,
                Type = model.DiscountType,
                Value = model.DiscountValue,
                MinOrderAmount = model.MinOrderAmount,
                UsageLimit = model.UsageLimit,
                EndDate = model.ExpiryDate,
                IsActive = model.IsActive,
                CreatedAt = DateTime.UtcNow
            };
            
            _context.Coupons.Add(coupon);
            await _context.SaveChangesAsync();
            
            TempData["SuccessMessage"] = "Tạo mã giảm giá thành công!";
            return RedirectToAction("Coupons");
        }

        [HttpGet("coupons/edit/{id}")]
        public async Task<IActionResult> EditCoupon(Guid id)
        {
            var coupon = await _context.Coupons.FindAsync(id);
            if (coupon == null)
            {
                return NotFound();
            }
            
            var model = new CouponCreateEditViewModel
            {
                Id = coupon.Id,
                Code = coupon.Code,
                Description = coupon.Description ?? "",
                DiscountType = coupon.Type,
                DiscountValue = coupon.Value,
                MinOrderAmount = coupon.MinOrderAmount,
                UsageLimit = coupon.UsageLimit,
                ExpiryDate = coupon.EndDate ?? DateTime.Now.AddDays(30),
                IsActive = coupon.IsActive
            };
            
            return View(model);
        }

        [HttpPost("coupons/edit/{id}")]
        public async Task<IActionResult> EditCoupon(Guid id, CouponCreateEditViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var coupon = await _context.Coupons.FindAsync(id);
            if (coupon == null)
            {
                return NotFound();
            }
            
            coupon.Code = model.Code.ToUpper();
            coupon.Description = model.Description;
            coupon.Type = model.DiscountType;
            coupon.Value = model.DiscountValue;
            coupon.MinOrderAmount = model.MinOrderAmount;
            coupon.UsageLimit = model.UsageLimit;
            coupon.EndDate = model.ExpiryDate;
            coupon.IsActive = model.IsActive;
            coupon.UpdatedAt = DateTime.UtcNow;
            
            await _context.SaveChangesAsync();
            
            TempData["SuccessMessage"] = "Cập nhật mã giảm giá thành công!";
            return RedirectToAction("Coupons");
        }

        [HttpPost("coupons/delete/{id}")]
        public async Task<IActionResult> DeleteCoupon(Guid id)
        {
            var coupon = await _context.Coupons.FindAsync(id);
            if (coupon == null)
            {
                return NotFound();
            }
            
            _context.Coupons.Remove(coupon);
            await _context.SaveChangesAsync();
            
            TempData["SuccessMessage"] = "Xóa mã giảm giá thành công!";
            return RedirectToAction("Coupons");
        }

        // MARK: - Quản lý Reviews
        [HttpGet("reviews")]
        public async Task<IActionResult> Reviews(int page = 1, int pageSize = 10, string search = "", int? rating = null, string status = "")
        {
            var currentUser = await _userManager.GetUserAsync(User);
            
            var query = _context.ProductReviews
                .Include(r => r.Product)
                .Include(r => r.User)
                .AsQueryable();
            
            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(r => r.Product.Name.Contains(search) || 
                                        (r.User.FirstName != null && r.User.FirstName.Contains(search)) || 
                                        (r.User.LastName != null && r.User.LastName.Contains(search)));
            }
            
            if (rating.HasValue)
            {
                query = query.Where(r => r.Rating == rating.Value);
            }
            
            if (!string.IsNullOrEmpty(status))
            {
                query = query.Where(r => r.IsApproved == (status == "approved"));
            }
            
            var totalCount = await query.CountAsync();
            var reviews = await query
                .OrderByDescending(r => r.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
            
            var model = new SellerReviewsViewModel
            {
                Reviews = reviews,
                CurrentPage = page,
                TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize),
                PageSize = pageSize,
                Search = search,
                Rating = rating,
                Status = status,
                TotalCount = totalCount
            };
            
            return View(model);
        }

        [HttpPost("reviews/approve/{id}")]
        public async Task<IActionResult> ApproveReview(Guid id)
        {
            var review = await _context.ProductReviews.FindAsync(id);
            if (review == null)
            {
                return NotFound();
            }
            
            review.IsApproved = true;
            await _context.SaveChangesAsync();
            
            TempData["SuccessMessage"] = "Phê duyệt đánh giá thành công!";
            return RedirectToAction("Reviews");
        }

        [HttpPost("reviews/reject/{id}")]
        public async Task<IActionResult> RejectReview(Guid id)
        {
            var review = await _context.ProductReviews.FindAsync(id);
            if (review == null)
            {
                return NotFound();
            }
            
            review.IsApproved = false;
            await _context.SaveChangesAsync();
            
            TempData["SuccessMessage"] = "Từ chối đánh giá thành công!";
            return RedirectToAction("Reviews");
        }

        // MARK: - Quản lý Notifications
        [HttpGet("notifications")]
        public async Task<IActionResult> Notifications(int page = 1, int pageSize = 10, string type = "", bool? isRead = null)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
                return Challenge();
            
            var query = _context.Notifications
                .Where(n => n.UserId == currentUser.Id)
                .AsQueryable();
            
            if (!string.IsNullOrEmpty(type))
            {
                query = query.Where(n => n.Type == type);
            }
            
            if (isRead.HasValue)
            {
                query = query.Where(n => n.IsRead == isRead.Value);
            }
            
            var totalCount = await query.CountAsync();
            var notifications = await query
                .OrderByDescending(n => n.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
            
            var model = new SellerNotificationsViewModel
            {
                Notifications = notifications,
                CurrentPage = page,
                TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize),
                PageSize = pageSize,
                Type = type,
                IsRead = isRead,
                TotalCount = totalCount,
                UnreadCount = await _context.Notifications.CountAsync(n => n.UserId == currentUser.Id && !n.IsRead)
            };
            
            return View(model);
        }

        [HttpPost("notifications/mark-read/{id}")]
        public async Task<IActionResult> MarkNotificationAsRead(Guid id)
        {
            var notification = await _context.Notifications.FindAsync(id);
            if (notification == null)
            {
                return NotFound();
            }
            
            notification.IsRead = true;
            notification.ReadAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            
            return Json(new { success = true });
        }

        [HttpPost("notifications/mark-all-read")]
        public async Task<IActionResult> MarkAllNotificationsAsRead()
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
                return Challenge();
            
            var unreadNotifications = await _context.Notifications
                .Where(n => n.UserId == currentUser.Id && !n.IsRead)
                .ToListAsync();
            
            foreach (var notification in unreadNotifications)
            {
                notification.IsRead = true;
                notification.ReadAt = DateTime.UtcNow;
            }
            
            await _context.SaveChangesAsync();
            
            TempData["SuccessMessage"] = "Đã đánh dấu tất cả thông báo là đã đọc!";
            return RedirectToAction("Notifications");
        }

        // MARK: - Commission Management
        [HttpGet("commissions")]
        public async Task<IActionResult> Commissions(DateTime? fromDate = null, DateTime? toDate = null)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            
            if (!fromDate.HasValue)
                fromDate = DateTime.Now.AddMonths(-3);
            if (!toDate.HasValue)
                toDate = DateTime.Now;
            
            var orders = await _context.Orders
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                .Where(o => o.CreatedAt >= fromDate && o.CreatedAt <= toDate && o.Status == "completed")
                .OrderByDescending(o => o.CreatedAt)
                .ToListAsync();
            
            var commissionRate = 0.15m; // 15% commission rate
            var totalSales = orders.Sum(o => o.TotalAmount);
            var totalCommission = totalSales * commissionRate;
            var totalOrders = orders.Count;
            
            var monthlyData = orders
                .GroupBy(o => new { o.CreatedAt.Year, o.CreatedAt.Month })
                .Select(g => new MonthlyCommissionData
                {
                    Month = $"{g.Key.Month:D2}/{g.Key.Year}",
                    Sales = g.Sum(o => o.TotalAmount),
                    Commission = g.Sum(o => o.TotalAmount) * commissionRate,
                    OrderCount = g.Count()
                })
                .OrderBy(x => x.Month)
                .ToList();
            
            var model = new SellerCommissionsViewModel
            {
                FromDate = fromDate.Value,
                ToDate = toDate.Value,
                TotalSales = totalSales,
                TotalCommission = totalCommission,
                TotalOrders = totalOrders,
                CommissionRate = commissionRate,
                MonthlyData = monthlyData,
                RecentOrders = orders.Take(10).ToList()
            };
            
            return View(model);
        }

        // MARK: - Quản lý Customers
        [HttpGet("customers")]
        public async Task<IActionResult> Customers(int page = 1, int pageSize = 10, string search = "", string sortBy = "totalSpent")
        {
            var currentUser = await _userManager.GetUserAsync(User);
            
            // Get customers who have placed orders
            var customersQuery = _context.Orders
                .Include(o => o.User)
                .Where(o => o.Status == "completed")
                .GroupBy(o => o.UserId)
                .Select(g => new CustomerInfo
                {
                    UserId = g.Key,
                    FullName = g.First().User.FirstName + " " + g.First().User.LastName,
                    Email = g.First().User.Email ?? "",
                    Phone = g.First().User.PhoneNumber ?? "",
                    FirstOrderDate = g.Min(o => o.CreatedAt),
                    LastOrderDate = g.Max(o => o.CreatedAt),
                    TotalOrders = g.Count(),
                    TotalSpent = g.Sum(o => o.TotalAmount),
                    AverageOrderValue = (double)g.Average(o => o.TotalAmount),
                    Status = g.Any(o => o.CreatedAt > DateTime.Now.AddDays(-30)) ? "Active" : "Inactive"
                })
                .AsQueryable();
            
            if (!string.IsNullOrEmpty(search))
            {
                customersQuery = customersQuery.Where(c => 
                    c.FullName.Contains(search) || 
                    c.Email.Contains(search) || 
                    c.Phone.Contains(search));
            }
            
            switch (sortBy.ToLower())
            {
                case "name":
                    customersQuery = customersQuery.OrderBy(c => c.FullName);
                    break;
                case "orders":
                    customersQuery = customersQuery.OrderByDescending(c => c.TotalOrders);
                    break;
                case "recent":
                    customersQuery = customersQuery.OrderByDescending(c => c.LastOrderDate);
                    break;
                default: // totalSpent
                    customersQuery = customersQuery.OrderByDescending(c => c.TotalSpent);
                    break;
            }
            
            var totalCount = await customersQuery.CountAsync();
            var customers = await customersQuery
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
            
            // Get top customers
            var topCustomers = await _context.Orders
                .Include(o => o.User)
                .Where(o => o.Status == "completed")
                .GroupBy(o => o.UserId)
                .Select(g => new CustomerInfo
                {
                    UserId = g.Key,
                    FullName = g.First().User.FirstName + " " + g.First().User.LastName,
                    Email = g.First().User.Email ?? "",
                    TotalOrders = g.Count(),
                    TotalSpent = g.Sum(o => o.TotalAmount)
                })
                .OrderByDescending(c => c.TotalSpent)
                .Take(5)
                .ToListAsync();
            
            // Get new customers (last 30 days)
            var newCustomers = await _context.Orders
                .Include(o => o.User)
                .Where(o => o.CreatedAt > DateTime.Now.AddDays(-30))
                .GroupBy(o => o.UserId)
                .Where(g => g.Min(o => o.CreatedAt) > DateTime.Now.AddDays(-30))
                .Select(g => new CustomerInfo
                {
                    UserId = g.Key,
                    FullName = g.First().User.FirstName + " " + g.First().User.LastName,
                    Email = g.First().User.Email ?? "",
                    FirstOrderDate = g.Min(o => o.CreatedAt),
                    TotalOrders = g.Count(),
                    TotalSpent = g.Sum(o => o.TotalAmount)
                })
                .OrderByDescending(c => c.FirstOrderDate)
                .Take(10)
                .ToListAsync();
            
            var model = new SellerCustomersViewModel
            {
                Customers = customers,
                TopCustomers = topCustomers,
                NewCustomers = newCustomers,
                CurrentPage = page,
                TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize),
                PageSize = pageSize,
                Search = search,
                TotalCount = totalCount
            };
            
            return View(model);
        }

        // MARK: - Advanced Reports
        [HttpGet("reports")]
        public async Task<IActionResult> Reports(DateTime? fromDate = null, DateTime? toDate = null)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            
            if (!fromDate.HasValue)
                fromDate = DateTime.Now.AddMonths(-3);
            if (!toDate.HasValue)
                toDate = DateTime.Now;
            
            // Get orders for the period
            var orders = await _context.Orders
                .Include(o => o.OrderItems)
                .Include(o => o.User)
                .Where(o => o.CreatedAt >= fromDate && o.CreatedAt <= toDate)
                .ToListAsync();
            
            var completedOrders = orders.Where(o => o.Status == "completed").ToList();
            
            // Calculate metrics
            var totalRevenue = completedOrders.Sum(o => o.TotalAmount);
            var totalOrders = completedOrders.Count;
            var totalCustomers = completedOrders.Select(o => o.UserId).Distinct().Count();
            
            var monthlyRevenue = completedOrders
                .Where(o => o.CreatedAt >= DateTime.Now.AddDays(-30))
                .Sum(o => o.TotalAmount);
            
            var monthlyOrders = completedOrders
                .Where(o => o.CreatedAt >= DateTime.Now.AddDays(-30))
                .Count();
            
            var newCustomers = await _context.Orders
                .Where(o => o.CreatedAt >= DateTime.Now.AddDays(-30))
                .GroupBy(o => o.UserId)
                .Where(g => g.Min(x => x.CreatedAt) >= DateTime.Now.AddDays(-30))
                .CountAsync();
            
            // Get products count
            var totalProducts = await _context.Products.CountAsync();
            var activeProducts = await _context.Products.CountAsync(p => p.IsActive);
            
            // Calculate rates
            var averageOrderValue = totalOrders > 0 ? (double)(totalRevenue / totalOrders) : 0;
            var conversionRate = 0.0; // This would need page view tracking
            
            // Sales chart data (last 7 days)
            var salesChartData = new List<decimal>();
            var salesChartLabels = new List<string>();
            
            for (int i = 6; i >= 0; i--)
            {
                var date = DateTime.Now.AddDays(-i);
                var dayRevenue = completedOrders
                    .Where(o => o.CreatedAt.Date == date.Date)
                    .Sum(o => o.TotalAmount);
                
                salesChartData.Add(dayRevenue);
                salesChartLabels.Add(date.ToString("dd/MM"));
            }
            
            // Orders chart data (last 7 days)
            var ordersChartData = new List<int>();
            var ordersChartLabels = new List<string>();
            
            for (int i = 6; i >= 0; i--)
            {
                var date = DateTime.Now.AddDays(-i);
                var dayOrders = completedOrders
                    .Where(o => o.CreatedAt.Date == date.Date)
                    .Count();
                
                ordersChartData.Add(dayOrders);
                ordersChartLabels.Add(date.ToString("dd/MM"));
            }
            
            var model = new SellerReportsViewModel
            {
                TotalRevenue = totalRevenue,
                MonthlyRevenue = monthlyRevenue,
                TotalOrders = totalOrders,
                MonthlyOrders = monthlyOrders,
                TotalProducts = totalProducts,
                ActiveProducts = activeProducts,
                TotalCustomers = totalCustomers,
                NewCustomers = newCustomers,
                AverageOrderValue = averageOrderValue,
                ConversionRate = conversionRate,
                SalesChartLabels = salesChartLabels,
                SalesChartData = salesChartData,
                OrdersChartLabels = ordersChartLabels,
                OrdersChartData = ordersChartData,
                FromDate = fromDate.Value,
                ToDate = toDate.Value
            };
            
            return View(model);
        }

        // MARK: - Product Performance
        [HttpGet("product-performance")]
        public async Task<IActionResult> ProductPerformance(DateTime? fromDate = null, DateTime? toDate = null)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            
            if (!fromDate.HasValue)
                fromDate = DateTime.Now.AddMonths(-3);
            if (!toDate.HasValue)
                toDate = DateTime.Now;
            
            // Get product performance data
            var productPerformance = await _context.OrderItems
                .Include(oi => oi.Order)
                .Include(oi => oi.Product)
                .Where(oi => oi.Order.CreatedAt >= fromDate && 
                            oi.Order.CreatedAt <= toDate && 
                            oi.Order.Status == "completed")
                .GroupBy(oi => oi.ProductId)
                .Select(g => new ProductPerformanceItem
                {
                    ProductId = g.Key,
                    ProductName = g.First().Product.Name,
                    ImageUrl = g.First().Product.FeaturedImageUrl ?? "",
                    Price = g.First().Product.Price,
                    TotalSold = g.Sum(x => x.Quantity),
                    Revenue = g.Sum(x => x.Price * x.Quantity),
                    ReviewCount = g.First().Product.ProductReviews.Count,
                    AverageRating = g.First().Product.ProductReviews.Any() ? g.First().Product.ProductReviews.Average(r => r.Rating) : 0
                })
                .ToListAsync();
            
            var topProducts = productPerformance
                .OrderByDescending(p => p.Revenue)
                .Take(10)
                .ToList();
            
            var lowPerformingProducts = productPerformance
                .OrderBy(p => p.TotalSold)
                .Take(10)
                .ToList();
            
            var model = new SellerProductPerformanceViewModel
            {
                TopProducts = topProducts,
                LowPerformingProducts = lowPerformingProducts,
                FromDate = fromDate.Value,
                ToDate = toDate.Value
            };
            
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
