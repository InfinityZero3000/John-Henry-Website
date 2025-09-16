using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using JohnHenryFashionWeb.Models;
using JohnHenryFashionWeb.Services;

namespace JohnHenryFashionWeb.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly SeoService _seoService;

    public HomeController(ILogger<HomeController> logger, SeoService seoService)
    {
        _logger = logger;
        _seoService = seoService;
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
            new BreadcrumbItem { Name = "FREELANCER For Her", Url = "" }
        };

        var breadcrumbJsonLd = _seoService.GenerateBreadcrumbJsonLd(breadcrumbs);

        ViewBag.BreadcrumbJsonLd = breadcrumbJsonLd;
        ViewBag.Breadcrumbs = breadcrumbs;
        ViewBag.MetaTitle = "FREELANCER For Her - Thời trang nữ hiện đại";
        ViewBag.MetaDescription = "Khám phá bộ sưu tập FREELANCER For Her với các sản phẩm thời trang nữ chất lượng cao, phong cách hiện đại và sang trọng dành riêng cho phái đẹp.";

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
