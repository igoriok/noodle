using System.ComponentModel;
using Noodle.App.Common;
using Spectre.Console.Cli;

namespace Noodle.App.Jobs;

public class HttpJobOptions : BaseJobOptions
{
    [CommandOption("-m|--method")]
    [DefaultValue("GET")]
    public string Method { get; set; } = "GET";
}