namespace Noodle.App.Stages;

public interface IStage
{
    Task ExecuteAsync(Func<CancellationToken, Task> next, CancellationToken cancellationToken);
}