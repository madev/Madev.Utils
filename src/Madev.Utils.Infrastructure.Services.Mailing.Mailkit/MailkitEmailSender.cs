using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using MailKit;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Options;
using MimeKit;

namespace Madev.Utils.Infrastructure.Services.Mailing.Mailkit
{
    public class MailkitEmailSender : IDisposable, IEmailSender
    {
        private readonly MailkitEmailSenderOptions _options;

        private SmtpClient _smtpClient;

        public MailkitEmailSender(IOptions<MailkitEmailSenderOptions> options)
        {
            _options = options.Value;

            if (_options.Sender == null && !_options.Username.Contains("@"))
            {
                throw new FormatException($"Username must be in email address format when Sender is not set.");
            }

            _smtpClient = new SmtpClient();
            if (_options.LocalDomain != null)
            {
                _smtpClient.LocalDomain = _options.LocalDomain;
            }
        }

        public async Task SendHtmlEmailAsync(EmailMessage message, CancellationToken cancellationToken = default)
        {
            var mimeMessage = ConstructHtmlMimeMessage(message);
            await SendMimeMessageAsync(mimeMessage, cancellationToken);
        }

        public async Task SendTextEmailAsync(EmailMessage message, CancellationToken cancellationToken = default)
        {
            var mimeMessage = ConstructTextMimeMessage(message);
            await SendMimeMessageAsync(mimeMessage, cancellationToken);
        }

        private MimeMessage ConstructHtmlMimeMessage(EmailMessage message)
        {
            var mimeMessage = new MimeMessage();
            mimeMessage = ConstructMessageHeaders(mimeMessage, message);
            mimeMessage = ConstructMessageBody(mimeMessage, message.Body, null, message.Attachments);
            return mimeMessage;
        }

        private MimeMessage ConstructTextMimeMessage(EmailMessage message)
        {
            var mimeMessage = new MimeMessage();
            mimeMessage = ConstructMessageHeaders(mimeMessage, message);
            mimeMessage = ConstructMessageBody(mimeMessage, null, message.Body, message.Attachments);
            return mimeMessage;
        }

        private MimeMessage ConstructMessageHeaders(MimeMessage mimeMessage, EmailMessage message)
        {
            mimeMessage.From.Add(MailboxAddress.Parse(message.From ?? _options.Sender ?? _options.Username));
            foreach (var address in message.To)
            {
                mimeMessage.To.Add(MailboxAddress.Parse(address));
            }
            foreach (var address in message.Cc)
            {
                mimeMessage.Cc.Add(MailboxAddress.Parse(address));
            }
            foreach (var address in message.Bcc)
            {
                mimeMessage.Bcc.Add(MailboxAddress.Parse(address));
            }
            mimeMessage.Subject = message.Subject;
            return mimeMessage;
        }

        private MimeMessage ConstructMessageBody(MimeMessage message, string? htmlBody, string? textBody, IEnumerable<IEmailAttachment> attachments)
        {
            var builder = new BodyBuilder();
            builder.HtmlBody = htmlBody;
            builder.TextBody = textBody;
            foreach (var attachment in attachments)
            {
                var convertedAttachment = attachment switch
                {
                    FilepathEmailAttachment att => builder.Attachments.Add(
                        Path.GetFileName(att.Path),
                        File.ReadAllBytes(att.Path),
                        ContentType.Parse(att.ContentType)
                    ),
                    ByteEmailAttachment att => builder.Attachments.Add(
                        att.Filename,
                        att.Content,
                        ContentType.Parse(att.ContentType)
                    ),
                    Base64EmailAttachment att => builder.Attachments.Add(
                        att.FileName,
                        Convert.FromBase64String(att.Content),
                        ContentType.Parse(att.ContentType)
                    ),
                    _ => throw new InvalidOperationException("Unknown attachment type.")
                };

                if (attachment.IsInline)
                {
                    convertedAttachment.ContentId = attachment.ContentId;
                    convertedAttachment.ContentDisposition = new ContentDisposition(ContentDisposition.Inline);
                }
            }
            message.Body = builder.ToMessageBody();
            return message;
        }

        private async Task SendMimeMessageAsync(MimeMessage message, CancellationToken cancellationToken)
        {
            try
            {
                await _smtpClient.SendAsync(message, cancellationToken);
                return;
            }
            catch (ServiceNotConnectedException)
            {
                Connect();
            }
            catch (SmtpProtocolException)
            {
                if (_smtpClient.IsConnected == false)
                {
                    _smtpClient.Dispose();
                    Connect();
                }
            }
            _smtpClient.Send(message, cancellationToken);
        }

        private void Connect()
        {
            _smtpClient = new SmtpClient();
            _smtpClient.Connect(_options.Server,_options.Port);
            _smtpClient.Authenticate(_options.Username, _options.Password);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_smtpClient != null)
                {
                    _smtpClient.Disconnect(true);
                    _smtpClient?.Dispose();
                }
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}