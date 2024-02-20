using Microsoft.Extensions.Logging;
using SharpRacer.SessionInfo.Yaml;
using SharpRacer.Telemetry;
using SharpRacer.Tools.TelemetryVariables.CommandLine;
using SharpRacer.Tools.TelemetryVariables.Import;
using SharpRacer.Tools.TelemetryVariables.Models;

namespace SharpRacer.Tools.TelemetryVariables.Commands;
internal class ImportTelemetryCommandHandler : ICommandHandler<ImportTelemetryCommandOptions>
{
    private readonly DataVariableImporter _dataVariableImporter;
    private readonly ILogger<ImportTelemetryCommandHandler> _logger;

    public ImportTelemetryCommandHandler(
        ImportTelemetryCommandOptions options,
        DataVariableImporter dataVariableImporter,
        ILogger<ImportTelemetryCommandHandler> logger)
    {
        Options = options ?? throw new ArgumentNullException(nameof(options));

        _dataVariableImporter = dataVariableImporter ?? throw new ArgumentNullException(nameof(dataVariableImporter));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public ImportTelemetryCommandOptions Options { get; }

    public async Task<int> ExecuteAsync(CancellationToken cancellationToken)
    {
        IEnumerable<TelemetryFileInfo> telemetryFiles;

        try { telemetryFiles = EnumerateInputFiles(cancellationToken); }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error reading input file(s).");
            return -1;
        }

        foreach (var telemetryFile in telemetryFiles)
        {
            Console.WriteLine($"Processing: {telemetryFile.FileName}");

            if (cancellationToken.IsCancellationRequested)
            {
                Console.WriteLine("Import canceled.");
                return -1;
            }

            try
            {
                // Import variables
                var variableModels = telemetryFile.DataVariables.Select(x => new DataVariableModel(x));

                await _dataVariableImporter.ImportAsync(variableModels, cancellationToken).ConfigureAwait(false);

                // Import car and associate its variables
                var sessionInfo = SessionInfoDocumentModel.FromYaml(telemetryFile.SessionInfo);
                var driverCar = sessionInfo.DriverInfo.Drivers.Single(x => x.CarIdx == sessionInfo.DriverInfo.DriverCarIdx);

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
                _logger.LogError(ex, "Error importing telemetry file: \"{FileName}\"", telemetryFile.FileName);

                return -1;
            }
        }

        Console.WriteLine($"Imported {_dataVariableImporter.VariablesAddedCount} variable(s) and {_dataVariableImporter.CarsAddedCount} car(s)");
        Console.WriteLine("Telemetry variable import completed.");

        return 0;
    }

    private IEnumerable<TelemetryFileInfo> EnumerateInputFiles(CancellationToken cancellationToken)
    {
        if (cancellationToken.IsCancellationRequested)
        {
            yield break;
        }

        if (Options.InputFileOrDirectory is DirectoryInfo inputDir)
        {
            var enumerationOptions = new EnumerationOptions()
            {
                IgnoreInaccessible = true,
                RecurseSubdirectories = Options.EnumerateInputFilesRecursively
            };

            foreach (var ibtFileInfo in inputDir.EnumerateFiles("*.ibt", enumerationOptions))
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    yield break;
                }

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
        else if (Options.InputFileOrDirectory is FileInfo inputFile)
        {
            // Throw any file read errors to caller
            yield return new TelemetryFileInfo(inputFile.FullName);
        }
    }
}
