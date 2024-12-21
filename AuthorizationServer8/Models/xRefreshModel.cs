namespace AuthorizationServer8.Models
{
    public class xRefreshModel
    {
        public required string AccessToken { get; set; }
        public required string RefreshToken { get; set; }
    }
}
