using Microsoft.CodeAnalysis;
using SharpRacer.SourceGenerators.TelemetryVariables.GeneratorModels;
using SharpRacer.SourceGenerators.TelemetryVariables.InputModels;

namespace SharpRacer.SourceGenerators.TelemetryVariables.Diagnostics;
internal static partial class GeneratorDiagnostics
{
    // Variables file: SR1100
    // Configuration file: SR1200

    public static string DefaultCategory = "SharpRacer.SourceGenerators";


    private static readonly DiagnosticDescriptor _MoreThanOneDescriptorGeneratorTarget
        = new DiagnosticDescriptor(
            "SR1010",
            "GenerateDataVariableDescriptorsAttribute can only decorate exactly one class",
            "Only one class in an assembly can be decorated with GenerateDataVariableDescriptorsAttribute",
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

    public static Diagnostic MoreThanOneDescriptorGeneratorTarget(Location? location = null)
    {
        return Diagnostic.Create(_MoreThanOneDescriptorGeneratorTarget, location);
    }

    public static Diagnostic Placeholder(Location? location = null)
    {
        return Diagnostic.Create(_PlaceholderDiagnostic, location);
    }

    public static Diagnostic VariableNameInUseByVariable(
        VariableInfo variableInfo,
        string variableName,
        VariableModel duplicatedVariable,
        Location? location = null)
    {
        return Diagnostic.Create(_VariableNameInUseByVariable, location, variableInfo.Name, variableName, duplicatedVariable.VariableInfo.Name);
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
            duplicatedModel.VariableInfo.Name);
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
            duplicatedModel.VariableInfo.Name);
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
