using Microsoft.AspNetCore.Mvc;

namespace JohnHenryFashionWeb.Controllers
{
    public class ContactController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
