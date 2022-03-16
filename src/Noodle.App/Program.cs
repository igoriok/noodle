using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Noodle.App.Commands;
using Noodle.App.Common;
using Noodle.App.Infrastructure;
using Noodle.App.Logic;
using Noodle.App.Options;
using Spectre.Console;
using Spectre.Console.Cli;

var configuration = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json")
    .Build();

var services = new ServiceCollection()
    .AddSingleton<IConfiguration>(configuration)
    .AddSingleton<JobConfiguration>()
    .AddSingleton<JobFactory>()
    .AddSingleton<JobStore>()
    .AddTransient<IJobFactory, JobFactory>();

var app = new CommandApp<DefaultCommand>(new TypeRegistrar(services));

app.Configure(cfg =>
{
    cfg.AddCommand<JobCommand<HttpOptions>>("http");

    cfg.SetExceptionHandler(e => AnsiConsole.WriteException(e));
});

return await app.RunAsync(args);