namespace Noodle.App.Common;

public interface IJob : IDisposable
{
    IJobOptions Options { get; }
    IJobStats Stats { get; }

    Task RunAsync(CancellationToken cancellationToken);
}