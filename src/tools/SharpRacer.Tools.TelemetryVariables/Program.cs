using System.CommandLine;
using System.CommandLine.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SharpRacer.Tools.TelemetryVariables.CommandLine;
using SharpRacer.Tools.TelemetryVariables.Commands;
using SharpRacer.Tools.TelemetryVariables.Data;
using SharpRacer.Tools.TelemetryVariables.Data.Stores;
using SharpRacer.Tools.TelemetryVariables.Import;

namespace SharpRacer.Tools.TelemetryVariables;

internal class Program
{
    public static Task<int> Main(string[] args)
    {
        var rootCommand = new CliRootCommand("SharpRacer Telemetry Variables Manager");

        rootCommand.Subcommands.Add(new DatabaseCommand());
        rootCommand.Subcommands.Add(new ExportCommand());
        rootCommand.Subcommands.Add(new ImportCommand());

        var cliConfig = new CliConfiguration(rootCommand)
            .UseHost(_ => Host.CreateDefaultBuilder(args), ConfigureHost);

        return cliConfig.InvokeAsync(args);
    }

    private static void ConfigureHost(IHostBuilder host)
    {
        var parseResult = host.GetParseResult();

        host.ConfigureLogging((ctx, logging) =>
        {
            logging.ClearProviders();

            logging.AddFilter("Microsoft", LogLevel.Warning);
            logging.AddSimpleConsole();

        });

        host.ConfigureServices(ConfigureServices);
    }

    private static void ConfigureServices(HostBuilderContext hostBuilderContext, IServiceCollection services)
    {
        // Data services
        var parseResult = hostBuilderContext.GetParseResult();

        if (parseResult.CommandResult.Command is IConfigureDbContextCommand dbContextCommand)
        {
            services.AddDbContext<TelemetryVariablesDbContext>((svc, db) => dbContextCommand.ConfigureDbContext(db, parseResult, svc));
        }

        services.AddScoped<ICarStore, CarStore>();
        services.AddScoped<ICarVariableStore, CarVariableStore>();
        services.AddScoped<IVariableStore, VariableStore>();

        services.AddScoped<VariableImporter>();
        services.AddScoped<CarImporter>();
    }
}
