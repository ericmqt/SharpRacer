using System.Text.Json;
using System.Text.Json.Serialization;

namespace SharpRacer.Tools.TelemetryVariables.Json;
internal static class JsonSerializerConfiguration
{
    public static JsonSerializerOptions SerializerOptions { get; } = CreateJsonSerializerOptions();

    private static JsonSerializerOptions CreateJsonSerializerOptions()
    {
        var serializerOptions = new JsonSerializerOptions()
        {
            WriteIndented = true
        };

        serializerOptions.Converters.Add(new ContentVersionConverter());
        serializerOptions.Converters.Add(new JsonStringEnumConverter());

        return serializerOptions;
    }
}
