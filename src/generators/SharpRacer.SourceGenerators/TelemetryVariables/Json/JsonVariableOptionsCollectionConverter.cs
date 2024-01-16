using System.Collections.Immutable;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.CodeAnalysis.Text;

namespace SharpRacer.SourceGenerators.TelemetryVariables.Json;
internal class JsonVariableOptionsCollectionConverter : JsonConverter<ImmutableArray<JsonVariableOptions>>
{
    public override ImmutableArray<JsonVariableOptions> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.StartObject)
        {
            throw new JsonException($"Expected token type '{JsonTokenType.StartObject}'");
        }

        var builder = ImmutableArray.CreateBuilder<JsonVariableOptions>();

        while (reader.Read())
        {
            if (reader.TokenType == JsonTokenType.EndObject)
            {
                break;
            }

            var key = ReadOptionsKey(ref reader, out var keySpan);
            var value = ReadOptionsValue(ref reader, out var valueSpan);
            var optionsValue = new JsonVariableOptions(key, keySpan, value, valueSpan);

            builder.Add(optionsValue);
        }

        return builder.ToImmutable();
    }

    public override void Write(Utf8JsonWriter writer, ImmutableArray<JsonVariableOptions> value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();

        foreach (var optionsValue in value)
        {
            writer.WritePropertyName(optionsValue.Key);

            JsonSerializer.Serialize(writer, optionsValue.Value, TelemetryGeneratorSerializationContext.Default.JsonVariableOptionsValue);
        }

        writer.WriteEndObject();
    }

    private string ReadOptionsKey(ref Utf8JsonReader reader, out TextSpan keySpan)
    {
        var keyStart = (int)reader.TokenStartIndex;
        var key = reader.GetString();

        if (string.IsNullOrEmpty(key))
        {
            throw new JsonException("Expected key that is not null or empty.");
        }

        keySpan = new TextSpan(keyStart, (int)reader.BytesConsumed - keyStart);

        return key!;
    }

    private JsonVariableOptionsValue ReadOptionsValue(ref Utf8JsonReader reader, out TextSpan valueSpan)
    {
        reader.Read();

        if (reader.TokenType != JsonTokenType.StartObject)
        {
            throw new JsonException($"Expected token type '{JsonTokenType.StartObject}'");
        }

        var valueStart = (int)reader.TokenStartIndex;
        var value = JsonSerializer.Deserialize(ref reader, TelemetryGeneratorSerializationContext.Default.JsonVariableOptionsValue);

        valueSpan = new TextSpan(valueStart, (int)reader.BytesConsumed - valueStart);

        return value;
    }
}
