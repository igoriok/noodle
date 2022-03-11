using Microsoft.Extensions.Configuration;
using Noodle.App.Commands;
using Noodle.App.Common;
using Noodle.App.Jobs;

namespace Noodle.App.Logic;

public class JobConfiguration
{
    private readonly IConfiguration _configuration;

    public JobConfiguration(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public IEnumerable<BaseJobOptions> Load()
    {
        var section = _configuration.GetSection("Jobs");

        foreach (var child in section.GetChildren())
        {
            var url = child.GetValue<Uri>("Url");

            yield return Deserialize(url, child);
        }
    }

    private BaseJobOptions Deserialize(Uri url, IConfiguration configuration)
    {
        return url.Scheme switch
        {
            "http" => configuration.Get<HttpJobOptions>(),
            "https" => configuration.Get<HttpJobOptions>(),
            _ => throw new InvalidOperationException("Unknown protocol"),
        };
    }
}