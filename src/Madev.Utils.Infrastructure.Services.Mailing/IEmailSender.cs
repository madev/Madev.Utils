using System.Collections.Generic;
using System.Threading.Tasks;

namespace Madev.Utils.Infrastructure.Services.Mailing
{
    public interface IEmailSender
    {
        Task SendTextEmailAsync(string toAddress, string subject, string body, IEnumerable<IEmailAttachment>? attachments = null);
        Task SendTextEmailAsync(IEnumerable<string> toAddress, string subject, string body, IEnumerable<IEmailAttachment>? attachments = null);
        Task SendHtmlEmailAsync(string toAddress, string subject, string body, IEnumerable<IEmailAttachment>? attachments = null);
        Task SendHtmlEmailAsync(IEnumerable<string> toAddress, string subject, string body, IEnumerable<IEmailAttachment>? attachments = null);
    }
}