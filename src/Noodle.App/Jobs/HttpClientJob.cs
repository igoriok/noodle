using Noodle.App.Common;
using Noodle.App.Settings;
using Noodle.App.Stages;

namespace Noodle.App.Jobs;

public class HttpClientJob : IJob
{
    private readonly HttpClient _httpClient;
    private readonly HttpSettings _settings;

    public string Name => "HTTP";
    public string Description => _settings.Url.ToString();

    public IEnumerable<IStage> Pipeline
    {
        get
        {
            yield return new ParallelStage(_settings.Concurrency ?? 1);
            yield return new RepeatStage();
        }
    }

    public HttpClientJob(HttpClient httpClient, HttpSettings settings)
    {
        _httpClient = httpClient;
        _settings = settings;
    }

    public async Task<string> RunAsync(CancellationToken cancellationToken)
    {
        using var request = new HttpRequestMessage(new HttpMethod(_settings.Method), _settings.Url);
        using var response = await _httpClient.SendAsync(request, cancellationToken);

        response.EnsureSuccessStatusCode();

        return $"{response.StatusCode} ({(int)response.StatusCode})";
    }

    public void Dispose()
    {
        _httpClient.Dispose();
    }
}