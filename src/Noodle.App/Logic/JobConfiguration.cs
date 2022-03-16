using Microsoft.Extensions.Configuration;
using Noodle.App.Common;
using Noodle.App.Options;

namespace Noodle.App.Logic;

public class JobConfiguration
{
    private readonly IConfiguration _configuration;

    public JobConfiguration(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public IEnumerable<IJobOptions> Load()
    {
        var section = _configuration.GetSection("Jobs");

        foreach (var child in section.GetChildren())
        {
            var url = child.GetValue<Uri>("Url");

            yield return Deserialize(url, child);
        }
    }

    private BaseOptions Deserialize(Uri url, IConfiguration configuration)
    {
        return url.Scheme switch
        {
            "http" => configuration.Get<HttpOptions>(),
            "https" => configuration.Get<HttpOptions>(),
            _ => throw new InvalidOperationException("Unknown protocol"),
        };
    }
}