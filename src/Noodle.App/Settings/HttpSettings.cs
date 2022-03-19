using Spectre.Console.Cli;

namespace Noodle.App.Settings;

public class HttpSettings : CommandSettings
{
    [CommandArgument(0, "<url>")]
    public Uri Url { get; set; }

    [CommandOption("-m|--method")]
    public string Method { get; set; } = "GET";

    [CommandOption("--user-agent")]
    public string UserAgent { get; set; } = "Mozilla/5.0 (X11; CrOS x86_64 8172.45.0) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/51.0.2704.64 Safari/537.36";

    [CommandOption("--ip")]
    public string[] IpAddresses { get; set; }

    [CommandOption("-c|--concurrency")]
    public int? Concurrency { get; set; }

    [CommandOption("-t|--timeout")]
    public int? Timeout { get; set; }

    [CommandOption("--throttle")]
    public int? Throttle { get; set; }
}