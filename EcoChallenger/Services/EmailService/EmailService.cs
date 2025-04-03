using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Protocols.Configuration;

public class EmailService : IEmailService
{
    private readonly EmailSettings _emailSettings;

    private readonly IConfiguration _configuration;
    private readonly ILogger<EmailService> _logger;

    public EmailService(IOptions<EmailSettings> emailSettings)
    {
        _emailSettings = emailSettings.Value;
    }
    
    /// <summary>
    /// Send an email to a given address
    /// </summary>
    /// <param name="email">The destination email address</param>
    /// <param name="token">The destination email address</param>
    /// <param name="subject">The title of the email</param>
    /// <param name="body">The body text of the email</param>
    /// <returns></returns>
    public async Task SendRecoveryEmailAsync(string email, string token)
    {
        if(_emailSettings == null) throw new InvalidConfigurationException("No email settings were configured.");

        string recoveryLink = GetResetPasswordUrl(token);

        using (var client = new SmtpClient(_emailSettings.SmtpServer, _emailSettings.SmtpPort))
        {
            client.EnableSsl = _emailSettings.UseSsl;
            client.Credentials = new NetworkCredential(_emailSettings.SmtpUser, _emailSettings.SmtpPass);
            string subject = "Echo-Challenger: Recuperação de Palavra-Passe";
            string body = "Para repore a sua palavra-passe, por favor aceda a este link: " + recoveryLink;

            var mailMessage = new MailMessage
            {
                From = new MailAddress(_emailSettings.Sender),
                Subject = subject,
                Body = body,
                IsBodyHtml = false
            };
            mailMessage.To.Add(email);

            try
            {
                await client.SendMailAsync(mailMessage);
            }
            catch (Exception ex)
            {
                // Log or handle the exception as needed
                throw new InvalidOperationException("Could not send email.", ex);
            }
        }
    }

    private string GetResetPasswordUrl(string token) {
        // Now we only need to send the email
        // To build the message we need to point the user to the correct URL
        string? baseUrl = _configuration.GetValue<string>("ApplicationSettings:FrontEndUrl");

        if(baseUrl == null){
            _logger.LogError("No application URL was configured in appsettings.json. Recovery emails are not being sent!");
        }

        // Make sure the baseURL value doesn't have a / at the end
        if(baseUrl.Substring(baseUrl.Length - 1) == "/")
            baseUrl = baseUrl.Substring(0, baseUrl.Length - 1);

        return baseUrl + "/reset-password/" + token;
    }
}
