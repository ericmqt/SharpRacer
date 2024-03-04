using System.CommandLine;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SharpRacer.Tools.TelemetryVariables.CommandLine;

namespace SharpRacer.Tools.TelemetryVariables.Commands;
internal class MigrateDatabaseCommand : CliCommand<MigrateDatabaseCommandHandler>, IConfigureDbContextCommand
{
    public MigrateDatabaseCommand()
        : base("migrate", "Migrates a telemetry variables database schema to the latest version.")
    {
        DatabaseFileArgument = new CliArgument<FileInfo>("database-file")
        {
            Arity = ArgumentArity.ExactlyOne,
            Description = $"The path of the database file to migrate.",
        };

        DatabaseFileArgument.AcceptExistingOnly();

        Arguments.Add(DatabaseFileArgument);
    }

    public CliArgument<FileInfo> DatabaseFileArgument { get; }

    public void ConfigureDbContext(DbContextOptionsBuilder builder, ParseResult parseResult, IServiceProvider serviceProvider)
    {
        var databaseFile = parseResult.GetValue(DatabaseFileArgument)!;

        var csb = new SqliteConnectionStringBuilder()
        {
            DataSource = databaseFile.FullName
        };

        builder.UseSqlite(csb.ConnectionString);
    }

    protected override async Task<int> InvokeAsync(
        MigrateDatabaseCommandHandler handler,
        IServiceProvider serviceProvider,
        CancellationToken cancellationToken)
    {
        var logger = serviceProvider.GetRequiredService<ILogger<MigrateDatabaseCommand>>();

        try { return await handler.ExecuteAsync(cancellationToken).ConfigureAwait(false); }
        catch (Exception ex)
        {
            logger.LogError(ex, "An unhandled exception was thrown while executing the operation.");

            return -2;
        }
    }
}
