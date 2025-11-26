using System;

namespace Madev.Utils.Infrastructure.Services.Mailing
{
    public class ByteEmailAttachment : IEmailAttachment
    {
        public ByteEmailAttachment(string filename, byte[] content, bool isInline = false, string? contentId = null, string? contentType = null)
        {
            Filename = filename ?? throw new ArgumentException($"{nameof(filename)} cannot be null.");
            Content = content ?? throw new ArgumentException($"{nameof(content)} cannot be empty.");
            IsInline = isInline;
            
            if(IsInline && contentId == null)
            {
                throw new ArgumentException("If an attachment is inline, ContentId must be defined");
            }
            ContentId = contentId;

            if(contentType != null)
            {
                ContentType = contentType;
            }
        }

        public string Filename { get; }
        public byte[] Content { get; }
        public string? ContentId { get; set; }
        public bool IsInline { get; set;}
        public string? ContentType { get; set; } = "application/octet-stream";
    }
}