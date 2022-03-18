using Noodle.App.Common;
using Spectre.Console.Cli;

namespace Noodle.App.Options;

public abstract class BaseOptions : CommandSettings, IJobOptions
{
    [CommandArgument(0, "<url>")]
    public Uri Url { get; set; }

    [CommandOption("--ip")]
    public string[] IpAddresses { get; set; }

    [CommandOption("-c")]
    public int Concurrency { get; set; } = 1;
}