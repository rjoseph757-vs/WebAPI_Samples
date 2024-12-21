using Microsoft.AspNetCore.Identity;

namespace AuthorizationServer.Data
{
    public class ApplicationRole : IdentityRole
    {
        public ApplicationRole()
        {
            
        }
        public ApplicationRole(string roleName) : base(roleName)
        {
        }

        //public Permissions Permissions { get; set; }
    }
}
