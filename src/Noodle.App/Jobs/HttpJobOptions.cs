using Noodle.App.Common;

namespace Noodle.App.Jobs;

public class HttpJobOptions : JobOptions
{
    public string Method { get; set; }
}