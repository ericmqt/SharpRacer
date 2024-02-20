using System.Runtime.Versioning;
using Microsoft.Extensions.Logging;
using SharpRacer.SessionInfo;
using SharpRacer.SessionInfo.Yaml;
using SharpRacer.Tools.TelemetryVariables.Models;

namespace SharpRacer.Tools.TelemetryVariables.Import;

[SupportedOSPlatform("windows5.1.2600")]
internal class SimulatorSessionImporter
{
    private readonly DataVariableImporter _dataVariableImporter;
    private readonly ILogger<SimulatorSessionImporter> _logger;

    public SimulatorSessionImporter(DataVariableImporter dataVariableImporter, ILogger<SimulatorSessionImporter> logger)
    {
        _dataVariableImporter = dataVariableImporter ?? throw new ArgumentNullException(nameof(dataVariableImporter));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<bool> ImportAsync(bool waitForConnection, CancellationToken cancellationToken = default)
    {
        if (cancellationToken.IsCancellationRequested)
        {
            return false;
        }

        using var connection = new SimulatorConnection();

        if (!await OpenConnectionAsync(connection, waitForConnection, cancellationToken).ConfigureAwait(false))
        {
            return false;
        }

        try
        {
            var variableModels = await ReadDataVariableModelsAsync(connection, cancellationToken).ConfigureAwait(false);

            if (!variableModels.Any())
            {
                Console.WriteLine("No variables found in the simulator session");
                return false;
            }

            var carModel = GetCarModel(connection, variableModels);

            Console.WriteLine($"Variables: {variableModels.Count}");
            Console.WriteLine($"Car: {carModel.Name}");

            // Import variables
            await _dataVariableImporter.ImportAsync(variableModels, cancellationToken).ConfigureAwait(false);

            // Import car and associate it with its variables
            await _dataVariableImporter.ImportCarAsync(carModel, cancellationToken).ConfigureAwait(false);
        }
        catch (OperationCanceledException)
        {
            Console.WriteLine("Operation canceled.");

            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error importing from simulator");

            return false;
        }

        Console.WriteLine($"Imported {_dataVariableImporter.VariablesAddedCount} variable(s) and {_dataVariableImporter.CarsAddedCount} car(s)");
        Console.WriteLine("Import completed");

        return true;
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
            Console.WriteLine("Operation canceled");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error connecting to simulator");
        }

        return false;
    }

    private async Task<IList<DataVariableModel>> ReadDataVariableModelsAsync(SimulatorConnection connection, CancellationToken cancellationToken = default)
    {
        Console.WriteLine("Reading telemetry variables from simulator...");
        await connection.WaitForDataReadyAsync(cancellationToken).ConfigureAwait(false);

        var dataVariables = connection.GetDataVariables();

        return dataVariables.Select(x => new DataVariableModel(x)).ToList();
    }

    private CarModel GetCarModel(SimulatorConnection connection, IEnumerable<DataVariableModel> dataVariableModels)
    {
        var sessionInfo = ReadSessionInfo(connection);

        var sessionInfoModel = SessionInfoDocumentModel.FromYaml(sessionInfo.YamlDocument);
        var driverNode = sessionInfoModel.DriverInfo.Drivers.Single(x => x.CarIdx == sessionInfoModel.DriverInfo.DriverCarIdx);

        return new CarModel(driverNode, dataVariableModels.Select(x => x.Name));
    }

    private SessionInfoDocument ReadSessionInfo(SimulatorConnection connection)
    {
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
