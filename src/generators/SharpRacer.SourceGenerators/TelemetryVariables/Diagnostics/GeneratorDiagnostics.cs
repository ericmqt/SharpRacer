using Microsoft.CodeAnalysis;

namespace SharpRacer.SourceGenerators.TelemetryVariables.Diagnostics;

internal static partial class GeneratorDiagnostics
{
    public static Diagnostic AdditionalTextContentReadError(AdditionalText additionalText, Location? location = null)
    {
        return Diagnostic.Create(GeneratorDiagnosticDescriptors.AdditionalTextContentReadError, location, additionalText.Path);
    }

    public static Diagnostic AdditionalTextFileReadException(AdditionalText additionalText, Exception exception, Location? location = null)
    {
        return Diagnostic.Create(
            GeneratorDiagnosticDescriptors.AdditionalTextFileReadException,
            location,
            exception.GetType(),
            additionalText.Path,
            exception.Message);
    }

    public static Diagnostic AmbiguousIncludedVariablesFileName(string fileName, Location? location = null)
    {
        return Diagnostic.Create(GeneratorDiagnosticDescriptors.AmbiguousIncludedVariablesFileName, location, fileName);
    }

    public static Diagnostic AmbiguousTelemetryVariablesFileName(string fileName, Location? location = null)
    {
        return Diagnostic.Create(GeneratorDiagnosticDescriptors.AmbiguousTelemetryVariablesFileName, location, fileName);
    }

    public static Diagnostic AmbiguousVariableOptionsFileName(string fileName, Location? location = null)
    {
        return Diagnostic.Create(GeneratorDiagnosticDescriptors.AmbiguousVariableOptionsFileName, location, fileName);
    }

    public static Diagnostic ContextClassConfiguredPropertyNameConflict(
        string contextClassName,
        string variableName,
        string configuredPropertyName,
        string existingVariableName,
        string existingPropertyName,
        Location? location = null)
    {
        return Diagnostic.Create(
            GeneratorDiagnosticDescriptors.ContextClassConfiguredPropertyNameConflict,
            location,
            contextClassName,
            variableName,
            configuredPropertyName,
            existingVariableName,
            existingPropertyName);
    }

    public static Diagnostic ContextClassIncludedVariableNotFound(
        string contextClassIdentifier,
        string variableName,
        Location? location = null)
    {
        return Diagnostic.Create(
            GeneratorDiagnosticDescriptors.ContextClassIncludedVariableNotFound,
            location,
            contextClassIdentifier,
            variableName);
    }

    public static Diagnostic ContextClassMustBeDeclaredPartial(string contextClassIdentifier, Location? location = null)
    {
        return Diagnostic.Create(GeneratorDiagnosticDescriptors.ContextClassMustBeDeclaredPartial, location, contextClassIdentifier);
    }

    public static Diagnostic ContextClassMustInheritITelemetryVariablesContextInterface(
        string contextClassIdentifier,
        Location? location = null)
    {
        return Diagnostic.Create(
            GeneratorDiagnosticDescriptors.ContextClassMustInheritITelemetryVariablesContextInterface,
            location,
            contextClassIdentifier);
    }

    public static Diagnostic ContextClassMustNotBeDeclaredStatic(string contextClassIdentifier, Location? location = null)
    {
        return Diagnostic.Create(GeneratorDiagnosticDescriptors.ContextClassMustNotBeDeclaredStatic, location, contextClassIdentifier);
    }

    public static Diagnostic ContextClassVariableNameCreatesPropertyNameConflict(
        string contextClassName,
        string variableName,
        string existingVariableName,
        string existingPropertyName,
        Location? location = null)
    {
        return Diagnostic.Create(
            GeneratorDiagnosticDescriptors.ContextClassVariableNameCreatesPropertyNameConflict,
            location,
            contextClassName,
            variableName,
            existingVariableName,
            existingPropertyName);
    }

    public static Diagnostic DeprecatingVariableNotFound(string variableName, string deprecatingVariableName, Location? location = null)
    {
        return Diagnostic.Create(GeneratorDiagnosticDescriptors.DeprecatingVariableNotFound, location, variableName, deprecatingVariableName);
    }

    public static Diagnostic DescriptorClassAlreadyExistsInAssembly(string redefiningClassName, string descriptorClassName, Location? location = null)
    {
        return Diagnostic.Create(GeneratorDiagnosticDescriptors.DescriptorClassAlreadyExistsInAssembly, location, redefiningClassName, descriptorClassName);
    }

