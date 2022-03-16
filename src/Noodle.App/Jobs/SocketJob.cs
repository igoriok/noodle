using System.Net;
using System.Net.Sockets;
using Noodle.App.Common;

namespace Noodle.App.Jobs;

public abstract class SocketJob : IJob
{
    private readonly EndPoint _endpoint;

    public IJobOptions Options { get; }

    protected SocketJob(IJobOptions options)
    {
        Options = options;

        _endpoint = Options.Url.HostNameType == UriHostNameType.Dns
            ? new DnsEndPoint(Options.Url.DnsSafeHost, Options.Url.Port)
            : new IPEndPoint(IPAddress.Parse(Options.Url.Host), Options.Url.Port);
    }

    public async Task<string> RunAsync(CancellationToken cancellationToken)
    {
        string result;

        using var socket = new Socket(SocketType.Stream, ProtocolType.Tcp);

        await socket.ConnectAsync(_endpoint, cancellationToken);

        await using (var stream = await GetStreamAsync(socket, cancellationToken))
        {
            result = await RunAsync(stream, cancellationToken);
        }

        await socket.DisconnectAsync(false, cancellationToken);

        return result;
    }

    protected virtual Task<Stream> GetStreamAsync(Socket socket, CancellationToken cancellationToken)
    {
        return Task.FromResult<Stream>(new NetworkStream(socket, false));
    }

    protected abstract Task<string> RunAsync(Stream stream, CancellationToken cancellationToken);

    public void Dispose()
    {
    }
}