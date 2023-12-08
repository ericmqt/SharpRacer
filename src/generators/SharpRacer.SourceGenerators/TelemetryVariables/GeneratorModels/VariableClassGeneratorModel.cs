using SharpRacer.SourceGenerators.TelemetryVariables.Pipeline;

namespace SharpRacer.SourceGenerators.TelemetryVariables.GeneratorModels;
internal class VariableClassGeneratorModel
{
    public VariableClassGeneratorModel(
        string typeName,
        string typeNamespace,
        DescriptorPropertyReference? descriptorPropertyReference)
    {
        TypeName = !string.IsNullOrEmpty(typeName)
            ? typeName
            : throw new ArgumentException($"'{nameof(typeName)}' cannot be null or empty.", nameof(typeName));

        TypeNamespace = !string.IsNullOrEmpty(typeNamespace)
            ? typeNamespace
            : throw new ArgumentException($"'{nameof(typeNamespace)}' cannot be null or empty.", nameof(typeNamespace));

        DescriptorPropertyReference = descriptorPropertyReference;
    }

    public DescriptorPropertyReference? DescriptorPropertyReference { get; }
    public string TypeName { get; }
    public string TypeNamespace { get; }

    public static VariableClassGeneratorModel Create(VariableModel variableModel, TypedVariableClassesDescriptorOptions generatorOptions)
    {
        if (generatorOptions.TryGetDescriptorPropertyReference(ref variableModel, out var descriptorPropertyReference))
        {
            return new VariableClassGeneratorModel(
                $"{variableModel.VariableName}Variable",
                generatorOptions.TargetNamespace,
                descriptorPropertyReference);
        }

        return new VariableClassGeneratorModel(
            $"{variableModel.VariableName}Variable",
            generatorOptions.TargetNamespace,
            null);
    }
}