    public static Diagnostic DescriptorClassMustBeDeclaredPartial(string className, Location? location = null)
    {
        return Diagnostic.Create(GeneratorDiagnosticDescriptors.DescriptorClassMustBeDeclaredPartial, location, className);
    }

    public static Diagnostic DescriptorClassMustBeDeclaredStatic(string className, Location? location = null)
    {
        return Diagnostic.Create(GeneratorDiagnosticDescriptors.DescriptorClassMustBeDeclaredStatic, location, className);
    }

    public static Diagnostic DescriptorNameConflictsWithExistingVariable(
        string variableName,
        string descriptorName,
        string definingVariableName,
        Location? location = null)
    {
        return Diagnostic.Create(
            GeneratorDiagnosticDescriptors.DescriptorNameConflictsWithExistingVariable,
            location,
            variableName,
            descriptorName,
            definingVariableName);
    }

    public static Diagnostic IncludedVariablesFileAlreadyIncludesVariable(string variableName, string fileName, Location? location = null)
    {
        return Diagnostic.Create(
            GeneratorDiagnosticDescriptors.IncludedVariablesFileAlreadyIncludesVariable,
            location,
            variableName,
            fileName);
    }

    public static Diagnostic IncludedVariablesFileContainsEmptyVariableName(Location? location = null)
    {
        return Diagnostic.Create(GeneratorDiagnosticDescriptors.IncludedVariablesFileContainsEmptyVariableName, location);
    }

    public static Diagnostic IncludedVariablesFileContainsNoVariables(AdditionalText additionalText, Location? location = null)
    {
        return Diagnostic.Create(GeneratorDiagnosticDescriptors.IncludedVariablesFileContainsNoVariables, location, additionalText.Path);
    }

    public static Diagnostic IncludedVariablesFileNotFound(string fileName, Location? location = null)
    {
        return Diagnostic.Create(GeneratorDiagnosticDescriptors.IncludedVariablesFileNotFound, location, fileName);
    }

    public static Diagnostic TelemetryVariablesFileContainsNoVariables(string fileName, Location? location = null)
    {
        return Diagnostic.Create(GeneratorDiagnosticDescriptors.TelemetryVariablesFileContainsNoVariables, location, fileName);
    }

    public static Diagnostic TelemetryVariablesFileNotFound(string fileName, Location? location = null)
    {
        return Diagnostic.Create(GeneratorDiagnosticDescriptors.TelemetryVariablesFileNotFound, location, fileName);
    }

    public static Diagnostic TelemetryVariableAlreadyDefined(string variableName, Location? location = null)
    {
        return Diagnostic.Create(GeneratorDiagnosticDescriptors.TelemetryVariableAlreadyDefined, location, variableName);
    }

    public static Diagnostic VariableClassConfiguredClassNameInUse(
        string className,
        string variableName,
        string conflictingVariableClassName,
        string conflictingVariableName,
        Location? location = null)
    {
        return Diagnostic.Create(
            GeneratorDiagnosticDescriptors.VariableClassConfiguredClassNameInUse,
            location,
            className,
            variableName,
            conflictingVariableClassName,
            conflictingVariableName);
    }

    public static Diagnostic VariableOptionsFileContainsDuplicateClassName(
        string variableKey,
        string configuredClassName,
        string duplicatedVariableKey,
        Location? location = null)
    {
        return Diagnostic.Create(
            GeneratorDiagnosticDescriptors.VariableOptionsFileContainsDuplicateClassName,
            location,
            variableKey,
            configuredClassName,
            duplicatedVariableKey);
    }

    public static Diagnostic VariableOptionsFileContainsDuplicateKey(string variableKey, Location? location = null)
    {
        return Diagnostic.Create(GeneratorDiagnosticDescriptors.VariableOptionsFileContainsDuplicateKey, location, variableKey);
    }

    public static Diagnostic VariableOptionsFileContainsDuplicateVariableName(
        string variableKey,
        string configuredVariableName,
        string duplicatedVariableKey,
        Location? location = null)
    {
        return Diagnostic.Create(
            GeneratorDiagnosticDescriptors.VariableOptionsFileContainsDuplicateVariableName,
            location,
            variableKey,
            configuredVariableName,
            duplicatedVariableKey);
    }
}
