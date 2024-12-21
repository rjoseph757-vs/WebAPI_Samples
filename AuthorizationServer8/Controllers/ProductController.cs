using Microsoft.AspNetCore.Mvc;

namespace AuthorizationServer8.Controllers
{
    public class ProductController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
