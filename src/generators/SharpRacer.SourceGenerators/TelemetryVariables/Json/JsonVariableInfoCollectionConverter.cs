using System.Collections.Immutable;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.CodeAnalysis.Text;

namespace SharpRacer.SourceGenerators.TelemetryVariables.Json;
internal class JsonVariableInfoCollectionConverter : JsonConverter<ImmutableArray<JsonVariableInfo>>
{
    public override ImmutableArray<JsonVariableInfo> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.StartArray)
        {
            throw new JsonException();
        }

        var builder = ImmutableArray.CreateBuilder<JsonVariableInfo>();

        while (reader.Read())
        {
            if (reader.TokenType == JsonTokenType.EndArray)
            {
                break;
            }

            builder.Add(ReadVariableInfo(ref reader));
        }

        return builder.ToImmutable();
    }

    public override void Write(Utf8JsonWriter writer, ImmutableArray<JsonVariableInfo> value, JsonSerializerOptions options)
    {
        writer.WriteStartArray();

        foreach (var variableInfoValue in value)
        {
            JsonSerializer.Serialize(writer, variableInfoValue, TelemetryGeneratorSerializationContext.Default.JsonVariableInfo);
        }

        writer.WriteEndArray();
    }

    private JsonVariableInfo ReadVariableInfo(ref Utf8JsonReader reader)
    {
        if (reader.TokenType != JsonTokenType.StartObject)
        {
            throw new JsonException();
        }

        var objStart = (int)reader.TokenStartIndex;

        var variableInfo = JsonSerializer.Deserialize(ref reader, TelemetryGeneratorSerializationContext.Default.JsonVariableInfo);

        var objSpan = GetTextSpanFromStartToCurrentPosition(objStart, ref reader);

        return new JsonVariableInfo(variableInfo, objSpan);
    }

    private TextSpan GetTextSpanFromStartToCurrentPosition(int start, ref Utf8JsonReader reader)
    {
        var currentPosition = (int)reader.BytesConsumed;

        if (start > currentPosition)
        {
            throw new ArgumentOutOfRangeException(nameof(start), $"Value '{nameof(start)}' cannot be greater than the current position.");
        }

        if (start == currentPosition)
        {
            return new TextSpan(start, 1);
        }

        return new TextSpan(start, currentPosition - start);
    }
}
