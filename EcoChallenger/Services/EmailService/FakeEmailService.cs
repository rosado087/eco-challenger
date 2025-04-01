using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Protocols.Configuration;

public class FakeEmailService : IEmailService
{

    public static Dictionary<string, string> Tokens;

    public FakeEmailService(IOptions<EmailSettings> _emailSettings)
    {
        Tokens = new Dictionary<string, string>();
    }
    
    /// <summary>
    /// Send an email to a given address
    /// </summary>
    /// <param name="email">The destination email address</param>
    /// <param name="token">The destination email address</param>
    /// <returns></returns>
    public Task SendRecoveryEmailAsync(string email, string token)
    {
        Tokens[email] = token;
        return Task.CompletedTask;
    }

     public static void Clear() => Tokens.Clear();
}
