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

    public IActionResult Index()
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

        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    public IActionResult JohnHenry()
    {
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

        return View();
    }

    public IActionResult Freelancer()
    {
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

        return View();
    }

    public IActionResult FreelancerDress()
    {
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

        return View();
    }

    public IActionResult FreelancerShirt()
    {
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

        return View();
    }

    public IActionResult FreelancerTrousers()
    {
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

        return View();
    }

    public IActionResult FreelancerSkirt()
    {
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

        return View();
    }

    public IActionResult JohnHenryShirt()
    {
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

        return View();
    }

    public IActionResult JohnHenryTrousers()
    {
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

        return View();
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

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
