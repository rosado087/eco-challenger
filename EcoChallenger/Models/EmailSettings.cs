public class EmailSettings
{
    public string? Sender { get; set; }
    public string? SmtpServer { get; set; }
    public int SmtpPort { get; set; }
    public string? SmtpUser { get; set; }
    public string? SmtpPass { get; set; }
    public bool UseSsl { get; set; }
}
