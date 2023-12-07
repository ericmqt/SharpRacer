using System.Collections.Immutable;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.CodeAnalysis.Text;
using SharpRacer.SourceGenerators.TelemetryVariables.InputModels;

namespace SharpRacer.SourceGenerators.TelemetryVariables.Json;
internal class IncludedVariableNameValueCollectionConverter : JsonConverter<ImmutableArray<IncludedVariableNameValue>>
{
    public override ImmutableArray<IncludedVariableNameValue> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.StartArray)
        {
            throw new JsonException();
        }

        var builder = ImmutableArray.CreateBuilder<IncludedVariableNameValue>();

        while (reader.Read())
        {
            if (reader.TokenType == JsonTokenType.EndArray)
            {
                break;
            }

            builder.Add(ReadIncludedVariableName(ref reader));
        }

        return builder.ToImmutable();
    }

    public override void Write(Utf8JsonWriter writer, ImmutableArray<IncludedVariableNameValue> value, JsonSerializerOptions options)
    {
        throw new NotImplementedException();
    }

    private IncludedVariableNameValue ReadIncludedVariableName(ref Utf8JsonReader reader)
    {
        if (reader.TokenType != JsonTokenType.String)
        {
            throw new JsonException();
        }

        var objStart = (int)reader.TokenStartIndex;

        var includedVariableName = reader.GetString() ?? string.Empty;

        var objSpan = GetTextSpanFromStartToCurrentPosition(objStart, ref reader);

        return new IncludedVariableNameValue(includedVariableName, objSpan);
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
