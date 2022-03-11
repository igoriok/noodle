using Noodle.App.Common;

namespace Noodle.App.Logic;

public class JobWorker : IDisposable
{
    public string Id { get; }

    public IJob Job { get; }

    public BaseJobOptions Options { get; }

    public JobStats Stats { get; }

    public JobWorker(IJob job, BaseJobOptions options)
    {
        Job = job;
        Options = options;

        Id = Guid.NewGuid().ToString("N");
        Stats = new JobStats();
    }

    public async Task RunAsync(CancellationToken cancellationToken)
    {
        var concurrency = Options.Concurrency;
        var tasks = new List<Task>();

        for (var i = 0; i < concurrency; i++)
        {
            tasks.Add(RepeatAsync(cancellationToken));
        }

        await Task.WhenAll(tasks);
    }

    public void Dispose()
    {
        Job.Dispose();
    }

    private async Task RepeatAsync(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            try
            {
                await Job.RunAsync(cancellationToken);

                lock (Stats)
                {
                    Stats.Successful++;
                    Stats.Status = "OK";
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
}