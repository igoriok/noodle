using System.ComponentModel;
using Spectre.Console.Cli;

namespace Noodle.App.Common;

public abstract class BaseJobOptions : CommandSettings
{
    [CommandArgument(0, "<url>")]
    public Uri Url { get; set; }

    [CommandOption("-c")]
    [DefaultValue(1)]
    public int Concurrency { get; set; } = 1;
}