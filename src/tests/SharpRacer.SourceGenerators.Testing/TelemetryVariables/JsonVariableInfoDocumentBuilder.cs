using System.Collections.Immutable;
using System.Text.Json;
using SharpRacer.SourceGenerators.TelemetryVariables;
using SharpRacer.SourceGenerators.TelemetryVariables.Json;
using SharpRacer.SourceGenerators.Testing.Text;

namespace SharpRacer.SourceGenerators.Testing.TelemetryVariables;
public class JsonVariableInfoDocumentBuilder
{
    private readonly List<JsonVariableInfo> _variables;

    public JsonVariableInfoDocumentBuilder()
    {
        _variables = new List<JsonVariableInfo>();
    }

    public JsonVariableInfoDocumentBuilder AddArray(
        string variableName,
        VariableValueType variableValueType,
        int valueCount,
        string description,
        string? unit)
    {
        return AddArray(variableName, variableValueType, valueCount, description, unit, null);
    }

    public JsonVariableInfoDocumentBuilder AddArray(
        string variableName,
        VariableValueType valueType,
        int valueCount,
        string description,
        string? unit,
        string? deprecatedBy)
    {
        var variableInfo = new JsonVariableInfo(
            variableName,
            valueType,
            valueCount,
            description,
            unit,
            false,
            !string.IsNullOrEmpty(deprecatedBy),
            deprecatedBy);

        _variables.Add(variableInfo);

        return this;
    }

    public JsonVariableInfoDocumentBuilder AddScalar(string variableName, VariableValueType valueType, string description, string? unit)
    {
        return AddScalar(variableName, valueType, description, unit, null);
    }

    public JsonVariableInfoDocumentBuilder AddScalar(
        string variableName,
        VariableValueType valueType,
        string description,
        string? unit,
        string? deprecatedBy)
    {
        var variableInfo = new JsonVariableInfo(
            variableName,
            valueType,
            1,
            description,
            unit,
            false,
            !string.IsNullOrEmpty(deprecatedBy),
            deprecatedBy);

        _variables.Add(variableInfo);

        return this;
    }

    public JsonVariableInfoDocumentBuilder AddTimeSliceArray(
        string variableName,
        VariableValueType valueType,
        int valueCount,
        string description,
        string? unit)
    {
        return AddTimeSliceArray(variableName, valueType, valueCount, description, unit, null);
    }

    public JsonVariableInfoDocumentBuilder AddTimeSliceArray(
        string variableName,
        VariableValueType variableValueType,
        int valueCount,
        string description,
        string? unit,
        string? deprecatedBy)
    {
        var variableInfo = new JsonVariableInfo(
            variableName,
            variableValueType,
            valueCount,
            description,
            unit,
            true,
            !string.IsNullOrEmpty(deprecatedBy),
            deprecatedBy);

        _variables.Add(variableInfo);

        return this;
    }

    public JsonVariableInfoDocument Build(string documentPath)
    {
        var json = JsonSerializer.Serialize(
            _variables.ToImmutableArray(),
            TelemetryGeneratorSerializationContext.Default.ImmutableArrayJsonVariableInfo);

        var jsonSourceText = new JsonSourceText(json);

        var variables = JsonSerializer.Deserialize(json, TelemetryGeneratorSerializationContext.Default.ImmutableArrayJsonVariableInfo);

        return new JsonVariableInfoDocument(documentPath, jsonSourceText, variables);
    }
}
