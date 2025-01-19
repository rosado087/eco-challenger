using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Protocols.Configuration;

public class EmailService : IEmailService
{
    private readonly EmailSettings _emailSettings;

    public EmailService(IOptions<EmailSettings> emailSettings)
    {
        _emailSettings = emailSettings.Value;
    }
    
    public async Task SendEmailAsync(string to, string subject, string body)
    {
        if(_emailSettings == null) throw new InvalidConfigurationException("No email settings were configured.");

        using (var client = new SmtpClient(_emailSettings.SmtpServer, _emailSettings.SmtpPort))
        {
            client.EnableSsl = _emailSettings.UseSsl;
            client.Credentials = new NetworkCredential(_emailSettings.SmtpUser, _emailSettings.SmtpPass);

            var mailMessage = new MailMessage
            {
                From = new MailAddress(_emailSettings.Sender),
                Subject = subject,
                Body = body,
                IsBodyHtml = false
            };
            mailMessage.To.Add(to);

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
}
