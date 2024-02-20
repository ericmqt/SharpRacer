using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SharpRacer.Tools.TelemetryVariables.CommandLine;
using SharpRacer.Tools.TelemetryVariables.Data;

namespace SharpRacer.Tools.TelemetryVariables.Commands;
internal class MigrateDatabaseCommandHandler : ICommandHandler
{
    private readonly TelemetryVariablesDbContext _dbContext;
    private readonly ILogger<MigrateDatabaseCommandHandler> _logger;

    public MigrateDatabaseCommandHandler(TelemetryVariablesDbContext dbContext, ILogger<MigrateDatabaseCommandHandler> logger)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<int> ExecuteAsync(CancellationToken cancellationToken = default)
    {
        // Check for pending migrations
        Console.WriteLine("Checking for pending migrations...");
        IEnumerable<string> migrations;

        try { migrations = await _dbContext.Database.GetPendingMigrationsAsync(cancellationToken).ConfigureAwait(false); }
        catch (OperationCanceledException)
        {
            Console.WriteLine("Database migration canceled.");
            return -1;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while examining database for pending migration.");
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

        try { await _dbContext.Database.MigrateAsync(cancellationToken).ConfigureAwait(false); }
        catch (OperationCanceledException)
        {
            Console.WriteLine("Database migration canceled.");
            return -1;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while migrating the database.");
            return -1;
        }

        Console.WriteLine("Database migrated successfully.");
        return 0;
    }
}
