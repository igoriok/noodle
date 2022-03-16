namespace Noodle.App.Common;

public interface IJob : IDisposable
{
    Task<string> RunAsync(CancellationToken cancellationToken);
}