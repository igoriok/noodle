namespace Noodle.App.Stages;

public class ParallelStage : IStage
{
    private readonly int _concurrency;

    public ParallelStage(int concurrency)
    {
        _concurrency = concurrency;
    }

    public async Task ExecuteAsync(Func<CancellationToken, Task> next, CancellationToken cancellationToken)
    {
        var tasks = new List<Task>(_concurrency);

        for (var i = 0; i < _concurrency; i++)
        {
            tasks.Add(next(cancellationToken));
        }

        await Task.WhenAll(tasks);
    }
}