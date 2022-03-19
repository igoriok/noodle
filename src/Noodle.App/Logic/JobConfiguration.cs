using Microsoft.Extensions.Configuration;
using Noodle.App.Settings;

namespace Noodle.App.Logic;

public class JobConfiguration
{
    private readonly IConfiguration _configuration;

    public JobConfiguration(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public IEnumerable<object> Load()
    {
        var section = _configuration.GetSection("Jobs");

        foreach (var child in section.GetChildren())
        {
            var url = child.GetValue<Uri>("Url");

            yield return Deserialize(url, child);
        }
    }

    private object Deserialize(Uri url, IConfiguration configuration)
    {
        return url.Scheme switch
        {
            "http" => configuration.Get<HttpSettings>(),
            "https" => configuration.Get<HttpSettings>(),
            "me" => configuration.Get<MeSettings>(),
            _ => throw new InvalidOperationException("Unknown protocol"),
        };
    }
}