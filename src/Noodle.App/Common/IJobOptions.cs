namespace Noodle.App.Common;

public interface IJobOptions
{
    public Uri Url { get; }

    public string [] IpAddresses { get; }

    public int Concurrency { get; }
}