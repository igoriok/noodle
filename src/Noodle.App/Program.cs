using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Noodle.App.Commands;
using Noodle.App.Common;
using Noodle.App.Infrastructure;
using Noodle.App.Jobs;
using Noodle.App.Logic;
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
    .AddTransient<IJobFactory<HttpJobOptions>, FastHttpJobFactory>();

var app = new CommandApp<DefaultCommand>(new TypeRegistrar(services));

app.Configure(cfg =>
{
    cfg.AddCommand<JobCommand<HttpJobOptions>>("http");

    cfg.SetExceptionHandler(e => AnsiConsole.WriteException(e));
});

return await app.RunAsync(args);