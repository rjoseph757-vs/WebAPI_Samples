using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using AuthorizationServer.Data.Constants;

namespace AuthorizationServer.Data
{
    public static class InitializerExtensions
    {
        public static async Task InitialiseDatabaseAsync(this WebApplication app)
        {
            using var scope = app.Services.CreateScope();

            var initializer = scope.ServiceProvider.GetRequiredService<ApplicationDbContextInitializer>();

            await initializer.InitializeAsync();

            await initializer.SeedAsync();
        }
    }

    public class ApplicationDbContextInitializer(ILogger<ApplicationDbContextInitializer> logger, ApplicationDbContext context, UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager)
    {
        private readonly ILogger<ApplicationDbContextInitializer> _logger = logger;
        private readonly ApplicationDbContext _context = context;
        private readonly UserManager<ApplicationUser> _userManager = userManager;
        private readonly RoleManager<ApplicationRole> _roleManager = roleManager;

        public async Task InitializeAsync()
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
            var superadminRole = new ApplicationRole(RolesConstants.SuperAdmin);
            if (_roleManager.Roles.All(r => r.Name != superadminRole.Name))
            {
                await _roleManager.CreateAsync(superadminRole);
            }

            var adminRole = new ApplicationRole(RolesConstants.Admin);
            if (_roleManager.Roles.All(r => r.Name != adminRole.Name))
            {
                await _roleManager.CreateAsync(adminRole);
            }
            var fieldManagerRole = new ApplicationRole(RolesConstants.FieldManager);
            if (_roleManager.Roles.All(r => r.Name !=fieldManagerRole.Name))
            {
                await _roleManager.CreateAsync(fieldManagerRole);
            }

            var vendorRole = new ApplicationRole(RolesConstants.Vendor);
            if (_roleManager.Roles.All(r => r.Name != vendorRole.Name))
            {
                await _roleManager.CreateAsync(vendorRole);
            }

            var userRole = new ApplicationRole(RolesConstants.User);
            if (_roleManager.Roles.All(r => r.Name != userRole.Name))
            {
                await _roleManager.CreateAsync(userRole);
            }

            // Default users
            var superadmin = new ApplicationUser { UserName = "superadmin@localhost", Email = "superadmin@localhost", EmailConfirmed = true };
            if (_userManager.Users.All(u => u.UserName != superadmin.UserName))
            {
                await _userManager.CreateAsync(superadmin, "SuperAdmin1!");
                if (!string.IsNullOrWhiteSpace(superadminRole.Name))
                {
                    await _userManager.AddToRolesAsync(superadmin, new[] { superadminRole.Name });
                }
            }

            var admin = new ApplicationUser { UserName = "admin@localhost", Email = "admin@localhost", EmailConfirmed = true };
            if (_userManager.Users.All(u => u.UserName != admin.UserName))
            {
                await _userManager.CreateAsync(admin, "Admin1!");
                if (!string.IsNullOrWhiteSpace(adminRole.Name))
                {
                    await _userManager.AddToRolesAsync(admin, new[] { adminRole.Name });
                }
            }

            var fieldmanager = new ApplicationUser { UserName = "fieldmanager@localhost", Email = "fieldmanager@localhost", EmailConfirmed = true };
            if (_userManager.Users.All(u => u.UserName != fieldmanager.UserName))
            {
                await _userManager.CreateAsync(fieldmanager, "Fieldmanager1!");
                if (!string.IsNullOrWhiteSpace(vendorRole.Name))
                {
                    await _userManager.AddToRolesAsync(fieldmanager, new[] { vendorRole.Name });
                }
            }

            var vendor = new ApplicationUser { UserName = "vendor@localhost", Email = "vendor@localhost", EmailConfirmed = true };
            if (_userManager.Users.All(u => u.UserName != vendor.UserName))
            {
                await _userManager.CreateAsync(vendor, "Vendor1!");
                if (!string.IsNullOrWhiteSpace(vendorRole.Name))
                {
                    await _userManager.AddToRolesAsync(vendor, new[] { vendorRole.Name });
                }
            }

            var user = new ApplicationUser { UserName = "user@localhost", Email = "user@localhost", EmailConfirmed = true };
            if (_userManager.Users.All(u => u.UserName != user.UserName))
            {
                await _userManager.CreateAsync(user, "User111!");
                if (!string.IsNullOrWhiteSpace(userRole.Name))
                {
                    await _userManager.AddToRolesAsync(user, new[] { userRole.Name });
                }
            }
        }
    }

}
