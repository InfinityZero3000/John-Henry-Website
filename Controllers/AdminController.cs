using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using JohnHenryFashionWeb.Data;
using JohnHenryFashionWeb.Models;
using JohnHenryFashionWeb.ViewModels;
using JohnHenryFashionWeb.Extensions;

namespace JohnHenryFashionWeb.Controllers
{
    [Authorize(Roles = UserRoles.Admin)]
    [Route("admin")]
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public AdminController(
            ApplicationDbContext context, 
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
            _webHostEnvironment = webHostEnvironment;
        }

        [HttpGet("")]
        [HttpGet("dashboard")]
        public async Task<IActionResult> Dashboard()
        {
            var stats = await GetDashboardStats();
            var viewModel = new AdminDashboardViewModel
            {
                Stats = stats,
                RecentOrders = await GetRecentOrders(10),
                TopProducts = await GetTopSellingProducts(5),
                RevenueChart = await GetMonthlyRevenueData(12)
            };

            return View(viewModel);
        }

        #region Dashboard Data Methods
        private async Task<DashboardStats> GetDashboardStats()
        {
            var totalProducts = await _context.Products.CountAsync();
            var totalOrders = await _context.Orders.CountAsync();
            var totalCustomers = await _userManager.GetUsersInRoleAsync(UserRoles.Customer);
            var totalSellers = await _userManager.GetUsersInRoleAsync(UserRoles.Seller);
            
            var totalRevenue = await _context.Orders
                .Where(o => o.Status == "completed")
                .SumAsync(o => o.TotalAmount);

            var currentMonth = DateTime.UtcNow.Month;
            var currentYear = DateTime.UtcNow.Year;
            var monthlyRevenue = await _context.Orders
                .Where(o => o.CreatedAt.Month == currentMonth && 
                           o.CreatedAt.Year == currentYear &&
                           o.Status == "completed")
                .SumAsync(o => o.TotalAmount);

            var pendingOrders = await _context.Orders
                .CountAsync(o => o.Status == "pending");

            var lowStockProducts = await _context.Products
                .CountAsync(p => p.StockQuantity <= 10);

            return new DashboardStats
            {
                TotalProducts = totalProducts,
                TotalOrders = totalOrders,
                TotalCustomers = totalCustomers.Count,
                TotalSellers = totalSellers.Count,
                TotalRevenue = totalRevenue,
                MonthlyRevenue = monthlyRevenue,
                PendingOrders = pendingOrders,
                LowStockProducts = lowStockProducts
            };
        }

        private async Task<List<RecentOrder>> GetRecentOrders(int count)
        {
            return await _context.Orders
                .Include(o => o.User)
                .OrderByDescending(o => o.CreatedAt)
                .Take(count)
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
        }

        private async Task<List<TopSellingProduct>> GetTopSellingProducts(int count)
        {
            return await _context.OrderItems
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
                .Take(count)
                .ToListAsync();
        }

        private async Task<List<MonthlyRevenue>> GetMonthlyRevenueData(int months)
        {
            var result = new List<MonthlyRevenue>();
            var currentDate = DateTime.UtcNow;

            for (int i = months - 1; i >= 0; i--)
            {
                var targetDate = currentDate.AddMonths(-i);
                var revenue = await _context.Orders
                    .Where(o => o.CreatedAt.Month == targetDate.Month && 
                               o.CreatedAt.Year == targetDate.Year &&
                               o.Status == "completed")
                    .SumAsync(o => o.TotalAmount);

                var orderCount = await _context.Orders
                    .CountAsync(o => o.CreatedAt.Month == targetDate.Month && 
                                    o.CreatedAt.Year == targetDate.Year &&
                                    o.Status == "completed");

                result.Add(new MonthlyRevenue
                {
                    Month = targetDate.Month,
                    Year = targetDate.Year,
                    Revenue = revenue,
                    OrderCount = orderCount
                });
            }

            return result;
        }
        #endregion

