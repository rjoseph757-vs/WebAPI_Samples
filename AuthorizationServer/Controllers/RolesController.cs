using AuthorizationServer.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AuthorizationServer.Controllers
{
    [Authorize(Roles = "SuperAdmin")]
    public class RolesController(
        RoleManager<ApplicationRole> roleManager,
        ILogger<RolesController> logger
            ) : ControllerBase
    {
        private readonly RoleManager<ApplicationRole> _roleManager = roleManager;
        private readonly ILogger<RolesController> _logger = logger;

        //[HttpGet("ListRoles", Name = nameof(ListRolesAsync))]
        public async Task<ActionResult<IList<ApplicationRole>>> ListRolesAsync()
        {
            var roles = await _roleManager.Roles.ToListAsync();
            if (roles.Count == 0)
            {
                return NotFound();
            }

            return Ok(roles);
        }
    }
}