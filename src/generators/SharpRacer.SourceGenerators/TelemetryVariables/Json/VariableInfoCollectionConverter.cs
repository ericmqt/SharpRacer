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
            throw new JsonException();
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
                throw new JsonException();
            }

            var objStart = (int)reader.TokenStartIndex;

            var variableInfo = JsonSerializer.Deserialize(ref reader, TelemetryGeneratorSerializationContext.Default.VariableInfo);

            var objSpan = GetTextSpanFromStartToCurrentPosition(objStart, ref reader);

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
