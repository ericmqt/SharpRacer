using Microsoft.CodeAnalysis;

namespace SharpRacer.SourceGenerators.TelemetryVariables.Pipeline;
internal class DescriptorClassGeneratorTarget
{
    public DescriptorClassGeneratorTarget(DescriptorClassGeneratorInfo classTypeInfo)
    {
        TargetClassSymbol = classTypeInfo.TargetClassSymbol;
        GeneratorAttributeLocation = classTypeInfo.GeneratorAttributeLocation;
    }

    public INamedTypeSymbol TargetClassSymbol { get; }
    public Location? GeneratorAttributeLocation { get; }
}
