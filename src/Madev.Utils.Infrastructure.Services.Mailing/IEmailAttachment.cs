namespace Madev.Utils.Infrastructure.Services.Mailing
{
    public interface IEmailAttachment{
        public bool IsInline { get; set; }
        public string? ContentId { get; set; }
        public string? ContentType { get; set; }
    }
}