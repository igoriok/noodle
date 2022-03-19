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
            HttpSettings httpSettings => new HttpJob(httpSettings),
            PingSettings pingSettings => new PingJob(pingSettings),
            _ => throw new InvalidOperationException("Unknown job type")
        };
    }
}