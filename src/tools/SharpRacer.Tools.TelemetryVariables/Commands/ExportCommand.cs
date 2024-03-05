using System.CommandLine;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SharpRacer.Tools.TelemetryVariables.CommandLine;
using SharpRacer.Tools.TelemetryVariables.Data;

namespace SharpRacer.Tools.TelemetryVariables.Commands;
internal class ExportCommand : CliCommand<ExportCommandHandler, ExportCommandOptions>, IConfigureDbContextCommand
{
    public ExportCommand()
        : base(name: "export", description: "Exports telemetry variable information.")
    {
        // Arguments
        OutputFileOrDirectoryArgument = new CliArgument<FileSystemInfo>("output-file-or-dir")
        {
            Arity = new ArgumentArity(0, 1),
            DefaultValueFactory = _ => new DirectoryInfo(Environment.CurrentDirectory)
        };

        OutputFileOrDirectoryArgument.AcceptLegalFilePathsOnly();

        // Options
        DatabaseFileOption = new CliOption<FileInfo>("--database", ["--database", "-d"])
        {
            Description = "The database file path.",
            Recursive = true,
            Required = true,
        };

        DatabaseFileOption.AcceptExistingOnly();

        IncludeDeprecatedOption = new CliOption<bool>("--include-deprecated")
        {
            DefaultValueFactory = _ => false
        };

        ExportVariablesOnlyOption = new CliOption<bool>("--variables-only") // TODO: Need a better name for this flag
        {
            Description = "Export variables only.",
            DefaultValueFactory = _ => false
        };

        Arguments.Add(OutputFileOrDirectoryArgument);
        Options.Add(DatabaseFileOption);
        Options.Add(IncludeDeprecatedOption);
        Options.Add(ExportVariablesOnlyOption);
    }

    public CliOption<FileInfo> DatabaseFileOption { get; }
    public CliOption<bool> ExportVariablesOnlyOption { get; }
    public CliOption<bool> IncludeDeprecatedOption { get; }
    public CliArgument<FileSystemInfo> OutputFileOrDirectoryArgument { get; }

    public void ConfigureDbContext(DbContextOptionsBuilder builder, ParseResult parseResult, IServiceProvider serviceProvider)
    {
        var databaseFile = parseResult.GetValue(DatabaseFileOption)!;

        var csb = new SqliteConnectionStringBuilder()
        {
            DataSource = databaseFile.FullName
        };

        builder.UseSqlite(csb.ConnectionString);
    }

    protected override ExportCommandOptions CreateOptions(ParseResult parseResult)
    {
        return new ExportCommandOptions
        {
            ExportVariablesOnly = parseResult.GetValue(ExportVariablesOnlyOption),
            IncludeDeprecated = parseResult.GetValue(IncludeDeprecatedOption),
            OutputFileOrDirectory = parseResult.GetValue(OutputFileOrDirectoryArgument)!
        };
    }

    protected override async Task<int> InvokeAsync(ExportCommandHandler handler, IServiceProvider serviceProvider, CancellationToken cancellationToken)
    {
        // Check database is up to date
        var dbContext = serviceProvider.GetRequiredService<TelemetryVariablesDbContext>();

        if (dbContext.Database.GetPendingMigrations().Any())
        {
            Console.WriteLine("Database must be updated before operation can proceed. See command: database migrate");

            return -1;
        }

        var logger = serviceProvider.GetRequiredService<ILogger<ExportCommand>>();

        try { return await handler.ExecuteAsync(cancellationToken).ConfigureAwait(false); }
        catch (Exception ex)
        {
            logger.LogError(ex, "An unhandled exception was thrown while executing the operation.");

            return -2;
        }
    }
}
