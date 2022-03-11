using Noodle.App.Common;
using Noodle.App.Jobs;

namespace Noodle.App.Logic;

public class JobFactory
{
    public IJob Create(JobOptions options)
    {
        switch (options)
        {
            case HttpJobOptions httpJobOptions: return new HttpJob(httpJobOptions);

            default: throw new InvalidOperationException("Unknown job type");
        }
    }
}