using AuthorizationServer.Extensions;
using SendGrid;
using SendGrid.Helpers.Mail;
using System.Net;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;

namespace AuthorizationServer.Services
{
    public partial class EmailSenderUI : IEmailSenderUI
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger _logger;

        public EmailSenderUI(IConfiguration configuration, ILogger<EmailSenderUI> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public async Task SendEmailAsync(string toEmail, string subject, string htmlMessage, string fromEmailName)
        {
            if (string.IsNullOrEmpty(fromEmailName))
            {
                fromEmailName = _configuration.GetSection("SendGridEmailFrom").Value!;
            }
            await Execute(toEmail, subject, htmlMessage, fromEmailName);
        }

        public async Task<Response> Execute(string toEmail, string subject, string htmlMessage, string fromEmailName)
        {
            // Get SendGrid client ready
            var apiKey = _configuration.GetSection("SENDGRID_API_KEY").Value!;
            string apiUrl = _configuration.GetSection("SendGridAPIURL").Value!;
            string apiVersion = _configuration.GetSection("SendGridAPIVersion").Value!;
            string fromEmail = _configuration.GetSection("SendGridEmailFrom").Value!;
            var from = new EmailAddress(fromEmail, fromEmailName);

            _logger.LogInformation("toEmail: {toEmail}", toEmail);
            // Split out emails if they are a semi-colon separated list
            var distinctToEmailList = new List<EmailAddress>();
            foreach (var toEm in toEmail.ToLower().Split(";", StringSplitOptions.None))
            {
                if (StringExtensions.IsValidEmail(toEm.Trim()))
                {
                    distinctToEmailList.Add(new EmailAddress(toEm.Trim()));
                }
                else
                {
                    // Try anyway, but log it for comparison
                    distinctToEmailList.Add(new EmailAddress(toEm.Trim()));
                    _logger.LogWarning("Invalid Email?: {toEm.Trim()}", toEm.Trim());
                }
            }
            distinctToEmailList = distinctToEmailList.Distinct().ToList();
            var client = new SendGridClient(apiKey, apiUrl, null, apiVersion);
            string contentPlainText = GetPlainTextFromHtml(htmlMessage);
            string contentHtml = htmlMessage;

            //--https://sendgrid.com/docs/for-developers/sending-email/v3-csharp-code-example/
            // The "sender" cannot have multiple email addresses!!!
            //Issue: --https://github.com/sendgrid/sendgrid-csharp/pull/1080/commits/66f0ca6999ef69218252e29b258cc42e4c1ad609
            //var msg = MailHelper.CreateSingleEmail(from, to, subject, contentPlainText, contentHtml); //Plain Text MUST be first according to RFC 1341, section 7.2
            var msg = MailHelper.CreateSingleEmailToMultipleRecipients(from, distinctToEmailList, subject, contentPlainText, contentHtml); //Plain Text MUST be first according to RFC 1341, section 7.2
            // Disable click tracking. - See --https://sendgrid.com/docs/User_Guide/Settings/tracking.html
            msg.SetClickTracking(false, false);

            // Send email and track any errors
            var response = await client.SendEmailAsync(msg).ConfigureAwait(false);
            if (response.StatusCode != HttpStatusCode.Accepted)
            {
                _logger.LogDebug("SendGrid problem: {response.StatusCode}", response.StatusCode);
                _logger.LogError("response.StatusCode: {response.StatusCode}", response.StatusCode);
                _logger.LogError("response.Body.ReadAsStringAsync().Result: {+ response.Body.ReadAsStringAsync().Result} ", response.Body.ReadAsStringAsync().Result);
                _logger.LogError("response.Headers.ToString(): {response.Headers.ToString()}", response.Headers.ToString());
                throw new ExternalException("Error sending message");
            }

            _logger.LogInformation("SendGrid Execute Subject:[{subject}] ToEmail:[{toEmail}]", subject, toEmail);
            return response;
        }

        private static string GetPlainTextFromHtml(string htmlString)
        {
            // First make concession for linebreaks
            htmlString = htmlString.Replace("<br />", Environment.NewLine).Replace("<br/>", Environment.NewLine).Replace("<br>", Environment.NewLine);
            string htmlTagPattern = "<.*?>";
            var regexCss = MyRegex();
            htmlString = regexCss.Replace(htmlString, string.Empty);
            htmlString = Regex.Replace(htmlString, htmlTagPattern, string.Empty);
            htmlString = MyRegex1().Replace(htmlString, "");
            htmlString = htmlString.Replace(" ", string.Empty);

            return htmlString;
        }

        [GeneratedRegex(@"(\\<script(.+?)\\)|(\\<style(.+?)\\)", RegexOptions.IgnoreCase | RegexOptions.Singleline, "en-US")]
        private static partial Regex MyRegex();

        [GeneratedRegex(@"^\s+$[\r\n]*", RegexOptions.Multiline)]
        private static partial Regex MyRegex1();
    }
}
