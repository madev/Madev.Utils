using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.ApplicationInsights.WorkerService;

namespace Madev.Utils.Infrastructure.ApplicationInsights.WorkerService;

internal class TelemetryProcessorFactory(string[] excludedOperations) : ITelemetryProcessorFactory
{
    public ITelemetryProcessor Create(ITelemetryProcessor nextProcessor)
    {
        return new ExcludeOperationsTelemetryProcessor(nextProcessor, excludedOperations);
    }
}