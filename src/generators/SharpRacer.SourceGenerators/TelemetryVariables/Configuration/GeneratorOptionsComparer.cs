namespace SharpRacer.SourceGenerators.TelemetryVariables.Configuration;
internal sealed class GeneratorOptionsComparer : IEqualityComparer<GeneratorOptions?>
{
    public static IEqualityComparer<GeneratorOptions?> Default { get; } = new GeneratorOptionsComparer();

    public bool Equals(GeneratorOptions? x, GeneratorOptions? y)
    {
        if (ReferenceEquals(x, y))
        {
            return true;
        }

        if (x is null || y is null)
        {
            return false;
        }

        return VariablesGeneratorOptionsComparer.Default.Equals(x.VariablesGenerator, y.VariablesGenerator) &&
            VariableOptionsDictionaryComparer.Default.Equals(x.VariableOptions, y.VariableOptions);
    }

    public int GetHashCode(GeneratorOptions? obj)
    {
        var hc = new HashCode();

        if (obj is null)
        {
            return hc.ToHashCode();
        }

        hc.Add(obj.VariablesGenerator, VariablesGeneratorOptionsComparer.Default);
        hc.Add(obj.VariableOptions, VariableOptionsDictionaryComparer.Default);

        return hc.ToHashCode();
    }
}
