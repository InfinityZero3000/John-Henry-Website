using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using JohnHenryFashionWeb.Models;
using JohnHenryFashionWeb.Services;
using JohnHenryFashionWeb.Data;
using JohnHenryFashionWeb.ViewModels;

namespace JohnHenryFashionWeb.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly SeoService _seoService;
    private readonly ApplicationDbContext _context;

    public HomeController(ILogger<HomeController> logger, SeoService seoService, ApplicationDbContext context)
    {
        _logger = logger;
        _seoService = seoService;
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        // Generate SEO data for homepage
        var websiteJsonLd = _seoService.GenerateWebsiteJsonLd();
        var organizationJsonLd = _seoService.GenerateOrganizationJsonLd();

        // Generate breadcrumbs for homepage
        var breadcrumbs = new List<BreadcrumbItem>
        {
            new BreadcrumbItem { Name = "Trang chủ", Url = "" }
        };

        var breadcrumbJsonLd = _seoService.GenerateBreadcrumbJsonLd(breadcrumbs);

        ViewBag.WebsiteJsonLd = websiteJsonLd;
        ViewBag.OrganizationJsonLd = organizationJsonLd;
        ViewBag.BreadcrumbJsonLd = breadcrumbJsonLd;
        ViewBag.MetaTitle = "John Henry Fashion - Thời trang nam nữ cao cấp";
        ViewBag.MetaDescription = "Khám phá bộ sưu tập thời trang nam nữ cao cấp tại John Henry Fashion. Phong cách hiện đại, chất lượng tuyệt vời với giá cả hợp lý.";

        // Load featured products for homepage collections
        var johnHenrySKUs = new[] {
            "JK25FH04C-PA", "KS25SS20P-SC", "WS25FH63P-LC", "JK25FH02P-CT",
            "KS25FH49C-SLWK", "KS25SS31T-SCWK", "WS25FH58C-CFBB", "JK25FH10T-KA"
        };

        var freelancerSKUs = new[] {
            "FWBZ23SS06C", "FWWS25SS02G", "FWTS24FH03G", "FWTS25SS14C",
            "FWDR24FH01G", "FWDR25SS29G", "FWDR25FH04C", "FWSK23SS14G"
        };

        var bestSellerSKUs = new[] {
            "FWSK25FH12C", "FWSK24FH13C", "KP25SS06T-NMWFSL", "FWDP24SS06C",
            "JK25FH03C-CT", "BZ25FH06C-SL", "BZ24FH02P-SL", "WS24SS15P-SCRG"
        };

        // Load John Henry featured products
        var johnHenryProducts = await _context.Products
            .Where(p => p.IsActive && p.IsFeatured && johnHenrySKUs.Contains(p.SKU))
            .OrderBy(p => Array.IndexOf(johnHenrySKUs, p.SKU))
            .Take(8)
            .ToListAsync();

        // Load Freelancer featured products
        var freelancerProducts = await _context.Products
            .Where(p => p.IsActive && p.IsFeatured && freelancerSKUs.Contains(p.SKU))
            .OrderBy(p => Array.IndexOf(freelancerSKUs, p.SKU))
            .Take(8)
            .ToListAsync();

        // Load Best Seller featured products
        var bestSellerProducts = await _context.Products
            .Where(p => p.IsActive && p.IsFeatured && bestSellerSKUs.Contains(p.SKU))
            .OrderBy(p => Array.IndexOf(bestSellerSKUs, p.SKU))
            .Take(8)
            .ToListAsync();

        ViewBag.JohnHenryProducts = johnHenryProducts;
        ViewBag.FreelancerProducts = freelancerProducts;
        ViewBag.BestSellerProducts = bestSellerProducts;

        return View();
    }

    // Debug endpoint to check product counts
    public async Task<IActionResult> DebugHomepage()
    {
        var freelancerSKUs = new[] {
            "FWBZ23SS06C", "FWWS25SS02G", "FWTS24FH03G", "FWTS25SS14C",
            "FWDR24FH01G", "FWDR25SS29G", "FWDR25FH04C", "FWSK23SS14G"
        };

        var freelancerProducts = await _context.Products
            .Where(p => p.IsActive && p.IsFeatured && freelancerSKUs.Contains(p.SKU))
            .OrderBy(p => Array.IndexOf(freelancerSKUs, p.SKU))
            .Select(p => new { p.SKU, p.Name, p.IsFeatured, p.IsActive })
            .ToListAsync();

        var missingSkus = freelancerSKUs.Where(sku => !freelancerProducts.Any(p => p.SKU == sku)).ToList();

        return Json(new
        {
            expectedCount = 8,
            actualCount = freelancerProducts.Count,
            products = freelancerProducts,
            missingSkus = missingSkus
        });
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public async Task<IActionResult> JohnHenry(int page = 1)
    {
        const int pageSize = 40; // 10 rows x 4 products per row

        // Generate breadcrumbs for John Henry page
        var breadcrumbs = new List<BreadcrumbItem>
        {
            new BreadcrumbItem { Name = "Trang chủ", Url = Url.Action("Index", "Home") ?? "/" },
            new BreadcrumbItem { Name = "JOHN HENRY", Url = "" }
        };

        var breadcrumbJsonLd = _seoService.GenerateBreadcrumbJsonLd(breadcrumbs);

        ViewBag.BreadcrumbJsonLd = breadcrumbJsonLd;
        ViewBag.Breadcrumbs = breadcrumbs;
        ViewBag.MetaTitle = "JOHN HENRY - Thời trang nam nữ cao cấp";
        ViewBag.MetaDescription = "Khám phá bộ sưu tập JOHN HENRY với các sản phẩm thời trang nam nữ chất lượng cao, phong cách hiện đại và sang trọng.";

        // Load products from database - John Henry collection (Products with SKU NOT starting with "FW")
        // Get total count for pagination
        var totalProducts = await _context.Products
            .Where(p => p.IsActive && !p.SKU.StartsWith("FW"))
            .CountAsync();

        // Calculate pagination
        var totalPages = (int)Math.Ceiling(totalProducts / (double)pageSize);
        page = Math.Max(1, Math.Min(page, totalPages)); // Ensure page is in valid range

        var products = await _context.Products
            .Include(p => p.Category)
            .Include(p => p.Brand)
            .Where(p => p.IsActive && !p.SKU.StartsWith("FW"))
            .OrderByDescending(p => p.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        // Map to ProductViewModel
        var productViewModels = products.Select(p => MapToProductViewModel(p)).ToList();

        // Pass pagination info to view
        ViewBag.CurrentPage = page;
        ViewBag.TotalPages = totalPages;
        ViewBag.TotalProducts = totalProducts;
        ViewBag.PageSize = pageSize;

        return View(productViewModels);
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public async Task<IActionResult> Freelancer(int page = 1)
    {
        const int pageSize = 40; // 10 rows x 4 products per row

        // Generate breadcrumbs for Freelancer page
        var breadcrumbs = new List<BreadcrumbItem>
        {
            new BreadcrumbItem { Name = "Trang chủ", Url = Url.Action("Index", "Home") ?? "/" },
            new BreadcrumbItem { Name = "FREELANCER", Url = "" }
        };

        var breadcrumbJsonLd = _seoService.GenerateBreadcrumbJsonLd(breadcrumbs);

        ViewBag.BreadcrumbJsonLd = breadcrumbJsonLd;
        ViewBag.Breadcrumbs = breadcrumbs;
        ViewBag.MetaTitle = "FREELANCER - Thời trang nữ hiện đại";
        ViewBag.MetaDescription = "Khám phá bộ sưu tập FREELANCER với các sản phẩm thời trang nữ chất lượng cao, phong cách hiện đại và sang trọng dành riêng cho phái đẹp.";

        // Load products from database - Freelancer collection (Products with SKU starting with "FW")
        // Get total count for pagination
        var totalProducts = await _context.Products
            .Where(p => p.IsActive && p.SKU.StartsWith("FW"))
            .CountAsync();

        // Calculate pagination
        var totalPages = (int)Math.Ceiling(totalProducts / (double)pageSize);
        page = Math.Max(1, Math.Min(page, totalPages)); // Ensure page is in valid range

        var products = await _context.Products
            .Include(p => p.Category)
            .Include(p => p.Brand)
            .Where(p => p.IsActive && p.SKU.StartsWith("FW"))
            .OrderByDescending(p => p.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        // Map to ProductViewModel
        var productViewModels = products.Select(p => MapToProductViewModel(p)).ToList();

        // Pass pagination info to view
        ViewBag.CurrentPage = page;
        ViewBag.TotalPages = totalPages;
        ViewBag.TotalProducts = totalProducts;
        ViewBag.PageSize = pageSize;

        return View(productViewModels);
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public async Task<IActionResult> FreelancerDress(int page = 1)
    {
        const int pageSize = 40; // 10 rows x 4 products per row

        // Generate breadcrumbs for Freelancer Dress page
        var breadcrumbs = new List<BreadcrumbItem>
        {
            new BreadcrumbItem { Name = "Trang chủ", Url = Url.Action("Index", "Home") ?? "/" },
            new BreadcrumbItem { Name = "FREELANCER", Url = Url.Action("Freelancer", "Home") ?? "/Home/Freelancer" },
            new BreadcrumbItem { Name = "VÁY", Url = "" }
        };

        var breadcrumbJsonLd = _seoService.GenerateBreadcrumbJsonLd(breadcrumbs);

        ViewBag.BreadcrumbJsonLd = breadcrumbJsonLd;
        ViewBag.Breadcrumbs = breadcrumbs;
        ViewBag.MetaTitle = "Váy FREELANCER - Thời trang nữ cao cấp";
        ViewBag.MetaDescription = "Khám phá bộ sưu tập váy FREELANCER với đa dạng các loại váy công sở, váy dạ tiệc, váy maxi chất lượng cao, phong cách hiện đại.";

        // Load women's dresses from database BY SUBCATEGORY AND SKU PATTERN
        var dressCategory = await _context.Categories
            .FirstOrDefaultAsync(c => c.Name == "Đầm nữ");

        if (dressCategory == null)
        {
            return View(new List<Product>());
        }

        // Get total count for pagination - Filter by category AND Freelancer SKU pattern
        var totalProducts = await _context.Products
            .Where(p => p.IsActive && p.CategoryId == dressCategory.Id && p.SKU.StartsWith("FW"))
            .CountAsync();

        // Calculate pagination
        var totalPages = (int)Math.Ceiling(totalProducts / (double)pageSize);
        page = Math.Max(1, Math.Min(page, totalPages));

        var products = await _context.Products
            .Where(p => p.IsActive && p.CategoryId == dressCategory.Id && p.SKU.StartsWith("FW"))
            .OrderByDescending(p => p.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        // Pass pagination info to view
        ViewBag.CurrentPage = page;
        ViewBag.TotalPages = totalPages;
        ViewBag.TotalProducts = totalProducts;
        ViewBag.PageSize = pageSize;

        return View(products);
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public async Task<IActionResult> FreelancerShirt(int page = 1)
    {
        const int pageSize = 40; // 10 rows x 4 products per row

        // Generate breadcrumbs for Freelancer Shirt page
        var breadcrumbs = new List<BreadcrumbItem>
        {
            new BreadcrumbItem { Name = "Trang chủ", Url = Url.Action("Index", "Home") ?? "/" },
            new BreadcrumbItem { Name = "FREELANCER", Url = Url.Action("Freelancer", "Home") ?? "/Home/Freelancer" },
            new BreadcrumbItem { Name = "ÁO NỮ", Url = "" }
        };

        var breadcrumbJsonLd = _seoService.GenerateBreadcrumbJsonLd(breadcrumbs);

        ViewBag.BreadcrumbJsonLd = breadcrumbJsonLd;
        ViewBag.Breadcrumbs = breadcrumbs;
        ViewBag.MetaTitle = "Áo Nữ FREELANCER - Thời trang nữ cao cấp";
        ViewBag.MetaDescription = "Khám phá bộ sưu tập áo nữ FREELANCER với đa dạng các loại áo blouse, áo thun, áo len, áo khoác chất lượng cao, phong cách hiện đại.";

        // Load women's shirts from database BY SUBCATEGORY AND SKU PATTERN
        var shirtCategory = await _context.Categories
            .FirstOrDefaultAsync(c => c.Name == "Áo nữ");

        if (shirtCategory == null)
        {
            return View(new List<Product>());
        }

        // Get total count for pagination - Filter by category AND Freelancer SKU pattern
        var totalProducts = await _context.Products
            .Where(p => p.IsActive && p.CategoryId == shirtCategory.Id && p.SKU.StartsWith("FW"))
            .CountAsync();

        // Calculate pagination
        var totalPages = (int)Math.Ceiling(totalProducts / (double)pageSize);
        page = Math.Max(1, Math.Min(page, totalPages));

        var products = await _context.Products
            .Where(p => p.IsActive && p.CategoryId == shirtCategory.Id && p.SKU.StartsWith("FW"))
            .OrderByDescending(p => p.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        // Pass pagination info to view
        ViewBag.CurrentPage = page;
        ViewBag.TotalPages = totalPages;
        ViewBag.TotalProducts = totalProducts;
        ViewBag.PageSize = pageSize;

        return View(products);
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public async Task<IActionResult> FreelancerTrousers(int page = 1)
    {
        const int pageSize = 40; // 10 rows x 4 products per row

        // Generate breadcrumbs for Freelancer Trousers page
        var breadcrumbs = new List<BreadcrumbItem>
        {
            new BreadcrumbItem { Name = "Trang chủ", Url = Url.Action("Index", "Home") ?? "/" },
            new BreadcrumbItem { Name = "FREELANCER", Url = Url.Action("Freelancer", "Home") ?? "/Home/Freelancer" },
            new BreadcrumbItem { Name = "QUẦN NỮ", Url = "" }
        };

        var breadcrumbJsonLd = _seoService.GenerateBreadcrumbJsonLd(breadcrumbs);

        ViewBag.BreadcrumbJsonLd = breadcrumbJsonLd;
        ViewBag.Breadcrumbs = breadcrumbs;
        ViewBag.MetaTitle = "Quần Nữ FREELANCER - Thời trang nữ cao cấp";
        ViewBag.MetaDescription = "Khám phá bộ sưu tập quần nữ FREELANCER với đa dạng các loại quần jean, quần âu, quần short chất lượng cao, phong cách hiện đại.";

        // Load women's trousers from database BY SUBCATEGORY AND SKU PATTERN
        var trousersCategory = await _context.Categories
            .FirstOrDefaultAsync(c => c.Name == "Quần nữ");

        if (trousersCategory == null)
        {
            return View(new List<Product>());
        }

        // Get total count for pagination - Filter by category AND Freelancer SKU pattern
        var totalProducts = await _context.Products
            .Where(p => p.IsActive && p.CategoryId == trousersCategory.Id && p.SKU.StartsWith("FW"))
            .CountAsync();

        // Calculate pagination
        var totalPages = (int)Math.Ceiling(totalProducts / (double)pageSize);
        page = Math.Max(1, Math.Min(page, totalPages));

        var products = await _context.Products
            .Where(p => p.IsActive && p.CategoryId == trousersCategory.Id && p.SKU.StartsWith("FW"))
            .OrderByDescending(p => p.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        // Pass pagination info to view
        ViewBag.CurrentPage = page;
        ViewBag.TotalPages = totalPages;
        ViewBag.TotalProducts = totalProducts;
        ViewBag.PageSize = pageSize;

        return View(products);
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public async Task<IActionResult> FreelancerSkirt(int page = 1)
    {
        const int pageSize = 40; // 10 rows x 4 products per row

        // Generate breadcrumbs for Freelancer Skirt page
        var breadcrumbs = new List<BreadcrumbItem>
        {
            new BreadcrumbItem { Name = "Trang chủ", Url = Url.Action("Index", "Home") ?? "/" },
            new BreadcrumbItem { Name = "FREELANCER", Url = Url.Action("Freelancer", "Home") ?? "/Home/Freelancer" },
            new BreadcrumbItem { Name = "CHÂN VÁY", Url = "" }
        };

        var breadcrumbJsonLd = _seoService.GenerateBreadcrumbJsonLd(breadcrumbs);

        ViewBag.BreadcrumbJsonLd = breadcrumbJsonLd;
        ViewBag.Breadcrumbs = breadcrumbs;
        ViewBag.MetaTitle = "Chân Váy FREELANCER - Thời trang nữ cao cấp";
        ViewBag.MetaDescription = "Khám phá bộ sưu tập chân váy FREELANCER với đa dạng các loại chân váy bút chì, chân váy xòe, chân váy midi chất lượng cao, phong cách hiện đại.";

        // Load women's skirts from database BY SUBCATEGORY AND SKU PATTERN
        var skirtCategory = await _context.Categories
            .FirstOrDefaultAsync(c => c.Name == "Chân váy nữ");

        if (skirtCategory == null)
        {
            return View(new List<Product>());
        }

        // Get total count for pagination - Filter by category AND Freelancer SKU pattern
        var totalProducts = await _context.Products
            .Where(p => p.IsActive && p.CategoryId == skirtCategory.Id && p.SKU.StartsWith("FW"))
            .CountAsync();

        // Calculate pagination
        var totalPages = (int)Math.Ceiling(totalProducts / (double)pageSize);
        page = Math.Max(1, Math.Min(page, totalPages));

        var products = await _context.Products
            .Where(p => p.IsActive && p.CategoryId == skirtCategory.Id && p.SKU.StartsWith("FW"))
            .OrderByDescending(p => p.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        // Pass pagination info to view
        ViewBag.CurrentPage = page;
        ViewBag.TotalPages = totalPages;
        ViewBag.TotalProducts = totalProducts;
        ViewBag.PageSize = pageSize;

        return View(products);
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public async Task<IActionResult> FreelancerAccessories(int page = 1)
    {
        const int pageSize = 40; // 10 rows x 4 products per row

        // Generate breadcrumbs for Freelancer Accessories page
        var breadcrumbs = new List<BreadcrumbItem>
        {
            new BreadcrumbItem { Name = "Trang chủ", Url = Url.Action("Index", "Home") ?? "/" },
            new BreadcrumbItem { Name = "FREELANCER", Url = Url.Action("Freelancer", "Home") ?? "/Home/Freelancer" },
            new BreadcrumbItem { Name = "PHỤ KIỆN NỮ", Url = "" }
        };

        var breadcrumbJsonLd = _seoService.GenerateBreadcrumbJsonLd(breadcrumbs);

        ViewBag.BreadcrumbJsonLd = breadcrumbJsonLd;
        ViewBag.Breadcrumbs = breadcrumbs;
        ViewBag.MetaTitle = "Phụ Kiện Nữ FREELANCER - Thời trang nữ cao cấp";
        ViewBag.MetaDescription = "Khám phá bộ sưu tập phụ kiện nữ FREELANCER với đa dạng túi xách, ví, thắt lưng, mắt kính, khăn, đồng hồ chất lượng cao, phong cách hiện đại.";

        // Load women's accessories from database BY SUBCATEGORY AND SKU PATTERN
        var accessoriesCategory = await _context.Categories
            .FirstOrDefaultAsync(c => c.Name == "Phụ kiện nữ");

        if (accessoriesCategory == null)
        {
            return View(new List<Product>());
        }

        // Get total count for pagination - Filter by category AND Freelancer SKU pattern
        var totalProducts = await _context.Products
            .Where(p => p.IsActive && p.CategoryId == accessoriesCategory.Id && p.SKU.StartsWith("FW"))
            .CountAsync();

        // Calculate pagination
        var totalPages = (int)Math.Ceiling(totalProducts / (double)pageSize);
        page = Math.Max(1, Math.Min(page, totalPages));

        var products = await _context.Products
            .Where(p => p.IsActive && p.CategoryId == accessoriesCategory.Id && p.SKU.StartsWith("FW"))
            .OrderByDescending(p => p.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        // Pass pagination info to view
        ViewBag.CurrentPage = page;
        ViewBag.TotalPages = totalPages;
        ViewBag.TotalProducts = totalProducts;
        ViewBag.PageSize = pageSize;

        return View(products);
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public async Task<IActionResult> JohnHenryShirt(int page = 1)
    {
        const int pageSize = 40; // 10 rows x 4 products per row

        // Generate breadcrumbs for John Henry Shirt page
        var breadcrumbs = new List<BreadcrumbItem>
        {
            new BreadcrumbItem { Name = "Trang chủ", Url = Url.Action("Index", "Home") ?? "/" },
            new BreadcrumbItem { Name = "JOHN HENRY", Url = Url.Action("JohnHenry", "Home") ?? "/Home/JohnHenry" },
            new BreadcrumbItem { Name = "ÁO NAM", Url = "" }
        };

        var breadcrumbJsonLd = _seoService.GenerateBreadcrumbJsonLd(breadcrumbs);

        ViewBag.BreadcrumbJsonLd = breadcrumbJsonLd;
        ViewBag.Breadcrumbs = breadcrumbs;
        ViewBag.MetaTitle = "Áo Nam JOHN HENRY - Thời trang nam cao cấp";
        ViewBag.MetaDescription = "Khám phá bộ sưu tập áo nam JOHN HENRY với đa dạng các loại áo polo, áo sơ mi, áo thun, áo len chất lượng cao, phong cách hiện đại.";

        // Load men's shirts from database BY SUBCATEGORY AND SKU PATTERN
        var shirtCategory = await _context.Categories
            .FirstOrDefaultAsync(c => c.Name == "Áo nam");

        if (shirtCategory == null)
        {
            return View(new List<Product>());
        }

        // Get total count for pagination - Filter by category AND John Henry SKU pattern (NOT starting with FW)
        var totalProducts = await _context.Products
            .Where(p => p.IsActive && p.CategoryId == shirtCategory.Id && !p.SKU.StartsWith("FW"))
            .CountAsync();

        // Calculate pagination
        var totalPages = (int)Math.Ceiling(totalProducts / (double)pageSize);
        page = Math.Max(1, Math.Min(page, totalPages));

        var products = await _context.Products
            .Where(p => p.IsActive && p.CategoryId == shirtCategory.Id && !p.SKU.StartsWith("FW"))
            .OrderByDescending(p => p.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        // Pass pagination info to view
        ViewBag.CurrentPage = page;
        ViewBag.TotalPages = totalPages;
        ViewBag.TotalProducts = totalProducts;
        ViewBag.PageSize = pageSize;

        return View(products);
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public async Task<IActionResult> JohnHenryTrousers(int page = 1)
    {
        const int pageSize = 40; // 10 rows x 4 products per row

        // Generate breadcrumbs for John Henry Trousers page
        var breadcrumbs = new List<BreadcrumbItem>
        {
            new BreadcrumbItem { Name = "Trang chủ", Url = Url.Action("Index", "Home") ?? "/" },
            new BreadcrumbItem { Name = "JOHN HENRY", Url = Url.Action("JohnHenry", "Home") ?? "/Home/JohnHenry" },
            new BreadcrumbItem { Name = "QUẦN NAM", Url = "" }
        };

        var breadcrumbJsonLd = _seoService.GenerateBreadcrumbJsonLd(breadcrumbs);

        ViewBag.BreadcrumbJsonLd = breadcrumbJsonLd;
        ViewBag.Breadcrumbs = breadcrumbs;
        ViewBag.MetaTitle = "Quần Nam JOHN HENRY - Thời trang nam cao cấp";
        ViewBag.MetaDescription = "Khám phá bộ sưu tập quần nam JOHN HENRY với đa dạng các loại quần jean, quần khaki, quần short chất lượng cao, phong cách hiện đại.";

        // Load men's trousers from database BY SUBCATEGORY AND SKU PATTERN
        var trousersCategory = await _context.Categories
            .FirstOrDefaultAsync(c => c.Name == "Quần nam");

        if (trousersCategory == null)
        {
            return View(new List<Product>());
        }

        // Get total count for pagination - Filter by category AND John Henry SKU pattern (NOT starting with FW)
        var totalProducts = await _context.Products
            .Where(p => p.IsActive && p.CategoryId == trousersCategory.Id && !p.SKU.StartsWith("FW"))
            .CountAsync();

        // Calculate pagination
        var totalPages = (int)Math.Ceiling(totalProducts / (double)pageSize);
        page = Math.Max(1, Math.Min(page, totalPages));

        var products = await _context.Products
            .Where(p => p.IsActive && p.CategoryId == trousersCategory.Id && !p.SKU.StartsWith("FW"))
            .OrderByDescending(p => p.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        // Pass pagination info to view
        ViewBag.CurrentPage = page;
        ViewBag.TotalPages = totalPages;
        ViewBag.TotalProducts = totalProducts;
        ViewBag.PageSize = pageSize;

        return View(products);
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public async Task<IActionResult> JohnHenryAccessories(int page = 1)
    {
        const int pageSize = 40; // 10 rows x 4 products per row

        // Generate breadcrumbs for John Henry Accessories page
        var breadcrumbs = new List<BreadcrumbItem>
        {
            new BreadcrumbItem { Name = "Trang chủ", Url = Url.Action("Index", "Home") ?? "/" },
            new BreadcrumbItem { Name = "JOHN HENRY", Url = Url.Action("JohnHenry", "Home") ?? "/Home/JohnHenry" },
            new BreadcrumbItem { Name = "PHỤ KIỆN NAM", Url = "" }
        };

        var breadcrumbJsonLd = _seoService.GenerateBreadcrumbJsonLd(breadcrumbs);

        ViewBag.BreadcrumbJsonLd = breadcrumbJsonLd;
        ViewBag.Breadcrumbs = breadcrumbs;
        ViewBag.MetaTitle = "Phụ Kiện Nam JOHN HENRY - Thời trang nam cao cấp";
        ViewBag.MetaDescription = "Khám phá bộ sưu tập phụ kiện nam JOHN HENRY với đa dạng các loại thắt lưng, cà vạt, ví da, túi xách chất lượng cao, phong cách hiện đại.";

        // Load men's accessories from database BY SUBCATEGORY AND SKU PATTERN
        var accessoriesCategory = await _context.Categories
            .FirstOrDefaultAsync(c => c.Name == "Phụ kiện nam");

        if (accessoriesCategory == null)
        {
            return View(new List<Product>());
        }

        // Get total count for pagination - Filter by category AND John Henry SKU pattern (NOT starting with FW)
        var totalProducts = await _context.Products
            .Where(p => p.IsActive && p.CategoryId == accessoriesCategory.Id && !p.SKU.StartsWith("FW"))
            .CountAsync();

        // Calculate pagination
        var totalPages = (int)Math.Ceiling(totalProducts / (double)pageSize);
        page = Math.Max(1, Math.Min(page, totalPages));

        var products = await _context.Products
            .Where(p => p.IsActive && p.CategoryId == accessoriesCategory.Id && !p.SKU.StartsWith("FW"))
            .OrderByDescending(p => p.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        // Pass pagination info to view
        ViewBag.CurrentPage = page;
        ViewBag.TotalPages = totalPages;
        ViewBag.TotalProducts = totalProducts;
        ViewBag.PageSize = pageSize;

        return View(products);
    }

    public IActionResult Blog()
    {
        // Generate breadcrumbs for Blog page
        var breadcrumbs = new List<BreadcrumbItem>
        {
            new BreadcrumbItem { Name = "Trang chủ", Url = Url.Action("Index", "Home") ?? "/" },
            new BreadcrumbItem { Name = "BLOG", Url = "" }
        };

        var breadcrumbJsonLd = _seoService.GenerateBreadcrumbJsonLd(breadcrumbs);

        ViewBag.BreadcrumbJsonLd = breadcrumbJsonLd;
        ViewBag.Breadcrumbs = breadcrumbs;
        ViewBag.MetaTitle = "BLOG - John Henry Fashion";
        ViewBag.MetaDescription = "Khám phá những bài viết mới nhất về thời trang, phong cách sống và xu hướng hiện đại.";

        return View();
    }

    public IActionResult Uniform()
    {
        // Generate breadcrumbs for Uniform page
        var breadcrumbs = new List<BreadcrumbItem>
        {
            new BreadcrumbItem { Name = "Trang chủ", Url = Url.Action("Index", "Home") ?? "/" },
            new BreadcrumbItem { Name = "UNIFORM", Url = "" }
        };

        var breadcrumbJsonLd = _seoService.GenerateBreadcrumbJsonLd(breadcrumbs);

        ViewBag.BreadcrumbJsonLd = breadcrumbJsonLd;
        ViewBag.Breadcrumbs = breadcrumbs;
        ViewBag.MetaTitle = "UNIFORM - Đồng phục chuyên nghiệp";
        ViewBag.MetaDescription = "Khám phá bộ sưu tập đồng phục chuyên nghiệp cho doanh nghiệp, salon làm đẹp, khách sạn với chất lượng cao và thiết kế hiện đại.";

        return View();
    }

    public IActionResult SeoTest()
    {
        // Generate breadcrumbs for SEO test page
        var breadcrumbs = new List<BreadcrumbItem>
        {
            new BreadcrumbItem { Name = "Trang chủ", Url = Url.Action("Index", "Home") ?? "/" },
            new BreadcrumbItem { Name = "SEO Test", Url = "" }
        };

        var breadcrumbJsonLd = _seoService.GenerateBreadcrumbJsonLd(breadcrumbs);

        ViewBag.BreadcrumbJsonLd = breadcrumbJsonLd;
        ViewBag.Breadcrumbs = breadcrumbs;
        ViewBag.MetaTitle = "SEO Test - John Henry Fashion";
        ViewBag.MetaDescription = "This is a test page to demonstrate SEO implementation with structured data and meta tags";

        return View();
    }

    private ProductViewModel MapToProductViewModel(Product product)
    {
        return new ProductViewModel
        {
            Id = product.Id,
            Name = product.Name,
            SKU = product.SKU,
            Price = product.Price, // Chỉ dùng giá gốc từ database
            FeaturedImageUrl = product.FeaturedImageUrl ?? "/images/default-product.jpg",
            StockQuantity = product.StockQuantity,
            CategoryName = product.Category?.Name ?? "Chưa phân loại",
            BrandName = product.Brand?.Name,
            Rating = product.Rating,
            ReviewCount = product.ReviewCount,
            IsNew = product.CreatedAt > DateTime.UtcNow.AddDays(-30),
            IsFeatured = product.IsFeatured
        };
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }

    // Temporary endpoint to update featured products
    public async Task<IActionResult> UpdateFeaturedProducts()
    {
        var johnHenrySKUs = new[] {
            "JK25FH04C-PA", "KS25SS20P-SC", "WS25FH63P-LC", "JK25FH02P-CT",
            "KS25FH49C-SLWK", "KS25SS31T-SCWK", "WS25FH58C-CFBB", "JK25FH10T-KA"
        };

        var freelancerSKUs = new[] {
            "FWBZ23SS06C", "FWWS25SS02G", "FWTS24FH03G", "FWTS25SS14C",
            "FWDR24FH01G", "FWDR25SS29G", "FWDR25FH04C", "FWSK23SS14G"
        };

        var bestSellerSKUs = new[] {
            "FWSK25FH12C", "FWSK24FH13C", "KP25SS06T-NMWFSL", "FWDP24SS06C",
            "JK25FH03C-CT", "BZ25FH06C-SL", "BZ24FH02P-SL", "WS24SS15P-SCRG"
        };

        // Reset all products to not featured
        await _context.Database.ExecuteSqlRawAsync("UPDATE \"Products\" SET \"IsFeatured\" = false");

        // Update John Henry products
        var allSKUs = johnHenrySKUs.Concat(freelancerSKUs).Concat(bestSellerSKUs).ToArray();

        var products = await _context.Products
            .Where(p => allSKUs.Contains(p.SKU))
            .ToListAsync();

        foreach (var product in products)
        {
            product.IsFeatured = true;
            product.UpdatedAt = DateTime.UtcNow;
        }

        await _context.SaveChangesAsync();

        return Json(new
        {
            success = true,
            message = $"Updated {products.Count} products as featured",
            johnHenryCount = products.Count(p => johnHenrySKUs.Contains(p.SKU)),
            freelancerCount = products.Count(p => freelancerSKUs.Contains(p.SKU)),
            bestSellerCount = products.Count(p => bestSellerSKUs.Contains(p.SKU)),
            products = products.Select(p => new { p.SKU, p.Name, p.IsFeatured })
        });
    }

    // Debug endpoint to analyze category distribution
    public async Task<IActionResult> CategoryAnalysis()
    {
        // Get all categories with product counts
        var categories = await _context.Categories
            .Select(c => new
            {
                CategoryId = c.Id,
                CategoryName = c.Name,
                CategorySlug = c.Slug,
                TotalProducts = _context.Products.Count(p => p.CategoryId == c.Id && p.IsActive),
                SampleProducts = _context.Products
                    .Where(p => p.CategoryId == c.Id && p.IsActive)
                    .OrderBy(p => p.SKU)
                    .Take(5)
                    .Select(p => new { p.SKU, p.Name, BrandName = p.Brand != null ? p.Brand.Name : null })
                    .ToList()
            })
            .OrderByDescending(c => c.TotalProducts)
            .ToListAsync();

        // Get brand distribution
        var brands = await _context.Brands
            .Select(b => new
            {
                BrandId = b.Id,
                BrandName = b.Name,
                TotalProducts = _context.Products.Count(p => p.BrandId == b.Id && p.IsActive)
            })
            .OrderByDescending(b => b.TotalProducts)
            .ToListAsync();

        // Check products with SKU patterns
        var johnHenryPattern = await _context.Products
            .Where(p => p.IsActive && !p.SKU.StartsWith("FW"))
            .GroupBy(p => p.Category!.Name)
            .Select(g => new { Category = g.Key, Count = g.Count() })
            .ToListAsync();

        var freelancerPattern = await _context.Products
            .Where(p => p.IsActive && p.SKU.StartsWith("FW"))
            .GroupBy(p => p.Category!.Name)
            .Select(g => new { Category = g.Key, Count = g.Count() })
            .ToListAsync();

        return Json(new
        {
            totalActiveProducts = await _context.Products.CountAsync(p => p.IsActive),
            categoriesAnalysis = categories,
            brandsAnalysis = brands,
            johnHenryPatternByCategory = johnHenryPattern,
            freelancerPatternByCategory = freelancerPattern,
            message = "Products are filtered by CategoryId. Check if category assignments match SKU patterns."
        });
    }

    /// <summary>
    /// Fix sunglasses products category assignment (Public endpoint for testing)
    /// Moves FWSG* products from "Chân váy nữ" to "Phụ kiện nữ"
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> FixSunglassesCategory()
    {
        try
        {
            // Get the correct category ID for "Phụ kiện nữ"
            var accessoriesCategory = await _context.Categories
                .FirstOrDefaultAsync(c => c.Name == "Phụ kiện nữ");
            
            if (accessoriesCategory == null)
            {
                return Json(new
                {
                    success = false,
                    message = "Category 'Phụ kiện nữ' not found"
                });
            }

            // Find all sunglasses products (SKU starting with FWSG)
            var sunglassesProducts = await _context.Products
                .Where(p => p.SKU.StartsWith("FWSG") && p.IsActive)
                .ToListAsync();

            if (!sunglassesProducts.Any())
            {
                return Json(new
                {
                    success = true,
                    message = "No sunglasses products found to update",
                    productsUpdated = 0
                });
            }

            // Get old categories for reporting
            var oldCategoryIds = new Dictionary<Guid, string>();
            foreach (var product in sunglassesProducts)
            {
                var oldCategory = await _context.Categories
                    .FirstOrDefaultAsync(c => c.Id == product.CategoryId);
                
                oldCategoryIds[product.Id] = oldCategory?.Name ?? "Unknown";
                product.CategoryId = accessoriesCategory.Id;
            }

            await _context.SaveChangesAsync();

            return Json(new
            {
                success = true,
                message = $"Successfully updated {sunglassesProducts.Count} sunglasses products from wrong category to 'Phụ kiện nữ'",
                productsUpdated = sunglassesProducts.Count,
                products = sunglassesProducts.Select(p => new
                {
                    sku = p.SKU,
                    name = p.Name,
                    oldCategory = oldCategoryIds[p.Id],
                    newCategory = "Phụ kiện nữ"
                }).ToList(),
                nextSteps = new[]
                {
                    "Restart application to clear cache",
                    "Check FreelancerSkirt page - should show only skirts",
                    "Check FreelancerAccessories page - should show 5 products"
                }
            });
        }
        catch (Exception ex)
        {
            return Json(new
            {
                success = false,
                message = $"Error: {ex.Message}",
                stackTrace = ex.StackTrace
            });
        }
    }
}
