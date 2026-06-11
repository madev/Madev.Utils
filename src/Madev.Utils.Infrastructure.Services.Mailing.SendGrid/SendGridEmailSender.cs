using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using global::SendGrid;
using global::SendGrid.Helpers.Mail;

namespace Madev.Utils.Infrastructure.Services.Mailing.SendGrid
{
    public class SendGridEmailSender : IEmailSender
    {
        private readonly SendGridClient _client;
        private readonly SendGridEmailSenderOptions _options;

        public SendGridEmailSender(IOptions<SendGridEmailSenderOptions> options)
        {
            _options = options.Value;
            _client = new SendGridClient(_options.ApiKey);
        }

        public Task SendHtmlEmailAsync(EmailMessage message, CancellationToken cancellationToken = default)
            => SendAsync(ConstructHtmlMessage(message), cancellationToken);

        public Task SendTextEmailAsync(EmailMessage message, CancellationToken cancellationToken = default)
            => SendAsync(ConstructTextMessage(message), cancellationToken);

        private async Task SendAsync(SendGridMessage sgMessage, CancellationToken cancellationToken)
        {
            var response = await _client.SendEmailAsync(sgMessage, cancellationToken);
            if (!response.IsSuccessStatusCode)
            {
                var responseBody = await response.Body.ReadAsStringAsync();
                throw new InvalidOperationException($"SendGrid returned {(int)response.StatusCode}: {responseBody}");
            }
        }

        private SendGridMessage ConstructHtmlMessage(EmailMessage message)
        {
            var sgMessage = ConstructBaseMessage(message);
            sgMessage.HtmlContent = message.Body;
            return sgMessage;
        }

        private SendGridMessage ConstructTextMessage(EmailMessage message)
        {
            var sgMessage = ConstructBaseMessage(message);
            sgMessage.PlainTextContent = message.Body;
            return sgMessage;
        }

        private SendGridMessage ConstructBaseMessage(EmailMessage message)
        {
            var from = message.From ?? _options.Sender
                ?? throw new InvalidOperationException("No sender specified: set EmailMessage.From or SendGridEmailSenderOptions.Sender.");

            var sgMessage = new SendGridMessage
            {
                From = new EmailAddress(from),
                Subject = message.Subject
            };

            foreach (var to in message.To)
            {
                sgMessage.AddTo(new EmailAddress(to));
            }
            foreach (var cc in message.Cc)
            {
                sgMessage.AddCc(new EmailAddress(cc));
            }
            foreach (var bcc in message.Bcc)
            {
                sgMessage.AddBcc(new EmailAddress(bcc));
            }

            foreach (var attachment in message.Attachments)
            {
                AddAttachment(sgMessage, attachment);
            }

            return sgMessage;
        }

        private static void AddAttachment(SendGridMessage sgMessage, IEmailAttachment attachment)
        {
            var (name, base64Content) = attachment switch
            {
                ByteEmailAttachment a => (a.Filename, Convert.ToBase64String(a.Content)),
                FilepathEmailAttachment a => (Path.GetFileName(a.Path), Convert.ToBase64String(File.ReadAllBytes(a.Path))),
                Base64EmailAttachment a => (a.FileName, a.Content),
                _ => throw new InvalidOperationException("Unknown attachment type.")
            };

            // For inline attachments the content_id must match the cid: reference in the HTML body.
            sgMessage.AddAttachment(
                filename: name,
                base64Content: base64Content,
                type: attachment.ContentType,
                disposition: attachment.IsInline ? "inline" : "attachment",
                content_id: attachment.IsInline ? attachment.ContentId : null);
        }
    }
}