        #region Product Management
        [HttpGet("products")]
        public async Task<IActionResult> Products(int page = 1, int pageSize = 10, string search = "", Guid? categoryId = null, string status = "")
        {
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
            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
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
                ExistingGalleryImages = product.GalleryImages,
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
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var product = await _context.Products.FindAsync(id);
                if (product == null)
                {
                    return NotFound();
                }

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

                await _context.SaveChangesAsync();

                TempData["Success"] = "Sản phẩm đã được cập nhật thành công!";
                return RedirectToAction(nameof(Products));
            }

            // Reload dropdown data if validation fails
            model.Categories = await _context.Categories.Where(c => c.IsActive).OrderBy(c => c.Name).ToListAsync();
            model.Brands = await _context.Brands.Where(b => b.IsActive).OrderBy(b => b.Name).ToListAsync();

            return View(model);
        }

        [HttpPost("products/delete/{id}")]
        public async Task<IActionResult> DeleteProduct(Guid id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Sản phẩm đã được xóa thành công!";
            return RedirectToAction(nameof(Products));
        }
        #endregion

        #region Category Management
        [HttpGet("categories")]
        public async Task<IActionResult> Categories(string search = "")
        {
            var query = _context.Categories
                .Include(c => c.Parent)
                .AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(c => c.Name.Contains(search) || c.Description.Contains(search));
            }

            var categories = await query
                .OrderBy(c => c.ParentId.HasValue ? 1 : 0)
                .ThenBy(c => c.SortOrder)
                .ThenBy(c => c.Name)
                .Select(c => new CategoryListItemViewModel
                {
                    Id = c.Id,
                    Name = c.Name,
                    Description = c.Description,
                    ImageUrl = c.ImageUrl,
                    ParentId = c.ParentId,
                    ParentName = c.Parent != null ? c.Parent.Name : null,
                    IsActive = c.IsActive,
                    SortOrder = c.SortOrder,
                    CreatedAt = c.CreatedAt,
                    ProductCount = c.Products.Count()
                })
                .ToListAsync();

            var viewModel = new CategoryListViewModel
            {
                Categories = categories,
                SearchTerm = search
            };

            return View(viewModel);
        }

        [HttpGet("categories/create")]
        public async Task<IActionResult> CreateCategory()
        {
            var viewModel = new CategoryCreateEditViewModel
            {
                ParentCategories = await _context.Categories
                    .Where(c => c.IsActive && !c.ParentId.HasValue)
                    .OrderBy(c => c.Name)
                    .ToListAsync()
            };

            return View(viewModel);
        }

        [HttpPost("categories/create")]
        public async Task<IActionResult> CreateCategory(CategoryCreateEditViewModel model)
        {
            if (ModelState.IsValid)
            {
                var category = new Category
                {
                    Id = Guid.NewGuid(),
                    Name = model.Name,
                    Description = model.Description,
                    ParentId = model.ParentId,
                    IsActive = model.IsActive,
                    SortOrder = model.SortOrder,
                    Slug = GenerateSlug(model.Name),
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                // Handle image upload
                if (model.Image != null)
                {
                    category.ImageUrl = await SaveUploadedFile(model.Image, "categories");
                }

                _context.Categories.Add(category);
                await _context.SaveChangesAsync();

                TempData["Success"] = "Danh mục đã được tạo thành công!";
                return RedirectToAction(nameof(Categories));
            }

            // Reload dropdown data if validation fails
            model.ParentCategories = await _context.Categories
                .Where(c => c.IsActive && !c.ParentId.HasValue)
                .OrderBy(c => c.Name)
                .ToListAsync();

            return View(model);
        }

        [HttpGet("categories/edit/{id}")]
        public async Task<IActionResult> EditCategory(Guid id)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category == null)
            {
                return NotFound();
            }

            var viewModel = new CategoryCreateEditViewModel
            {
                Id = category.Id,
                Name = category.Name,
                Description = category.Description,
                ParentId = category.ParentId,
                IsActive = category.IsActive,
                SortOrder = category.SortOrder,
                ImageUrl = category.ImageUrl,
                ParentCategories = await _context.Categories
                    .Where(c => c.IsActive && !c.ParentId.HasValue && c.Id != id)
                    .OrderBy(c => c.Name)
                    .ToListAsync()
            };

            return View(viewModel);
        }

        [HttpPost("categories/edit/{id}")]
        public async Task<IActionResult> EditCategory(Guid id, CategoryCreateEditViewModel model)
        {
            if (id != model.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var category = await _context.Categories.FindAsync(id);
                if (category == null)
                {
                    return NotFound();
                }

                category.Name = model.Name;
                category.Description = model.Description;
                category.ParentId = model.ParentId;
                category.IsActive = model.IsActive;
                category.SortOrder = model.SortOrder;
                category.Slug = GenerateSlug(model.Name);
                category.UpdatedAt = DateTime.UtcNow;

                // Handle image upload
                if (model.Image != null)
                {
                    category.ImageUrl = await SaveUploadedFile(model.Image, "categories");
                }

                await _context.SaveChangesAsync();

                TempData["Success"] = "Danh mục đã được cập nhật thành công!";
                return RedirectToAction(nameof(Categories));
            }

            // Reload dropdown data if validation fails
            model.ParentCategories = await _context.Categories
                .Where(c => c.IsActive && !c.ParentId.HasValue && c.Id != id)
                .OrderBy(c => c.Name)
                .ToListAsync();

            return View(model);
        }

        [HttpPost("categories/delete/{id}")]
        public async Task<IActionResult> DeleteCategory(Guid id)
        {
            var category = await _context.Categories
                .Include(c => c.Products)
                .Include(c => c.Children)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (category == null)
            {
                return NotFound();
            }

            // Check if category has products or subcategories
            if (category.Products.Any() || category.Children.Any())
            {
                TempData["Error"] = "Không thể xóa danh mục này vì đang có sản phẩm hoặc danh mục con!";
                return RedirectToAction(nameof(Categories));
            }

            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Danh mục đã được xóa thành công!";
            return RedirectToAction(nameof(Categories));
        }

        [HttpGet("categories/from-images")]
        public async Task<IActionResult> CreateCategoriesFromImages()
        {
            var categoriesCreated = 0;
            var imagesPath = Path.Combine(_webHostEnvironment.WebRootPath, "images");
            
            if (Directory.Exists(imagesPath))
            {
                var categoryFolders = Directory.GetDirectories(imagesPath)
                    .Where(d => !Path.GetFileName(d).StartsWith(".") && 
                               !Path.GetFileName(d).Equals("store", StringComparison.OrdinalIgnoreCase) &&
                               !Path.GetFileName(d).Equals("Banner", StringComparison.OrdinalIgnoreCase))
                    .ToList();

                foreach (var folder in categoryFolders)
                {
                    var folderName = Path.GetFileName(folder);
                    var slug = GenerateSlug(folderName);

                    // Check if category already exists
                    var existingCategory = await _context.Categories
                        .FirstOrDefaultAsync(c => c.Slug == slug);

                    if (existingCategory == null)
                    {
                        var category = new Category
                        {
                            Id = Guid.NewGuid(),
                            Name = folderName,
                            Slug = slug,
                            Description = $"Danh mục {folderName}",
                            IsActive = true,
                            SortOrder = categoriesCreated,
                            CreatedAt = DateTime.UtcNow,
                            UpdatedAt = DateTime.UtcNow
                        };

                        // Try to find a representative image
                        var imageFiles = Directory.GetFiles(folder, "*.*", SearchOption.TopDirectoryOnly)
                            .Where(file => file.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase) ||
                                          file.EndsWith(".jpeg", StringComparison.OrdinalIgnoreCase) ||
                                          file.EndsWith(".png", StringComparison.OrdinalIgnoreCase))
                            .FirstOrDefault();

                        if (imageFiles != null)
                        {
                            var relativePath = Path.GetRelativePath(_webHostEnvironment.WebRootPath, imageFiles)
                                .Replace("\\", "/");
                            category.ImageUrl = "/" + relativePath;
                        }

                        _context.Categories.Add(category);
                        categoriesCreated++;
                    }
                }

                if (categoriesCreated > 0)
                {
                    await _context.SaveChangesAsync();
                    TempData["Success"] = $"Đã tạo {categoriesCreated} danh mục từ thư mục hình ảnh!";
                }
                else
                {
                    TempData["Info"] = "Tất cả danh mục đã tồn tại!";
                }
            }
            else
            {
                TempData["Error"] = "Không tìm thấy thư mục hình ảnh!";
            }

            return RedirectToAction(nameof(Categories));
        }
        #endregion

        #region Product Management (continued)
        [HttpGet("products/import-from-images")]
        public async Task<IActionResult> ImportProductsFromImages()
        {
            var productsCreated = 0;
            var imagesPath = Path.Combine(_webHostEnvironment.WebRootPath, "images");
            
            if (Directory.Exists(imagesPath))
            {
                var categoryFolders = Directory.GetDirectories(imagesPath)
                    .Where(d => !Path.GetFileName(d).StartsWith(".") && 
                               !Path.GetFileName(d).Equals("store", StringComparison.OrdinalIgnoreCase) &&
                               !Path.GetFileName(d).Equals("Banner", StringComparison.OrdinalIgnoreCase))
                    .ToList();

                foreach (var folder in categoryFolders)
                {
                    var folderName = Path.GetFileName(folder);
                    
                    // Find or create category
                    var category = await _context.Categories
                        .FirstOrDefaultAsync(c => c.Name == folderName);
                    
                    if (category == null)
                    {
                        category = new Category
                        {
                            Id = Guid.NewGuid(),
                            Name = folderName,
                            Slug = GenerateSlug(folderName),
                            Description = $"Danh mục {folderName}",
                            IsActive = true,
                            CreatedAt = DateTime.UtcNow,
                            UpdatedAt = DateTime.UtcNow
                        };
                        _context.Categories.Add(category);
                        await _context.SaveChangesAsync();
                    }

                    // Get all image files in the folder
                    var imageFiles = Directory.GetFiles(folder, "*.*", SearchOption.TopDirectoryOnly)
                        .Where(file => file.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase) ||
                                      file.EndsWith(".jpeg", StringComparison.OrdinalIgnoreCase) ||
                                      file.EndsWith(".png", StringComparison.OrdinalIgnoreCase))
                        .ToList();

                    foreach (var imageFile in imageFiles)
                    {
                        var fileName = Path.GetFileNameWithoutExtension(imageFile);
                        var productName = FormatProductName(fileName);
                        var sku = GenerateProductSKU(fileName);

                        // Check if product already exists
                        var existingProduct = await _context.Products
                            .FirstOrDefaultAsync(p => p.SKU == sku);

                        if (existingProduct == null)
                        {
                            var relativePath = Path.GetRelativePath(_webHostEnvironment.WebRootPath, imageFile)
                                .Replace("\\", "/");

                            // Extract price and details from filename if possible
                            var (price, size, color) = ExtractProductDetails(fileName);

                            var product = new Product
                            {
                                Id = Guid.NewGuid(),
                                Name = productName,
                                SKU = sku,
                                Slug = GenerateSlug(productName),
                                Price = price,
                                CategoryId = category.Id,
                                Description = $"Sản phẩm {productName} thuộc danh mục {folderName}",
                                ShortDescription = $"Sản phẩm thời trang {productName}",
                                StockQuantity = 100, // Default stock
                                ManageStock = true,
                                InStock = true,
                                IsActive = true,
                                IsFeatured = false,
                                Status = "active",
                                FeaturedImageUrl = "/" + relativePath,
                                Size = size,
                                Color = color,
                                CreatedAt = DateTime.UtcNow,
                                UpdatedAt = DateTime.UtcNow
                            };

                            _context.Products.Add(product);
                            productsCreated++;
                        }
                    }
                }

                if (productsCreated > 0)
                {
                    await _context.SaveChangesAsync();
                    TempData["Success"] = $"Đã tạo {productsCreated} sản phẩm từ thư mục hình ảnh!";
                }
                else
                {
                    TempData["Info"] = "Tất cả sản phẩm đã tồn tại!";
                }
            }
            else
            {
                TempData["Error"] = "Không tìm thấy thư mục hình ảnh!";
            }

            return RedirectToAction(nameof(Products));
        }

        [HttpPost("products/{id}/update-stock")]
        public async Task<IActionResult> UpdateProductStock(Guid id, [FromBody] UpdateStockRequest request)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return Json(new { success = false, message = "Không tìm thấy sản phẩm" });
            }

            product.StockQuantity = request.Stock;
            product.InStock = request.Stock > 0;
            product.Status = request.Stock > 0 ? "active" : "out_of_stock";
            product.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return Json(new { success = true, message = "Cập nhật tồn kho thành công" });
        }

        public class UpdateStockRequest
        {
            public int Stock { get; set; }
        }
        #endregion

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

        private string FormatProductName(string fileName)
        {
            // Clean up filename to create a readable product name
            return fileName
                .Replace("-", " ")
                .Replace("_", " ")
                .Trim()
                .ToTitleCase();
        }

        private string GenerateProductSKU(string fileName)
        {
            // Generate SKU from filename
            var cleanName = fileName.Replace(" ", "").Replace("-", "").Replace("_", "").ToUpper();
            if (cleanName.Length > 10)
            {
                cleanName = cleanName.Substring(0, 10);
            }
            return $"JH{cleanName}{DateTime.Now.ToString("MMdd")}";
        }

        private (decimal price, string size, string color) ExtractProductDetails(string fileName)
        {
            var price = 299000m; // Default price
            string size = null;
            string color = null;

            // Try to extract size
            var sizeMatches = new[] { "XS", "S", "M", "L", "XL", "XXL", "36", "38", "40", "42", "44", "46" };
            foreach (var sizeMatch in sizeMatches)
            {
                if (fileName.ToUpper().Contains(sizeMatch))
                {
                    size = sizeMatch;
                    break;
                }
            }

            // Try to extract color from Vietnamese color names
            var colorMatches = new Dictionary<string, string>
            {
                { "đen", "Đen" }, { "trắng", "Trắng" }, { "xanh", "Xanh" },
                { "đỏ", "Đỏ" }, { "vàng", "Vàng" }, { "hồng", "Hồng" },
                { "tím", "Tím" }, { "nâu", "Nâu" }, { "xám", "Xám" },
                { "black", "Đen" }, { "white", "Trắng" }, { "blue", "Xanh" },
                { "red", "Đỏ" }, { "yellow", "Vàng" }, { "pink", "Hồng" },
                { "purple", "Tím" }, { "brown", "Nâu" }, { "gray", "Xám" }
            };

            foreach (var colorMatch in colorMatches)
            {
                if (fileName.ToLower().Contains(colorMatch.Key))
                {
                    color = colorMatch.Value;
                    break;
                }
            }

            // Adjust price based on category
            if (fileName.ToLower().Contains("vest") || fileName.ToLower().Contains("suit"))
                price = 899000m;
            else if (fileName.ToLower().Contains("áo khoác") || fileName.ToLower().Contains("jacket"))
                price = 599000m;
            else if (fileName.ToLower().Contains("áo sơ mi") || fileName.ToLower().Contains("shirt"))
                price = 399000m;
            else if (fileName.ToLower().Contains("quần") || fileName.ToLower().Contains("pants"))
                price = 499000m;
            else if (fileName.ToLower().Contains("đầm") || fileName.ToLower().Contains("dress"))
                price = 699000m;

            return (price, size, color);
        }
        #endregion
    }
}

// Extension method for string title case
namespace JohnHenryFashionWeb.Extensions
{
    public static class StringExtensions
    {
        public static string ToTitleCase(this string input)
        {
            if (string.IsNullOrEmpty(input))
                return string.Empty;

            var words = input.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < words.Length; i++)
            {
                if (words[i].Length > 0)
                {
                    words[i] = char.ToUpper(words[i][0]) + (words[i].Length > 1 ? words[i].Substring(1).ToLower() : "");
                }
            }
            return string.Join(' ', words);
        }
    }
}
