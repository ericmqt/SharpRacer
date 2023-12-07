using Microsoft.CodeAnalysis;
using SharpRacer.SourceGenerators.TelemetryVariables.Json;

namespace SharpRacer.SourceGenerators.TelemetryVariables.InputModels;

internal readonly struct VariableOptions : IEquatable<VariableOptions>
{
    public VariableOptions(JsonVariableOptions jsonVariableOptions, Location variableKeyLocation, Location valueLocation)
    {
        VariableKey = jsonVariableOptions.Key;
        VariableKeyLocation = variableKeyLocation;
        ValueLocation = valueLocation;
        ContextPropertyName = jsonVariableOptions.Value.ContextPropertyName;
        DescriptorName = jsonVariableOptions.Value.DescriptorName;
        Name = jsonVariableOptions.Value.Name;
    }

    public readonly string? ContextPropertyName { get; }
    public readonly string? DescriptorName { get; }
    public readonly string? Name { get; }
    public Location ValueLocation { get; }
    public readonly string VariableKey { get; }
    public readonly Location VariableKeyLocation { get; }

    public string GetConfiguredName(VariableInfo variableInfo)
    {
        if (!string.IsNullOrEmpty(Name))
        {
            return Name!;
        }

        return variableInfo.Name;
    }

    public string GetConfiguredContextPropertyName(VariableInfo variableInfo)
    {
        if (!string.IsNullOrEmpty(ContextPropertyName))
        {
            return ContextPropertyName!;
        }

        return variableInfo.Name;
    }

    public string GetConfiguredDescriptorName(VariableInfo variableInfo)
    {
        if (!string.IsNullOrEmpty(DescriptorName))
        {
            return DescriptorName!;
        }

        return variableInfo.Name;
    }

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
            StringComparer.Ordinal.Equals(ContextPropertyName, other.ContextPropertyName) &&
            StringComparer.Ordinal.Equals(DescriptorName, other.DescriptorName);
    }

    public override int GetHashCode()
    {
        var hc = new HashCode();

        hc.Add(VariableKey, StringComparer.Ordinal);
        hc.Add(VariableKeyLocation);
        hc.Add(ValueLocation);
        hc.Add(Name, StringComparer.Ordinal);
        hc.Add(ContextPropertyName, StringComparer.Ordinal);
        hc.Add(DescriptorName, StringComparer.Ordinal);

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
