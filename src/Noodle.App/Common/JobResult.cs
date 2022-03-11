namespace Noodle.App.Common;

public class JobResult
{
    public TimeSpan TimeSpan { get; }
    public bool IsSuccessful { get; }
    public string Description { get; }

    public JobResult(TimeSpan timeSpan, bool isSuccessful, string description = null)
    {
        TimeSpan = timeSpan;
        IsSuccessful = isSuccessful;
        Description = description;
    }
}