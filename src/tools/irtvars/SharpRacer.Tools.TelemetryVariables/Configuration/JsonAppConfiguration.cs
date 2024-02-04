using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace SharpRacer.Tools.TelemetryVariables.Configuration;
internal class JsonAppConfiguration
{
    private static readonly JsonSerializerOptions _serializerOptions = CreateJsonSerializerOptions();

    public JsonAppConfiguration()
    {
        ExtensionData = new Dictionary<string, object>();

        DatabaseOptions = new DatabaseOptions();
    }

    [JsonPropertyName("Database")]
    public DatabaseOptions DatabaseOptions { get; set; }

    /// <summary>
    /// Holds any JSON key/value pairs not present in the object model that will be preserved during serialization.
    /// </summary>
    [JsonExtensionData]
    public IDictionary<string, object> ExtensionData { get; set; }

    public async Task SaveAsync(CancellationToken cancellationToken = default)
    {
        var json = JsonSerializer.Serialize(this, _serializerOptions);

        await File.WriteAllTextAsync(GetPath(), json, cancellationToken);
    }

    public static string GetPath()
    {
        return Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!, "irtvars.config.json");
    }

    public static JsonAppConfiguration Load()
    {
        var path = GetPath();

        if (!File.Exists(path))
        {
            return new JsonAppConfiguration();
        }

        var json = File.ReadAllText(path);
        return JsonSerializer.Deserialize<JsonAppConfiguration>(json, _serializerOptions) ?? new JsonAppConfiguration();
    }

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
