namespace SharpRacer.SourceGenerators.TelemetryVariables.GeneratorModels;
internal readonly struct DescriptorPropertyModel : IEquatable<DescriptorPropertyModel>
{
    public DescriptorPropertyModel(VariableModel variableModel)
    {
        PropertyName = variableModel.DescriptorName;
        PropertyXmlSummary = variableModel.VariableInfo.Description;
        VariableName = variableModel.VariableInfo.Name;
        VariableValueType = variableModel.VariableInfo.ValueType;
        VariableValueCount = variableModel.VariableInfo.ValueCount;
    }

    public readonly string PropertyName { get; }
    public readonly string? PropertyXmlSummary { get; }
    public readonly string VariableName { get; }
    public readonly VariableValueType VariableValueType { get; }
    public readonly int VariableValueCount { get; }

    public override bool Equals(object obj)
    {
        return obj is DescriptorPropertyModel other && Equals(other);
    }

    public bool Equals(DescriptorPropertyModel other)
    {
        return StringComparer.Ordinal.Equals(VariableName, other.VariableName) &&
            StringComparer.Ordinal.Equals(PropertyName, other.PropertyName) &&
            StringComparer.Ordinal.Equals(PropertyXmlSummary, other.PropertyXmlSummary) &&
            VariableValueType == other.VariableValueType &&
            VariableValueCount == other.VariableValueCount;
    }

    public override int GetHashCode()
    {
        var hc = new HashCode();

        hc.Add(VariableName, StringComparer.Ordinal);
        hc.Add(PropertyName, StringComparer.Ordinal);
        hc.Add(PropertyXmlSummary, StringComparer.Ordinal);
        hc.Add(VariableValueType);
        hc.Add(VariableValueCount);

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
