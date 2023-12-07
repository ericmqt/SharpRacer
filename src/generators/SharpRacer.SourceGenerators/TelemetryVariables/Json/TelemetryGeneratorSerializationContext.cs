using System.Collections.Immutable;
using System.Text.Json.Serialization;
using SharpRacer.SourceGenerators.TelemetryVariables.InputModels;

namespace SharpRacer.SourceGenerators.TelemetryVariables.Json;

[JsonSourceGenerationOptions(
    AllowTrailingCommas = true,
    Converters =
    [
        typeof(IncludedVariableNameValueCollectionConverter),
        typeof(JsonStringEnumConverter<VariableValueType>),
        typeof(JsonVariableInfoCollectionConverter),
        typeof(JsonVariableOptionsCollectionConverter)
    ])]
[JsonSerializable(typeof(ImmutableArray<IncludedVariableNameValue>))]
[JsonSerializable(typeof(ImmutableArray<JsonVariableInfo>))]
[JsonSerializable(typeof(ImmutableArray<JsonVariableOptions>))]
[JsonSerializable(typeof(IncludedVariableNameValue))]
[JsonSerializable(typeof(JsonVariableOptionsValue))]
[JsonSerializable(typeof(VariableValueType))]
[JsonSerializable(typeof(bool))]
[JsonSerializable(typeof(string))]
internal partial class TelemetryGeneratorSerializationContext : JsonSerializerContext
{
}
