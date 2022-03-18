using System.Net;
using System.Net.Sockets;
using Noodle.App.Common;
using Noodle.App.Logic;

namespace Noodle.App.Jobs;

public abstract class SocketJob : IJob
{
    private EndPoint[] _endpoints;

    public IJobOptions Options { get; }

    protected SocketJob(IJobOptions options)
    {
        Options = options;
    }

    public async Task<string> RunAsync(CancellationToken cancellationToken)
    {
        var endpoint = await GetEndpointAsync(cancellationToken);

        string result;

        using var socket = new Socket(SocketType.Stream, ProtocolType.Tcp);

        await socket.ConnectAsync(endpoint, cancellationToken);

        await using (var stream = await GetStreamAsync(socket, cancellationToken))
        {
            result = await RunAsync(stream, cancellationToken);
        }

        await socket.DisconnectAsync(false, cancellationToken);

        return result;
    }

    protected virtual async Task<EndPoint> GetEndpointAsync(CancellationToken cancellationToken)
    {
        if (_endpoints == null)
        {
            if (Options.IpAddresses?.Length > 0)
            {
                _endpoints = Options.IpAddresses
                    .Select(a => new IPEndPoint(IPAddress.Parse(a), Options.Url.Port))
                    .ToArray<EndPoint>();
            }
            else if (Options.Url.HostNameType == UriHostNameType.Dns)
            {
                var addresses = await Dns.GetHostAddressesAsync(Options.Url.IdnHost, cancellationToken);

                _endpoints = addresses
                    .Select(a => new IPEndPoint(a, Options.Url.Port))
                    .ToArray<EndPoint>();
            }
            else
            {
                var address = IPAddress.Parse(Options.Url.Host);

                _endpoints = new EndPoint[]
                {
                    new IPEndPoint(address, Options.Url.Port),
                };
            }
        }

        return _endpoints.Random();
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