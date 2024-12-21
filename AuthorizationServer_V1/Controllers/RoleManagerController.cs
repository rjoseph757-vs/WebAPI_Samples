using AuthorizationServer.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AuthorizationServer.Controllers
{
    public class RoleManagerController : Controller
    {
        private readonly RoleManager<ApplicationRole> _roleManager;
        public RoleManagerController(RoleManager<ApplicationRole> roleManager)
        {
            _roleManager = roleManager;
        }

        public async Task<IActionResult> Index()
        {
            var roles = await _roleManager.Roles.ToListAsync();
            return View(roles);
        }

        [HttpPost]
        public async Task<IActionResult> AddRole(string roleName)
        {
            if (roleName != null)
            {
                await _roleManager.CreateAsync(new ApplicationRole(roleName.Trim()));
            }
            return RedirectToAction("Index");
        }
    }
}
