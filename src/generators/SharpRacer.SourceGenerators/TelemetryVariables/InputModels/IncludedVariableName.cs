using Microsoft.CodeAnalysis;

namespace SharpRacer.SourceGenerators.TelemetryVariables.InputModels;
internal readonly struct IncludedVariableName : IEquatable<IncludedVariableName>, IEquatable<string?>
{
    public IncludedVariableName(string variableName, Location sourceLocation)
    {
        Value = !string.IsNullOrEmpty(variableName)
            ? variableName
            : throw new ArgumentException($"'{nameof(variableName)}' cannot be null or empty.", nameof(variableName));

        SourceLocation = sourceLocation ?? throw new ArgumentNullException(nameof(sourceLocation));
    }

    public Location SourceLocation { get; }
    public readonly string Value { get; }

    public override bool Equals(object obj)
    {
        return obj is IncludedVariableName other && Equals(other);
    }

    public bool Equals(IncludedVariableName other)
    {
        return StringComparer.Ordinal.Equals(Value, other.Value) && SourceLocation == other.SourceLocation;
    }

    public bool Equals(string? other)
    {
        return StringComparer.Ordinal.Equals(Value, other);
    }

    public override int GetHashCode()
    {
        var hc = new HashCode();

        hc.Add(Value, StringComparer.Ordinal);
        hc.Add(SourceLocation);

        return hc.ToHashCode();
    }

    public static bool operator ==(IncludedVariableName left, IncludedVariableName right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(IncludedVariableName left, IncludedVariableName right)
    {
        return !left.Equals(right);
    }

    public static bool operator ==(IncludedVariableName left, string? right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(IncludedVariableName left, string? right)
    {
        return !left.Equals(right);
    }

    public static implicit operator string(IncludedVariableName variableName)
    {
        return variableName.Value;
    }
}
