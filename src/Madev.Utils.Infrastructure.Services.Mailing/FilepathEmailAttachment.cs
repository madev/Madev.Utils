using System;

namespace Madev.Utils.Infrastructure.Services.Mailing
{
    public class FilepathEmailAttachment : IEmailAttachment
    {
        public FilepathEmailAttachment(string path)
        {
            Path = path ?? throw new ArgumentException($"{nameof(path)} cannot be empty.");
        }

        public string Path { get; }
    }
}