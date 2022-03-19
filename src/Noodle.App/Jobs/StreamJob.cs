using System.Net.Sockets;

namespace Noodle.App.Jobs;

public abstract class StreamJob : SocketJob
{
    protected override async Task<string> RunAsync(Socket socket, CancellationToken cancellationToken)
    {
        await using var stream = await GetStreamAsync(socket, cancellationToken);

        return await RunAsync(stream, cancellationToken);
    }

    protected abstract Task<string> RunAsync(Stream stream, CancellationToken cancellationToken);

    protected virtual Task<Stream> GetStreamAsync(Socket socket, CancellationToken cancellationToken)
    {
        return Task.FromResult<Stream>(new NetworkStream(socket, false));
    }
}