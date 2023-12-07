namespace SharpRacer.SourceGenerators.TelemetryVariables.GeneratorModels;
internal readonly struct DescriptorPropertyReference : IEquatable<DescriptorPropertyReference>
{
    public DescriptorPropertyReference(DescriptorClassGeneratorModel generatorModel, VariableModel variableModel)
    {
        if (generatorModel is null)
        {
            throw new ArgumentNullException(nameof(generatorModel));
        }

        if (variableModel == default)
        {
            throw new ArgumentException($"'{nameof(variableModel)}' cannot be a default value.", nameof(variableModel));
        }

        DescriptorClassNamespace = generatorModel.TypeNamespace;
        DescriptorClassName = generatorModel.TypeName;
        PropertyName = variableModel.DescriptorName;
        VariableName = variableModel.VariableInfo.Name;
    }

    public readonly string DescriptorClassNamespace { get; }
    public readonly string DescriptorClassName { get; }
    public readonly string PropertyName { get; }
    public readonly string VariableName { get; }

    public override bool Equals(object obj)
    {
        return obj is DescriptorPropertyReference other && Equals(other);
    }

    public bool Equals(DescriptorPropertyReference other)
    {
        return StringComparer.Ordinal.Equals(VariableName, other.VariableName) &&
            StringComparer.Ordinal.Equals(PropertyName, other.PropertyName) &&
            StringComparer.Ordinal.Equals(DescriptorClassNamespace, other.DescriptorClassNamespace) &&
            StringComparer.Ordinal.Equals(DescriptorClassName, other.DescriptorClassName);
    }

    public override int GetHashCode()
    {
        var hc = new HashCode();

        hc.Add(DescriptorClassNamespace, StringComparer.Ordinal);
        hc.Add(DescriptorClassName, StringComparer.Ordinal);
        hc.Add(PropertyName, StringComparer.Ordinal);
        hc.Add(VariableName, StringComparer.Ordinal);

        return hc.ToHashCode();
    }

    public static bool operator ==(DescriptorPropertyReference left, DescriptorPropertyReference right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(DescriptorPropertyReference left, DescriptorPropertyReference right)
    {
        return !left.Equals(right);
    }
}
