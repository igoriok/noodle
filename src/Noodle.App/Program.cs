using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Spectre.Console;
using Spectre.Console.Rendering;
using Noodle.App;
using Noodle.App.Logic;
using Noodle.App.UI;

await new HostBuilder()
    .ConfigureAppConfiguration(cfg =>
    {
        cfg.AddJsonFile("appsettings.json", false, true);
    })
    .ConfigureServices(services =>
    {
        services.AddSingleton(_ => AnsiConsole.Create(new AnsiConsoleSettings()));
        services.AddSingleton<JobConfiguration>();
        services.AddSingleton<JobFactory>();
        services.AddSingleton<JobStore>();
        services.AddSingleton<Application>();
        services.AddHostedService<Service>();
    })
    .RunConsoleAsync();

public class Service : BackgroundService
{
    private readonly Application _app;

    public Service(Application app)
    {
        _app = app;
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        return _app.RunAsync(stoppingToken);
    }
}