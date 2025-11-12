using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using JohnHenryFashionWeb.Data;
using JohnHenryFashionWeb.Models;
using System.Security.Claims;

namespace JohnHenryFashionWeb.Controllers;

[Authorize(Roles = "Seller")]
[Route("seller/products")]
public class SellerProductsController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly IWebHostEnvironment _webHostEnvironment;
    private readonly ILogger<SellerProductsController> _logger;

    public SellerProductsController(
        ApplicationDbContext context,
        IWebHostEnvironment webHostEnvironment,
        ILogger<SellerProductsController> logger)
    {
        _context = context;
        _webHostEnvironment = webHostEnvironment;
        _logger = logger;
    }

    // GET: seller/products
    [HttpGet("")]
    public async Task<IActionResult> Index(string? search, Guid? categoryId, int page = 1, int pageSize = 20)
    {
        var currentUserId = User.FindFirstValue(System.Security.Claims.ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(currentUserId))
        {
            return RedirectToAction("Login", "Account");
        }
        
        var query = _context.Products
            .Include(p => p.Category)
            .Include(p => p.Brand)
            .Where(p => p.SellerId == currentUserId);

        // Filter by search
        if (!string.IsNullOrEmpty(search))
        {
            query = query.Where(p => p.Name.Contains(search) || p.SKU.Contains(search));
        }

        // Filter by category
        if (categoryId.HasValue)
        {
            query = query.Where(p => p.CategoryId == categoryId.Value);
        }

        var totalItems = await query.CountAsync();
        var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

        var products = await query
            .OrderByDescending(p => p.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        // Pass data to view
        ViewBag.Search = search;
        ViewBag.CategoryId = categoryId;
        ViewBag.Page = page;
        ViewBag.PageSize = pageSize;
        ViewBag.TotalItems = totalItems;
        ViewBag.TotalPages = totalPages;
        ViewBag.Categories = await _context.Categories.ToListAsync();

        return View(products);
    }

    // GET: seller/products/create
    [HttpGet("create")]
    public async Task<IActionResult> Create()
    {
        ViewBag.Categories = new SelectList(await _context.Categories.ToListAsync(), "Id", "Name");
        ViewBag.Brands = new SelectList(await _context.Brands.ToListAsync(), "Id", "Name");
        return View();
    }

    // POST: seller/products/create
    [HttpPost("create")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Product product, IFormFile? imageFile)
    {
        if (ModelState.IsValid)
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(currentUserId))
            {
                return RedirectToAction("Login", "Account");
            }
            
            // Handle image upload
            if (imageFile != null && imageFile.Length > 0)
            {
                var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images", "products");
                Directory.CreateDirectory(uploadsFolder);

                var uniqueFileName = $"{Guid.NewGuid()}_{imageFile.FileName}";
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await imageFile.CopyToAsync(fileStream);
                }

                product.FeaturedImageUrl = $"~/images/products/{uniqueFileName}";
            }

            // Generate slug from name
            product.Slug = GenerateSlug(product.Name);
            product.CreatedAt = DateTime.UtcNow;
            product.UpdatedAt = DateTime.UtcNow;
            product.IsActive = true;
            
            // Set SellerId to current user
            product.SellerId = currentUserId;

            _context.Add(product);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Tạo sản phẩm thành công!";
            return RedirectToAction(nameof(Index));
        }

        ViewBag.Categories = new SelectList(await _context.Categories.ToListAsync(), "Id", "Name", product.CategoryId);
        ViewBag.Brands = new SelectList(await _context.Brands.ToListAsync(), "Id", "Name", product.BrandId);
        return View(product);
    }

    // GET: seller/products/{id}
    [HttpGet("{id}")]
    public async Task<IActionResult> Edit(Guid id)
    {
        var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(currentUserId))
        {
            return RedirectToAction("Login", "Account");
        }
        
        var product = await _context.Products.FindAsync(id);
        if (product == null)
        {
            return NotFound();
        }

        // Check if product belongs to current seller
        if (product.SellerId != currentUserId)
        {
            TempData["Error"] = "Bạn không có quyền chỉnh sửa sản phẩm này!";
            return RedirectToAction(nameof(Index));
        }

        ViewBag.Categories = new SelectList(await _context.Categories.ToListAsync(), "Id", "Name", product.CategoryId);
        ViewBag.Brands = new SelectList(await _context.Brands.ToListAsync(), "Id", "Name", product.BrandId);
        return View(product);
    }

    // POST: seller/products/{id}
    [HttpPost("{id}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Guid id, Product product, IFormFile? imageFile)
    {
        if (id != product.Id)
        {
            return NotFound();
        }

        var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(currentUserId))
        {
            return RedirectToAction("Login", "Account");
        }

        if (ModelState.IsValid)
        {
            try
            {
                var existingProduct = await _context.Products.AsNoTracking().FirstOrDefaultAsync(p => p.Id == id);
                if (existingProduct == null)
                {
                    return NotFound();
                }

                // Check if product belongs to current seller
                if (existingProduct.SellerId != currentUserId)
                {
                    TempData["Error"] = "Bạn không có quyền chỉnh sửa sản phẩm này!";
                    return RedirectToAction(nameof(Index));
                }

                // Handle image upload
                if (imageFile != null && imageFile.Length > 0)
                {
                    // Delete old image if exists
                    if (!string.IsNullOrEmpty(existingProduct.FeaturedImageUrl))
                    {
                        var oldImagePath = existingProduct.FeaturedImageUrl.Replace("~/", "");
                        var oldImageFullPath = Path.Combine(_webHostEnvironment.WebRootPath, oldImagePath);
                        if (System.IO.File.Exists(oldImageFullPath))
                        {
                            System.IO.File.Delete(oldImageFullPath);
                        }
                    }

                    var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images", "products");
                    Directory.CreateDirectory(uploadsFolder);

                    var uniqueFileName = $"{Guid.NewGuid()}_{imageFile.FileName}";
                    var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await imageFile.CopyToAsync(fileStream);
                    }

                    product.FeaturedImageUrl = $"~/images/products/{uniqueFileName}";
                }
                else
                {
                    // Keep existing image
                    product.FeaturedImageUrl = existingProduct.FeaturedImageUrl;
                }

                // Update slug if name changed
                if (product.Name != existingProduct.Name)
                {
                    product.Slug = GenerateSlug(product.Name);
                }
                else
                {
                    product.Slug = existingProduct.Slug;
                }

                product.CreatedAt = existingProduct.CreatedAt;
                product.UpdatedAt = DateTime.UtcNow;

                _context.Update(product);
                await _context.SaveChangesAsync();

                TempData["Success"] = "Cập nhật sản phẩm thành công!";
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProductExists(product.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
        }

        ViewBag.Categories = new SelectList(await _context.Categories.ToListAsync(), "Id", "Name", product.CategoryId);
        ViewBag.Brands = new SelectList(await _context.Brands.ToListAsync(), "Id", "Name", product.BrandId);
        return View(product);
    }

    // POST: seller/products/{id}/delete
    [HttpPost("{id}/delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(Guid id)
    {
        var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(currentUserId))
        {
            return RedirectToAction("Login", "Account");
        }
        
        var product = await _context.Products.FindAsync(id);
        if (product == null)
        {
            return NotFound();
        }

        // Check if product belongs to current seller
        if (product.SellerId != currentUserId)
        {
            TempData["Error"] = "Bạn không có quyền xóa sản phẩm này!";
            return RedirectToAction(nameof(Index));
        }

        // Delete image file if exists
        if (!string.IsNullOrEmpty(product.FeaturedImageUrl))
        {
            var imagePath = product.FeaturedImageUrl.Replace("~/", "");
            var imageFullPath = Path.Combine(_webHostEnvironment.WebRootPath, imagePath);
            if (System.IO.File.Exists(imageFullPath))
            {
                System.IO.File.Delete(imageFullPath);
            }
        }

        _context.Products.Remove(product);
        await _context.SaveChangesAsync();

        TempData["Success"] = "Xóa sản phẩm thành công!";
        return RedirectToAction(nameof(Index));
    }

    private bool ProductExists(Guid id)
    {
        return _context.Products.Any(e => e.Id == id);
    }

    private string GenerateSlug(string name)
    {
        // Simple slug generation - remove accents and special characters
        var slug = name.ToLower();
        slug = System.Text.RegularExpressions.Regex.Replace(slug, @"[^a-z0-9\s-]", "");
        slug = System.Text.RegularExpressions.Regex.Replace(slug, @"\s+", "-");
        slug = System.Text.RegularExpressions.Regex.Replace(slug, @"-+", "-");
        return slug.Trim('-');
    }
}
