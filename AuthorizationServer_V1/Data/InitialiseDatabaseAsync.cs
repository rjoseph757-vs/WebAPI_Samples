using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using AuthorizationServer.Data.Enums;

namespace AuthorizationServer.Data
{
    public static class InitialiserExtensions
    {
        public static async Task InitialiseDatabaseAsync(this WebApplication app)
        {
            using var scope = app.Services.CreateScope();

            var initialiser = scope.ServiceProvider.GetRequiredService<ApplicationDbContextInitialiser>();

            await initialiser.InitialiseAsync();

            await initialiser.SeedAsync();
        }
    }

    public class ApplicationDbContextInitialiser(ILogger<ApplicationDbContextInitialiser> logger, ApplicationDbContext context, UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager)
    {
        private readonly ILogger<ApplicationDbContextInitialiser> _logger = logger;
        private readonly ApplicationDbContext _context = context;
        private readonly UserManager<ApplicationUser> _userManager = userManager;
        private readonly RoleManager<ApplicationRole> _roleManager = roleManager;

        public async Task InitialiseAsync()
        {
            try
            {
                await _context.Database.MigrateAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while initialising the database.");
                throw;
            }
        }

        public async Task SeedAsync()
        {
            try
            {
                await TrySeedAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while seeding the database.");
                throw;
            }
        }

        public async Task TrySeedAsync()
        {
            // Default roles
            var superadminRole = new ApplicationRole(Roles.SuperAdmin.ToString());
            if (_roleManager.Roles.All(r => r.Name != superadminRole.Name))
            {
                await _roleManager.CreateAsync(superadminRole);
            }

            var adminRole = new ApplicationRole(Roles.Admin.ToString());
            if (_roleManager.Roles.All(r => r.Name != adminRole.Name))
            {
                await _roleManager.CreateAsync(adminRole);
            }
            var fieldManagerRole = new ApplicationRole(Roles.FieldManager.ToString());
            if (_roleManager.Roles.All(r => r.Name !=fieldManagerRole.Name))
            {
                await _roleManager.CreateAsync(fieldManagerRole);
            }

            var vendorRole = new ApplicationRole(Roles.Vendor.ToString());
            if (_roleManager.Roles.All(r => r.Name != vendorRole.Name))
            {
                await _roleManager.CreateAsync(vendorRole);
            }

            var userRole = new ApplicationRole(Roles.User.ToString());
            if (_roleManager.Roles.All(r => r.Name != userRole.Name))
            {
                await _roleManager.CreateAsync(userRole);
            }

            // Default users
            var superadmin = new ApplicationUser { UserName = "superadmin@localhost", Email = "superadmin@localhost" };
            if (_userManager.Users.All(u => u.UserName != superadmin.UserName))
            {
                await _userManager.CreateAsync(superadmin, "SuperAdmin1!");
                if (!string.IsNullOrWhiteSpace(superadminRole.Name))
                {
                    await _userManager.AddToRolesAsync(superadmin, new[] { superadminRole.Name });
                }
            }

            var admin = new ApplicationUser { UserName = "admin@localhost", Email = "admin@localhost" };
            if (_userManager.Users.All(u => u.UserName != admin.UserName))
            {
                await _userManager.CreateAsync(admin, "Admin1!");
                if (!string.IsNullOrWhiteSpace(adminRole.Name))
                {
                    await _userManager.AddToRolesAsync(admin, new[] { adminRole.Name });
                }
            }

            var fieldmanager = new ApplicationUser { UserName = "fieldmanager@localhost", Email = "fieldmanager@localhost" };
            if (_userManager.Users.All(u => u.UserName != fieldmanager.UserName))
            {
                await _userManager.CreateAsync(fieldmanager, "Fieldmanager1!");
                if (!string.IsNullOrWhiteSpace(vendorRole.Name))
                {
                    await _userManager.AddToRolesAsync(fieldmanager, new[] { vendorRole.Name });
                }
            }

            var vendor = new ApplicationUser { UserName = "vendor@localhost", Email = "vendor@localhost" };
            if (_userManager.Users.All(u => u.UserName != vendor.UserName))
            {
                await _userManager.CreateAsync(vendor, "Vendor1!");
                if (!string.IsNullOrWhiteSpace(vendorRole.Name))
                {
                    await _userManager.AddToRolesAsync(vendor, new[] { vendorRole.Name });
                }
            }

            var user = new ApplicationUser { UserName = "user@localhost", Email = "user@localhost" };
            if (_userManager.Users.All(u => u.UserName != user.UserName))
            {
                await _userManager.CreateAsync(user, "User1!");
                if (!string.IsNullOrWhiteSpace(userRole.Name))
                {
                    await _userManager.AddToRolesAsync(user, new[] { userRole.Name });
                }
            }
        }
    }

}
