using Microsoft.CodeAnalysis;
using SharpRacer.SourceGenerators.TelemetryVariables.Json;

namespace SharpRacer.SourceGenerators.TelemetryVariables.InputModels;

internal readonly struct VariableOptions : IEquatable<VariableOptions>
{
    public VariableOptions(string variableKey)
    {
        VariableKey = !string.IsNullOrEmpty(variableKey)
            ? variableKey
            : throw new ArgumentException($"'{nameof(variableKey)}' cannot be null or empty.", nameof(variableKey));

        VariableKeyLocation = Location.None;
        ValueLocation = Location.None;
    }

    public VariableOptions(string variableKey, string? name, string? className)
        : this (variableKey)
    {
        Name = name;
        ClassName = className;

        VariableKeyLocation = Location.None;
        ValueLocation = Location.None;
    }

    public VariableOptions(JsonVariableOptions jsonVariableOptions, Location variableKeyLocation, Location valueLocation)
    {
        VariableKey = jsonVariableOptions.Key;
        VariableKeyLocation = variableKeyLocation;
        ValueLocation = valueLocation;
        
        ClassName = jsonVariableOptions.Value.ClassName;
        Name = jsonVariableOptions.Value.Name;
    }

    public readonly string? ClassName { get; }
    public readonly string? Name { get; }
    public readonly Location ValueLocation { get; }
    public readonly string VariableKey { get; }
    public readonly Location VariableKeyLocation { get; }

    public override bool Equals(object obj)
    {
        return obj is VariableOptions other && Equals(other);
    }

    public bool Equals(VariableOptions other)
    {
        return StringComparer.Ordinal.Equals(VariableKey, other.VariableKey) &&
            VariableKeyLocation == other.VariableKeyLocation &&
            ValueLocation == other.ValueLocation &&
            StringComparer.Ordinal.Equals(Name, other.Name) &&
            StringComparer.Ordinal.Equals(ClassName, other.ClassName);
    }

    public override int GetHashCode()
    {
        var hc = new HashCode();

        hc.Add(VariableKey);
        hc.Add(VariableKeyLocation);
        hc.Add(ValueLocation);
        hc.Add(Name);
        hc.Add(ClassName);

        return hc.ToHashCode();
    }

    public static bool operator ==(VariableOptions left, VariableOptions right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(VariableOptions left, VariableOptions right)
    {
        return !(left == right);
    }
}
