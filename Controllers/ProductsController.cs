using Microsoft.AspNetCore.Mvc;

namespace JohnHenryFashionWeb.Controllers
{
    public class ProductsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Details(int id)
        {
            ViewData["ProductId"] = id;
            return View();
        }
    }
}
