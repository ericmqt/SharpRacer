using System.Collections.Immutable;
using SharpRacer.SourceGenerators.TelemetryVariables.InputModels;
using SharpRacer.SourceGenerators.TelemetryVariables.Json;
using SharpRacer.SourceGenerators.Testing.Text;

namespace SharpRacer.SourceGenerators.Testing.TelemetryVariables;
public class JsonVariableInfoDocument
{
    public JsonVariableInfoDocument(string documentPath, JsonSourceText sourceText, ImmutableArray<JsonVariableInfo> variables)
    {
        if (string.IsNullOrEmpty(documentPath))
        {
            throw new ArgumentException($"'{nameof(documentPath)}' cannot be null or empty.", nameof(documentPath));
        }

        SourceText = sourceText;
        Variables = variables.GetEmptyIfDefault();

        SourceLocationFactory = new JsonLocationFactory(documentPath, SourceText);
    }

    public JsonLocationFactory SourceLocationFactory { get; }
    public JsonSourceText SourceText { get; }
    public ImmutableArray<JsonVariableInfo> Variables { get; }

    public ImmutableArray<VariableInfo> ToVariableInfoArray()
    {
        return Variables.Select(x => new VariableInfo(x, SourceLocationFactory.GetLocation(x.JsonSpan))).ToImmutableArray();
    }
}
