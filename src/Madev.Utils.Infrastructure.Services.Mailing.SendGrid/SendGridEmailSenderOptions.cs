namespace Madev.Utils.Infrastructure.Services.Mailing.SendGrid
{
    public class SendGridEmailSenderOptions
    {
        public string ApiKey { get; set; } = null!;
        public string? Sender { get; set; }
    }
}
