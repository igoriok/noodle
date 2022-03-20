using System.Buffers;
using System.Net.Sockets;
using Noodle.App.Common;
using Noodle.App.Logic;
using Noodle.App.Settings;
using Noodle.App.Stages;

namespace Noodle.App.Jobs;

public class UdpJob : SocketJob, IJob
{
    private readonly UdpSettings _settings;

    protected override string Host => _settings.Host;
    protected override int Port => _settings.Port;
    protected override string[] IpAddresses => _settings.IpAddresses;
    protected override SocketType SocketType => SocketType.Dgram;
    protected override ProtocolType ProtocolType => ProtocolType.Udp;
    protected override int? SendTimeout => _settings.Timeout;
    protected override int? ReceiveTimeout => _settings.Timeout;

    public string Name => "UDP";
    public string Description => $"{Host}:{Port}";

    public IEnumerable<IStage> Pipeline
    {
        get
        {
            if (_settings.Concurrency.HasValue)
                yield return new ParallelStage(_settings.Concurrency.Value);

            yield return new RepeatStage();

            if (_settings.Throttle.HasValue)
                yield return new ThrottleStage(_settings.Throttle.Value);
        }
    }

    public UdpJob(UdpSettings settings)
    {
        _settings = settings;
    }

    protected override async Task<string> RunAsync(Socket socket, CancellationToken cancellationToken)
    {
        var buffer = ArrayPool<byte>.Shared.Rent(1024);

        try
        {
            buffer.Generate();

            var endpoint = await GetEndpointAsync(cancellationToken);

            await socket.SendToAsync(buffer, SocketFlags.None, endpoint, cancellationToken);
        }
        finally
        {
            ArrayPool<byte>.Shared.Return(buffer);
        }

        return "OK";
    }
}