
using System.ComponentModel.DataAnnotations;

namespace AuthorizationServer8.ViewModels
{
    public class xLoginViewModel
    {
        [Required]
        public string Username { get; set; }
        [Required]
        public string Password { get; set; }
        public string ReturnUrl { get; set; }
    }
}
