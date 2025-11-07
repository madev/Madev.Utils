using Microsoft.ApplicationInsights.AspNetCore;
using Microsoft.ApplicationInsights.Extensibility;

namespace Madev.Utils.Infrastructure.ApplicationInsights.AspNetCore;

internal class TelemetryProcessorFactory(string[] excludedOperations) : ITelemetryProcessorFactory
{
    public ITelemetryProcessor Create(ITelemetryProcessor nextProcessor)
    {
        return new ExcludeOperationsTelemetryProcessor(nextProcessor, excludedOperations);
    }
}