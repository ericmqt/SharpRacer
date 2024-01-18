using System.Collections.Immutable;

namespace SharpRacer.SourceGenerators.TelemetryVariables.InputModels;
public readonly struct IncludedVariables : IEquatable<IncludedVariables>
{
    public IncludedVariables(ImmutableArray<IncludedVariableName> variableNames)
    {
        VariableNames = variableNames.GetEmptyIfDefault();
    }

    public readonly ImmutableArray<IncludedVariableName> VariableNames { get; }

    public bool IncludeAll()
    {
        return VariableNames.IsDefault;
    }

    public override bool Equals(object obj)
    {
        return obj is IncludedVariables other && Equals(other);
    }

    public bool Equals(IncludedVariables other)
    {
        return VariableNames.SequenceEqualDefaultTolerant(other.VariableNames);
    }

    public override int GetHashCode()
    {
        var hc = new HashCode();

        if (!VariableNames.IsDefault)
        {
            for (int i = 0; i < VariableNames.Length; i++)
            {
                hc.Add(VariableNames[i]);
            }
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
