using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Protocols.Configuration;

public class FakeEmailService : IEmailService
{

    public static Dictionary<string, string> Tokens = new Dictionary<string, string>();

    public FakeEmailService(IOptions<EmailSettings> _emailSettings) {}
    
    /// <summary>
    /// Send an email to a given address
    /// </summary>
    /// <param name="email">The destination email address</param>
    /// <param name="token">The recovery token</param>
    /// <returns></returns>
    public Task SendRecoveryEmailAsync(string email, string token)
    {
        if(Tokens.ContainsKey(email)) Tokens.Remove(email);
        
        Tokens.Add(email, token);
        return Task.CompletedTask;
    }

    public static void Clear() => Tokens.Clear();
}
