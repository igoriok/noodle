using Noodle.App.Common;
using Noodle.App.Settings;
using Noodle.App.Stages;

namespace Noodle.App.Jobs;

public class MeJob : IJob
{
    private readonly MeSettings _settings;

    public string Name => "ME";
    public string Description => string.Empty;

    public MeJob()
        : this(new MeSettings())
    {
    }

    public MeJob(MeSettings settings)
    {
        _settings = settings;
    }

    public IEnumerable<IStage> Pipeline => Enumerable.Empty<IStage>();

    public async Task<string> RunAsync(CancellationToken cancellationToken)
    {
        using var client = new HttpClient();

        return await client.GetStringAsync("https://ifconfig.me", cancellationToken);
    }
}