using System.Text.Json;
using System.Text.Json.Serialization;

namespace SharpRacer.Tools.TelemetryVariables.Configuration;
internal static class JsonSerializerConfiguration
{
    public static JsonSerializerOptions SerializerOptions { get; } = CreateJsonSerializerOptions();

    private static JsonSerializerOptions CreateJsonSerializerOptions()
    {
        var enumConverter = new JsonStringEnumConverter();
        var serializerOptions = new JsonSerializerOptions()
        {
            WriteIndented = true
        };

        serializerOptions.Converters.Add(enumConverter);

        return serializerOptions;
    }
}
