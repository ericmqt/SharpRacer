using System.Collections.Immutable;
using SharpRacer.SourceGenerators.TelemetryVariables.GeneratorModels;
using SharpRacer.SourceGenerators.TelemetryVariables.InputModels;

namespace SharpRacer.SourceGenerators.TelemetryVariables.Pipeline;
internal readonly struct TypedVariableClassesDescriptorOptions : IEquatable<TypedVariableClassesDescriptorOptions>
{
    public TypedVariableClassesDescriptorOptions()
    {
        IsGeneratorEnabled = false;
        TargetNamespace = "SharpRacer.Telemetry.Variables.Generated";
        DescriptorPropertyReferences = ImmutableArray<DescriptorPropertyReference>.Empty;
    }

    public TypedVariableClassesDescriptorOptions(
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

    public static TypedVariableClassesDescriptorOptions Create(GeneratorConfiguration generatorConfiguration, DescriptorClassGeneratorProvider descriptorGeneratorProvider)
    {
        return new TypedVariableClassesDescriptorOptions(
            generatorConfiguration.GenerateTypedVariableClasses,
            generatorConfiguration.TelemetryVariableClassesNamespace,
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
        return obj is TypedVariableClassesDescriptorOptions other && Equals(other);
    }

    public bool Equals(TypedVariableClassesDescriptorOptions other)
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
