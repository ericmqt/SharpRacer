using System.Collections.Immutable;
using Microsoft.CodeAnalysis;

namespace SharpRacer.SourceGenerators.TelemetryVariables.GeneratorModels;
internal readonly struct DescriptorClassModel : IEquatable<DescriptorClassModel>
{
    public DescriptorClassModel(INamedTypeSymbol classSymbol, Location? generatorAttributeLocation)
    {
        TypeName = classSymbol.Name;
        TypeNamespace = classSymbol.ContainingNamespace.ToString();
        GeneratorAttributeLocation = generatorAttributeLocation;

        DescriptorProperties = ImmutableArray<DescriptorPropertyModel>.Empty;
    }

    private DescriptorClassModel(
        string typeName,
        string typeNamespace,
        Location? generatorAttributeLocation,
        ImmutableArray<DescriptorPropertyModel> descriptorProperties)
    {
        TypeName = typeName;
        TypeNamespace = typeNamespace;
        GeneratorAttributeLocation = generatorAttributeLocation;
        DescriptorProperties = descriptorProperties.GetEmptyIfDefault();
    }

    public readonly ImmutableArray<DescriptorPropertyModel> DescriptorProperties { get; }
    public readonly Location? GeneratorAttributeLocation { get; }
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
            DescriptorProperties.GetEmptyIfDefault().SequenceEqual(other.DescriptorProperties.GetEmptyIfDefault());
    }

    public override int GetHashCode()
    {
        var hc = new HashCode();

        hc.Add(TypeName);
        hc.Add(TypeNamespace);
        hc.Add(GeneratorAttributeLocation);

        if (!DescriptorProperties.IsDefaultOrEmpty)
        {
            for (int i = 0; i < DescriptorProperties.Length; i++)
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
