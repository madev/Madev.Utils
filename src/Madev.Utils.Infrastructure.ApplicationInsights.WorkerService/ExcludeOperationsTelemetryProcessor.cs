using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.Extensibility;

namespace Madev.Utils.Infrastructure.ApplicationInsights.WorkerService;

internal class ExcludeOperationsTelemetryProcessor(ITelemetryProcessor nextProcessor, string[] excludedOperations)
    : ITelemetryProcessor
{
    public void Process(ITelemetry item)
    {
        if (excludedOperations.Contains(item.Context.Operation.Name))
        {
            return;
        }

        nextProcessor.Process(item);
    }
}