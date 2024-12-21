using AuthorizationServer8.Constants;
using AuthorizationServer8.Data;
using AuthorizationServer8.Models;
using Microsoft.AspNetCore.Identity;

namespace AuthorizationServer8.Seeds
{
    public static class DefaultRoles
    {
        public static async Task SeedAsync(UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager)
        {
            await roleManager.CreateAsync(new ApplicationRole(Roles.SuperAdmin.ToString()));
            await roleManager.CreateAsync(new ApplicationRole(Roles.Admin.ToString()));
            await roleManager.CreateAsync(new ApplicationRole(Roles.User.ToString()));
        }
    }
}
