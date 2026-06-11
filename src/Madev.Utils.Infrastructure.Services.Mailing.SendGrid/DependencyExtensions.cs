using System;
using Microsoft.Extensions.DependencyInjection;

namespace Madev.Utils.Infrastructure.Services.Mailing.SendGrid
{
    public static class DependencyExtensions
    {
        public static IServiceCollection AddSendGridEmailSender(this IServiceCollection services, Action<SendGridEmailSenderOptions> options)
        {
            services.AddScoped<IEmailSender, SendGridEmailSender>()
                .Configure(options);
            return services;
        }
    }
}
