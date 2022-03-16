using System.IO.Pipelines;
using Nerdbank.Streams;
using Noodle.App.Common;

namespace Noodle.App.Jobs;

public abstract class PipeJob : SocketJob
{
    protected PipeJob(IJobOptions options)
        : base(options)
    {
    }

    protected override Task<string> RunAsync(Stream stream, CancellationToken cancellationToken)
    {
        var pipe = new DuplexPipe(PipeReader.Create(stream), PipeWriter.Create(stream));

        return RunAsync(pipe, cancellationToken);
    }

    protected abstract Task<string> RunAsync(IDuplexPipe pipe, CancellationToken cancellationToken);
}