using System;

namespace Madev.Utils.Infrastructure.Services.Mailing
{
    public class Base64EmailAttachment : IEmailAttachment
    {
        public Base64EmailAttachment(string filename, string content, bool isInline = false, string? contentId = null, string? contentType = null)
        {
            FileName = filename;
            Content = content;
            IsInline = isInline;

            if (IsInline && contentId == null)
            {
                throw new ArgumentException("If an attachment is inline, ContentId must be defined");
            }
            ContentId = contentId;

            if(contentType != null)
            {
                ContentType = contentType;
            }
        }

        public string FileName { get; }
        public string Content { get; }
        public string? ContentId { get; set; }
        public bool IsInline { get; set; }
        public string? ContentType { get; set; } = "application/octet-stream";
    }
}