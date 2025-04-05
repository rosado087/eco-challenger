public interface IEmailService
{
    Task SendRecoveryEmailAsync(string email, string token);
}
