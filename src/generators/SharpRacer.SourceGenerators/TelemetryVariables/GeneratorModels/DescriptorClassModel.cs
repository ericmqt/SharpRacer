using System.Collections.Immutable;
using Microsoft.CodeAnalysis;

namespace SharpRacer.SourceGenerators.TelemetryVariables.GeneratorModels;
public readonly struct DescriptorClassModel : IEquatable<DescriptorClassModel>
{
    public DescriptorClassModel(string typeName, string typeNamespace, Location generatorAttributeLocation)
    {
        TypeName = !string.IsNullOrEmpty(typeName)
            ? typeName
            : throw new ArgumentException($"'{nameof(typeName)}' cannot be null or empty.", nameof(typeName));

        TypeNamespace = !string.IsNullOrEmpty(typeNamespace)
            ? typeNamespace
            : throw new ArgumentException($"'{nameof(typeNamespace)}' cannot be null or empty.", nameof(typeNamespace));

        GeneratorAttributeLocation = generatorAttributeLocation;

        DescriptorProperties = ImmutableArray<DescriptorPropertyModel>.Empty;
    }

    private DescriptorClassModel(
        string typeName,
        string typeNamespace,
        Location generatorAttributeLocation,
        ImmutableArray<DescriptorPropertyModel> descriptorProperties)
    {
        TypeName = typeName;
        TypeNamespace = typeNamespace;
        GeneratorAttributeLocation = generatorAttributeLocation;
        DescriptorProperties = descriptorProperties.GetEmptyIfDefault();
    }

    public readonly ImmutableArray<DescriptorPropertyModel> DescriptorProperties { get; }
    public readonly Location GeneratorAttributeLocation { get; }
    public readonly string TypeName { get; }
    public readonly string TypeNamespace { get; }

    public DescriptorClassModel WithDescriptorProperties(ImmutableArray<DescriptorPropertyModel> descriptorProperties)
    {
        return new DescriptorClassModel(
            TypeName,
            TypeNamespace,
            GeneratorAttributeLocation,
            descriptorProperties.GetEmptyIfDefault());
    }

    public override bool Equals(object obj)
    {
        return obj is DescriptorClassModel other && Equals(other);
    }

    public bool Equals(DescriptorClassModel other)
    {
        return StringComparer.Ordinal.Equals(TypeName, other.TypeName) &&
            StringComparer.Ordinal.Equals(TypeNamespace, other.TypeNamespace) &&
            GeneratorAttributeLocation == other.GeneratorAttributeLocation &&
            DescriptorProperties.SequenceEqualDefaultTolerant(other.DescriptorProperties);
    }

    public override int GetHashCode()
    {
        var hc = new HashCode();

        hc.Add(TypeName);
        hc.Add(TypeNamespace);
        hc.Add(GeneratorAttributeLocation);

        if (!DescriptorProperties.IsDefault)
        {
            for (int i = 0; i < DescriptorProperties.Length; ++i)
            {
                hc.Add(DescriptorProperties[i]);
            }
        }

        return hc.ToHashCode();
    }

    public static bool operator ==(DescriptorClassModel left, DescriptorClassModel right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(DescriptorClassModel left, DescriptorClassModel right)
    {
        return !left.Equals(right);
    }
}
