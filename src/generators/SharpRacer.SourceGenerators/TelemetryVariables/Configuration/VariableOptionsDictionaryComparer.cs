using SharpRacer.SourceGenerators.TelemetryVariables.InputModels;

namespace SharpRacer.SourceGenerators.TelemetryVariables.Configuration;
internal sealed class VariableOptionsDictionaryComparer : IEqualityComparer<Dictionary<string, VariableOptions>?>
{
    public static IEqualityComparer<Dictionary<string, VariableOptions>?> Default { get; } = new VariableOptionsDictionaryComparer();

    public bool Equals(Dictionary<string, VariableOptions>? x, Dictionary<string, VariableOptions>? y)
    {
        if (ReferenceEquals(x, y))
        {
            return true;
        }

        if (x is null || y is null)
        {
            return false;
        }

        if (x.Count != y.Count)
        {
            return false;
        }

        foreach (var kvp in x)
        {
            if (!y.ContainsKey(kvp.Key))
            {
                return false;
            }

            if (!VariableOptionsComparer.Default.Equals(kvp.Value, y[kvp.Key]))
            {
                return false;
            }
        }

        return true;
    }

    public int GetHashCode(Dictionary<string, VariableOptions>? obj)
    {
        var hc = new HashCode();

        if (obj is null)
        {
            return hc.ToHashCode();
        }

        foreach (var kvp in obj)
        {
            hc.Add(kvp.Key);
            hc.Add(kvp.Value, VariableOptionsComparer.Default);
        }

        return hc.ToHashCode();
    }
}
