using SharpRacer.SourceGenerators.TelemetryVariables.InputModels;

namespace SharpRacer.SourceGenerators.TelemetryVariables.Configuration;
internal sealed class VariableOptionsComparer : IEqualityComparer<VariableOptions?>
{
    public static IEqualityComparer<VariableOptions?> Default { get; } = new VariableOptionsComparer();

    public bool Equals(VariableOptions? x, VariableOptions? y)
    {
        if (ReferenceEquals(x, y))
        {
            return true;
        }

        if (x is null || y is null)
        {
            return false;
        }

        return StringComparer.Ordinal.Equals(x.Name, y.Name) &&
            StringComparer.Ordinal.Equals(x.DescriptorName, y.DescriptorName) &&
            StringComparer.Ordinal.Equals(x.ContextPropertyName, y.ContextPropertyName);
    }

    public int GetHashCode(VariableOptions? obj)
    {
        var hc = new HashCode();

        if (obj is null)
        {
            return hc.ToHashCode();
        }

        hc.Add(obj.Name);
        hc.Add(obj.DescriptorName);
        hc.Add(obj.ContextPropertyName);

        return hc.ToHashCode();
    }
}
