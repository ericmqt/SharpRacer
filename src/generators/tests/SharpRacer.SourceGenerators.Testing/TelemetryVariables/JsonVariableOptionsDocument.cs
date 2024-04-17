using System.Collections.Immutable;
using SharpRacer.SourceGenerators.TelemetryVariables.Json;
using SharpRacer.SourceGenerators.Testing.Text;

namespace SharpRacer.SourceGenerators.Testing.TelemetryVariables;
public class JsonVariableOptionsDocument
{
    public JsonVariableOptionsDocument(string documentPath, JsonSourceText sourceText, ImmutableArray<JsonVariableOptions> variableOptions)
    {
        if (string.IsNullOrEmpty(documentPath))
        {
            throw new ArgumentException($"'{nameof(documentPath)}' cannot be null or empty.", nameof(documentPath));
        }

        SourceText = sourceText ?? throw new ArgumentNullException(nameof(sourceText));
        VariableOptions = variableOptions.GetEmptyIfDefault();

        SourceLocationFactory = new JsonLocationFactory(documentPath, SourceText);
    }

    public JsonLocationFactory SourceLocationFactory { get; }
    public JsonSourceText SourceText { get; }
    public ImmutableArray<JsonVariableOptions> VariableOptions { get; }
}
