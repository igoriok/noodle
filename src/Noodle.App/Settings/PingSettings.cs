using Spectre.Console.Cli;

namespace Noodle.App.Settings;

public class PingSettings : CommandSettings
{
    [CommandArgument(0, "<host>")]
    public string Host { get; set; }

    [CommandOption("--ip")]
    public string[] IpAddresses { get; set; }

    [CommandOption("--throttle")]
    public long? Throttle { get; set; }

    [CommandOption("-t|--timeout")]
    public long? Timeout { get; set; }

    public override string ToString()
    {
        return Host;
    }
}