using System;

namespace Madev.Utils.Infrastructure.Services.Mailing
{
    public class ByteEmailAttachment : IEmailAttachment
    {
        public ByteEmailAttachment(string filename, byte[] content)
        {
            Filename = filename ?? throw new ArgumentException($"{nameof(filename)} cannot be null.");
            Content = content ?? throw new ArgumentException($"{nameof(content)} cannot be empty.");
        }

        public string Filename { get; }
        public byte[] Content { get; }
    }
}