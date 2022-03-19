using System.Net;
using System.Net.Sockets;

namespace Noodle.App.Jobs;

public abstract class SocketJob : EndpointJob
{
    protected abstract int Port { get; }

    public virtual async Task<string> RunAsync(CancellationToken cancellationToken)
    {
        var endPoint = await GetEndpointAsync(cancellationToken);

        string result;

        using var socket = new Socket(SocketType.Stream, ProtocolType.Tcp);

        await socket.ConnectAsync(endPoint, cancellationToken);

        await using (var stream = await GetStreamAsync(socket, cancellationToken))
        {
            result = await RunAsync(stream, cancellationToken);
        }

        await socket.DisconnectAsync(false, cancellationToken);

        return result;
    }

    protected virtual async Task<EndPoint> GetEndpointAsync(CancellationToken cancellationToken)
    {
        return new IPEndPoint(await GetAddressAsync(cancellationToken), Port);
    }

    protected virtual Task<Stream> GetStreamAsync(Socket socket, CancellationToken cancellationToken)
    {
        return Task.FromResult<Stream>(new NetworkStream(socket, false));
    }

    protected abstract Task<string> RunAsync(Stream stream, CancellationToken cancellationToken);
}