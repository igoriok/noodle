using Noodle.App.Common;
using Noodle.App.Protocols.Http;

namespace Noodle.App.Jobs;

public class FastHttpJobFactory : BaseJobFactory<HttpJobOptions>
{
    protected override IJob CreateJobCore(HttpJobOptions options, IJobStats stats)
    {
        return new FastHttpJob(options, stats);
    }

    public class FastHttpJob : BaseJob
    {
        private readonly HttpJobOptions _options;

        private readonly FastHttpConnection _connection;
        private readonly FastHttpRequest _request;

        public FastHttpJob(HttpJobOptions options, IJobStats stats)
            : base(options, stats)
        {
            _options = options;
            _connection = new FastHttpConnection(_options.Url);

            _request = BuildRequest();
        }

        public override async Task RunAsync(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    var response = await _connection.SendAsync(_request, cancellationToken);

                    lock (Stats)
                    {
                        Stats.Successful++;
                        Stats.Status = $"{response.StatusCode}";
                    }
                }
                catch (OperationCanceledException) { }
                catch (Exception exception)
                {
                    lock (Stats)
                    {
                        Stats.Failed++;
                        Stats.Status = exception.Message;
                    }
                }
            }
        }

        protected override Task RunCoreAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        public override void Dispose()
        {
            _connection.Dispose();
        }

        private FastHttpRequest BuildRequest()
        {
            return new FastHttpRequest(_options.Url, _options.Method)
            {
                UserAgent = _options.UserAgent,
            };
        }
    }
}