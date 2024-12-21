using Microsoft.AspNetCore.Identity;

namespace AuthorizationServer8.Data
{
    public static class ContextSeed
    {
        public static async Task SeedRolesAsync(UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager)
        {
            //Seed Roles
            await roleManager.CreateAsync(new ApplicationRole(Enums.Roles.SuperAdmin.ToString()));
            await roleManager.CreateAsync(new ApplicationRole(Enums.Roles.Admin.ToString()));
            await roleManager.CreateAsync(new ApplicationRole(Enums.Roles.FieldManager.ToString()));
            await roleManager.CreateAsync(new ApplicationRole(Enums.Roles.Vendor.ToString()));
            await roleManager.CreateAsync(new ApplicationRole(Enums.Roles.User.ToString()));
            await SeedSuperAdminAsync(userManager);
        }

        public static async Task SeedSuperAdminAsync(UserManager<ApplicationUser> userManager)
        {
            //Seed Default User
            var defaultUser = new ApplicationUser
            {
                UserName = "superadmin",
                Email = "superadmin@gmail.com",
                FirstName = "Super",
                LastName = "Admin",
                EmailConfirmed = true,
                PhoneNumberConfirmed = true
            };
            if (userManager.Users.All(u => u.Id != defaultUser.Id))
            {
                var user = await userManager.FindByEmailAsync(defaultUser.Email);
                if (user == null)
                {
                    await userManager.CreateAsync(defaultUser, "123Pa$$word.");
                    await userManager.AddToRoleAsync(defaultUser, Enums.Roles.User.ToString());
                    await userManager.AddToRoleAsync(defaultUser, Enums.Roles.Vendor.ToString());
                    await userManager.AddToRoleAsync(defaultUser, Enums.Roles.Admin.ToString());
                    await userManager.AddToRoleAsync(defaultUser, Enums.Roles.SuperAdmin.ToString());
                }
            }
        }
    }
}
