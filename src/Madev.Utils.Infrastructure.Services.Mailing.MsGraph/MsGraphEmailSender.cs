using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Graph;
using Microsoft.Graph.Models;
using Microsoft.Graph.Users.Item.SendMail;

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

        public async Task SendHtmlEmailAsync(EmailMessage message, CancellationToken cancellationToken = default)
        {
            await SendEmailAsync(email => email
                .To(message.To)
                .Cc(message.Cc)
                .Bcc(message.Bcc)
                .From(message.From)
                .Subject(message.Subject)
                .Body(message.Body, BodyType.Html)
                .Attachments(message.Attachments), cancellationToken);
        }

        public async Task SendTextEmailAsync(EmailMessage message, CancellationToken cancellationToken = default)
        {
            await SendEmailAsync(email => email
                .To(message.To)
                .Cc(message.Cc)
                .Bcc(message.Bcc)
                .From(message.From)
                .Subject(message.Subject)
                .Body(message.Body, BodyType.Text)
                .Attachments(message.Attachments), cancellationToken);
        }

        private async Task SendEmailAsync(Action<EmailBuilder> builder, CancellationToken cancellationToken)
        {
            var message = new Message();
            builder(new EmailBuilder(message));
            await SendEmailInternalAsync(message, cancellationToken);
        }

        private async Task SendEmailInternalAsync(Message message, CancellationToken cancellationToken)
        {
            var requestBody = new SendMailPostRequestBody
            {
                Message = message,
                SaveToSentItems = true
            };

            await _client.Users[_sender]
                .SendMail
                .PostAsync(requestBody, cancellationToken: cancellationToken);
        }
    }
}