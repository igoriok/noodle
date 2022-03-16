using Noodle.App.Common;

namespace Noodle.App.Logic;

public class JobRunner : IDisposable
{
    public IJobOptions Options { get; }
    public IJobStats Stats { get; }

    public IReadOnlyList<IJob> Jobs { get; }

    public JobRunner(IJobFactory factory, IJobOptions options)
    {
        Options = options;
        Stats = new JobStats();

        Jobs = Enumerable.Repeat(Options, Options.Concurrency).Select(factory.CreateJob).ToArray();
    }

    public async Task RunAsync(CancellationToken cancellationToken)
    {
        var tasks = new List<Task>(Jobs.Count);

        foreach (var job in Jobs)
        {
            tasks.Add(RunAsync(job, cancellationToken));
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

    private async Task RunAsync(IJob job, CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            try
            {
                // TODO: Add metrics
                var status = await job.RunAsync(cancellationToken);

                lock (Stats)
                {
                    Stats.Status = status;
                    Stats.Successful++;
                }
            }
            catch (OperationCanceledException) { }
            catch (Exception exception)
            {
                lock (Stats)
                {
                    Stats.Failed++;
                    Stats.Status = exception.Message;
                }
            }
        }
    }

    private class JobStats : IJobStats
    {
        public string Status { get; set; }

        public int Successful { get; set; }

        public int Failed { get; set; }
    }
}