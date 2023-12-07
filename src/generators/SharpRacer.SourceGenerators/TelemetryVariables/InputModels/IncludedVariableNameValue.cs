using System.Text.Json.Serialization;
using Microsoft.CodeAnalysis.Text;

namespace SharpRacer.SourceGenerators.TelemetryVariables.InputModels;
internal readonly struct IncludedVariableNameValue : IEquatable<IncludedVariableNameValue>
{
    [JsonConstructor]
    public IncludedVariableNameValue(string variableName)
        : this(variableName, default)
    {

    }

    public IncludedVariableNameValue(IncludedVariableNameValue variableNameValue, TextSpan valueSpan)
        : this(variableNameValue.Value, valueSpan)
    {

    }

    public IncludedVariableNameValue(string variableName, TextSpan valueSpan)
    {
        // Allow string.Empty but not null
        Value = variableName ?? throw new ArgumentNullException($"'{nameof(variableName)}' cannot be null.", nameof(variableName));

        ValueSpan = valueSpan;
    }

    public readonly string Value { get; }
    public TextSpan ValueSpan { get; }

    public override bool Equals(object obj)
    {
        return obj is IncludedVariableNameValue other && Equals(other);
    }

    public bool Equals(IncludedVariableNameValue other)
    {
        return StringComparer.Ordinal.Equals(Value, other.Value) && ValueSpan == other.ValueSpan;
    }

    public override int GetHashCode()
    {
        var hc = new HashCode();

        hc.Add(Value, StringComparer.Ordinal);
        hc.Add(ValueSpan);

        return hc.ToHashCode();
    }

    public static bool operator ==(IncludedVariableNameValue left, IncludedVariableNameValue right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(IncludedVariableNameValue left, IncludedVariableNameValue right)
    {
        return !left.Equals(right);
    }
}
