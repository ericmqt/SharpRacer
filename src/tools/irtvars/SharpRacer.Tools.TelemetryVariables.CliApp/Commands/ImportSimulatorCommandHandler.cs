using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using System.Text;
using Microsoft.Extensions.Logging;
using SharpRacer.Interop;
using SharpRacer.SessionInfo;
using SharpRacer.SessionInfo.Yaml;
using SharpRacer.Tools.TelemetryVariables.Models;
using SharpRacer.Tools.TelemetryVariables.Services;

namespace SharpRacer.Tools.TelemetryVariables.Commands;

[SupportedOSPlatform("windows5.1.2600")]
internal class ImportSimulatorCommandHandler
{
    private readonly DataVariableImporter _dataVariableImporter;
    private readonly ILogger<ImportSimulatorCommandHandler> _logger;

    public ImportSimulatorCommandHandler(DataVariableImporter dataVariableImporter, ILogger<ImportSimulatorCommandHandler> logger)
    {
        _dataVariableImporter = dataVariableImporter ?? throw new ArgumentNullException(nameof(dataVariableImporter));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<int> ExecuteAsync(CancellationToken cancellationToken)
    {
        using var connection = new SimulatorConnection();
        connection.StateChanged += (sender, e) => Console.WriteLine($"ConnectionState: {e.NewState}");

        Console.WriteLine("Waiting for simulator...");

        try
        {
            await connection.OpenAsync(cancellationToken).ConfigureAwait(false);
        }
        catch (OperationCanceledException)
        {
            Console.WriteLine("Import canceled.");
            return -1;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error connecting to simulator");
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

        var header = MemoryMarshal.Read<DataFileHeader>(connection.Data);
        var version = header.SessionInfoVersion;

        var sessionInfoSpan = connection.Data.Slice(header.SessionInfoOffset, header.SessionInfoLength);
        var sessionInfoString = Encoding.Latin1.GetString(sessionInfoSpan);

        // Check the version hasn't changed
        var checkVersion = MemoryMarshal.Read<int>(connection.Data.Slice(DataFileHeader.FieldOffsets.SessionInfoVersion, sizeof(int)));

        if (checkVersion != version)
        {
            // Try again
            return ReadSessionInfo(connection);
        }

        return new SessionInfoDocument(sessionInfoString, version);
    }


}
