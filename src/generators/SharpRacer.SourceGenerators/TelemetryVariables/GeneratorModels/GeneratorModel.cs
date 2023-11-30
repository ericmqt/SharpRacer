using System.Collections.Immutable;
using SharpRacer.SourceGenerators.TelemetryVariables.Pipeline;

namespace SharpRacer.SourceGenerators.TelemetryVariables.GeneratorModels;
internal class GeneratorModel
{
    public GeneratorModel(DescriptorClassGeneratorModel? descriptorClassGeneratorModel, ImmutableArray<VariableModel> variables)
    {
        DescriptorClassGeneratorModel = descriptorClassGeneratorModel;
        Variables = variables;
    }

    public DescriptorClassGeneratorModel? DescriptorClassGeneratorModel { get; }
    public ImmutableArray<VariableModel> Variables { get; }

    public static GeneratorModel Create(DescriptorClassGeneratorInfo? descriptorClassGeneratorTarget, ImmutableArray<VariableModel> variables)
    {
        var descriptorClassGeneratorModel = descriptorClassGeneratorTarget != null
            ? new DescriptorClassGeneratorModel(descriptorClassGeneratorTarget, variables)
            : null;

        return new GeneratorModel(descriptorClassGeneratorModel, variables);
    }
}
