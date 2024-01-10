﻿namespace SharpRacer.SourceGenerators.TelemetryVariables.GeneratorModels;
internal readonly struct VariableClassGeneratorOptions : IEquatable<VariableClassGeneratorOptions>
{
    public VariableClassGeneratorOptions(
        bool isGeneratorEnabled,
        string targetNamespace)
    {
        IsGeneratorEnabled = isGeneratorEnabled;
        TargetNamespace = !string.IsNullOrEmpty(targetNamespace) ? targetNamespace : "SharpRacer.Telemetry.Variables.Generated";

        ClassNameFormat = "{0}Variable";
    }

    public readonly string ClassNameFormat { get; }
    public readonly bool IsGeneratorEnabled { get; }
    public readonly string TargetNamespace { get; }

    public readonly string FormatClassName(string className)
    {
        return string.Format(ClassNameFormat, className);
    }

    public override bool Equals(object obj)
    {
        return obj is VariableClassGeneratorOptions other && Equals(other);
    }

    public bool Equals(VariableClassGeneratorOptions other)
    {
        return IsGeneratorEnabled == other.IsGeneratorEnabled &&
            StringComparer.Ordinal.Equals(ClassNameFormat, other.ClassNameFormat) &&
            StringComparer.Ordinal.Equals(TargetNamespace, other.TargetNamespace);
    }

    public override int GetHashCode()
    {
        var hc = new HashCode();

        hc.Add(IsGeneratorEnabled);
        hc.Add(TargetNamespace);
        hc.Add(ClassNameFormat);

        return hc.ToHashCode();
    }
}
