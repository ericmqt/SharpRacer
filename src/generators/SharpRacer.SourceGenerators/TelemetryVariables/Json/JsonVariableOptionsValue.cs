using System.Text.Json.Serialization;

namespace SharpRacer.SourceGenerators.TelemetryVariables.Json;
internal readonly struct JsonVariableOptionsValue : IEquatable<JsonVariableOptionsValue>
{
    [JsonConstructor]
    public JsonVariableOptionsValue(string? name, string? descriptorName, string? contextPropertyName)
    {
        Name = name;
        DescriptorName = descriptorName;
        ContextPropertyName = contextPropertyName;
    }

    public readonly string? ContextPropertyName { get; }
    public readonly string? DescriptorName { get; }
    public readonly string? Name { get; }

    public override bool Equals(object obj)
    {
        return obj is JsonVariableOptionsValue other && Equals(other);
    }

    public bool Equals(JsonVariableOptionsValue other)
    {
        return StringComparer.Ordinal.Equals(Name, other.Name) &&
            StringComparer.Ordinal.Equals(ContextPropertyName, other.ContextPropertyName) &&
            StringComparer.Ordinal.Equals(DescriptorName, other.DescriptorName);
    }

    public override int GetHashCode()
    {
        var hc = new HashCode();

        hc.Add(Name);
        hc.Add(ContextPropertyName);
        hc.Add(DescriptorName);

        return hc.ToHashCode();
    }

    public static bool operator ==(JsonVariableOptionsValue left, JsonVariableOptionsValue right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(JsonVariableOptionsValue left, JsonVariableOptionsValue right)
    {
        return !(left == right);
    }
}
