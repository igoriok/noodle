using Noodle.App.Common;
using Noodle.App.Logic;
using Noodle.App.UI;
using Spectre.Console;
using Spectre.Console.Cli;

namespace Noodle.App.Commands;

public class DefaultCommand : AsyncCommand
{
    private readonly IAnsiConsole _console;
    private readonly IJobFactory _factory;
    private readonly JobConfiguration _configuration;

    public DefaultCommand(IAnsiConsole console, IJobFactory factory, JobConfiguration configuration)
    {
        _console = console;
        _factory = factory;
        _configuration = configuration;
    }

    public override async Task<int> ExecuteAsync(CommandContext context)
    {
        var cts = new CancellationTokenSource();

        Console.CancelKeyPress += (_, _) => cts.Cancel();

        await ExecuteAsync(cts.Token);

        return 1;
    }

    private async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        var jobs = _configuration.Load().Select(o => new JobRunner(_factory.CreateJob(o))).ToArray();
        var view = new JobsView(jobs);

        var tasks = jobs.Select(job => job.RunAsync(cancellationToken)).ToArray();

        await _console
            .Live(view)
            .StartAsync(async ctx =>
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    await Task.Delay(TimeSpan.FromSeconds(0.5), cancellationToken);

                    ctx.Refresh();
                }
            });

        await Task.WhenAll(tasks);
    }
}