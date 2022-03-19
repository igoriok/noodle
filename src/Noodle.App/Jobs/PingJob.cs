using System.Net.NetworkInformation;
using Noodle.App.Common;
using Noodle.App.Settings;
using Noodle.App.Stages;

namespace Noodle.App.Jobs;

public class PingJob : HostJob, IJob
{
    private readonly PingSettings _settings;

    public string Name => "PING";
    public string Description => _settings.Host;

    public IEnumerable<IStage> Pipeline
    {
        get
        {
            yield return new RepeatStage();

            if (_settings.Throttle.HasValue)
                yield return new ThrottleStage(_settings.Throttle.Value);
        }
    }

    protected override string Host => _settings.Host;

    protected override string[] IpAddresses => _settings.IpAddresses;

    public PingJob(PingSettings settings)
    {
        _settings = settings;
    }

    public async Task<string> RunAsync(CancellationToken cancellationToken)
    {
        var address = await GetAddressAsync(cancellationToken);

        using var ping = new Ping();

        var reply = await ping.SendPingAsync(address);

        return $"{reply.Status}";
    }
}