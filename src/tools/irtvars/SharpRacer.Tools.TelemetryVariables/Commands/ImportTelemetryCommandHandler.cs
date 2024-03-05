using Microsoft.Extensions.Logging;
using SharpRacer.SessionInfo.Yaml;
using SharpRacer.Telemetry;
using SharpRacer.Tools.TelemetryVariables.CommandLine;
using SharpRacer.Tools.TelemetryVariables.Import;
using SharpRacer.Tools.TelemetryVariables.Models;

namespace SharpRacer.Tools.TelemetryVariables.Commands;
internal class ImportTelemetryCommandHandler : ICommandHandler<ImportTelemetryCommandOptions>
{
    private readonly CarImporter _carImporter;
    private readonly ILogger<ImportTelemetryCommandHandler> _logger;
    private readonly VariableImporter _variableImporter;

    public ImportTelemetryCommandHandler(
        ImportTelemetryCommandOptions options,
        VariableImporter variableImporter,
        CarImporter carImporter,
        ILogger<ImportTelemetryCommandHandler> logger)
    {
        Options = options ?? throw new ArgumentNullException(nameof(options));

        _variableImporter = variableImporter ?? throw new ArgumentNullException(nameof(variableImporter));
        _carImporter = carImporter ?? throw new ArgumentNullException(nameof(carImporter));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public ImportTelemetryCommandOptions Options { get; }

    public async Task<int> ExecuteAsync(CancellationToken cancellationToken)
    {
        IEnumerable<TelemetryFileInfo> telemetryFiles;

        try { telemetryFiles = EnumerateInputFiles(cancellationToken); }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error reading input file(s)");
            return -1;
        }

        if (!telemetryFiles.Any())
        {
            Console.WriteLine("No telemetry files (*.IBT) were found");
            return 0;
        }

        foreach (var telemetryFile in telemetryFiles)
        {
            Console.WriteLine($"Importing: {telemetryFile.FileName}");

            try { await ImportTelemetryFileAsync(telemetryFile, cancellationToken).ConfigureAwait(false); }
            catch (OperationCanceledException)
            {
                Console.WriteLine("Import canceled");

                return -1;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error importing telemetry file: \"{FileName}\"", telemetryFile.FileName);

                return -1;
            }
        }

        Console.WriteLine("Import complete");

        return 0;
    }

    private async Task ImportTelemetryFileAsync(TelemetryFileInfo telemetryFile, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var sessionInfo = SessionInfoDocumentModel.FromYaml(telemetryFile.SessionInfo);

        // Import variables
        var variableModels = telemetryFile.DataVariables.Select(x => new DataVariableModel(x, sessionInfo.WeekendInfo.BuildVersion));

        await _variableImporter.ImportVariablesAsync(variableModels, cancellationToken).ConfigureAwait(false);

        // Import car and associate its variables
        var driverCar = sessionInfo.DriverInfo.Drivers.Single(x => x.CarIdx == sessionInfo.DriverInfo.DriverCarIdx);

        var carVersion = sessionInfo.DriverInfo.DriverCarVersion;
        var carModel = new CarModel(driverCar, variableModels.Select(x => x.Name), carVersion);

        await _carImporter.ImportAsync(carModel, cancellationToken).ConfigureAwait(false);
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
            TelemetryFileInfo? telemetryFile = null;

            try { telemetryFile = new TelemetryFileInfo(inputFile.FullName); }
            catch
            {
                Console.WriteLine($"Unable to read telemetry file: {inputFile.FullName}");
            }

            if (telemetryFile != null)
            {
                yield return telemetryFile;
            }
        }
    }
}
