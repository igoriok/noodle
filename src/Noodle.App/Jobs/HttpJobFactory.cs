using Noodle.App.Common;

namespace Noodle.App.Jobs;

public class HttpJobFactory : BaseJobFactory<HttpJobOptions>, IDisposable
{
    private readonly HttpClient _httpClient = new HttpClient();

    protected override IJob CreateJobCore(HttpJobOptions options, IJobStats stats)
    {
        return new HttpJob(_httpClient, options, stats);
    }

    public void Dispose()
    {
        _httpClient.Dispose();
    }

    private class HttpJob : BaseJob
    {
        private readonly HttpClient _httpClient;
        private readonly HttpJobOptions _options;

        public HttpJob(HttpClient httpClient, HttpJobOptions options, IJobStats stats)
            : base(options, stats)
        {
            _httpClient = httpClient;
            _options = options;
        }

        protected override async Task RunCoreAsync(CancellationToken cancellationToken)
        {
            using var request = new HttpRequestMessage(new HttpMethod(_options.Method), _options.Url);
            using var response = await _httpClient.SendAsync(request, cancellationToken);

            response.EnsureSuccessStatusCode();
        }
    }
}