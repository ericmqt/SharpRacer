using System.Text.Json.Serialization;
using Microsoft.CodeAnalysis.Text;

namespace SharpRacer.SourceGenerators.TelemetryVariables.Json;
internal readonly struct JsonVariableInfo : IEquatable<JsonVariableInfo>
{
    public JsonVariableInfo(JsonVariableInfo value, TextSpan jsonSpan)
    {
        Name = value.Name;
        ValueType = value.ValueType;
        ValueCount = value.ValueCount;
        Description = value.Description;
        ValueUnit = value.ValueUnit;
        IsTimeSliceArray = value.IsTimeSliceArray;
        IsDeprecated = value.IsDeprecated;
        DeprecatedBy = value.DeprecatedBy;
        JsonSpan = jsonSpan;
    }

    [JsonConstructor]
    public JsonVariableInfo(
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
    }

    public readonly string? DeprecatedBy { get; }
    public readonly string Description { get; }
    public readonly bool IsDeprecated { get; }
    public readonly bool IsTimeSliceArray { get; }
    public readonly TextSpan JsonSpan { get; }
    public readonly string Name { get; }
    public readonly int ValueCount { get; }
    public readonly VariableValueType ValueType { get; }
    public readonly string? ValueUnit { get; }

    public override bool Equals(object obj)
    {
        return obj is JsonVariableInfo other && Equals(other);
    }

    public bool Equals(JsonVariableInfo other)
    {
        return StringComparer.Ordinal.Equals(Name, other.Name) &&
            JsonSpan == other.JsonSpan &&
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
        hc.Add(JsonSpan);
        hc.Add(ValueType);
        hc.Add(ValueCount);
        hc.Add(Description);
        hc.Add(ValueUnit);
        hc.Add(IsTimeSliceArray);
        hc.Add(IsDeprecated);
        hc.Add(DeprecatedBy);

        return hc.ToHashCode();
    }

    public static bool operator ==(JsonVariableInfo left, JsonVariableInfo right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(JsonVariableInfo left, JsonVariableInfo right)
    {
        return !(left == right);
    }
}
