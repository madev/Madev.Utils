using System.Collections.Generic;

namespace Madev.Utils.Infrastructure.Services.Mailing
{
    public class EmailMessage
    {
        public IEnumerable<string> To { get; set; } = new List<string>();
        public string Subject { get; set; } = string.Empty;
        public string Body { get; set; } = string.Empty;
        public string? From { get; set; }
        public IEnumerable<string> Cc { get; set; } = new List<string>();
        public IEnumerable<string> Bcc { get; set; } = new List<string>();
        public IEnumerable<IEmailAttachment> Attachments { get; set; } = new List<IEmailAttachment>();
    }
}
