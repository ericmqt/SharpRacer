using System.Diagnostics.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using SharpRacer.SourceGenerators.TelemetryVariables;
using SharpRacer.SourceGenerators.Testing.Configuration;

namespace SharpRacer.SourceGenerators.Testing.TelemetryVariables;
public class VariablesGeneratorGlobalOptions : AnalyzerConfigOptions
{
    private readonly MSBuildProperty _generateVariableClassesProperty;
    private readonly MSBuildProperty _rootNamespaceProperty;
    private readonly MSBuildProperty _variableClassesNamespaceProperty;
    private readonly MSBuildProperty _variableInfoFileNameProperty;
    private readonly MSBuildProperty _variableOptionsFileNameProperty;

    private readonly List<MSBuildProperty> _msBuildProperties;

    public VariablesGeneratorGlobalOptions()
    {
        _generateVariableClassesProperty = new MSBuildProperty(MSBuildProperties.GenerateVariableClassesKey);
        _rootNamespaceProperty = new MSBuildProperty(MSBuildProperties.RootNamespace);
        _variableClassesNamespaceProperty = new MSBuildProperty(MSBuildProperties.VariableClassesNamespaceKey);
        _variableInfoFileNameProperty = new MSBuildProperty(MSBuildProperties.VariableInfoFileNameKey);
        _variableOptionsFileNameProperty = new MSBuildProperty(MSBuildProperties.VariableOptionsFileNameKey);

        _msBuildProperties =
        [
            _generateVariableClassesProperty,
            _rootNamespaceProperty,
            _variableClassesNamespaceProperty,
            _variableInfoFileNameProperty,
            _variableOptionsFileNameProperty,
        ];
    }

    public VariablesGeneratorGlobalOptions(VariablesGeneratorGlobalOptionsValues optionsValues)
    {
        _generateVariableClassesProperty = new MSBuildProperty(
            MSBuildProperties.GenerateVariableClassesKey, optionsValues.GenerateTelemetryVariableClasses);

        _rootNamespaceProperty = new MSBuildProperty(MSBuildProperties.RootNamespace, optionsValues.RootNamespace);

        _variableClassesNamespaceProperty = new MSBuildProperty(
            MSBuildProperties.VariableClassesNamespaceKey, optionsValues.VariableClassesNamespace);

        _variableInfoFileNameProperty = new MSBuildProperty(
            MSBuildProperties.VariableInfoFileNameKey, optionsValues.VariableInfoFileName);

        _variableOptionsFileNameProperty = new MSBuildProperty(
            MSBuildProperties.VariableOptionsFileNameKey, optionsValues.VariableOptionsFileName);

        _msBuildProperties =
        [
            _generateVariableClassesProperty,
            _rootNamespaceProperty,
            _variableClassesNamespaceProperty,
            _variableInfoFileNameProperty,
            _variableOptionsFileNameProperty,
        ];
    }

    public VariablesGeneratorGlobalOptionsValues GetOptionsValues()
    {
        return new VariablesGeneratorGlobalOptionsValues
        {
            GenerateTelemetryVariableClasses = _generateVariableClassesProperty.PropertyValue.Value,
            RootNamespace = _rootNamespaceProperty.PropertyValue.Value,
            VariableClassesNamespace = _variableClassesNamespaceProperty.PropertyValue.Value,
            VariableInfoFileName = _variableInfoFileNameProperty.PropertyValue.Value,
            VariableOptionsFileName = _variableOptionsFileNameProperty.PropertyValue.Value
        };
    }

    internal VariablesGeneratorGlobalOptions Mutate(Action<VariablesGeneratorGlobalOptionsValues>? configure)
    {
        var optionsValues = GetOptionsValues();

        configure?.Invoke(optionsValues);

        return new VariablesGeneratorGlobalOptions(optionsValues);
    }

    public override bool TryGetValue(string key, [NotNullWhen(true)] out string? value)
    {
        var option = _msBuildProperties.SingleOrDefault(x => x.PropertyKey.Key.Equals(key, StringComparison.Ordinal));

        if (option is null || !option.PropertyValue.Exists)
        {
            value = null;
            return false;
        }

        value = option.PropertyValue.Value!;
        return true;
    }
}
