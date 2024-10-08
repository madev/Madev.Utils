using Azure.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Graph;

namespace Madev.Utils.Infrastructure.Services.Mailing.MsGraph
{
    public static class DependencyExtensions
    {
        public static void AddMsGraphEmailSender(this IServiceCollection services, string sender, TokenCredential credential)
        {
            services.AddSingleton<IEmailSender, MsGraphEmailSender>(provider =>
            {
                var graphClient = new GraphServiceClient(credential);
                return new MsGraphEmailSender(sender, graphClient);
            });
        }
    }
}