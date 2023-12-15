using System.Collections.Immutable;

namespace SharpRacer.SourceGenerators.TelemetryVariables.GeneratorModels;
internal readonly struct VariableClassGeneratorOptions : IEquatable<VariableClassGeneratorOptions>
{
    public VariableClassGeneratorOptions(
        bool isGeneratorEnabled,
        string targetNamespace,
        ImmutableArray<DescriptorPropertyReference> descriptorPropertyReferences)
    {
        IsGeneratorEnabled = isGeneratorEnabled;
        TargetNamespace = !string.IsNullOrEmpty(targetNamespace) ? targetNamespace : "SharpRacer.Telemetry.Variables.Generated";

        DescriptorPropertyReferences = !descriptorPropertyReferences.IsDefault
            ? descriptorPropertyReferences
            : ImmutableArray<DescriptorPropertyReference>.Empty;

        ClassNameFormat = "{0}Variable";
    }

    public readonly string ClassNameFormat { get; }
    public readonly ImmutableArray<DescriptorPropertyReference> DescriptorPropertyReferences { get; }
    public readonly bool IsGeneratorEnabled { get; }
    public readonly string TargetNamespace { get; }

    public string FormatClassName(string className)
    {
        return string.Format(ClassNameFormat, className);
    }

    public DescriptorPropertyReference? GetDescriptorPropertyReference(ref readonly VariableModel variableModel)
    {
        var variableKey = variableModel.VariableInfo.Name;

        if (DescriptorPropertyReferences.Any(x => x.VariableName.Equals(variableKey, StringComparison.Ordinal)))
        {
            return DescriptorPropertyReferences.First(x => x.VariableName.Equals(variableKey, StringComparison.Ordinal));
        }

        return null;
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
        return obj is VariableClassGeneratorOptions other && Equals(other);
    }

    public bool Equals(VariableClassGeneratorOptions other)
    {
        return IsGeneratorEnabled == other.IsGeneratorEnabled &&
            StringComparer.Ordinal.Equals(ClassNameFormat, other.ClassNameFormat) &&
            StringComparer.Ordinal.Equals(TargetNamespace, other.TargetNamespace) &&
            DescriptorPropertyReferences.SequenceEqual(other.DescriptorPropertyReferences);
    }

    public override int GetHashCode()
    {
        var hc = new HashCode();

        hc.Add(IsGeneratorEnabled);
        hc.Add(TargetNamespace);
        hc.Add(ClassNameFormat);

        for (int i = 0; i < DescriptorPropertyReferences.Length; i++)
        {
            hc.Add(DescriptorPropertyReferences[i]);
        }

        return hc.ToHashCode();
    }
}
