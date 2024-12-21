namespace AuthorizationServer8.Models
{
    public class xLoginResponse
    {
        public required string JwtToken { get; set; }
        public DateTime Expiration { get; set; }
        public required string RefreshToken { get; set; }
    }
}
