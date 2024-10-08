using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Graph;

namespace Madev.Utils.Infrastructure.Services.Mailing.MsGraph
{
    public class MsGraphEmailSender : IEmailSender
    {
        private readonly GraphServiceClient _client;
        private readonly string _sender;

        public MsGraphEmailSender(string sender, GraphServiceClient client)
        {
            _client = client;
            _sender = sender;
        }

        public async Task SendTextEmailAsync(string toAddress, string subject, string body, IEnumerable<IEmailAttachment>? attachments = null)
        {
            attachments ??= new List<IEmailAttachment>();
            await SendTextEmailAsync(new List<string>() { toAddress }, subject, body, attachments);
        }

        public async Task SendTextEmailAsync(IEnumerable<string> toAddress, string subject, string body, IEnumerable<IEmailAttachment>? attachments = null)
        {
            attachments ??= new List<IEmailAttachment>();
            await SendEmailAsync(email => email.To(toAddress)
                .Subject(subject)
                .Body(body, BodyType.Text)
                .Attachments(attachments));
        }

        public async Task SendHtmlEmailAsync(string toAddress, string subject, string body, IEnumerable<IEmailAttachment>? attachments = null)
        {
            attachments ??= new List<IEmailAttachment>();
            await SendHtmlEmailAsync(new List<string>() { toAddress }, subject, body, attachments);
        }

        public async Task SendHtmlEmailAsync(IEnumerable<string> toAddress, string subject, string body, IEnumerable<IEmailAttachment>? attachments = null)
        {
            attachments ??= new List<IEmailAttachment>();
            await SendEmailAsync(email => email.To(toAddress)
                .Subject(subject)
                .Body(body, BodyType.Html)
                .Attachments(attachments));
        }

        private async Task SendEmailAsync(Action<EmailBuilder> builder)
        {
            var message = new Message();
            builder(new EmailBuilder(message));
            await SendEmailInternalAsync(message);
        }

        private async Task SendEmailInternalAsync(Message message)
        {
            await _client.Users[_sender]
                .SendMail(message)
                .Request()
                .PostAsync();
        }
    }
}