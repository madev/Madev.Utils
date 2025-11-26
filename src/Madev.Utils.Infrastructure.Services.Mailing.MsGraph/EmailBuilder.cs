using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Graph.Models;

namespace Madev.Utils.Infrastructure.Services.Mailing.MsGraph
{
    internal class EmailBuilder
    {
        private readonly Message _email;

        public EmailBuilder(Message email)
        {
            _email = email;
        }

        public EmailBuilder To(IEnumerable<string> recipients)
        {
            _email.ToRecipients = recipients.Select(x => new Recipient
            {
                EmailAddress = new EmailAddress
                {
                    Address = x
                }
            }).ToList();
            return this;
        }

        public EmailBuilder Subject(string subject)
        {
            _email.Subject = subject;
            return this;
        }

        public EmailBuilder Body(string body, BodyType bodyType)
        {
            _email.Body = new ItemBody
            {
                ContentType = bodyType,
                Content = body
            };
            return this;
        }

        public EmailBuilder Attachments(IEnumerable<IEmailAttachment> attachments)
        {
            var fileAttachments = new List<Attachment>();
            foreach (var attachment in attachments)
            {
                fileAttachments.Add(ConvertToFileAttachment(attachment));
            }

            _email.Attachments = fileAttachments;
            return this;
        }

        private FileAttachment ConvertToFileAttachment(IEmailAttachment attachment)
        {
            var convertedAttachment = attachment switch
            {
                ByteEmailAttachment att => new FileAttachment
                {
                    Name = att.Filename,
                    ContentBytes = att.Content,
                    ContentType = attachment.ContentType
                },
                FilepathEmailAttachment att => new FileAttachment
                {
                    Name = Path.GetFileName(att.Path),
                    ContentBytes = File.ReadAllBytes(att.Path),
                    ContentType = attachment.ContentType
                },
                Base64EmailAttachment att => new FileAttachment
                {
                    Name = att.FileName,
                    ContentBytes = Convert.FromBase64String(att.Content),
                    ContentType = attachment.ContentType
                },
                _ => throw new InvalidOperationException("Unknown attachment type.")
            };

            if (attachment.IsInline)
            {
                convertedAttachment.IsInline = true;
                convertedAttachment.ContentId = attachment.ContentId;
            }

            return convertedAttachment;
        }
    }
}