using Microsoft.AspNetCore.Identity;

namespace AuthorizationServer.Data
{
    public class ApplicationUser : IdentityUser
    {
        //public virtual ICollection<IdentityUserClaim<string>> Claims { get; set; }
        //public virtual ICollection<IdentityUserLogin<string>> Logins { get; set; }
        //public virtual ICollection<IdentityUserToken<string>> Tokens { get; set; }

        public string FullName => $"{FirstName} {LastName}";
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int UsernameChangeLimit { get; set; } = 10;
        public byte[] ProfilePicture { get; set; }
    }
}
//--https://stackoverflow.com/questions/51004516/net-core-2-1-identity-get-all-users-with-their-associated-roles/71321924#71321924