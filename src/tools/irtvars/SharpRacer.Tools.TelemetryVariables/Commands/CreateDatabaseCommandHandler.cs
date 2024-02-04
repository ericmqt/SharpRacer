using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SharpRacer.Tools.TelemetryVariables.Data;

namespace SharpRacer.Tools.TelemetryVariables.Commands;
internal class CreateDatabaseCommandHandler
{
    private readonly ILogger<CreateDatabaseCommandHandler> _logger;

    public CreateDatabaseCommandHandler(CreateDatabaseCommandOptions options, ILogger<CreateDatabaseCommandHandler> logger)
    {
        Options = options ?? throw new ArgumentNullException(nameof(options));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public CreateDatabaseCommandOptions Options { get; }

    public async Task<int> ExecuteAsync(CancellationToken cancellationToken = default)
    {
        if (Options.DatabaseFile.Exists)
        {
            Console.WriteLine($"Database already exists: {Options.DatabaseFile.FullName}");

            return -1;
        }

        // Create parent directory if it does not exist, otherwise SQLite throws error 14
        if (Options.DatabaseFile.Directory != null && !Options.DatabaseFile.Directory.Exists)
        {
            Options.DatabaseFile.Directory.Create();
        }

        try
        {
            using var dbContext = CreateDbContext();

            await dbContext.Database.MigrateAsync(cancellationToken).ConfigureAwait(false);
        }
        catch (OperationCanceledException)
        {
            Console.WriteLine($"Database creation canceled.");

            File.Delete(Options.DatabaseFile.FullName);

            return -1;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating database");

            return -1;
        }

        Console.WriteLine($"Created database: {Options.DatabaseFile.FullName}");
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
