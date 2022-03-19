using Noodle.App.Common;
using Noodle.App.Jobs;
using Noodle.App.Settings;

namespace Noodle.App.Logic;

public class JobFactory : IJobFactory
{
    public IJob CreateJob(object settings)
    {
        return settings switch
        {
            MeSettings meSettings => new MeJob(meSettings),
            PingSettings pingSettings => new PingJob(pingSettings),
            TcpSettings tcpSettings => new TcpJob(tcpSettings),
            UdpSettings udpSettings => new UdpJob(udpSettings),
            HttpSettings httpSettings => new HttpJob(httpSettings),
            _ => throw new InvalidOperationException("Unknown job type")
        };
    }
}