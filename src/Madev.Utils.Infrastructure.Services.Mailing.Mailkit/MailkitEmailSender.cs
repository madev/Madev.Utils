using System;
using System.Collections.Generic;
using System.IO;
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

            var isAddressFormat = _options.Username.Contains("@");
            if (isAddressFormat == false)
            {
                throw new FormatException($"Username must be in email address format.");
            }

            _smtpClient = new SmtpClient();
            if (_options.LocalDomain != null)
            {
                _smtpClient.LocalDomain = _options.LocalDomain;
            }
        }

        public async Task SendTextEmailAsync(string toAddress, string subject, string body, IEnumerable<IEmailAttachment> attachments = null)
        {
            attachments ??= new List<IEmailAttachment>();
            await SendTextEmailAsync(new List<string>() { toAddress }, subject, body, attachments);
        }

        public async Task SendTextEmailAsync(IEnumerable<string> toAddress, string subject, string body, IEnumerable<IEmailAttachment> attachments = null)
        {
            attachments ??= new List<IEmailAttachment>();
            var message = ConstructTextMimeMessage(toAddress, subject, body, attachments);
            await SendMimeMessageAsync(message);
        }

        public async Task SendHtmlEmailAsync(string toAddress, string subject, string body, IEnumerable<IEmailAttachment> attachments = null)
        {
            attachments ??= new List<IEmailAttachment>();
            await SendHtmlEmailAsync(new List<string>() { toAddress }, subject, body, attachments);
        }

        public async Task SendHtmlEmailAsync(IEnumerable<string> toAddress, string subject, string body, IEnumerable<IEmailAttachment> attachments = null)
        {
            attachments ??= new List<IEmailAttachment>();
            var message = ConstructHtmlMimeMessage(toAddress, subject, body, attachments);
            await SendMimeMessageAsync(message);
        }

        private MimeMessage ConstructTextMimeMessage(IEnumerable<string> toAddress,
            string subject, string body, IEnumerable<IEmailAttachment> attachments)
        {
            var message = new MimeMessage();
            message = ConstructMessageHeaders(message, toAddress, subject);
            message = ConstructMessageBody(message, null, body, attachments);
            return message;
        }

        private MimeMessage ConstructHtmlMimeMessage(IEnumerable<string> toAddress,
            string subject, string body, IEnumerable<IEmailAttachment> attachments)
        {
            var message = new MimeMessage();
            message = ConstructMessageHeaders(message, toAddress, subject);
            message = ConstructMessageBody(message, body, null, attachments);
            return message;
        }

        private MimeMessage ConstructMessageHeaders(MimeMessage message, IEnumerable<string> toAddress, string subject)
        {
            message.From.Add(MailboxAddress.Parse(_options.Username));
            foreach (var address in toAddress)
            {
                message.To.Add(MailboxAddress.Parse(address));
            }
            message.Subject = subject;
            return message;
        }

        private MimeMessage ConstructMessageBody(MimeMessage message, string htmlBody, string textBody, IEnumerable<IEmailAttachment> attachments)
        {
            var builder = new BodyBuilder();
            builder.HtmlBody = htmlBody;
            builder.TextBody = textBody;
            if (attachments != null)
            {
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
            }
            message.Body = builder.ToMessageBody();
            return message;
        }

        private async Task SendMimeMessageAsync(MimeMessage message)
        {
            try
            {
                await _smtpClient.SendAsync(message);
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
            _smtpClient.Send(message);
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