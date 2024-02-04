using System.Text.Json;
using SharpRacer.Tools.TelemetryVariables.Configuration;
using SharpRacer.Tools.TelemetryVariables.Models;
using SharpRacer.Tools.TelemetryVariables.Services;

namespace SharpRacer.Tools.TelemetryVariables.Commands;
internal class ImportJsonCommandHandler
{
    private readonly DataVariableImporter _variableImporter;

    public ImportJsonCommandHandler(ImportJsonCommandOptions options, DataVariableImporter variableImporter)
    {
        Options = options ?? throw new ArgumentNullException(nameof(options));

        _variableImporter = variableImporter ?? throw new ArgumentNullException(nameof(variableImporter));
    }

    public ImportJsonCommandOptions Options { get; }

    public async Task<int> ExecuteAsync(CancellationToken cancellationToken = default)
    {
        var document = await ReadInputFileAsync(cancellationToken).ConfigureAwait(false);

        if (document is null)
        {
            Console.WriteLine("Input file returned null.");
            return -1;
        }

        await _variableImporter.ImportAsync(document.Variables, cancellationToken).ConfigureAwait(false);

        foreach (var car in document.Cars)
        {
            await _variableImporter.ImportCarAsync(car, cancellationToken).ConfigureAwait(false);
        }

        Console.WriteLine($"Imported {_variableImporter.VariablesAddedCount} variable(s) and {_variableImporter.CarsAddedCount} car(s)");
        Console.WriteLine("Telemetry variable import completed.");

        return 0;
    }

    private async Task<TelemetryVariablesDataModel?> ReadInputFileAsync(CancellationToken cancellationToken = default)
    {
        using var inputFileStream = Options.InputFile.OpenRead();

        var document = await JsonSerializer.DeserializeAsync<TelemetryVariablesDataModel>(
            inputFileStream,
            JsonSerializerConfiguration.SerializerOptions,
            cancellationToken)
            .ConfigureAwait(false);

        return document;
    }
}
