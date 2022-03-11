namespace Noodle.App.Common;

public abstract class JobOptions
{
    public Uri Url { get; set; }

    public int Concurrency { get; set; } = 1;
}