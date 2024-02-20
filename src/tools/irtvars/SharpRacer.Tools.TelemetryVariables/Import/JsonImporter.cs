using System.Text.Json;
using Microsoft.Extensions.Logging;
using SharpRacer.Tools.TelemetryVariables.Configuration;
using SharpRacer.Tools.TelemetryVariables.Models;

namespace SharpRacer.Tools.TelemetryVariables.Import;
internal class JsonImporter
{
    private readonly DataVariableImporter _dataVariableImporter;
    private readonly ILogger<JsonImporter> _logger;

    public JsonImporter(DataVariableImporter dataVariableImporter, ILogger<JsonImporter> logger)
    {
        _dataVariableImporter = dataVariableImporter ?? throw new ArgumentNullException(nameof(dataVariableImporter));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<bool> ImportAsync(FileInfo jsonFile, CancellationToken cancellationToken = default)
    {
        var document = await ReadInputFileAsync(jsonFile, cancellationToken).ConfigureAwait(false);

        if (document is null)
        {
            _logger.LogError("Failed to read input file.");

            return false;
        }

        await _dataVariableImporter.ImportAsync(document.Variables, cancellationToken).ConfigureAwait(false);

        foreach (var car in document.Cars)
        {
            cancellationToken.ThrowIfCancellationRequested();

            await _dataVariableImporter.ImportCarAsync(car, cancellationToken).ConfigureAwait(false);
        }

        Console.WriteLine($"Imported {_dataVariableImporter.VariablesAddedCount} variable(s) and {_dataVariableImporter.CarsAddedCount} car(s)");
        Console.WriteLine("Telemetry variable import completed.");

        return true;
    }

    private async Task<TelemetryVariablesDataModel?> ReadInputFileAsync(FileInfo jsonFile, CancellationToken cancellationToken = default)
    {
        using var inputFileStream = jsonFile.OpenRead();

        var document = await JsonSerializer.DeserializeAsync<TelemetryVariablesDataModel>(
            inputFileStream,
            JsonSerializerConfiguration.SerializerOptions,
            cancellationToken)
            .ConfigureAwait(false);

        return document;
    }
}
