using Spectre.Console.Cli;

namespace Noodle.App.Settings;

public class UdpSettings : CommandSettings
{
    [CommandArgument(0, "<host>")]
    public string Host { get; set; }

    [CommandArgument(1, "<port>")]
    public int Port { get; set; }

    [CommandOption("--ip")]
    public string[] IpAddresses { get; set; }

    [CommandOption("-c|--concurrency")]
    public int? Concurrency { get; set; }

    [CommandOption("-t|--timeout")]
    public int? Timeout { get; set; }

    [CommandOption("--throttle")]
    public int? Throttle { get; set; }
}