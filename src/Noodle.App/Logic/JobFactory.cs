using Microsoft.Extensions.DependencyInjection;
using Noodle.App.Common;

namespace Noodle.App.Logic;

public class JobFactory : IJobFactory<IJobOptions>
{
    private readonly IServiceProvider _provider;

    public JobFactory(IServiceProvider provider)
    {
        _provider = provider;
    }

    public IJob CreateJob(IJobOptions options)
    {
        var optionsType = options.GetType();
        var wrapperType = typeof(JobWrapper<>).MakeGenericType(optionsType);

        return (IJob)ActivatorUtilities.CreateInstance(_provider, wrapperType);
    }

    private class JobWrapper<TOptions> : IJob
        where TOptions : IJobOptions
    {
        private readonly IJob _job;

        public IJobOptions Options => _job.Options;
        public IJobStats Stats => _job.Stats;

        public JobWrapper(IJobFactory<TOptions> factory, TOptions options)
        {
            _job = factory.CreateJob(options);
        }

        public Task RunAsync(CancellationToken cancellationToken)
        {
            return _job.RunAsync(cancellationToken);
        }

        public void Dispose()
        {
            _job.Dispose();
        }
    }
}