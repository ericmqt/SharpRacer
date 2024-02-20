using Microsoft.Extensions.Logging;
using SharpRacer.SessionInfo.Yaml;
using SharpRacer.Telemetry;
using SharpRacer.Tools.TelemetryVariables.Models;

namespace SharpRacer.Tools.TelemetryVariables.Import;
internal class TelemetryFileImporter
{
    private readonly DataVariableImporter _dataVariableImporter;
    private readonly ILogger<TelemetryFileImporter> _logger;

    public TelemetryFileImporter(DataVariableImporter dataVariableImporter, ILogger<TelemetryFileImporter> logger)
    {
        _dataVariableImporter = dataVariableImporter ?? throw new ArgumentNullException(nameof(dataVariableImporter));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<bool> ImportAsync(FileSystemInfo importFileOrDirectory, bool recurse, CancellationToken cancellationToken = default)
    {
        try
        {
            foreach (var telemetryFile in EnumerateInputFiles(importFileOrDirectory, recurse, cancellationToken))
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    Console.WriteLine("Operation canceled.");

                    return false;
                }

                await ImportFileAsync(telemetryFile, cancellationToken).ConfigureAwait(false);
            }

            Console.WriteLine($"Imported {_dataVariableImporter.VariablesAddedCount} variable(s) and {_dataVariableImporter.CarsAddedCount} car(s)");
            Console.WriteLine("Telemetry variable import completed.");

            return true;
        }
        catch (OperationCanceledException)
        {
            Console.WriteLine("Operation canceled.");

            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error importing telemetry files");

            return false;
        }
    }

    private async Task ImportFileAsync(TelemetryFileInfo telemetryFile, CancellationToken cancellationToken = default)
    {
        Console.WriteLine($"Processing: {telemetryFile.FileName}");

        try
        {
            // Import variables
            var variableModels = telemetryFile.DataVariables.Select(x => new DataVariableModel(x));

            var sessionInfo = SessionInfoDocumentModel.FromYaml(telemetryFile.SessionInfo);
            var driverCar = sessionInfo.DriverInfo.Drivers.Single(x => x.CarIdx == sessionInfo.DriverInfo.DriverCarIdx);

            var carModel = new CarModel(driverCar, variableModels.Select(x => x.Name));

            await _dataVariableImporter.ImportAsync(variableModels, cancellationToken).ConfigureAwait(false);

            // Import car and associate its variables
            await _dataVariableImporter.ImportCarAsync(carModel, cancellationToken).ConfigureAwait(false);
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error importing telemetry file: \"{FileName}\"", telemetryFile.FileName);
        }
    }

    private IEnumerable<TelemetryFileInfo> EnumerateInputFiles(FileSystemInfo importFileOrDirectory, bool recurse, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (importFileOrDirectory is FileInfo inputFile)
        {
            // Throw any file read errors to caller
            yield return new TelemetryFileInfo(inputFile.FullName);
            yield break;
        }

        if (importFileOrDirectory is not DirectoryInfo importDirectory)
        {
            yield break;
        }

        var enumerationOptions = new EnumerationOptions()
        {
            IgnoreInaccessible = true,
            RecurseSubdirectories = recurse
        };

        foreach (var ibtFileInfo in importDirectory.EnumerateFiles("*.ibt", enumerationOptions))
        {
            cancellationToken.ThrowIfCancellationRequested();

            // Tolerate file read errors
            TelemetryFileInfo? telemetryFile = null;

            try { telemetryFile = new TelemetryFileInfo(ibtFileInfo.FullName); }
            catch
            {
                Console.WriteLine($"Unable to read telemetry file: {ibtFileInfo.FullName}");
            }

            if (telemetryFile != null)
            {
                yield return telemetryFile;
            }
        }
    }
}
