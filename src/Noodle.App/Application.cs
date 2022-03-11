using Noodle.App.Logic;
using Noodle.App.UI;
using Spectre.Console;

namespace Noodle.App;

public class Application
{
    private readonly IAnsiConsole _console;
    private readonly JobStore _store;

    public Application(IAnsiConsole console, JobStore store)
    {
        _console = console;
        _store = store;
    }

    public async Task RunAsync(CancellationToken cancellationToken)
    {
        var jobs = _store.Load().ToArray();
        var view = new JobsView(jobs);

        var tasks = jobs.Select(job => job.RunAsync(cancellationToken)).ToArray();

        await _console
            .Live(view)
            .StartAsync(async ctx =>
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    await Task.Delay(TimeSpan.FromSeconds(0.5), cancellationToken);

                    view.Refresh();
                    ctx.Refresh();
                }
            });

        await Task.WhenAll(tasks);

        _store.Release(jobs);
    }
}