using Noodle.App.Common;

namespace Noodle.App.Stages;

public class JobStage : IStage
{
    private readonly IJob _job;

    public JobStage(IJob job)
    {
        _job = job;
    }

    public Task ExecuteAsync(Func<CancellationToken, Task> next, CancellationToken cancellationToken)
    {
        return _job.RunAsync(cancellationToken);
    }
}