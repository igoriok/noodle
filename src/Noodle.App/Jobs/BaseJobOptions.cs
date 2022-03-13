using System.ComponentModel;
using Noodle.App.Common;
using Spectre.Console.Cli;

namespace Noodle.App.Jobs;

public abstract class BaseJobOptions : CommandSettings, IJobOptions
{
    [CommandArgument(0, "<url>")]
    public Uri Url { get; set; }

    [CommandOption("-c")]
    [DefaultValue(1)]
    public int Concurrency { get; set; } = 1;
}