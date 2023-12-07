using System.Collections.Immutable;

namespace SharpRacer.SourceGenerators.TelemetryVariables.InputModels;
internal readonly struct IncludedVariables : IEquatable<IncludedVariables>
{
    public IncludedVariables()
    {
        FileName = default;
        VariableNames = ImmutableArray<IncludedVariableName>.Empty;
    }

    public IncludedVariables(IncludedVariablesFileName fileName, ImmutableArray<IncludedVariableName> variableNames)
    {
        FileName = fileName;
        VariableNames = !variableNames.IsDefault ? variableNames : ImmutableArray<IncludedVariableName>.Empty;
    }

    public readonly IncludedVariablesFileName FileName { get; } = default;
    public readonly ImmutableArray<IncludedVariableName> VariableNames { get; }

    public bool IncludeAll()
    {
        return this == default || VariableNames.Length == 0;
    }

    public override bool Equals(object obj)
    {
        return obj is IncludedVariables other && Equals(other);
    }

    public bool Equals(IncludedVariables other)
    {
        return FileName == other.FileName && VariableNames.SequenceEqual(other.VariableNames);
    }

    public override int GetHashCode()
    {
        var hc = new HashCode();

        hc.Add(FileName);

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
