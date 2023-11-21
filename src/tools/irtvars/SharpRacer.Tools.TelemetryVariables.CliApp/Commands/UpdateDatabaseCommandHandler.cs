using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using SharpRacer.Tools.TelemetryVariables.Data;

namespace SharpRacer.Tools.TelemetryVariables.Commands;
internal class UpdateDatabaseCommandHandler
{
    public UpdateDatabaseCommandHandler(UpdateDatabaseCommandOptions options)
    {
        Options = options ?? throw new ArgumentNullException(nameof(options));
    }

    public UpdateDatabaseCommandOptions Options { get; }

    public async Task<int> ExecuteAsync(CancellationToken cancellationToken = default)
    {
        if (!Options.DatabaseFile.Exists)
        {
            Console.WriteLine($"Database file does not exist: {Options.DatabaseFile.FullName}");

            return -1;
        }

        using var dbContext = CreateDbContext();

        // Check for pending migrations
        Console.WriteLine("Checking for pending migrations...");
        var migrations = Enumerable.Empty<string>();

        try { migrations = await dbContext.Database.GetPendingMigrationsAsync(cancellationToken).ConfigureAwait(false); }
        catch (OperationCanceledException)
        {
            Console.WriteLine("Database update canceled.");
            return -1;
        }

        if (!migrations.Any())
        {
            Console.WriteLine("Database is up-to-date.");
            return 0;
        }

        // Apply migrations
        Console.WriteLine($"Found {migrations.Count()} pending migrations:");

        foreach (var migration in migrations)
        {
            Console.WriteLine($"\t{migration}");
        }

        Console.WriteLine("Applying migrations...");

        try { await dbContext.Database.MigrateAsync(cancellationToken).ConfigureAwait(false); }
        catch (OperationCanceledException)
        {
            Console.WriteLine("Database update canceled.");
            return -1;
        }

        Console.WriteLine("Database updated successfully.");
        return 0;
    }

    private TelemetryVariablesDbContext CreateDbContext()
    {
        var csb = new SqliteConnectionStringBuilder()
        {
            DataSource = Options.DatabaseFile.FullName
        };

        var builder = new DbContextOptionsBuilder<TelemetryVariablesDbContext>();

        builder.UseSqlite(csb.ConnectionString,
            sqlite =>
            {
                sqlite.MigrationsAssembly("SharpRacer.Tools.TelemetryVariables.Data");
            });

        return new TelemetryVariablesDbContext(builder.Options);
    }
}
