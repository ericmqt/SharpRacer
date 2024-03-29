﻿using System.Text.Json.Serialization;

namespace SharpRacer.SourceGenerators.TelemetryVariables.Json;
public readonly struct JsonVariableOptionsValue : IEquatable<JsonVariableOptionsValue>
{
    [JsonConstructor]
    public JsonVariableOptionsValue(string? name, string? className)
    {
        Name = name;
        ClassName = className;
    }

    public readonly string? ClassName { get; }
    public readonly string? Name { get; }

    public override bool Equals(object obj)
    {
        return obj is JsonVariableOptionsValue other && Equals(other);
    }

    public bool Equals(JsonVariableOptionsValue other)
    {
        return StringComparer.Ordinal.Equals(Name, other.Name) &&
            StringComparer.Ordinal.Equals(ClassName, other.ClassName);
    }

    public override int GetHashCode()
    {
        var hc = new HashCode();

        hc.Add(Name);
        hc.Add(ClassName);

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
