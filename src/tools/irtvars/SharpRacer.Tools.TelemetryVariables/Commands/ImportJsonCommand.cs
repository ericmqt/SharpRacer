using System.CommandLine;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SharpRacer.Tools.TelemetryVariables.CommandLine;
using SharpRacer.Tools.TelemetryVariables.Data;

namespace SharpRacer.Tools.TelemetryVariables.Commands;
internal sealed class ImportJsonCommand : CliCommand<ImportJsonCommandHandler, ImportJsonCommandOptions>, IConfigureDbContextCommand
{
    public ImportJsonCommand()
        : base("json", "Imports telemetry variables from a JSON document exported by this tool.")
    {
        DatabaseFileOption = new CliOption<FileInfo>("--database", ["--database", "-d"])
        {
            Description = "The database file path.",
            Required = true,
        };

        DatabaseFileOption.AcceptExistingOnly();

        InputFileArgument = new CliArgument<FileInfo>("input-file")
        {
            Arity = ArgumentArity.ExactlyOne,
            Description = "Path to JSON-formatted variables database file."
        }
        .AcceptExistingOnly();

        Arguments.Add(InputFileArgument);
        Options.Add(DatabaseFileOption);
    }

    public CliOption<FileInfo> DatabaseFileOption { get; }
    public CliArgument<FileInfo> InputFileArgument { get; }

    public void ConfigureDbContext(DbContextOptionsBuilder builder, ParseResult parseResult, IServiceProvider serviceProvider)
    {
        var databaseFile = parseResult.GetValue(DatabaseFileOption)!;

        var csb = new SqliteConnectionStringBuilder()
        {
            DataSource = databaseFile.FullName
        };

        builder.UseSqlite(csb.ConnectionString);
    }

    protected override ImportJsonCommandOptions CreateOptions(ParseResult parseResult)
    {
        return new ImportJsonCommandOptions()
        {
            InputFile = parseResult.GetValue(InputFileArgument)!
        };
    }

    protected override async Task<int> InvokeAsync(
        ImportJsonCommandHandler handler,
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

        var logger = serviceProvider.GetRequiredService<ILogger<ImportJsonCommand>>();

        try { return await handler.ExecuteAsync(cancellationToken).ConfigureAwait(false); }
        catch (Exception ex)
        {
            logger.LogError(ex, "An unhandled exception was thrown while executing the operation.");

            return -2;
        }
    }
}
