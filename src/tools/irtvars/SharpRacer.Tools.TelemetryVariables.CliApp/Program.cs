using System.CommandLine;
using System.CommandLine.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SharpRacer.Tools.TelemetryVariables.CommandLine;
using SharpRacer.Tools.TelemetryVariables.Configuration;
using SharpRacer.Tools.TelemetryVariables.Data;
using SharpRacer.Tools.TelemetryVariables.Services;

namespace SharpRacer.Tools.TelemetryVariables;

internal class Program
{
    public static Task<int> Main(string[] args)
    {
        var cliConfig = new CliConfiguration(CreateRootCommand());

        cliConfig.UseHost(
            _ => Host.CreateDefaultBuilder(args),
            host =>
            {
                host.ConfigureAppConfiguration(Configure);

                host.ConfigureLogging((ctx, logging) =>
                {
                    var logConfig = ctx.Configuration.GetSection("Logging");

                    logging.ClearProviders();
                    logging.AddConfiguration(ctx.Configuration.GetSection("Logging"));

                    logging.AddConsole();

                });

                host.ConfigureServices((ctx, svc) => ConfigureServices(svc, ctx.Configuration));
            });

        return cliConfig.InvokeAsync(args);
    }

    private static void Configure(HostBuilderContext context, IConfigurationBuilder configuration)
    {
        configuration.AddJsonFile(JsonAppConfiguration.GetPath(), optional: false);
    }

    private static void ConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<DatabaseOptions>(configuration.GetSection("Database"));

        services.AddTelemetryVariablesDbContext(
            (svc, csb) =>
            {
                var dbOptions = svc.GetRequiredService<IOptions<DatabaseOptions>>().Value;
                var dbFile = new FileInfo(dbOptions.Path);

                csb.DataSource = dbFile.FullName;
            })
            .AddTelemetryVariablesDataServices();

        services.AddScoped<DataVariableImporter>();
    }

    private static CliRootCommand CreateRootCommand()
    {
        var cmd = new CliRootCommand("SharpRacer Telemetry Variables Manager");

        cmd.Subcommands.Add(CommandFactory.ConfigureCommand());
        cmd.Subcommands.Add(DatabaseCommandFactory.DatabaseCommand());
        cmd.Subcommands.Add(CommandFactory.ExportCommand());
        cmd.Subcommands.Add(ImportCommandFactory.ImportCommand());

        return cmd;
    }
}
