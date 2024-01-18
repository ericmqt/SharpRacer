using System.Collections.Immutable;
using System.Text.Json;
using SharpRacer.SourceGenerators.TelemetryVariables;
using SharpRacer.SourceGenerators.TelemetryVariables.InputModels;
using SharpRacer.SourceGenerators.TelemetryVariables.Json;
using SharpRacer.SourceGenerators.Testing.Text;

namespace SharpRacer.SourceGenerators.Testing.TelemetryVariables;
public class VariableInfoDocumentBuilder
{
    private readonly List<VariableInfo> _variables;

    public VariableInfoDocumentBuilder()
    {
        _variables = new List<VariableInfo>();
    }

    public VariableInfoDocumentBuilder AddArray(
        string variableName,
        VariableValueType variableValueType,
        int valueCount,
        string description,
        string? unit)
    {
        return AddArray(variableName, variableValueType, valueCount, description, unit, null);
    }

    public VariableInfoDocumentBuilder AddArray(
        string variableName,
        VariableValueType valueType,
        int valueCount,
        string description,
        string? unit,
        string? deprecatedBy)
    {
        var variableInfo = new VariableInfo(
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

    public VariableInfoDocumentBuilder AddScalar(string variableName, VariableValueType valueType, string description, string? unit)
    {
        return AddScalar(variableName, valueType, description, unit, null);
    }

    public VariableInfoDocumentBuilder AddScalar(
        string variableName,
        VariableValueType valueType,
        string description,
        string? unit,
        string? deprecatedBy)
    {
        var variableInfo = new VariableInfo(
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

    public VariableInfoDocumentBuilder AddTimeSliceArray(
        string variableName,
        VariableValueType valueType,
        int valueCount,
        string description,
        string? unit)
    {
        return AddTimeSliceArray(variableName, valueType, valueCount, description, unit, null);
    }

    public VariableInfoDocumentBuilder AddTimeSliceArray(
        string variableName,
        VariableValueType variableValueType,
        int valueCount,
        string description,
        string? unit,
        string? deprecatedBy)
    {
        var variableInfo = new VariableInfo(
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

    public VariableInfoDocument Build(string documentPath)
    {
        var json = JsonSerializer.Serialize(
            _variables.ToImmutableArray(),
            TelemetryGeneratorSerializationContext.Default.ImmutableArrayVariableInfo);

        var jsonSourceText = new JsonSourceText(json);

        var variables = JsonSerializer.Deserialize(json, TelemetryGeneratorSerializationContext.Default.ImmutableArrayVariableInfo);

        return new VariableInfoDocument(documentPath, jsonSourceText, variables);
    }

    public AdditionalTextFile ToAdditionalTextFile(string documentPath)
    {
        var json = JsonSerializer.Serialize(
           _variables.ToImmutableArray(),
           TelemetryGeneratorSerializationContext.Default.ImmutableArrayVariableInfo);

        var jsonSourceText = new JsonSourceText(json);

        return new AdditionalTextFile(documentPath, jsonSourceText);
    }

    public VariableInfoFile ToVariableInfoFile(string documentPath)
    {
        var additionalText = ToAdditionalTextFile(documentPath);
        var sourceText = additionalText.GetText()!;

        return new VariableInfoFile(new VariableInfoFileName(documentPath), additionalText, sourceText);
    }
}
