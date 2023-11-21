using System.Text.Json;
using Microsoft.Extensions.Logging;
using SharpRacer.Tools.TelemetryVariables.Configuration;
using SharpRacer.Tools.TelemetryVariables.Data;
using SharpRacer.Tools.TelemetryVariables.Models;

namespace SharpRacer.Tools.TelemetryVariables.Commands;
internal class ExportCommandHandler
{
    private readonly ICarManager _carManager;
    private readonly ILogger<ExportCommandHandler> _logger;
    private readonly IVariableManager _variableManager;

    public ExportCommandHandler(
        ExportCommandOptions options,
        IVariableManager variableManager,
        ICarManager carManager,
        ILogger<ExportCommandHandler> logger)
    {
        Options = options ?? throw new ArgumentNullException(nameof(options));

        _variableManager = variableManager ?? throw new ArgumentNullException(nameof(variableManager));
        _carManager = carManager ?? throw new ArgumentNullException(nameof(carManager));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public ExportCommandOptions Options { get; }

    public async Task<int> ExecuteAsync(CancellationToken cancellationToken = default)
    {
        var variableModels = await GetVariableModelsAsync(cancellationToken).ConfigureAwait(false);
        var carModels = await GetCarModelsAsync(cancellationToken).ConfigureAwait(false);

        var exportFileInfo = GetExportFileInfo();

        using (var exportFileStream = exportFileInfo.Create())
        {
            var documentModel = new TelemetryVariablesDataModel()
            {
                Cars = carModels.ToList(),
                Variables = variableModels.ToList()
            };

            await JsonSerializer.SerializeAsync(exportFileStream, documentModel, JsonSerializerConfiguration.SerializerOptions, cancellationToken).ConfigureAwait(false);
        }

        return 0;
    }

    private async Task<IEnumerable<CarModel>> GetCarModelsAsync(CancellationToken cancellationToken = default)
    {
        var results = new List<CarModel>();

        var carEntities = await _carManager.ListAsync(cancellationToken).ConfigureAwait(false);

        foreach (var car in carEntities)
        {
            var carVariableNames = await _carManager.GetVariableNamesAsync(car, cancellationToken).ConfigureAwait(false);

            results.Add(new CarModel(car) { VariableNames = carVariableNames });
        }

        return results;
    }

    private async Task<IEnumerable<DataVariableModel>> GetVariableModelsAsync(CancellationToken cancellationToken = default)
    {
        var sessionVariables = await _variableManager.GetSessionVariablesAsync(cancellationToken).ConfigureAwait(false);

        var carDependentVariables = await _variableManager.GetCarDependentVariablesAsync(cancellationToken).ConfigureAwait(false);

        return sessionVariables.Concat(carDependentVariables).Select(x => new DataVariableModel(x));
    }

    private FileInfo GetExportFileInfo()
    {
        if (Options.OutputFileOrDirectory is DirectoryInfo outputDirectory)
        {
            return new FileInfo(Path.Combine(outputDirectory.FullName, "TelemetryVariables.json"));
        }
        else if (Options.OutputFileOrDirectory is FileInfo outputFile)
        {
            return outputFile;
        }

        throw new InvalidOperationException(
            $"{nameof(Options)}.{nameof(Options.OutputFileOrDirectory)} is neither {nameof(DirectoryInfo)} nor {nameof(FileInfo)}.");
    }
}
