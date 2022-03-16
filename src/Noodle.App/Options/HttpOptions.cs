using Spectre.Console.Cli;

namespace Noodle.App.Options;

public class HttpOptions : BaseOptions
{
    [CommandOption("-m|--method")]
    public string Method { get; set; } = "GET";

    [CommandOption("--user-agent")]
    public string UserAgent { get; set; } = "Mozilla/5.0 (X11; CrOS x86_64 8172.45.0) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/51.0.2704.64 Safari/537.36";
}