using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;
using SharpRacer.SourceGenerators.TelemetryVariables.InputModels;

namespace SharpRacer.SourceGenerators.TelemetryVariables.Json;

[JsonSourceGenerationOptions(
    AllowTrailingCommas = true,
    Converters =
    [
        typeof(IncludedVariableNameValueCollectionConverter),
        typeof(JsonStringEnumConverter<VariableValueType>),
        typeof(JsonVariableOptionsCollectionConverter),
        typeof(VariableInfoCollectionConverter)
    ],
    WriteIndented = true)]
[JsonSerializable(typeof(ImmutableArray<IncludedVariableNameValue>))]
[JsonSerializable(typeof(ImmutableArray<VariableInfo>))]
[JsonSerializable(typeof(ImmutableArray<JsonVariableOptions>))]
[JsonSerializable(typeof(IncludedVariableNameValue))]
[JsonSerializable(typeof(JsonVariableOptionsValue))]
[JsonSerializable(typeof(VariableInfo))]
[JsonSerializable(typeof(VariableValueType))]
[JsonSerializable(typeof(bool))]
[JsonSerializable(typeof(string))]
[ExcludeFromCodeCoverage]
internal partial class TelemetryGeneratorSerializationContext : JsonSerializerContext
{
}
