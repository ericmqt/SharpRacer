using System.Text.Json.Serialization;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace SharpRacer.SourceGenerators.TelemetryVariables.InputModels;
public readonly struct VariableInfo : IEquatable<VariableInfo>
{
    public VariableInfo(
        string name,
        VariableValueType valueType,
        int valueCount,
        string description,
        string? valueUnit,
        bool isTimeSliceArray,
        bool isDeprecated,
        string? deprecatedBy,
        TextSpan jsonSpan,
        Location jsonLocation)
    {
        Name = name;
        ValueType = valueType;
        ValueCount = valueCount;
        Description = description;
        ValueUnit = valueUnit;
        IsTimeSliceArray = isTimeSliceArray;
        IsDeprecated = isDeprecated;
        DeprecatedBy = deprecatedBy;

        JsonSpan = jsonSpan;
        JsonLocation = jsonLocation;
    }

    [JsonConstructor]
    public VariableInfo(
        string name,
        VariableValueType valueType,
        int valueCount,
        string description,
        string? valueUnit,
        bool isTimeSliceArray,
        bool isDeprecated,
        string? deprecatedBy)
    {
        Name = name;
        ValueType = valueType;
        ValueCount = valueCount;
        Description = description;
        ValueUnit = valueUnit;
        IsTimeSliceArray = isTimeSliceArray;
        IsDeprecated = isDeprecated;
        DeprecatedBy = deprecatedBy;

        JsonLocation = Location.None;
        JsonSpan = new TextSpan(0, 0);
    }

    public readonly string? DeprecatedBy { get; }
    public readonly string Description { get; }
    public readonly bool IsDeprecated { get; }
    public readonly bool IsTimeSliceArray { get; }

    [JsonIgnore]
    public readonly Location JsonLocation { get; }

    [JsonIgnore]
    public readonly TextSpan JsonSpan { get; }

    public readonly string Name { get; }
    public readonly int ValueCount { get; }
    public readonly VariableValueType ValueType { get; }
    public readonly string? ValueUnit { get; }

    public readonly VariableInfo WithJsonLocation(Location jsonLocation)
    {
        return new VariableInfo(
            Name,
            ValueType,
            ValueCount,
            Description,
            ValueUnit,
            IsTimeSliceArray,
            IsDeprecated,
            DeprecatedBy,
            JsonSpan,
            jsonLocation);
    }

    public readonly VariableInfo WithJsonSpan(TextSpan jsonSpan)
    {
        return new VariableInfo(
            Name,
            ValueType,
            ValueCount,
            Description,
            ValueUnit,
            IsTimeSliceArray,
            IsDeprecated,
            DeprecatedBy,
            jsonSpan,
            JsonLocation);
    }

    public override bool Equals(object obj)
    {
        return obj is VariableInfo other && Equals(other);
    }

    public bool Equals(VariableInfo other)
    {
        return StringComparer.Ordinal.Equals(DeprecatedBy, other.DeprecatedBy) &&
            StringComparer.Ordinal.Equals(Description, other.Description) &&
            IsDeprecated == other.IsDeprecated &&
            IsTimeSliceArray == other.IsTimeSliceArray &&
            JsonLocation == other.JsonLocation &&
            JsonSpan == other.JsonSpan &&
            StringComparer.Ordinal.Equals(Name, other.Name) &&
            ValueCount == other.ValueCount &&
            ValueType == other.ValueType &&
            StringComparer.Ordinal.Equals(ValueUnit, other.ValueUnit);
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
        hc.Add(DeprecatedBy);

        hc.Add(JsonLocation);
        hc.Add(JsonSpan);

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
