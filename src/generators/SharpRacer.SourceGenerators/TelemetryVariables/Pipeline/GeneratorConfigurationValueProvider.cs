using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using SharpRacer.SourceGenerators.TelemetryVariables.InputModels;

namespace SharpRacer.SourceGenerators.TelemetryVariables.Pipeline;
internal static class GeneratorConfigurationValueProvider
{
    public static IncrementalValueProvider<GeneratorConfiguration> GetValueProvider(
        IncrementalValueProvider<AnalyzerConfigOptionsProvider> analyzerConfigOptionsProvider)
    {
        return analyzerConfigOptionsProvider.Select(static (x, _) => GetValue(x))
            .WithTrackingName(TrackingNames.GeneratorConfigurationValueProvider_GetValueProvider);
    }

    public static GeneratorConfiguration GetValue(AnalyzerConfigOptionsProvider analyzerOptionsProvider)
    {
        var generateTypedVariableClassesPropertyValue = analyzerOptionsProvider.GlobalOptions
            .GetMSBuildProperty(MSBuildProperties.GenerateVariableClassesKey);

        bool generateTypedVariableClasses = generateTypedVariableClassesPropertyValue
            .GetBooleanOrDefault(GeneratorConfigurationDefaults.GenerateTypedVariableClasses);

        var variableClassesNamespace = GetVariableClassesNamespace(analyzerOptionsProvider);

        // Get VariableInfoFileName
        var variableInfoFileNameProperty = analyzerOptionsProvider.GlobalOptions
            .GetMSBuildProperty(MSBuildProperties.VariableInfoFileNameKey);

        var variableInfoFileName = variableInfoFileNameProperty.Exists
            ? new VariableInfoFileName(variableInfoFileNameProperty.Value!)
            : GeneratorConfigurationDefaults.VariableInfoFileName;

        // Get VariableOptionsFileName
        var variableOptionsFileNameProperty = analyzerOptionsProvider.GlobalOptions
            .GetMSBuildProperty(MSBuildProperties.VariableOptionsFileNameKey);

        var variableOptionsFileName = variableOptionsFileNameProperty.Exists
            ? new VariableOptionsFileName(variableOptionsFileNameProperty.Value!)
            : GeneratorConfigurationDefaults.VariableOptionsFileName;

        return new GeneratorConfiguration(
            variableInfoFileName,
            variableOptionsFileName,
            generateTypedVariableClasses,
            variableClassesNamespace);
    }

    private static string GetVariableClassesNamespace(AnalyzerConfigOptionsProvider analyzerOptionsProvider)
    {
        var defaultValue = GeneratorConfigurationDefaults.TelemetryVariableClassesNamespace;

        var variableClassesNamespacePropertyValue = analyzerOptionsProvider.GlobalOptions
            .GetMSBuildProperty(MSBuildProperties.VariableClassesNamespaceKey);

        // Fall back to the project's RootNamespace property
        if (!variableClassesNamespacePropertyValue.Exists)
        {
            return analyzerOptionsProvider.GlobalOptions
                .GetMSBuildProperty(MSBuildProperties.RootNamespace)
                .GetValueOrDefault(defaultValue);
        }

        return variableClassesNamespacePropertyValue.GetValueOrDefault(defaultValue);
    }
}
