namespace Noodle.App.Stages;

public class RepeatStage : IStage
{
    public async Task ExecuteAsync(Func<CancellationToken, Task> next, CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            await next(cancellationToken);
        }
    }
}