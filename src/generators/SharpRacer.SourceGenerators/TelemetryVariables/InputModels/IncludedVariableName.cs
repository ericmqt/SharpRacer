using System.Collections.Immutable;
using Microsoft.CodeAnalysis;

namespace SharpRacer.SourceGenerators.TelemetryVariables.InputModels;
internal readonly struct IncludedVariableName : IEquatable<IncludedVariableName>
{
    public IncludedVariableName(string variableName, Location sourceLocation)
        : this(variableName, sourceLocation, ImmutableArray<Diagnostic>.Empty)
    {

    }

    public IncludedVariableName(string variableName, Location sourceLocation, ImmutableArray<Diagnostic> diagnostics)
    {
        // Allow string.Empty but not null
        Value = variableName ?? throw new ArgumentException($"'{nameof(variableName)}' cannot be null.", nameof(variableName));

        SourceLocation = sourceLocation ?? throw new ArgumentNullException(nameof(sourceLocation));

        Diagnostics = diagnostics.GetEmptyIfDefault();
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
        return StringComparer.Ordinal.Equals(Value, other.Value) &&
            SourceLocation == other.SourceLocation &&
            Diagnostics.SequenceEqualDefaultTolerant(other.Diagnostics);
    }

    public override int GetHashCode()
    {
        var hc = new HashCode();

        hc.Add(Value);
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
        return variableName.Value ?? string.Empty;
    }
}
