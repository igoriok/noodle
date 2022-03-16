namespace Noodle.App.Common;

public interface IJobFactory
{
    IJob CreateJob(IJobOptions options);
}