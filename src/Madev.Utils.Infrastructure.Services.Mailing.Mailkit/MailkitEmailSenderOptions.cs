namespace Madev.Utils.Infrastructure.Services.Mailing.Mailkit
{
    public class MailkitEmailSenderOptions
    {
        public string Server { get; set; } = null!;
        public int Port { get; set; }
        public string Username { get; set; } = null!;
        public string Password { get; set; } = null!;
        public string? LocalDomain { get; set; }
    }
}