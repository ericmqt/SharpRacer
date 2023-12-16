using System.Text.Json.Serialization;
using Microsoft.CodeAnalysis.Text;

namespace SharpRacer.SourceGenerators.TelemetryVariables.Json;
public readonly struct JsonVariableOptions
{
    public JsonVariableOptions(string key, TextSpan keySpan, JsonVariableOptionsValue value, TextSpan valueSpan)
    {
        Key = key;
        KeySpan = keySpan;
        Value = value;
        ValueSpan = valueSpan;
    }

    public readonly string Key { get; }

    [JsonIgnore]
    public readonly TextSpan KeySpan { get; }

    public readonly JsonVariableOptionsValue Value { get; }

    [JsonIgnore]
    public readonly TextSpan ValueSpan { get; }

    public override bool Equals(object obj)
    {
        return obj is JsonVariableOptions other && Equals(other);
    }

    public bool Equals(JsonVariableOptions other)
    {
        return StringComparer.Ordinal.Equals(Key, other.Key) &&
            KeySpan == other.KeySpan &&
            Value == other.Value &&
            ValueSpan == other.ValueSpan;
    }

    public override int GetHashCode()
    {
        var hc = new HashCode();

        hc.Add(Key, StringComparer.Ordinal);
        hc.Add(KeySpan);
        hc.Add(Value);
        hc.Add(ValueSpan);

        return hc.ToHashCode();
    }

    public static bool operator ==(JsonVariableOptions left, JsonVariableOptions right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(JsonVariableOptions left, JsonVariableOptions right)
    {
        return !(left == right);
    }
}
