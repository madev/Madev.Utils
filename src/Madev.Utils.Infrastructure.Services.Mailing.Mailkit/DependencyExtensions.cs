using System;
using Microsoft.Extensions.DependencyInjection;

namespace Madev.Utils.Infrastructure.Services.Mailing.Mailkit
{
    public static class DependencyExtensions
    {
        public static IServiceCollection AddMailkitEmailSender(this IServiceCollection services, Action<MailkitEmailSenderOptions> options)
        {
            services.AddScoped<IEmailSender, MailkitEmailSender>()
                .Configure(options);
            return services;
        }
    }
}