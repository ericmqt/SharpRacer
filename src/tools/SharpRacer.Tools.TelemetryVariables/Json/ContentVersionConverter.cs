using System.Text.Json;
using System.Text.Json.Serialization;

namespace SharpRacer.Tools.TelemetryVariables.Json;
internal class ContentVersionConverter : JsonConverter<ContentVersion>
{
    public override ContentVersion Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return ContentVersion.Parse(reader.GetString()!, null);
    }

    public override void Write(Utf8JsonWriter writer, ContentVersion value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString());
    }
}
