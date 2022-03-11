using Noodle.App.Common;
using Noodle.App.Logic;
using Noodle.App.UI;
using Spectre.Console;
using Spectre.Console.Cli;

namespace Noodle.App.Commands;

public class JobCommand<TOptions> : AsyncCommand<TOptions>
    where TOptions : BaseJobOptions
{
    private readonly IAnsiConsole _console;
    private readonly JobFactory _factory;

    public JobCommand(IAnsiConsole console, JobFactory factory)
    {
        _console = console;
        _factory = factory;
    }

    public override async Task<int> ExecuteAsync(CommandContext context, TOptions settings)
    {
        var cts = new CancellationTokenSource();

        Console.CancelKeyPress += (_, _) => cts.Cancel();

        await ExecuteAsync(settings, cts.Token);

        return 1;
    }

    private async Task ExecuteAsync(TOptions options, CancellationToken cancellationToken)
    {
        using (var job = _factory.Create(options))
        {
            var worker = new JobWorker(job, options);
            var view = new JobsView(worker);

            var task = worker.RunAsync(cancellationToken);

            try
            {
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

                await task;
            }
            catch (TaskCanceledException) { }
        }
    }
}