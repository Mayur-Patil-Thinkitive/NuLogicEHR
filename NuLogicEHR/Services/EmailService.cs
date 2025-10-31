using SendGrid;
using SendGrid.Helpers.Mail;
using Microsoft.Extensions.Options;

namespace NuLogicEHR.Services
{
    public class EmailService
    {
        private readonly ISendGridClient _sendGridClient;
        private readonly EmailSettings _emailSettings;
        private readonly ILogger<EmailService> _logger;

        public EmailService(ISendGridClient sendGridClient, IOptions<EmailSettings> emailSettings, ILogger<EmailService> logger)
        {
            _sendGridClient = sendGridClient;
            _emailSettings = emailSettings.Value;
            _logger = logger;
        }
        public async Task<bool> SendIntakeFormEmailAsync(string toEmail, string patientName, string soberLivingHomeName = null)
        {
            try
            {
                var from = new EmailAddress(_emailSettings.FromEmail, _emailSettings.FromName);
                var to = new EmailAddress(toEmail, patientName);
                var subject = "Complete Your Intake Form for NuLease";

                var htmlContent = GetIntakeFormEmailTemplate(patientName, soberLivingHomeName);
                var plainTextContent = $"Hello {patientName}, Welcome to NuLease. Please complete your Intake Form to get started with your care.";

                var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);
                var response = await _sendGridClient.SendEmailAsync(msg);

                if (response.StatusCode == System.Net.HttpStatusCode.Accepted)
                {
                    _logger.LogInformation("Intake form email sent successfully to {Email}", toEmail);
                    return true;
                }
                else
                {
                    _logger.LogError("Failed to send intake form email to {Email}. Status: {Status}", toEmail, response.StatusCode);
                    return false;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending intake form email to {Email}", toEmail);
                return false;
            }
        }
        private string GetIntakeFormEmailTemplate(string patientName, string soberLivingHomeName)
        {
            return $@"
<!DOCTYPE html>
<html>
<head>
    <meta charset='utf-8'>
    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
    <title>Complete Your Intake Form for NuLease</title>
</head>
<body style='font-family: Arial, sans-serif; font-size: 14px; line-height: 1.4; color: #000; margin: 0; padding: 20px; background-color: #ffffff;'>
    
    <div style='max-width: none; width: 100%;'>
        <div style='margin-bottom: 15px;'>
            Hello {patientName},
        </div>

        <div style='margin-bottom: 15px;'>
            Welcome to NuLease. To get started with your care, we need you to complete your Intake Form. This will help us gather your medical, personal, and insurance information before your first visit.
        </div>

        <div style='margin-bottom: 15px;'>
            Please click the link below to securely fill out your Intake Form:
        </div>

        <div style='margin-bottom: 15px;'>
            <a href='#' style='color: #1a73e8; text-decoration: none;'>Intake Form Link</a>
        </div>

        <div style='margin-bottom: 15px;'>
            It should only take 10-15 minutes to complete. You can save and return later if needed.
        </div>

        <div style='margin-bottom: 10px;'>
            Thank you,<br>
            NuLease Care Team
        </div>

    </div>

</body>
</html>";
        }
    }
    public class EmailSettings
    {
        public string ApiKey { get; set; }
        public string FromEmail { get; set; }
        public string FromName { get; set; }
    }
}
