using System.CommandLine;
using System.Runtime.Versioning;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SharpRacer.Tools.TelemetryVariables.CommandLine;
using SharpRacer.Tools.TelemetryVariables.Data;

namespace SharpRacer.Tools.TelemetryVariables.Commands;

[SupportedOSPlatform("windows")]
internal sealed class ImportSimulatorCommand
    : CliCommand<ImportSimulatorCommandHandler, ImportSimulatorCommandOptions>, IConfigureDbContextCommand
{
    public ImportSimulatorCommand()
        : base("simulator", "Imports telemetry variables from a simulator session.")
    {
        DatabaseFileOption = new CliOption<FileInfo>("--database", ["--database", "-d"])
        {
            Description = "The database file path.",
            Required = true,
        };

        DatabaseFileOption.AcceptExistingOnly();

        WaitForConnectionOption = new CliOption<bool>("--wait", ["--wait", "-w"])
        {
            DefaultValueFactory = _ => false,
            Description = "Wait for a connection to the simulator if it is not running instead of exiting immediately."
        };

        Options.Add(DatabaseFileOption);
        Options.Add(WaitForConnectionOption);
    }

    public CliOption<FileInfo> DatabaseFileOption { get; }
    public CliOption<bool> WaitForConnectionOption { get; }

    public void ConfigureDbContext(DbContextOptionsBuilder builder, ParseResult parseResult, IServiceProvider serviceProvider)
    {
        var databaseFile = parseResult.GetValue(DatabaseFileOption)!;

        var csb = new SqliteConnectionStringBuilder()
        {
            DataSource = databaseFile.FullName
        };

        builder.UseSqlite(csb.ConnectionString);
    }

    protected override ImportSimulatorCommandOptions CreateOptions(ParseResult parseResult)
    {
        return new ImportSimulatorCommandOptions()
        {
            WaitForConnection = parseResult.GetValue(WaitForConnectionOption)
        };
    }

    protected override async Task<int> InvokeAsync(
        ImportSimulatorCommandHandler handler,
        IServiceProvider serviceProvider,
        CancellationToken cancellationToken)
    {
        // Check database is up to date
        var dbContext = serviceProvider.GetRequiredService<TelemetryVariablesDbContext>();

        if (dbContext.Database.GetPendingMigrations().Any())
        {
            Console.WriteLine("Database must be updated before operation can proceed. See command: database migrate");

            return -1;
        }

        var logger = serviceProvider.GetRequiredService<ILogger<ImportSimulatorCommand>>();

        try { return await handler.ExecuteAsync(cancellationToken).ConfigureAwait(false); }
        catch (Exception ex)
        {
            logger.LogError(ex, "An unhandled exception was thrown while executing the operation.");

            return -2;
        }
    }
}
