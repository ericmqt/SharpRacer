namespace SharpRacer.SourceGenerators.TelemetryVariables.Configuration;
internal sealed class VariablesGeneratorOptionsComparer : IEqualityComparer<VariablesGeneratorOptions?>
{
    public static IEqualityComparer<VariablesGeneratorOptions?> Default { get; } = new VariablesGeneratorOptionsComparer();

    public bool Equals(VariablesGeneratorOptions? x, VariablesGeneratorOptions? y)
    {
        if (ReferenceEquals(x, y))
        {
            return true;
        }

        if (x is null || y is null)
        {
            return false;
        }

        return x.GenerateTypedVariableClasses == y.GenerateTypedVariableClasses;
    }

    public int GetHashCode(VariablesGeneratorOptions? obj)
    {
        var hc = new HashCode();

        if (obj is null)
        {
            return hc.ToHashCode();
        }

        hc.Add(obj.GenerateTypedVariableClasses);

        return hc.ToHashCode();
    }
}
