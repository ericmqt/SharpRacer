﻿using System.Text.Json;
using Microsoft.Extensions.Logging;
using SharpRacer.Tools.TelemetryVariables.CommandLine;
using SharpRacer.Tools.TelemetryVariables.Json;
using SharpRacer.Tools.TelemetryVariables.Data.Stores;
using SharpRacer.Tools.TelemetryVariables.Models;

namespace SharpRacer.Tools.TelemetryVariables.Commands;
internal class ExportCommandHandler : ICommandHandler<ExportCommandOptions>
{
    private readonly ICarStore _carStore;
    private readonly ICarVariableStore _carVariableStore;
    private readonly ILogger<ExportCommandHandler> _logger;
    private readonly IVariableStore _variableStore;

    public ExportCommandHandler(
        ExportCommandOptions options,
        IVariableStore variableStore,
        ICarStore carStore,
        ICarVariableStore carVariableStore,
        ILogger<ExportCommandHandler> logger)
    {
        Options = options ?? throw new ArgumentNullException(nameof(options));

        _variableStore = variableStore ?? throw new ArgumentNullException(nameof(variableStore));
        _carStore = carStore ?? throw new ArgumentNullException(nameof(carStore));
        _carVariableStore = carVariableStore ?? throw new ArgumentNullException(nameof(carVariableStore));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public ExportCommandOptions Options { get; }

    public async Task<int> ExecuteAsync(CancellationToken cancellationToken = default)
    {
        var exportFileInfo = GetExportFileInfo();

        var exportTask = Options.ExportVariablesOnly
            ? ExportVariablesAsync(exportFileInfo, cancellationToken)
            : ExportAllAsync(exportFileInfo, cancellationToken);

        try
        {
            await exportTask.ConfigureAwait(false);

            return 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error exporting variables database.");

            return -1;
        }
    }

    private async Task ExportAllAsync(FileInfo exportFile, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(exportFile);

        var variableModels = await GetVariableModelsAsync(cancellationToken).ConfigureAwait(false);
        var carModels = await GetCarModelsAsync(cancellationToken).ConfigureAwait(false);

        var documentModel = new TelemetryVariablesDataModel()
        {
            Cars = carModels.ToList(),
            Variables = variableModels.ToList()
        };

        using var exportFileStream = exportFile.Create();

        await JsonSerializer.SerializeAsync(
            exportFileStream,
            documentModel,
            JsonSerializerConfiguration.SerializerOptions,
            cancellationToken)
            .ConfigureAwait(false);
    }

    private async Task ExportVariablesAsync(FileInfo exportFile, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(exportFile);

        var variableModels = await GetVariableModelsAsync(cancellationToken).ConfigureAwait(false);

        using var exportFileStream = exportFile.Create();

        await JsonSerializer.SerializeAsync(
            exportFileStream,
            variableModels,
            JsonSerializerConfiguration.SerializerOptions,
            cancellationToken)
            .ConfigureAwait(false);
    }

    private async Task<IEnumerable<CarModel>> GetCarModelsAsync(CancellationToken cancellationToken = default)
    {
        var results = new List<CarModel>();

        var carEntities = await _carStore.ListAsync(cancellationToken).ConfigureAwait(false);

        foreach (var car in carEntities)
        {
            var carVariableNames = await _carVariableStore.GetVariableNamesAsync(car, cancellationToken).ConfigureAwait(false);

            results.Add(new CarModel(car) { VariableNames = carVariableNames });
        }

        return results;
    }

    private async Task<IEnumerable<DataVariableModel>> GetVariableModelsAsync(CancellationToken cancellationToken = default)
    {
        // Order session variables first in the resulting collection
        var sessionVariables = await _variableStore.ListSessionVariablesAsync(cancellationToken).ConfigureAwait(false);
        var carDependentVariables = await _variableStore.ListCarDependentVariablesAsync(cancellationToken).ConfigureAwait(false);

        return sessionVariables.Concat(carDependentVariables).Select(x => new DataVariableModel(x));
    }

    private FileInfo GetExportFileInfo()
    {
        if (Options.OutputFileOrDirectory is DirectoryInfo outputDirectory)
        {
            return new FileInfo(Path.Combine(outputDirectory.FullName, GetDefaultExportFileName()));
        }
        else if (Options.OutputFileOrDirectory is FileInfo outputFile)
        {
            return outputFile;
        }

        throw new InvalidOperationException(
            $"{nameof(Options)}.{nameof(Options.OutputFileOrDirectory)} is neither {nameof(DirectoryInfo)} nor {nameof(FileInfo)}.");
    }

    private string GetDefaultExportFileName()
    {
        if (Options.ExportVariablesOnly)
        {
            return "TelemetryVariables.json";
        }

        return "TelemetryVariablesDb.json";
    }
}
