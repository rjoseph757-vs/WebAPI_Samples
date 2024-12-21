namespace AuthorizationServer8.Models
{
    public class PermissionViewModel
    {
        public string RoleId { get; set; }
        public IList<RoleClaimsViewModel> RoleClaims { get; set; }
    }
}