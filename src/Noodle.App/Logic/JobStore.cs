using Noodle.App.Common;

namespace Noodle.App.Logic;

public class JobStore
{
    private readonly JobConfiguration _configuration;
    private readonly IJobFactory _factory;

    public JobStore(JobConfiguration configuration, IJobFactory factory)
    {
        _configuration = configuration;
        _factory = factory;
    }

    public IEnumerable<JobRunner> Load()
    {
        foreach (var options in _configuration.Load())
        {
            yield return new JobRunner(_factory, options);
        }
    }

    public void Release(IEnumerable<JobRunner> jobs)
    {
        foreach (var job in jobs)
        {
            job.Dispose();
        }
    }
}