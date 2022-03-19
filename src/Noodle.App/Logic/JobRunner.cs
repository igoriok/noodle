using Noodle.App.Common;
using Noodle.App.Stages;

namespace Noodle.App.Logic;

public class JobRunner
{
    private readonly IJobFactory _factory;
    public object Options { get; }
    public IJobStats Stats { get; }

    public JobRunner(IJobFactory factory, object options)
    {
        _factory = factory;
        Options = options;
        Stats = new JobStats();
    }

    public Task RunAsync(CancellationToken cancellationToken)
    {
        var job = _factory.CreateJob(Options);
        var stages = job.Pipeline
            .Concat(new[]
            {
                new StatsStage(job, Stats)
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

    private class JobStats : IJobStats
    {
        public string Status { get; set; }

        public int Successful { get; set; }

        public int Failed { get; set; }
    }
}