using System.Net;
using System.Net.Sockets;

namespace Noodle.App.Jobs;

public abstract class SocketJob : HostJob
{
    protected abstract int Port { get; }
    protected virtual SocketType SocketType => SocketType.Stream;
    protected virtual ProtocolType ProtocolType => ProtocolType.Tcp;
    protected virtual int? SendTimeout { get; }
    protected virtual int? ReceiveTimeout { get; }

    public virtual async Task<string> RunAsync(CancellationToken cancellationToken)
    {
        var endPoint = await GetEndpointAsync(cancellationToken);

        string result;

        using var socket = CreateSocket();

        try
        {
            await socket.ConnectAsync(endPoint, cancellationToken);

            result = await RunAsync(socket, cancellationToken);
        }
        finally
        {
            await socket.DisconnectAsync(false, cancellationToken);
        }

        return result;
    }

    protected virtual Socket CreateSocket()
    {
        var socket = new Socket(SocketType, ProtocolType);

        if (SendTimeout.HasValue)
            socket.SendTimeout = SendTimeout.Value;

        if (ReceiveTimeout.HasValue)
            socket.ReceiveTimeout = ReceiveTimeout.Value;

        return socket;
    }

    protected virtual async Task<EndPoint> GetEndpointAsync(CancellationToken cancellationToken)
    {
        return new IPEndPoint(await GetAddressAsync(cancellationToken), Port);
    }

    protected abstract Task<string> RunAsync(Socket socket, CancellationToken cancellationToken);
}