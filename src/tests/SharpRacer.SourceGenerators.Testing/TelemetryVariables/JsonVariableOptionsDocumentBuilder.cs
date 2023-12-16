using System.Collections.Immutable;
using System.Text.Json;
using SharpRacer.SourceGenerators.TelemetryVariables.InputModels;
using SharpRacer.SourceGenerators.TelemetryVariables.Json;
using SharpRacer.SourceGenerators.Testing.Text;

namespace SharpRacer.SourceGenerators.Testing.TelemetryVariables;
public class JsonVariableOptionsDocumentBuilder
{
    private readonly List<JsonVariableOptions> _variableOptions;

    public JsonVariableOptionsDocumentBuilder()
    {
        _variableOptions = new List<JsonVariableOptions>();
    }

    public JsonVariableOptionsDocumentBuilder Add(string key, string? name, string? className)
    {
        return Add(key, new JsonVariableOptionsValue(name, className));
    }

    public JsonVariableOptionsDocumentBuilder Add(string key, JsonVariableOptionsValue value)
    {
        var options = new JsonVariableOptions(key, default, value, default);

        _variableOptions.Add(options);

        return this;
    }

    public JsonVariableOptionsDocument Build(string documentPath)
    {
        var json = ToJson();

        var sourceText = new JsonSourceText(json);

        var variableOptions = JsonSerializer.Deserialize(json, TelemetryGeneratorSerializationContext.Default.ImmutableArrayJsonVariableOptions);

        return new JsonVariableOptionsDocument(documentPath, sourceText, variableOptions);
    }

    public AdditionalTextFile ToAdditionalTextFile(string documentPath)
    {
        var json = ToJson();
        var sourceText = new JsonSourceText(json);

        return new AdditionalTextFile(documentPath, sourceText);
    }

    public string ToJson()
    {
        return JsonSerializer.Serialize(
            _variableOptions.ToImmutableArray(), TelemetryGeneratorSerializationContext.Default.ImmutableArrayJsonVariableOptions);
    }

    public VariableOptionsFile ToVariableOptionsFile(string documentPath)
    {
        var additionalText = ToAdditionalTextFile(documentPath);
        var sourceText = additionalText.GetText()!;

        return new VariableOptionsFile(new VariableOptionsFileName(documentPath), additionalText, sourceText);
    }
}
