using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.Extensibility;

namespace Madev.Utils.Infrastructure.ApplicationInsights.WorkerService;

internal class RoleNameTelemetryInitializer(string roleName) 
    : ITelemetryInitializer
{
    public void Initialize(ITelemetry telemetry)
    {
        telemetry.Context.Cloud.RoleName = roleName;
    }
}