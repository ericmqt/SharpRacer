using SharpRacer.SourceGenerators.TelemetryVariables.InputModels;

namespace SharpRacer.SourceGenerators.TelemetryVariables.GeneratorModels;
internal readonly struct DescriptorPropertyModel : IEquatable<DescriptorPropertyModel>
{
    public DescriptorPropertyModel(string propertyName, string propertyXmlSummary, VariableInfo variableInfo)
    {
        PropertyName = propertyName;
        PropertyXmlSummary = propertyXmlSummary;
        VariableInfo = variableInfo;
    }

    public readonly string PropertyName { get; }
    public readonly string? PropertyXmlSummary { get; }
    public readonly VariableInfo VariableInfo { get; }

    public override bool Equals(object obj)
    {
        return obj is DescriptorPropertyModel other && Equals(other);
    }

    public bool Equals(DescriptorPropertyModel other)
    {
        return StringComparer.Ordinal.Equals(PropertyName, other.PropertyName) &&
            StringComparer.Ordinal.Equals(PropertyXmlSummary, other.PropertyXmlSummary) &&
            VariableInfo == other.VariableInfo;
    }

    public override int GetHashCode()
    {
        var hc = new HashCode();

        hc.Add(PropertyName);
        hc.Add(PropertyXmlSummary);
        hc.Add(VariableInfo);

        return hc.ToHashCode();
    }

    public static bool operator ==(DescriptorPropertyModel left, DescriptorPropertyModel right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(DescriptorPropertyModel left, DescriptorPropertyModel right)
    {
        return !left.Equals(right);
    }
}
