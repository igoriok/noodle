using Noodle.App.Commands;
using Noodle.App.Common;

namespace Noodle.App.Jobs;

public class HttpJob : IJob
{
    private readonly HttpJobOptions _options;
    private readonly HttpClient _httpClient;

    public BaseJobOptions Options => _options;

    public HttpJob(HttpJobOptions options)
    {
        _options = options;
        _httpClient = new HttpClient();
    }

    public async Task RunAsync(CancellationToken cancellationToken)
    {
        using var request = new HttpRequestMessage(new HttpMethod(_options.Method), _options.Url);
        using var response = await _httpClient.SendAsync(request, cancellationToken);
    }

    public void Dispose()
    {
        _httpClient.Dispose();
    }
}