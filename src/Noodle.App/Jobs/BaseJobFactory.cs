using Noodle.App.Common;

namespace Noodle.App.Jobs;

public abstract class BaseJobFactory<TOptions> : IJobFactory<TOptions>
    where TOptions : BaseJobOptions
{
    public IJob CreateJob(TOptions options)
    {
        var stats = new JobStats();
        var jobs = Enumerable.Repeat(options, options.Concurrency).Select(o => CreateJobCore(o, stats));

        return new ConcurrentJob(jobs, options, stats);
    }

    protected abstract IJob CreateJobCore(TOptions options, IJobStats stats);

    private class JobStats : IJobStats
    {
        public string Status { get; set; }

        public int Successful { get; set; }

        public int Failed { get; set; }
    }

    private class ConcurrentJob : IJob
    {
        public IList<IJob> Jobs { get; }

        public IJobOptions Options { get; }
        public IJobStats Stats { get; }

        public ConcurrentJob(IEnumerable<IJob> jobs, IJobOptions options, IJobStats stats)
        {
            Jobs = jobs.ToArray();
            Options = options;
            Stats = stats;
        }

        public async Task RunAsync(CancellationToken cancellationToken)
        {
            var tasks = new List<Task>(Jobs.Count);

            foreach (var job in Jobs)
            {
                tasks.Add(job.RunAsync(cancellationToken));
            }

            await Task.WhenAll(tasks);
        }

        public void Dispose()
        {
            foreach (var job in Jobs)
            {
                job.Dispose();
            }
        }
    }
}