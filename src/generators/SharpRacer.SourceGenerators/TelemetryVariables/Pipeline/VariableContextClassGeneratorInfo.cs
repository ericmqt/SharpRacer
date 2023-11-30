using Microsoft.CodeAnalysis;
using SharpRacer.SourceGenerators.Syntax;

namespace SharpRacer.SourceGenerators.TelemetryVariables.Pipeline;
internal class VariableContextClassGeneratorInfo : ClassGeneratorInfo
{
    public VariableContextClassGeneratorInfo(
        INamedTypeSymbol targetClassSymbol,
        AttributeData generatorAttributeData,
        Location? generatorAttributeLocation)
        : base(targetClassSymbol, generatorAttributeData, generatorAttributeLocation)
    {
        IncludedVariableNamesArgumentValue =
            GenerateDataVariablesContextAttributeInfo.FindIncludedVariableNamesFileArgumentValue(generatorAttributeData);
    }

    public string? IncludedVariableNamesArgumentValue { get; }

    public static IncrementalValuesProvider<VariableContextClassGeneratorInfo> GetValuesProvider(
        SyntaxValueProvider syntaxValueProvider)
    {
        return GetValuesProvider(
            syntaxValueProvider,
            GenerateDataVariablesContextAttributeInfo.FullTypeName,
            predicate: classDecl => classDecl.HasAttributes() && classDecl.IsPartialClass() && !classDecl.IsStaticClass(),
            factory: (targetClassSymbol, attributeData, attributeLocation) =>
            {
                return new VariableContextClassGeneratorInfo(targetClassSymbol, attributeData, attributeLocation);
            },
            EqualityComparer.Default);
    }

    internal class EqualityComparer : IEqualityComparer<VariableContextClassGeneratorInfo?>
    {
        private EqualityComparer() { }

        public static IEqualityComparer<VariableContextClassGeneratorInfo?> Default { get; } = new EqualityComparer();

        public bool Equals(VariableContextClassGeneratorInfo? x, VariableContextClassGeneratorInfo? y)
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
                x.GeneratorAttributeLocation == y.GeneratorAttributeLocation &&
                StringComparer.Ordinal.Equals(x.IncludedVariableNamesArgumentValue, y.IncludedVariableNamesArgumentValue);
        }

        public int GetHashCode(VariableContextClassGeneratorInfo? obj)
        {
            var hc = new HashCode();

            if (obj is null)
            {
                return hc.ToHashCode();
            }

            hc.Add(obj.TargetClassSymbol, SymbolEqualityComparer.Default);
            hc.Add(obj.GeneratorAttributeData);
            hc.Add(obj.GeneratorAttributeLocation);
            hc.Add(obj.IncludedVariableNamesArgumentValue);

            return hc.ToHashCode();
        }
    }
}
