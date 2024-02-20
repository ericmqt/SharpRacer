using System.Runtime.Versioning;
using Microsoft.Extensions.Logging;
using SharpRacer.SessionInfo;
using SharpRacer.SessionInfo.Yaml;
using SharpRacer.Tools.TelemetryVariables.CommandLine;
using SharpRacer.Tools.TelemetryVariables.Import;
using SharpRacer.Tools.TelemetryVariables.Models;

namespace SharpRacer.Tools.TelemetryVariables.Commands;

[SupportedOSPlatform("windows5.1.2600")]
internal sealed class ImportSimulatorCommandHandler : ICommandHandler<ImportSimulatorCommandOptions>
{
    private readonly DataVariableImporter _dataVariableImporter;
    private readonly ILogger<ImportSimulatorCommandHandler> _logger;

    public ImportSimulatorCommandHandler(
        ImportSimulatorCommandOptions options,
        DataVariableImporter dataVariableImporter,
        ILogger<ImportSimulatorCommandHandler> logger)
    {
        Options = options ?? throw new ArgumentNullException(nameof(options));

        _dataVariableImporter = dataVariableImporter ?? throw new ArgumentNullException(nameof(dataVariableImporter));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public ImportSimulatorCommandOptions Options { get; }

    public async Task<int> ExecuteAsync(CancellationToken cancellationToken)
    {
        using var connection = new SimulatorConnection();
        connection.StateChanged += (sender, e) => Console.WriteLine($"ConnectionState: {e.NewState}");

        try
        {
            if (Options.WaitForConnection)
            {
                Console.WriteLine("Waiting for simulator...");
                await connection.OpenAsync(cancellationToken).ConfigureAwait(false);
            }
            else
            {
                await connection.OpenAsync(TimeSpan.FromMilliseconds(64), cancellationToken).ConfigureAwait(false);
            }
        }
        catch (TimeoutException)
        {
            Console.WriteLine("Simulator is not running.");

            return 0;
        }
        catch (OperationCanceledException)
        {
            return 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error connecting to the simulator");

            return -1;
        }

        try
        {
            await connection.WaitForDataReadyAsync(cancellationToken).ConfigureAwait(false);

            var simulatorVariables = connection.GetDataVariables();
            var sessionInfo = ReadSessionInfo(connection);

            Console.WriteLine($"Importing {simulatorVariables.Count()} variables...");

            // Import variables
            var variableModels = simulatorVariables.Select(x => new DataVariableModel(x));

            await _dataVariableImporter.ImportAsync(variableModels, cancellationToken).ConfigureAwait(false);

            // Import car and associate it with its variables
            var sessionInfoModel = SessionInfoDocumentModel.FromYaml(sessionInfo.YamlDocument);
            var driverCar = sessionInfoModel.DriverInfo.Drivers.Single(x => x.CarIdx == sessionInfoModel.DriverInfo.DriverCarIdx);

            var carModel = new CarModel(driverCar, variableModels.Select(x => x.Name));

            await _dataVariableImporter.ImportCarAsync(carModel, cancellationToken).ConfigureAwait(false);
        }
        catch (OperationCanceledException)
        {
            Console.WriteLine("Operation canceled.");

            return -1;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error importing from simulator");

            return -1;
        }

        Console.WriteLine($"Imported {_dataVariableImporter.VariablesAddedCount} variable(s) and {_dataVariableImporter.CarsAddedCount} car(s)");
        Console.WriteLine("Import completed");

        return 0;
    }

    private SessionInfoDocument ReadSessionInfo(SimulatorConnection connection)
    {
        if (connection.State != SimulatorConnectionState.Open)
        {
            throw new InvalidOperationException("The connection is not open.");
        }

        var reader = new SimulatorDataReader(connection);

        var sessionInfoVersion = reader.ReadSessionInfoVersion();
        var sessionInfoString = reader.ReadSessionInfo();

        // Check the version hasn't changed
        if (sessionInfoVersion != reader.ReadSessionInfoVersion())
        {
            // Try again
            return ReadSessionInfo(connection);
        }

        return new SessionInfoDocument(sessionInfoString, sessionInfoVersion);
    }


}
