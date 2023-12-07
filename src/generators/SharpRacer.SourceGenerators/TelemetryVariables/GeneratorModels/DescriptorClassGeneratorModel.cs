using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using SharpRacer.SourceGenerators.TelemetryVariables.InputModels;

namespace SharpRacer.SourceGenerators.TelemetryVariables.GeneratorModels;
internal class DescriptorClassGeneratorModel
{
    public DescriptorClassGeneratorModel(ClassWithGeneratorAttribute generatorClassResult, ImmutableArray<DescriptorPropertyModel> descriptorProperties)
    {
        TypeName = generatorClassResult.ClassSymbol.Name;
        TypeNamespace = generatorClassResult.ClassSymbol.ContainingNamespace.ToString();
        GeneratorAttributeLocation = generatorClassResult.AttributeLocation;
        DescriptorProperties = descriptorProperties;
    }

    public ImmutableArray<DescriptorPropertyModel> DescriptorProperties { get; }
    public Location? GeneratorAttributeLocation { get; }
    public string TypeName { get; }
    public string TypeNamespace { get; }

    internal class EqualityComparer : IEqualityComparer<DescriptorClassGeneratorModel?>
    {
        private EqualityComparer() { }

        public static IEqualityComparer<DescriptorClassGeneratorModel?> Default { get; } = new EqualityComparer();

        public bool Equals(DescriptorClassGeneratorModel? x, DescriptorClassGeneratorModel? y)
        {
            if (ReferenceEquals(x, y))
            {
                return true;
            }

            if (x is null || y is null)
            {
                return false;
            }

            if (x.GeneratorAttributeLocation != y.GeneratorAttributeLocation)
            {
                return false;
            }

            if (!StringComparer.Ordinal.Equals(x.TypeName, y.TypeName))
            {
                return false;
            }

            if (!StringComparer.Ordinal.Equals(x.TypeNamespace, y.TypeNamespace))
            {
                return false;
            }

            return x.DescriptorProperties.SequenceEqual(y.DescriptorProperties);
        }

        public int GetHashCode(DescriptorClassGeneratorModel? obj)
        {
            var hc = new HashCode();

            if (obj is null)
            {
                return hc.ToHashCode();
            }

            hc.Add(obj.TypeName, StringComparer.Ordinal);
            hc.Add(obj.TypeNamespace, StringComparer.Ordinal);
            hc.Add(obj.GeneratorAttributeLocation);

            for (int i = 0; i < obj.DescriptorProperties.Length; i++)
            {
                hc.Add(obj.DescriptorProperties[i]);
            }

            return hc.ToHashCode();
        }
    }
}
