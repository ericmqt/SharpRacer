namespace SharpRacer.SourceGenerators.TelemetryVariables.GeneratorModels;
internal readonly struct DescriptorPropertyReference : IEquatable<DescriptorPropertyReference>
{
    public DescriptorPropertyReference(DescriptorClassModel generatorModel, DescriptorPropertyModel propertyModel)
    {
        if (generatorModel is null)
        {
            throw new ArgumentNullException(nameof(generatorModel));
        }

        if (propertyModel == default)
        {
            throw new ArgumentException($"'{nameof(propertyModel)}' cannot be a default value.", nameof(propertyModel));
        }

        DescriptorClassNamespace = generatorModel.TypeNamespace;
        DescriptorClassName = generatorModel.TypeName;
        PropertyName = propertyModel.PropertyName;
        VariableName = propertyModel.VariableInfo.Name;
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
