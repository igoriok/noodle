using Noodle.App.Logic;
using Spectre.Console;
using Spectre.Console.Rendering;

namespace Noodle.App.UI;

public class JobsView : JustInTimeRenderable
{
    private readonly JobRunner[] _jobs;

    public JobsView(params JobRunner[] jobs)
    {
        _jobs = jobs;
    }

    protected override bool HasDirtyChildren()
    {
        return true;
    }

    protected override IRenderable Build()
    {
        var table = new Table()
            .AddColumn("Url")
            .AddColumn("Status")
            .AddColumn("Successful")
            .AddColumn("Failed");

        foreach (var job in _jobs)
        {
            table.AddRow(
                new Text($"{job.Options}"),
                new Text($"{job.Stats.Status}"),
                new Text($"{job.Stats.Successful}"),
                new Text($"{job.Stats.Failed}")
            );
        }

        return table;
    }
}