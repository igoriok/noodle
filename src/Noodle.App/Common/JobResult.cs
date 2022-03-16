namespace Noodle.App.Common;

public class JobResult
{
    public string Status { get; }

    public JobResult(string status = null)
    {
        Status = status;
    }
}