using System.Collections.Immutable;

namespace SharpRacer.SourceGenerators.TelemetryVariables.InputModels;
public readonly struct IncludedVariables : IEquatable<IncludedVariables>
{
    private readonly bool _isInitialized;

    public IncludedVariables(ImmutableArray<IncludedVariableName> variableNames)
    {
        VariableNames = variableNames.GetEmptyIfDefault();

        _isInitialized = true;
    }

    public readonly ImmutableArray<IncludedVariableName> VariableNames { get; }

    public bool IncludeAll()
    {
        return !_isInitialized;
    }

    public override bool Equals(object obj)
    {
        return obj is IncludedVariables other && Equals(other);
    }

    public bool Equals(IncludedVariables other)
    {
        if (!_isInitialized)
        {
            return !other._isInitialized;
        }

        // Check if other is initialized, otherwise VariableNames.SequenceEqual(...) throws NullReferenceException
        if (!other._isInitialized)
        {
            return false;
        }

        return VariableNames.SequenceEqual(other.VariableNames);
    }

    public override int GetHashCode()
    {
        var hc = new HashCode();

        if (!_isInitialized)
        {
            return hc.ToHashCode();
        }

        for (int i = 0; i < VariableNames.Length; i++)
        {
            hc.Add(VariableNames[i]);
        }

        return hc.ToHashCode();
    }

    public static bool operator ==(IncludedVariables left, IncludedVariables right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(IncludedVariables left, IncludedVariables right)
    {
        return !left.Equals(right);
    }
}
