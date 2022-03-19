using Noodle.App.Common;

namespace Noodle.App.Stages;

public class StatsStage : IStage
{
    private readonly IJob _job;
    private readonly JobStats _stats;

    public StatsStage(IJob job, JobStats stats)
    {
        _job = job;
        _stats = stats;
    }

    public async Task ExecuteAsync(Func<CancellationToken, Task> next, CancellationToken cancellationToken)
    {
        try
        {
            // TODO: Add metrics
            var status = await _job.RunAsync(cancellationToken);

            lock (_stats)
            {
                _stats.Status = status;
                _stats.Successful++;
            }
        }
        catch (Exception exception)
        {
            lock (_stats)
            {
                _stats.Status = exception.Message;
                _stats.Failed++;
            }
        }
    }
}