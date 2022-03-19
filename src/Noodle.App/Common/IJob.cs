using Noodle.App.Stages;

namespace Noodle.App.Common;

public interface IJob
{
    IEnumerable<IStage> Pipeline { get; }

    Task<string> RunAsync(CancellationToken cancellationToken);
}