namespace Noodle.App.Stages;

public class TimeoutStage : IStage
{
    private readonly TimeSpan _value;

    public TimeoutStage(long value)
    {
        _value = TimeSpan.FromMilliseconds(value);
    }

    public async Task ExecuteAsync(Func<CancellationToken, Task> next, CancellationToken cancellationToken)
    {
        using var timeoutSource = new CancellationTokenSource(_value);
        using var linkedSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, timeoutSource.Token);

        try
        {
            await next(linkedSource.Token);
        }
        catch (OperationCanceledException) when (timeoutSource.IsCancellationRequested)
        {
        }
    }
}