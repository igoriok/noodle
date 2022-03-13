namespace Noodle.App.Common;

public interface IJobFactory<in TOptions>
{
    IJob CreateJob(TOptions options);
}