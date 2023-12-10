using System.Collections.Immutable;
using SharpRacer.SourceGenerators.TelemetryVariables.InputModels;
using SharpRacer.SourceGenerators.TelemetryVariables.Pipeline;

namespace SharpRacer.SourceGenerators.TelemetryVariables.GeneratorModels;
internal readonly struct TypedVariableClassesGeneratorOptions : IEquatable<TypedVariableClassesGeneratorOptions>
{
    public TypedVariableClassesGeneratorOptions()
    {
        IsGeneratorEnabled = false;
        TargetNamespace = "SharpRacer.Telemetry.Variables.Generated";
        DescriptorPropertyReferences = ImmutableArray<DescriptorPropertyReference>.Empty;
    }

    public TypedVariableClassesGeneratorOptions(
        bool isGeneratorEnabled,
        string targetNamespace,
        ImmutableArray<DescriptorPropertyReference> descriptorPropertyReferences)
    {
        IsGeneratorEnabled = isGeneratorEnabled;
        TargetNamespace = !string.IsNullOrEmpty(targetNamespace) ? targetNamespace : "SharpRacer.Telemetry.Variables.Generated";

        DescriptorPropertyReferences = !descriptorPropertyReferences.IsDefault
            ? descriptorPropertyReferences
            : ImmutableArray<DescriptorPropertyReference>.Empty;
    }

    public readonly ImmutableArray<DescriptorPropertyReference> DescriptorPropertyReferences { get; }
    public readonly bool IsGeneratorEnabled { get; }
    public readonly string TargetNamespace { get; }

    public static TypedVariableClassesGeneratorOptions Create(GeneratorConfiguration generatorConfiguration, DescriptorClassGeneratorProvider descriptorGeneratorProvider)
    {
        return new TypedVariableClassesGeneratorOptions(
            generatorConfiguration.GenerateVariableClasses,
            generatorConfiguration.VariableClassesNamespace,
            descriptorGeneratorProvider.DescriptorPropertyReferences);
    }

    public bool TryGetDescriptorPropertyReference(ref readonly VariableModel variableModel, out DescriptorPropertyReference descriptorPropertyReference)
    {
        var variableKey = variableModel.VariableInfo.Name;

        if (DescriptorPropertyReferences.Any(x => x.VariableName.Equals(variableKey, StringComparison.Ordinal)))
        {
            descriptorPropertyReference = DescriptorPropertyReferences.First(x => x.VariableName.Equals(variableKey, StringComparison.Ordinal));
            return true;
        }

        descriptorPropertyReference = default;
        return false;
    }

    public override bool Equals(object obj)
    {
        return obj is TypedVariableClassesGeneratorOptions other && Equals(other);
    }

    public bool Equals(TypedVariableClassesGeneratorOptions other)
    {
        return IsGeneratorEnabled == other.IsGeneratorEnabled &&
            StringComparer.Ordinal.Equals(TargetNamespace, other.TargetNamespace) &&
            DescriptorPropertyReferences.SequenceEqual(other.DescriptorPropertyReferences);
    }

    public override int GetHashCode()
    {
        var hc = new HashCode();

        hc.Add(IsGeneratorEnabled);
        hc.Add(TargetNamespace, StringComparer.Ordinal);

        for (int i = 0; i < DescriptorPropertyReferences.Length; i++)
        {
            hc.Add(DescriptorPropertyReferences[i]);
        }

        return hc.ToHashCode();
    }
}
