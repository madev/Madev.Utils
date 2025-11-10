using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.ApplicationInsights.WorkerService;
using Microsoft.Extensions.DependencyInjection;

namespace Madev.Utils.Infrastructure.ApplicationInsights.WorkerService;

public static class DependencyExtensions
{
    public static IServiceCollection AddMadevApplicationInsightsWorkerServiceTelemetry(this IServiceCollection services,
        string connectionString,
        string roleName,
        string[] excludedOperations = null)
    {
        services.AddApplicationInsightsTelemetryWorkerService(config => config.ConnectionString = connectionString);
        services.AddSingleton<ITelemetryInitializer>(new RoleNameTelemetryInitializer(roleName));

        excludedOperations ??= [];
        if (excludedOperations.Any())
        {
            services.AddSingleton<ITelemetryProcessorFactory>(new TelemetryProcessorFactory(excludedOperations));
        }

        return services;
    }
}