using System.Text.Json;
using Microsoft.Extensions.Logging;
using SharpRacer.Tools.TelemetryVariables.CommandLine;
using SharpRacer.Tools.TelemetryVariables.Json;
using SharpRacer.Tools.TelemetryVariables.Import;
using SharpRacer.Tools.TelemetryVariables.Models;

namespace SharpRacer.Tools.TelemetryVariables.Commands;
internal sealed class ImportJsonCommandHandler : ICommandHandler<ImportJsonCommandOptions>
{
    private readonly CarImporter _carImporter;
    private readonly ILogger<ImportJsonCommandHandler> _logger;
    private readonly VariableImporter _variableImporter;

    public ImportJsonCommandHandler(
        ImportJsonCommandOptions options,
        VariableImporter variableImporter,
        CarImporter carImporter,
        ILogger<ImportJsonCommandHandler> logger)
    {
        Options = options ?? throw new ArgumentNullException(nameof(options));

        _variableImporter = variableImporter ?? throw new ArgumentNullException(nameof(variableImporter));
        _carImporter = carImporter ?? throw new ArgumentNullException(nameof(carImporter));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public ImportJsonCommandOptions Options { get; }

    public async Task<int> ExecuteAsync(CancellationToken cancellationToken = default)
    {
        TelemetryVariablesDataModel document;

        try { document = await ReadInputFileAsync(cancellationToken).ConfigureAwait(false); }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error reading input file");

            return -1;
        }

        try
        {
            await _variableImporter.ImportVariablesAsync(document.Variables, cancellationToken).ConfigureAwait(false);

            foreach (var car in document.Cars)
            {
                await _carImporter.ImportAsync(car, cancellationToken).ConfigureAwait(false);
            }
        }
        catch (OperationCanceledException)
        {
            Console.WriteLine("Import canceled");

            return -1;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error importing data");

            return -1;
        }

        Console.WriteLine("Import completed");

        return 0;
    }

    private async Task<TelemetryVariablesDataModel> ReadInputFileAsync(CancellationToken cancellationToken = default)
    {
        using var inputFileStream = Options.InputFile.OpenRead();

        var document = await JsonSerializer.DeserializeAsync<TelemetryVariablesDataModel>(
            inputFileStream,
            JsonSerializerConfiguration.SerializerOptions,
            cancellationToken)
            .ConfigureAwait(false);

        if (document is null)
        {
            throw new Exception("Input file deserialized as null.");
        }

        return document;
    }
}
