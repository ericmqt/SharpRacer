using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SharpRacer.Tools.TelemetryVariables.CommandLine;
using SharpRacer.Tools.TelemetryVariables.Data;

namespace SharpRacer.Tools.TelemetryVariables.Commands;
internal class CreateDatabaseCommandHandler : ICommandHandler<CreateDatabaseCommandOptions>
{
    private readonly TelemetryVariablesDbContext _dbContext;
    private readonly ILogger<CreateDatabaseCommandHandler> _logger;

    public CreateDatabaseCommandHandler(CreateDatabaseCommandOptions options, TelemetryVariablesDbContext dbContext, ILogger<CreateDatabaseCommandHandler> logger)
    {
        Options = options ?? throw new ArgumentNullException(nameof(options));

        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public CreateDatabaseCommandOptions Options { get; }

    public async Task<int> ExecuteAsync(CancellationToken cancellationToken = default)
    {
        // Create parent directory if it does not exist, otherwise SQLite throws error 14
        if (Options.DatabaseFile.Directory != null && !Options.DatabaseFile.Directory.Exists)
        {
            Options.DatabaseFile.Directory.Create();
        }

        try
        {
            await _dbContext.Database.MigrateAsync(cancellationToken).ConfigureAwait(false);
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
}
