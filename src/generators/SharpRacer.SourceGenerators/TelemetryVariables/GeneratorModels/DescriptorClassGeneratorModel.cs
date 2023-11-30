using System.Collections.Immutable;
using SharpRacer.SourceGenerators.TelemetryVariables.Pipeline;

namespace SharpRacer.SourceGenerators.TelemetryVariables.GeneratorModels;
internal class DescriptorClassGeneratorModel
{
    public DescriptorClassGeneratorModel(DescriptorClassGeneratorInfo target, ImmutableArray<VariableModel> variables)
    {
        if (target is null)
        {
            throw new ArgumentNullException(nameof(target));
        }

        TypeName = target.TargetClassSymbol.Name;
        TypeNamespace = target.TargetClassSymbol.ContainingNamespace.ToString();
        Variables = variables;
    }

    public string TypeName { get; }
    public string TypeNamespace { get; }
    public ImmutableArray<VariableModel> Variables { get; }
}
