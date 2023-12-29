using Microsoft.CodeAnalysis;
using SharpRacer.SourceGenerators.TelemetryVariables.GeneratorModels;
using SharpRacer.SourceGenerators.TelemetryVariables.InputModels;

namespace SharpRacer.SourceGenerators.TelemetryVariables.Diagnostics;
internal static partial class GeneratorDiagnostics
{
    public static string DefaultCategory = "SharpRacer.SourceGenerators";

    private static readonly DiagnosticDescriptor _VariableClassNameInUse
        = new DiagnosticDescriptor(
            DiagnosticIds.VariableClassNameInUse,
            "Variable class name in use by another variable",
            "Variable class '{0}' for variable '{1}' is in use by variable '{2}'",
            DefaultCategory,
            DiagnosticSeverity.Error,
            isEnabledByDefault: true);

    private static readonly DiagnosticDescriptor _VariableNameInUseByVariable
        = new DiagnosticDescriptor(
            "SR1020",
            "Variable name is already defined",
            "Variable '{0}' is configured with name '{1}' but the variable name is already defined by variable '{2}'",
            DefaultCategory,
            DiagnosticSeverity.Error,
            isEnabledByDefault: true);

    private static readonly DiagnosticDescriptor _VariableContextPropertyNameInUseByVariable
        = new DiagnosticDescriptor(
            "SR1021",
            "Variable context property name is already defined",
            "Variable '{0}' is configured with context property name '{1}' but the property name is already defined by variable '{2}'",
            DefaultCategory,
            DiagnosticSeverity.Error,
            isEnabledByDefault: true);

    private static readonly DiagnosticDescriptor _VariableDescriptorNameInUseByVariable
        = new DiagnosticDescriptor(
            "SR1022",
            "Variable descriptor name is already defined",
            "Variable '{0}' is configured with descriptor name '{1}' but the descriptor name is already defined by variable '{2}'",
            DefaultCategory,
            DiagnosticSeverity.Error,
            isEnabledByDefault: true);

    private static readonly DiagnosticDescriptor _PlaceholderDiagnostic
        = new DiagnosticDescriptor(
            "SR9999",
            "Placeholder diagnostic",
            "Placeholder diagnostic",
            DefaultCategory,
            DiagnosticSeverity.Error,
            isEnabledByDefault: true);

    public static Diagnostic Placeholder(Location? location = null)
    {
        return Diagnostic.Create(_PlaceholderDiagnostic, location);
    }

    public static Diagnostic VariableClassNameInUse(string className, string variableName, string conflictingVariableName, Location? location = null)
    {
        return Diagnostic.Create(_VariableClassNameInUse, location, className, variableName, conflictingVariableName);
    }

    public static Diagnostic VariableNameInUseByVariable(
        VariableInfo variableInfo,
        string variableName,
        VariableModel duplicatedVariable,
        Location? location = null)
    {
        return Diagnostic.Create(_VariableNameInUseByVariable, location, variableInfo.Name, variableName, duplicatedVariable.VariableName);
    }

    public static Diagnostic VariableContextPropertyNameInUseByVariable(
        VariableInfo duplicatingVariableInfo,
        string contextPropertyName,
        VariableModel duplicatedModel,
        Location? location = null)
    {
        return Diagnostic.Create(
            _VariableContextPropertyNameInUseByVariable,
            location,
            duplicatingVariableInfo.Name,
            contextPropertyName,
            duplicatedModel.VariableName);
    }

    public static Diagnostic VariableDescriptorNameInUseByVariable(
        VariableInfo duplicatingVariableInfo,
        string descriptorName,
        VariableModel duplicatedModel,
        Location? location = null)
    {
        return Diagnostic.Create(
            _VariableDescriptorNameInUseByVariable,
            location,
            duplicatingVariableInfo.Name,
            descriptorName,
            duplicatedModel.VariableName);
    }

    internal static DiagnosticDescriptor CreateErrorDescriptor(string id, string title, string messageFormat)
    {
        return CreateDescriptor(id, title, messageFormat, DiagnosticSeverity.Error);
    }

    internal static DiagnosticDescriptor CreateDescriptor(string id, string title, string messageFormat, DiagnosticSeverity severity)
    {
        return new DiagnosticDescriptor(id, title, messageFormat, GeneratorDiagnostics.DefaultCategory, severity, isEnabledByDefault: true);
    }
}
