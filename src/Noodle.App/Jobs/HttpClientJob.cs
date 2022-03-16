using Noodle.App.Common;
using Noodle.App.Options;

namespace Noodle.App.Jobs;

public class HttpClientJob : IJob
{
    private readonly HttpClient _httpClient;
    private readonly HttpOptions _options;

    public HttpClientJob(HttpClient httpClient, HttpOptions options, IJobStats stats)
    {
        _httpClient = httpClient;
        _options = options;
    }

    public async Task<string> RunAsync(CancellationToken cancellationToken)
    {
        using var request = new HttpRequestMessage(new HttpMethod(_options.Method), _options.Url);
        using var response = await _httpClient.SendAsync(request, cancellationToken);

        response.EnsureSuccessStatusCode();

        return $"{response.StatusCode} ({(int)response.StatusCode})";
    }

    public void Dispose()
    {
        _httpClient.Dispose();
    }
}