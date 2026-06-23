using Azure.Core;
using Microsoft.Extensions.DependencyInjection;

namespace Madev.Utils.Infrastructure.Services.OpenAi;

public static class DependencyExtensions
{
    public static IServiceCollection AddOpenAiService(
        this IServiceCollection services,
        OpenAiServiceOptions options,
        TokenCredential credential)
    {
        services.AddSingleton<IOpenAiService>(_ => new OpenAiService(options, credential));
        return services;
    }

    public static IServiceCollection AddOpenAiService(
        this IServiceCollection services,
        string serviceKey,
        OpenAiServiceOptions options,
        TokenCredential credential)
    {
        services.AddKeyedSingleton<IOpenAiService>(serviceKey, (_, _) => new OpenAiService(options, credential));
        return services;
    }
}
