using System.CommandLine;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SharpRacer.Tools.TelemetryVariables.CommandLine;
using SharpRacer.Tools.TelemetryVariables.Commands;
using SharpRacer.Tools.TelemetryVariables.Data;
using SharpRacer.Tools.TelemetryVariables.Data.Stores;
using SharpRacer.Tools.TelemetryVariables.Import;

namespace SharpRacer.Tools.TelemetryVariables;

internal class Program
{
    public static async Task<int> Main(string[] args)
    {
        // Parse command line
        var rootCommand = new RootCommand("SharpRacer Telemetry Variables Manager");

        rootCommand.Subcommands.Add(new DatabaseCommand());
        rootCommand.Subcommands.Add(new ExportCommand());
        rootCommand.Subcommands.Add(new ImportCommand());

        var parseResult = rootCommand.Parse(args);

        // Configure dependency injection
        var serviceCollection = new ServiceCollection();

        ConfigureServices(serviceCollection, parseResult);

        // Execute the command
        using var serviceProvider = serviceCollection.BuildServiceProvider();

        var invocationConfiguration = new DependencyInjectionInvocationConfiguration(serviceProvider);

        return await parseResult.InvokeAsync(invocationConfiguration);
    }

    private static void ConfigureServices(IServiceCollection services, ParseResult parseResult)
    {
        services.AddLogging(builder =>
        {
            builder.ClearProviders();
            builder.AddFilter("Microsoft", LogLevel.Warning);

            builder.AddSimpleConsole();
        });

        services.AddScoped<ICarStore, CarStore>();
        services.AddScoped<ICarVariableStore, CarVariableStore>();
        services.AddScoped<IVariableStore, VariableStore>();

        services.AddScoped<VariableImporter>();
        services.AddScoped<CarImporter>();

        // Data services
        if (parseResult.CommandResult.Command is IConfigureDbContextCommand dbContextCommand)
        {
            services.AddDbContext<TelemetryVariablesDbContext>((svc, db) => dbContextCommand.ConfigureDbContext(db, parseResult, svc));
        }
    }
}
