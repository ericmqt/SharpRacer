using Microsoft.Extensions.Options;
using SharpRacer.Tools.TelemetryVariables.Configuration;

namespace SharpRacer.Tools.TelemetryVariables.Commands;
internal class ConfigureCommandHandler
{
    private readonly DatabaseOptions _databaseOptions;

    public ConfigureCommandHandler(ConfigureCommandOptions options, IOptions<DatabaseOptions> databaseOptions)
    {
        ArgumentNullException.ThrowIfNull(options);
        ArgumentNullException.ThrowIfNull(databaseOptions);

        Options = options;
        _databaseOptions = databaseOptions.Value;
    }

    public ConfigureCommandOptions Options { get; }

    public async Task<int> ExecuteAsync(CancellationToken cancellationToken = default)
    {
        var appConfig = JsonAppConfiguration.Load();

        if (Options.DatabaseFile != null)
        {
            Console.WriteLine($"Current database path: {_databaseOptions.Path}");
            Console.WriteLine($"Setting database path: {Options.DatabaseFile.FullName}");
            _databaseOptions.Path = Options.DatabaseFile.FullName;
        }

        appConfig.DatabaseOptions = _databaseOptions;
        await appConfig.SaveAsync(cancellationToken);

        return 0;
    }
}
