namespace AuthorizationServer8.Services
{
    public class AuthMessageSenderOptions
    {
        public string SendGridKey { get; set; }
        public string SendGridEmailFrom { get; set; } = "donotreply@tmgusa.co";
        public string SendGridEmailFromName { get; set; } = "Password Recovery";
    }
}
