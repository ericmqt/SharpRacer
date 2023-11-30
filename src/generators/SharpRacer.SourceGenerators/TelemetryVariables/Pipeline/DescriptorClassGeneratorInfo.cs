using Microsoft.CodeAnalysis;
using SharpRacer.SourceGenerators.Syntax;

namespace SharpRacer.SourceGenerators.TelemetryVariables.Pipeline;
internal class DescriptorClassGeneratorInfo : ClassGeneratorInfo
{
    public DescriptorClassGeneratorInfo(
        INamedTypeSymbol targetClassSymbol,
        AttributeData generatorAttributeData,
        Location? generatorAttributeLocation)
        : base(targetClassSymbol, generatorAttributeData, generatorAttributeLocation)
    {
    }

    public static IncrementalValuesProvider<DescriptorClassGeneratorInfo> GetValuesProvider(
       SyntaxValueProvider syntaxValueProvider)
    {
        return GetValuesProvider(
            syntaxValueProvider,
            GenerateDataVariableDescriptorsAttributeInfo.FullTypeName,
            predicate: classDecl => classDecl.HasAttributes() && classDecl.IsStaticPartialClass(),
            factory: (targetClassSymbol, attributeData, attributeLocation) =>
            {
                return new DescriptorClassGeneratorInfo(targetClassSymbol, attributeData, attributeLocation);
            },
            EqualityComparer.Default);
    }

    internal class EqualityComparer : IEqualityComparer<DescriptorClassGeneratorInfo?>
    {
        private EqualityComparer() { }

        public static IEqualityComparer<DescriptorClassGeneratorInfo?> Default { get; } = new EqualityComparer();

        public bool Equals(DescriptorClassGeneratorInfo? x, DescriptorClassGeneratorInfo? y)
        {
            if (ReferenceEquals(x, y))
            {
                return true;
            }

            if (x is null || y is null)
            {
                return false;
            }

            return SymbolEqualityComparer.Default.Equals(x.TargetClassSymbol, y.TargetClassSymbol) &&
                //SymbolEqualityComparer.Default.Equals(x.GeneratorAttributeData.AttributeClass, y.GeneratorAttributeData.AttributeClass) &&
                x.GeneratorAttributeData == y.GeneratorAttributeData &&
                x.GeneratorAttributeLocation == y.GeneratorAttributeLocation;
        }

        public int GetHashCode(DescriptorClassGeneratorInfo? obj)
        {
            var hc = new HashCode();

            if (obj is null)
            {
                return hc.ToHashCode();
            }

            hc.Add(obj.TargetClassSymbol, SymbolEqualityComparer.Default);
            hc.Add(obj.GeneratorAttributeData);
            hc.Add(obj.GeneratorAttributeLocation);

            return hc.ToHashCode();
        }
    }
}
