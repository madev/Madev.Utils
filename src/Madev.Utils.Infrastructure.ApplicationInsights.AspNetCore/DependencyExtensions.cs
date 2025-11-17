using Microsoft.ApplicationInsights.AspNetCore;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.Extensions.DependencyInjection;

namespace Madev.Utils.Infrastructure.ApplicationInsights.AspNetCore;

public static class DependencyExtensions
{
    public static IServiceCollection AddMadevApplicationInsightsTelemetry(this IServiceCollection services,
        string connectionString,
        string roleName,
        string[]? excludedOperations = null)
    {
        services.AddApplicationInsightsTelemetry(conf => conf.ConnectionString = connectionString);
        RegisterRest(services, roleName, excludedOperations);
        return services;
    }

    public static IServiceCollection AddMadevApplicationInsightsTelemetry(this IServiceCollection services,
        string roleName,
        string[]? excludedOperations = null)
    {
        services.AddApplicationInsightsTelemetry();
        RegisterRest(services, roleName, excludedOperations);
        return services;
    }

    private static void RegisterRest(IServiceCollection services, string roleName, string[]? excludedOperations)
    {
        services.AddSingleton<ITelemetryInitializer>(new RoleNameTelemetryInitializer(roleName));

        excludedOperations ??= [];
        if (excludedOperations.Any())
        {
            services.AddSingleton<ITelemetryProcessorFactory>(new TelemetryProcessorFactory(excludedOperations));
        }
    }
}