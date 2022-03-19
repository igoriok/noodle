using Noodle.App.Common;
using Noodle.App.Jobs;
using Noodle.App.Logic;
using Noodle.App.UI;
using Spectre.Console;
using Spectre.Console.Cli;

namespace Noodle.App.Commands;

public class JobCommand<TSettings> : AsyncCommand<TSettings>
    where TSettings : CommandSettings
{
    private readonly IAnsiConsole _console;
    private readonly IJobFactory _factory;

    public JobCommand(IAnsiConsole console, IJobFactory factory)
    {
        _console = console;
        _factory = factory;
    }

    public override async Task<int> ExecuteAsync(CommandContext context, TSettings settings)
    {
        var cts = new CancellationTokenSource();

        Console.CancelKeyPress += (_, _) => cts.Cancel();

        await ExecuteAsync(settings, cts.Token);

        return 1;
    }

    private async Task ExecuteAsync(TSettings settings, CancellationToken cancellationToken)
    {
        var jobs = new[]
        {
            new JobRunner(new MeJob()),
            new JobRunner(_factory.CreateJob(settings))
        };

        var view = new JobsView(jobs);

        var tasks = jobs.Select(job => job.RunAsync(cancellationToken)).ToArray();

        await _console
            .Live(view)
            .StartAsync(async ctx =>
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    try
                    {
                        await Task.Delay(TimeSpan.FromSeconds(0.5), cancellationToken);
                    }
                    catch (TaskCanceledException) { }

                    ctx.Refresh();
                }
            });

        await Task.WhenAll(tasks);
    }
}