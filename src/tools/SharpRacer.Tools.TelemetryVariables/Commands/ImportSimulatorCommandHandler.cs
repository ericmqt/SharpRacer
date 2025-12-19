using System.Runtime.Versioning;
using Microsoft.Extensions.Logging;
using SharpRacer.Interop;
using SharpRacer.SessionInfo;
using SharpRacer.SessionInfo.Yaml;
using SharpRacer.Tools.TelemetryVariables.CommandLine;
using SharpRacer.Tools.TelemetryVariables.Import;
using SharpRacer.Tools.TelemetryVariables.Models;

namespace SharpRacer.Tools.TelemetryVariables.Commands;

[SupportedOSPlatform("windows5.1.2600")]
internal sealed class ImportSimulatorCommandHandler : ICommandHandler<ImportSimulatorCommandOptions>
{
    private readonly CarImporter _carImporter;
    private readonly ILogger<ImportSimulatorCommandHandler> _logger;
    private readonly VariableImporter _variableImporter;

    public ImportSimulatorCommandHandler(
        ImportSimulatorCommandOptions options,
        VariableImporter variableImporter,
        CarImporter carImporter,
        ILogger<ImportSimulatorCommandHandler> logger)
    {
        Options = options ?? throw new ArgumentNullException(nameof(options));

        _variableImporter = variableImporter ?? throw new ArgumentNullException(nameof(variableImporter));
        _carImporter = carImporter ?? throw new ArgumentNullException(nameof(carImporter));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public ImportSimulatorCommandOptions Options { get; }

    public async Task<int> ExecuteAsync(CancellationToken cancellationToken)
    {
        using var connection = new SimulatorConnection();

        if (!await OpenConnectionAsync(connection, Options.WaitForConnection, cancellationToken).ConfigureAwait(false))
        {
            return -1;
        }

        try
        {
            Console.WriteLine("Reading telemetry variables from simulator...");
            await connection.WaitForDataReadyAsync(cancellationToken).ConfigureAwait(false);

            // Read session info
            var sessionInfo = ReadSessionInfo(connection);
            var sessionInfoModel = SessionInfoDocumentModel.FromYaml(sessionInfo.YamlDocument);

            // Import variables
            var variableModels = connection.DataVariables.Select(x => new DataVariableModel(x, sessionInfoModel.WeekendInfo.BuildVersion));

            await _variableImporter.ImportVariablesAsync(variableModels, cancellationToken).ConfigureAwait(false);

            // Import car and associate it with its variables
            var driverCar = sessionInfoModel.DriverInfo.Drivers.Single(x => x.CarIdx == sessionInfoModel.DriverInfo.DriverCarIdx);

            var carModel = new CarModel(driverCar, variableModels.Select(x => x.Name), sessionInfoModel.DriverInfo.DriverCarVersion);

            await _carImporter.ImportAsync(carModel, cancellationToken).ConfigureAwait(false);
        }
        catch (OperationCanceledException)
        {
            Console.WriteLine("Import canceled");

            return -1;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error importing from simulator");

            return -1;
        }

        Console.WriteLine("Import completed");

        return 0;
    }

    private async Task<bool> OpenConnectionAsync(
        SimulatorConnection simulatorConnection,
        bool waitForConnection,
        CancellationToken cancellationToken = default)
    {
        try
        {
            if (waitForConnection)
            {
                Console.WriteLine("Waiting for simulator...");
                await simulatorConnection.OpenAsync(cancellationToken).ConfigureAwait(false);
            }
            else
            {
                await simulatorConnection.OpenAsync(TimeSpan.FromMilliseconds(64), cancellationToken).ConfigureAwait(false);
            }

            Console.WriteLine("Connected to simulator");
            return true;
        }
        catch (TimeoutException)
        {
            Console.WriteLine("Simulator is not running");
        }
        catch (OperationCanceledException)
        {
            Console.WriteLine("Import canceled");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error connecting to simulator");
        }

        return false;
    }

    private static SessionInfoDocument ReadSessionInfo(SimulatorConnection connection)
    {
        if (connection.State != SimulatorConnectionState.Open)
        {
            throw new InvalidOperationException("The connection is not open.");
        }

        var sessionInfoVersion = ReadSessionInfoVersion(connection);
        var sessionInfoString = ReadSessionInfoString(connection);

        // Check the version hasn't changed
        if (sessionInfoVersion != ReadSessionInfoVersion(connection))
        {
            // Try again
            return ReadSessionInfo(connection);
        }

        return new SessionInfoDocument(sessionInfoString, sessionInfoVersion);
    }

    private static string ReadSessionInfoString(ISimulatorConnection connection)
    {
        return SessionInfoString.Read(connection);
    }

    private static int ReadSessionInfoVersion(ISimulatorConnection connection)
    {
        return SessionInfoString.ReadVersion(connection);
    }
}
