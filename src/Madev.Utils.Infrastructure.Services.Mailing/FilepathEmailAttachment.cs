using System;

namespace Madev.Utils.Infrastructure.Services.Mailing
{
    public class FilepathEmailAttachment : IEmailAttachment
    {
        public FilepathEmailAttachment(string path, bool isInline = false, string? contentId = null, string? contentType = null)
        {
            Path = path ?? throw new ArgumentException($"{nameof(path)} cannot be empty.");
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

        public string Path { get; }
        public string? ContentId { get; set; }
        public bool IsInline { get ; set ; }
        public string? ContentType { get ; set ; } = "application/octet-stream";
    }
}