using Noodle.App.Stages;

namespace Noodle.App.Common;

public interface IJob
{
    string Name { get; }
    string Description { get; }

    IEnumerable<IStage> Pipeline { get; }

    Task<string> RunAsync(CancellationToken cancellationToken);
}