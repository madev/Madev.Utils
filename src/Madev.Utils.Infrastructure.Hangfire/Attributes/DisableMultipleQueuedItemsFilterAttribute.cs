using Hangfire.Common;
using Hangfire.States;
using Hangfire.Storage;

namespace Madev.Utils.Infrastructure.Hangfire.Attributes;
public sealed class DisableMultipleQueuedItemsFilterAttribute : JobFilterAttribute, IElectStateFilter
{
    private readonly string _jobName;

    public DisableMultipleQueuedItemsFilterAttribute(string jobName)
    {
        _jobName = jobName;
    }

    public void OnStateElection(ElectStateContext context)
    {
        var monitoring = context.Storage.GetMonitoringApi();

        if (ActiveProcess(monitoring) && !context.CandidateState.IsFinal && context.CandidateState.Name != FailedState.StateName && context.CurrentState != ScheduledState.StateName)
        {
            context.CandidateState = new DeletedState
            {
                Reason = $"It is not allowed to perform multiple same tasks: {_jobName}"
            };
        }
    }

    private bool ActiveProcess(IMonitoringApi monitoring)
    {
        var processingJobs = monitoring.ProcessingJobs(0, 2000);
        var scheduledJobs = monitoring.ScheduledJobs(0, 2000);

        var hasProcessingJob = processingJobs.Where(x =>
            x.Value.Job.Type.Name.Equals(_jobName, StringComparison.InvariantCultureIgnoreCase));
        var hasScheduledJob = scheduledJobs.Where(x =>
            x.Value.Job.Type.Name.Equals(_jobName, StringComparison.InvariantCultureIgnoreCase));

        return hasProcessingJob.Count() + hasScheduledJob.Count() > 0;
    }
}