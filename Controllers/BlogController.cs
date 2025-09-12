using Microsoft.AspNetCore.Mvc;

namespace JohnHenryFashionWeb.Controllers
{
    public class BlogController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Details(int id)
        {
            ViewBag.Id = id;
            return View();
        }
    }
}
