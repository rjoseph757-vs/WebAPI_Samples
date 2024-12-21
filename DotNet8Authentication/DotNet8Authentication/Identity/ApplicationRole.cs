using Microsoft.AspNetCore.Identity;

namespace DotNet8Authentication.Identity
{
    public class ApplicationRole : IdentityRole
    {
        // Permissions Permissions { get; set; }
        public ApplicationRole(string roleName) : base(roleName)
        {
        }
    }
}
