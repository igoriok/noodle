using Noodle.App.Common;

namespace Noodle.App.Logic;

public class JobStore
{
    private readonly JobConfiguration _configuration;
    private readonly IJobFactory<IJobOptions> _factory;

    public JobStore(JobConfiguration configuration, IJobFactory<IJobOptions> factory)
    {
        _configuration = configuration;
        _factory = factory;
    }

    public IEnumerable<IJob> Load()
    {
        foreach (var options in _configuration.Load())
        {
            var job = _factory.CreateJob(options);

            yield return job;
        }
    }

    public void Release(IEnumerable<IJob> jobs)
    {
        foreach (var job in jobs)
        {
            job.Dispose();
        }
    }
}