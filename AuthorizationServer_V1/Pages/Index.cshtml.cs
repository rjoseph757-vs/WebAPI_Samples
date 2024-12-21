using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AuthorizationServer.Pages
{
    public class IndexModel : PageModel
    {
        public void OnGet()
        {
            ViewData["heading"] = "Welcome to ASP.NET Core Razor Pages for Act!";
        }
    }
}