using Microsoft.AspNetCore.Mvc;

namespace AuthorizationServer8.Controllers
{
    //[Authorize]
    public class xUserController : Controller
    {
        private readonly ILogger<xUserController> _logger;

        public xUserController(ILogger<xUserController> logger)
        {
            _logger = logger;
        }

        public IActionResult Claims()
        {
            return View();
        }
    }
}
