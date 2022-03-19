using System.Diagnostics;
using System.Runtime.ExceptionServices;

namespace Noodle.App.Stages;

public class ThrottleStage : IStage
{
    private readonly TimeSpan _rate;

    public ThrottleStage(long value)
    {
        _rate = TimeSpan.FromMilliseconds(value);
    }

    public async Task ExecuteAsync(Func<CancellationToken, Task> next, CancellationToken cancellationToken)
    {
        ExceptionDispatchInfo exception = null;

        var sw = Stopwatch.StartNew();

        try
        {
            await next(cancellationToken);
        }
        catch (Exception e)
        {
            exception = ExceptionDispatchInfo.Capture(e);
        }

        sw.Stop();

        if (_rate > sw.Elapsed)
        {
            await Task.Delay(_rate - sw.Elapsed, cancellationToken);
        }

        exception?.Throw();
    }
}