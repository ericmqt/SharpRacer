using Microsoft.CodeAnalysis;
using SharpRacer.SourceGenerators.TelemetryVariables.Json;

namespace SharpRacer.SourceGenerators.TelemetryVariables.InputModels;
internal readonly struct VariableInfo : IEquatable<VariableInfo>
{
    public VariableInfo(JsonVariableInfo value, Location jsonLocation)
    {
        Name = value.Name;
        ValueType = value.ValueType;
        ValueCount = value.ValueCount;
        Description = value.Description;
        ValueUnit = value.ValueUnit;
        IsTimeSliceArray = value.IsTimeSliceArray;
        IsDeprecated = value.IsDeprecated;
        DeprecatedBy = value.DeprecatedBy;
        JsonLocation = jsonLocation;
    }

    public readonly string? DeprecatedBy { get; }
    public readonly string Description { get; }
    public readonly bool IsDeprecated { get; }
    public readonly bool IsTimeSliceArray { get; }
    public Location JsonLocation { get; }
    public readonly string Name { get; }
    public readonly int ValueCount { get; }
    public readonly VariableValueType ValueType { get; }
    public readonly string? ValueUnit { get; }

    public override bool Equals(object obj)
    {
        return obj is VariableInfo other && Equals(other);
    }

    public bool Equals(VariableInfo other)
    {
        return StringComparer.Ordinal.Equals(Name, other.Name) &&
            JsonLocation == other.JsonLocation &&
            ValueType == other.ValueType &&
            ValueCount == other.ValueCount &&
            StringComparer.Ordinal.Equals(Description, other.Description) &&
            StringComparer.Ordinal.Equals(ValueUnit, other.ValueUnit) &&
            IsTimeSliceArray == other.IsTimeSliceArray &&
            IsDeprecated == other.IsDeprecated &&
            StringComparer.Ordinal.Equals(DeprecatedBy, other.DeprecatedBy);
    }

    public override int GetHashCode()
    {
        var hc = new HashCode();

        hc.Add(Name);
        hc.Add(ValueType);
        hc.Add(ValueCount);
        hc.Add(Description);
        hc.Add(ValueUnit);
        hc.Add(IsTimeSliceArray);
        hc.Add(IsDeprecated);
        hc.Add(JsonLocation);
        hc.Add(DeprecatedBy);

        return hc.ToHashCode();
    }

    public static bool operator ==(VariableInfo left, VariableInfo right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(VariableInfo left, VariableInfo right)
    {
        return !(left == right);
    }
}
