using System.Threading;
using System.Threading.Tasks;

namespace Madev.Utils.Infrastructure.Services.Mailing
{
    public interface IEmailSender
    {
        Task SendHtmlEmailAsync(EmailMessage message, CancellationToken cancellationToken = default);
        Task SendTextEmailAsync(EmailMessage message, CancellationToken cancellationToken = default);
    }
}
