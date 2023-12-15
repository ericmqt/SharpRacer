using Microsoft.CodeAnalysis;

namespace SharpRacer.SourceGenerators.TelemetryVariables.Diagnostics;
internal static class DescriptorClassDiagnostics
{
    private const string _Category = "SharpRacer.VariableInfo";

    private static readonly DiagnosticDescriptor _AssemblyAlreadyContainsDescriptorClassTarget
        = new(
            DiagnosticIds.DescriptorClass_AssemblyAlreadyContainsDescriptorClassTarget,
            "GenerateDataVariableDescriptorsAttribute may decorate only one class in an assembly",
            "'{0}' is decorated with GenerateDataVariableDescriptorsAttribute but a descriptor class is already defined by '{1}'. An assembly may only have one descriptor class.",
            _Category,
            DiagnosticSeverity.Error,
            isEnabledByDefault: true);

    private static readonly DiagnosticDescriptor _DescriptorNameConflictsWithExistingVariable
        = new(
            "xxxx",
            "Variable descriptor name conflict",
            "Variable '{0}' has descriptor name '{1}' which conflicts with existing descriptor for variable '{2}'",
            _Category,
            DiagnosticSeverity.Error,
            isEnabledByDefault: true);

    public static Diagnostic AssemblyAlreadyContainsDescriptorClassTarget(string redefiningClassName, string descriptorClassName, Location? location = null)
    {
        return Diagnostic.Create(_AssemblyAlreadyContainsDescriptorClassTarget, location, redefiningClassName, descriptorClassName);
    }

    public static Diagnostic DescriptorNameConflictsWithExistingVariable(
        string variableName,
        string descriptorName,
        string definingVariableName,
        Location? location = null)
    {
        return Diagnostic.Create(_DescriptorNameConflictsWithExistingVariable, location, variableName, descriptorName, definingVariableName);
    }
}
