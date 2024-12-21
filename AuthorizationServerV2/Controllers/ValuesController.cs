using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AuthorizationServer.Controllers
{
    [Authorize(Roles = "SuperAdmin")] // Role-based authorization
    [ApiController]
    [Route("api/[controller]")]
    public class ValuesController : ControllerBase
    {
        [HttpGet]
        public IActionResult Get()
        {
            return Ok(new { message = "This is a protected resource." });
        }

        // Other endpoints
    }
}
