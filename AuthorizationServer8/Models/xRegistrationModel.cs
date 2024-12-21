using System.ComponentModel.DataAnnotations;

namespace AuthorizationServer8.Models
{
    public class xRegistrationModel
    {
        public required string Username { get; set; }
        public required string Password { get; set; }
        [EmailAddress]
        public required string Email { get; set; }
    }
}
