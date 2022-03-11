namespace Noodle.App.Logic;

public class JobStore
{
    private readonly JobConfiguration _configuration;
    private readonly JobFactory _factory;

    public JobStore(JobConfiguration configuration, JobFactory factory)
    {
        _configuration = configuration;
        _factory = factory;
    }

    public IEnumerable<JobWorker> Load()
    {
        foreach (var options in _configuration.Load())
        {
            var job = _factory.Create(options);

            yield return new JobWorker(job, options);
        }
    }

    public void Release(IEnumerable<JobWorker> jobs)
    {
        foreach (var job in jobs)
        {
            job.Dispose();
        }
    }
}