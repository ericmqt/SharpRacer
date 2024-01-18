using System.Collections.Immutable;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.CodeAnalysis.Text;
using SharpRacer.SourceGenerators.TelemetryVariables.InputModels;

namespace SharpRacer.SourceGenerators.TelemetryVariables.Json;
internal class VariableInfoCollectionConverter : JsonConverter<ImmutableArray<VariableInfo>>
{
    public override ImmutableArray<VariableInfo> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.StartArray)
        {
            throw new JsonException($"Expected token type '{JsonTokenType.StartArray}'");
        }

        var builder = ImmutableArray.CreateBuilder<VariableInfo>();

        while (reader.Read())
        {
            if (reader.TokenType == JsonTokenType.EndArray)
            {
                break;
            }

            if (reader.TokenType != JsonTokenType.StartObject)
            {
                throw new JsonException($"Expected token type '{JsonTokenType.StartObject}'");
            }

            var objStart = (int)reader.TokenStartIndex;

            var variableInfo = JsonSerializer.Deserialize(ref reader, TelemetryGeneratorSerializationContext.Default.VariableInfo);

            var objSpan = new TextSpan(objStart, (int)reader.BytesConsumed - objStart);

            variableInfo = variableInfo.WithJsonSpan(objSpan);

            builder.Add(variableInfo);
        }

        return builder.ToImmutable();
    }

    public override void Write(Utf8JsonWriter writer, ImmutableArray<VariableInfo> value, JsonSerializerOptions options)
    {
        writer.WriteStartArray();

        foreach (var variableInfoValue in value)
        {
            JsonSerializer.Serialize(writer, variableInfoValue, TelemetryGeneratorSerializationContext.Default.VariableInfo);
        }

        writer.WriteEndArray();
    }
}
