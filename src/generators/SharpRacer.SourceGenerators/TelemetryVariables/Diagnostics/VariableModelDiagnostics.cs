using Microsoft.CodeAnalysis;

namespace SharpRacer.SourceGenerators.TelemetryVariables.Diagnostics;
internal class VariableModelDiagnostics
{
    private const string _Category = "SharpRacer.CodeGen";

    public static Diagnostic ConfiguredNameCreatesContextPropertyNameCollision(
        string variableName,
        string descriptorName,
        string definingVariableName,
        string replacementDescriptorName,
        Location? location = null)
    {
        return Diagnostic.Create(
            _ConfiguredNameResultsInContextPropertyNameCollision,
            location,
            variableName,
            descriptorName,
            definingVariableName,
            replacementDescriptorName);
    }

    public static Diagnostic ConfiguredNameCreatesDescriptorNameCollision(
        string variableName,
        string descriptorName,
        string definingVariableName,
        string replacementDescriptorName,
        Location? location = null)
    {
        return Diagnostic.Create(
            _ConfiguredNameResultsInDescriptorNameCollision,
            location,
            variableName,
            descriptorName,
            definingVariableName,
            replacementDescriptorName);
    }

    public static Diagnostic ContextPropertyNameInUse(
        string variableName,
        string contextPropertyName,
        string definingVariableName,
        Location? location = null)
    {
        return Diagnostic.Create(_ContextPropertyNameInUse, location, variableName, contextPropertyName, definingVariableName);
    }

    public static Diagnostic DeprecatingVariableNotFoundWarning(
        string variableName,
        string deprecatingVariableName,
        Location? location = null)
    {
        return Diagnostic.Create(_DeprecatingVariableNotFoundWarning, location, variableName, deprecatingVariableName);
    }

    public static Diagnostic DescriptorNameInUse(
        string variableName,
        string contextPropertyName,
        string definingVariableName,
        Location? location = null)
    {
        return Diagnostic.Create(_DescriptorNameInUse, location, variableName, contextPropertyName, definingVariableName);
    }

    private static readonly DiagnosticDescriptor _ConfiguredNameResultsInContextPropertyNameCollision
        = new(
            DiagnosticIds.VariableModels_ConfiguredNameResultsInContextPropertyNameCollision,
            "Configured variable name results in context property name collision",
            "Variable '{0}' is configured as '{1}' which creates a context property name conflict with variable '{2}'. Context property name '{3}' will be used instead.",
            _Category,
            DiagnosticSeverity.Warning,
            isEnabledByDefault: true);

    private static readonly DiagnosticDescriptor _ConfiguredNameResultsInDescriptorNameCollision
        = new(
            DiagnosticIds.VariableModels_ConfiguredNameResultsInDescriptorNameCollision,
            "Configured variable name results in descriptor name collision",
            "Variable '{0}' is configured as '{1}' which creates a descriptor name conflict with variable '{2}'. Descriptor name '{3}' will be used instead.",
            _Category,
            DiagnosticSeverity.Warning,
            isEnabledByDefault: true);

    private static readonly DiagnosticDescriptor _ContextPropertyNameInUse
        = new DiagnosticDescriptor(
            DiagnosticIds.VariableModels_ContextPropertyNameInUse,
            "Value for ContextPropertyName is already defined by another variable",
            "Variable '{0}' has ContextPropertyName '{1}' but that value is already defined by variable '{2}'",
            _Category,
            DiagnosticSeverity.Error,
            isEnabledByDefault: true);

    private static readonly DiagnosticDescriptor _DeprecatingVariableNotFoundWarning
        = new(
            DiagnosticIds.VariableModels_DeprecatingVariableNotFound,
            "Deprecating variable not found",
            "Variable '{0}' is deprecated but deprecating variable '{1}' was not found",
            _Category,
            DiagnosticSeverity.Warning,
            isEnabledByDefault: true);

    private static readonly DiagnosticDescriptor _DescriptorNameInUse
        = new DiagnosticDescriptor(
            DiagnosticIds.VariableModels_DescriptorNameInUse,
            "Value for DescriptorName is already defined by another variable",
            "Variable '{0}' has DescriptorName '{1}' but that value is already defined by variable '{2}",
            _Category,
            DiagnosticSeverity.Error,
            isEnabledByDefault: true);


}
