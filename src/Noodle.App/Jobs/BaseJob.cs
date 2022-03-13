using Noodle.App.Common;

namespace Noodle.App.Jobs;

public abstract class BaseJob : IJob
{
    public IJobOptions Options { get; }
    public IJobStats Stats { get; }

    protected BaseJob(IJobOptions options, IJobStats stats)
    {
        Options = options;
        Stats = stats;
    }

    public virtual async Task RunAsync(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            try
            {
                await RunCoreAsync(cancellationToken);

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

    protected abstract Task RunCoreAsync(CancellationToken cancellationToken);

    public virtual void Dispose()
    {
    }
}