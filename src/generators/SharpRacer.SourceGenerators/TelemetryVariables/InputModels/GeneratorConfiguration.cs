﻿using Microsoft.CodeAnalysis.Diagnostics;

namespace SharpRacer.SourceGenerators.TelemetryVariables.InputModels;
internal readonly struct GeneratorConfiguration : IEquatable<GeneratorConfiguration>
{
    public GeneratorConfiguration(
        VariableInfoFileName variableInfoFileName,
        VariableOptionsFileName variableOptionsFileName,
        bool generateTypedVariableClasses,
        string? telemetryVariableClassesNamespace)
    {
        GenerateTypedVariableClasses = generateTypedVariableClasses;
        VariableInfoFileName = variableInfoFileName;
        VariableOptionsFileName = variableOptionsFileName;

        TelemetryVariableClassesNamespace = telemetryVariableClassesNamespace ?? GeneratorConfigurationDefaults.TelemetryVariableClassesNamespace;
    }

    public bool GenerateTypedVariableClasses { get; }
    public string TelemetryVariableClassesNamespace { get; }
    public VariableInfoFileName VariableInfoFileName { get; }
    public VariableOptionsFileName VariableOptionsFileName { get; }

    public static GeneratorConfiguration FromAnalyzerConfigOptionsProvider(AnalyzerConfigOptionsProvider analyzerOptionsProvider)
    {
        if (!analyzerOptionsProvider.GlobalOptions.TryGetBool(
            BuildPropertyKeys.GenerateVariableClassesProperty,
            out var generateTypedVariableClasses))
        {
            generateTypedVariableClasses = GeneratorConfigurationDefaults.GenerateTypedVariableClasses;
        }

        var telemetryVariableClassesNamespace = GetTelemetryVariableClassesNamespace(analyzerOptionsProvider);

        var variableInfoFileName = VariableInfoFileName.GetFromConfigurationOrDefault(analyzerOptionsProvider);
        var variableOptionsFileName = VariableOptionsFileName.FromConfigurationOrDefault(analyzerOptionsProvider);

        return new GeneratorConfiguration(
            variableInfoFileName,
            variableOptionsFileName,
            generateTypedVariableClasses,
            telemetryVariableClassesNamespace);
    }

    private static string GetTelemetryVariableClassesNamespace(AnalyzerConfigOptionsProvider analyzerOptionsProvider)
    {
        if (analyzerOptionsProvider.GlobalOptions.TryGetValue(
            BuildPropertyKeys.TelemetryVariableClassesNamespaceProperty,
            out var configuredNamespaceValue))
        {
            if (!string.IsNullOrEmpty(configuredNamespaceValue))
            {
                return configuredNamespaceValue;
            }
        }

        if (analyzerOptionsProvider.GlobalOptions.TryGetMSBuildProperty("RootNamespace", out var rootNamespace))
        {
            return rootNamespace;
        }

        return GeneratorConfigurationDefaults.TelemetryVariableClassesNamespace;
    }

    public override bool Equals(object? obj)
    {
        return obj is GeneratorConfiguration other && Equals(other);
    }

    public bool Equals(GeneratorConfiguration other)
    {
        return GenerateTypedVariableClasses == other.GenerateTypedVariableClasses &&
            StringComparer.Ordinal.Equals(TelemetryVariableClassesNamespace, other.TelemetryVariableClassesNamespace) &&
            VariableInfoFileName == other.VariableInfoFileName &&
            VariableOptionsFileName == other.VariableOptionsFileName;
    }

    public override int GetHashCode()
    {
        var hc = new HashCode();

        hc.Add(GenerateTypedVariableClasses);
        hc.Add(TelemetryVariableClassesNamespace);
        hc.Add(VariableInfoFileName);
        hc.Add(VariableOptionsFileName);

        return hc.ToHashCode();
    }

    public static bool operator ==(GeneratorConfiguration left, GeneratorConfiguration right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(GeneratorConfiguration left, GeneratorConfiguration right)
    {
        return !(left == right);
    }
}
