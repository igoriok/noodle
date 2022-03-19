using Noodle.App.Common;
using Noodle.App.Stages;

namespace Noodle.App.Logic;

public class JobRunner
{
    private readonly IJob _job;

    public string Name => _job.Name;
    public string Description => _job.Description;

    public JobStats Stats { get; }

    public JobRunner(IJob job)
    {
        _job = job;

        Stats = new JobStats();
    }

    public Task RunAsync(CancellationToken cancellationToken)
    {
        var stages = _job.Pipeline
            .Concat(new[]
            {
                new StatsStage(_job, Stats)
            })
            .ToArray();

        return Next(stages, 0)(cancellationToken);
    }

    private static Func<CancellationToken, Task> Next(IList<IStage> pipeline, int index)
    {
        return async (cancellationToken) =>
        {
            if (index >= pipeline.Count)
                return;

            var stage = pipeline[index];

            await stage.ExecuteAsync(Next(pipeline, index + 1), cancellationToken);
        };
    }
}