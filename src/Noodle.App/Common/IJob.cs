namespace Noodle.App.Common;

public interface IJob : IDisposable
{
    Task RunAsync(CancellationToken cancellationToken);
}