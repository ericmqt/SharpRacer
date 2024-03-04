using System.CommandLine;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SharpRacer.Tools.TelemetryVariables.CommandLine;
using SharpRacer.Tools.TelemetryVariables.Data;

namespace SharpRacer.Tools.TelemetryVariables.Commands;
internal sealed class ImportTelemetryCommand
    : CliCommand<ImportTelemetryCommandHandler, ImportTelemetryCommandOptions>, IConfigureDbContextCommand
{
    public ImportTelemetryCommand()
        : base("telemetry", "Imports telemetry variables from telemetry files (*.IBT).")
    {
        DatabaseFileOption = new CliOption<FileInfo>("--database", ["--database", "-d"])
        {
            Description = "The database file path.",
            Required = true,
        };

        DatabaseFileOption.AcceptExistingOnly();

        InputFileOrDirectoryArgument = new CliArgument<FileSystemInfo>("input-file-or-dir")
        {
            Arity = new ArgumentArity(0, 1),
            DefaultValueFactory = _ => ImportTelemetryCommandOptions.GetDefaultInputDirectory(),
            Description = "Input file name or directory. Defaults to iRacing telemetry folder."
        };

        RecursiveOption = new CliOption<bool>("--recurse", ["--recurse", "-r"]);

        Arguments.Add(InputFileOrDirectoryArgument);
        Options.Add(DatabaseFileOption);
        Options.Add(RecursiveOption);
    }

    public CliOption<FileInfo> DatabaseFileOption { get; }
    public CliArgument<FileSystemInfo> InputFileOrDirectoryArgument { get; }
    public CliOption<bool> RecursiveOption { get; }

    public void ConfigureDbContext(DbContextOptionsBuilder builder, ParseResult parseResult, IServiceProvider serviceProvider)
    {
        var databaseFile = parseResult.GetValue(DatabaseFileOption)!;

        var csb = new SqliteConnectionStringBuilder()
        {
            DataSource = databaseFile.FullName
        };

        builder.UseSqlite(csb.ConnectionString);
    }

    protected override ImportTelemetryCommandOptions CreateOptions(ParseResult parseResult)
    {
        return new ImportTelemetryCommandOptions()
        {
            EnumerateInputFilesRecursively = parseResult.GetValue(RecursiveOption),
            InputFileOrDirectory = parseResult.GetValue(InputFileOrDirectoryArgument) ?? ImportTelemetryCommandOptions.GetDefaultInputDirectory()
        };
    }

    protected override async Task<int> InvokeAsync(
        ImportTelemetryCommandHandler handler,
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

        var logger = serviceProvider.GetRequiredService<ILogger<ImportTelemetryCommand>>();

        try { return await handler.ExecuteAsync(cancellationToken).ConfigureAwait(false); }
        catch (Exception ex)
        {
            logger.LogError(ex, "An unhandled exception was thrown while executing the operation.");

            return -2;
        }
    }
}
