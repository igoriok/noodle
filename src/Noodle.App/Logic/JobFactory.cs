using Noodle.App.Common;
using Noodle.App.Jobs;
using Noodle.App.Options;

namespace Noodle.App.Logic;

public class JobFactory : IJobFactory
{
    public IJob CreateJob(IJobOptions options)
    {
        return options switch
        {
            HttpOptions httpOptions => new HttpJob(httpOptions),
            _ => throw new InvalidOperationException("Unknown job type")
        };
    }
}