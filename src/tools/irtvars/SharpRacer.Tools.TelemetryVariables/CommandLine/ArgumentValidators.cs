using System.CommandLine;
using System.CommandLine.Parsing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using SharpRacer.Tools.TelemetryVariables.Data;

namespace SharpRacer.Tools.TelemetryVariables.CommandLine;
internal static class ArgumentValidators
{
    internal static void SqliteDbContextFileHasNoPendingMigrations(OptionResult optionResult)
    {
        if (optionResult.Option is not CliOption<FileInfo> databaseFileOption)
        {
            return;
        }

        var databaseFile = optionResult.GetValue(databaseFileOption);

        if (databaseFile is null || !databaseFile.Exists)
        {
            return;
        }

        var csb = new SqliteConnectionStringBuilder()
        {
            DataSource = databaseFile.FullName
        };

        var dbOptionsBuilder = new DbContextOptionsBuilder<TelemetryVariablesDbContext>()
            .UseSqlite(
                csb.ConnectionString,
                sqlite =>
                {
                    sqlite.MigrationsAssembly("SharpRacer.Tools.TelemetryVariables.Data");
                });

        using var dbContext = new TelemetryVariablesDbContext(dbOptionsBuilder.Options);

        // Check for pending migrations
        try
        {
            if (dbContext.Database.GetPendingMigrations().Any())
            {
                optionResult.AddError("Database must be updated before operation can proceed. See command: database migrate");
            }
        }
        catch (SqliteException sqliteEx)
        {
            optionResult.AddError(sqliteEx.Message);
        }
    }
}
