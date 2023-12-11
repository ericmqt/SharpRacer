using System.Collections.Immutable;
using Microsoft.CodeAnalysis;

namespace SharpRacer.SourceGenerators.TelemetryVariables.InputModels;
internal readonly struct IncludedVariableName : IEquatable<IncludedVariableName>
{
    private readonly bool _isInitialized;

    public IncludedVariableName(string variableName, Location sourceLocation)
        : this(variableName, sourceLocation, ImmutableArray<Diagnostic>.Empty)
    {

    }

    public IncludedVariableName(string variableName, Location sourceLocation, ImmutableArray<Diagnostic> diagnostics)
    {
        Value = !string.IsNullOrEmpty(variableName)
            ? variableName
            : throw new ArgumentException($"'{nameof(variableName)}' cannot be null or empty.", nameof(variableName));

        SourceLocation = sourceLocation ?? throw new ArgumentNullException(nameof(sourceLocation));

        Diagnostics = !diagnostics.IsDefault ? diagnostics : ImmutableArray<Diagnostic>.Empty;

        _isInitialized = true;
    }

    public readonly ImmutableArray<Diagnostic> Diagnostics { get; }
    public readonly Location SourceLocation { get; }
    public readonly string Value { get; }

    public override bool Equals(object obj)
    {
        return obj is IncludedVariableName other && Equals(other);
    }

    public bool Equals(IncludedVariableName other)
    {
        if (!_isInitialized)
        {
            return !other._isInitialized;
        }

        if (!other._isInitialized)
        {
            return false;
        }

        return StringComparer.Ordinal.Equals(Value, other.Value) &&
            SourceLocation == other.SourceLocation &&
            Diagnostics.SequenceEqual(other.Diagnostics);
    }

    public override int GetHashCode()
    {
        var hc = new HashCode();

        hc.Add(Value, StringComparer.Ordinal);
        hc.Add(SourceLocation);
        hc.AddDiagnosticArray(Diagnostics);

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

    public static implicit operator string(IncludedVariableName variableName)
    {
        return variableName.Value;
    }
}
