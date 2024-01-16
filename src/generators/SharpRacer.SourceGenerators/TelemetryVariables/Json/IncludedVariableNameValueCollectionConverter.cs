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
            throw new JsonException($"Expected token type '{JsonTokenType.StartArray}'");
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
            throw new JsonException($"Expected token type '{JsonTokenType.String}'");
        }

        var objStart = (int)reader.TokenStartIndex;

        // With check for TokenType == JsonTokenType.String above, GetString() won't return null
        var includedVariableName = reader.GetString()!;

        var objSpan = new TextSpan(objStart, (int)reader.BytesConsumed - objStart);

        return new IncludedVariableNameValue(includedVariableName, objSpan);
    }
}
