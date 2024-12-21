using Microsoft.AspNetCore.Mvc;

namespace AuthorizationServer.Controllers
{
    //[Authorize]
    public class UserController : Controller
    {
        private readonly ILogger<UserController> _logger;

        public UserController(ILogger<UserController> logger)
        {
            _logger = logger;
        }

        public IActionResult Claims()
        {
            return View();
        }
    }
}
