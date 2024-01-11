using Microsoft.CodeAnalysis;

namespace SharpRacer.SourceGenerators.TelemetryVariables.Diagnostics;
internal static partial class GeneratorDiagnosticDescriptors
{
    public static DiagnosticDescriptor AdditionalTextContentReadError
        = new(
            DiagnosticIds.AdditionalText_ContentReadError,
            "Unable to read contents of source generator input file",
            "Unable to read contents of source generator input file '{0}'",
            GeneratorDiagnosticCategories.InputFiles,
            DiagnosticSeverity.Error,
            isEnabledByDefault: true);

    public static DiagnosticDescriptor AdditionalTextFileReadException
        = new(
            DiagnosticIds.AdditionalText_FileReadException,
            "Exception thrown while reading source generator input file",
            "{0} thrown while reading source generator input file '{1}': {2}",
            GeneratorDiagnosticCategories.InputFiles,
            DiagnosticSeverity.Error,
            isEnabledByDefault: true);

    public static readonly DiagnosticDescriptor AmbiguousIncludedVariablesFileName
        = new(
            DiagnosticIds.IncludedVariables_AmbiguousFileName,
            "More than one variable includes file found matching the expected file name",
            "Variable includes filename '{0}' matches multiple files",
            GeneratorDiagnosticCategories.InputFiles,
            DiagnosticSeverity.Error,
            isEnabledByDefault: true);

    public static DiagnosticDescriptor AmbiguousTelemetryVariablesFileName
        = new(
            DiagnosticIds.TelemetryVariablesFile_AmbiguousFileName,
            "More than one telemetry variables file found matching the expected file name",
            "Telemetry variables filename '{0}' matches multiple files",
            GeneratorDiagnosticCategories.InputFiles,
            DiagnosticSeverity.Error,
            isEnabledByDefault: true);

    public static readonly DiagnosticDescriptor AmbiguousVariableOptionsFileName
        = new(
            DiagnosticIds.VariableOptionsFile_AmbiguousFileName,
            "More than one telemetry variables options file found matching the expected file name",
            "Telemetry variables options filename '{0}' matches multiple files",
            GeneratorDiagnosticCategories.InputFiles,
            DiagnosticSeverity.Error,
            isEnabledByDefault: true);

    public static readonly DiagnosticDescriptor ContextClassConfiguredPropertyNameConflict
        = new(
            DiagnosticIds.ContextClassConfiguredPropertyNameConflict,
            "Configured variable property name conflicts with existing context variable property",
            "Context class variable '{0}' is configured with Name option value '{1}' which conflicts with existing property '{3}' for variable '{2}'",
            GeneratorDiagnosticCategories.Validation,
            DiagnosticSeverity.Error,
            isEnabledByDefault: true);

    public static readonly DiagnosticDescriptor ContextClassIncludedVariableNotFound
        = new(
            DiagnosticIds.ContextClassIncludedVariableNotFound,
            "Included variable was not found",
            "Variable context '{0}' includes variable '{1}' but the variable was not found",
            GeneratorDiagnosticCategories.Validation,
            DiagnosticSeverity.Error,
            isEnabledByDefault: true);

    public static DiagnosticDescriptor ContextClassMustBeDeclaredPartial
        = new(
            DiagnosticIds.ContextClassMustBeDeclaredPartial,
            "Variable context class must be declared 'partial'",
            "Variable context '{0}' must be a partial class for code generation to run",
            GeneratorDiagnosticCategories.Syntax,
            DiagnosticSeverity.Warning,
            isEnabledByDefault: true);

    public static DiagnosticDescriptor ContextClassMustInheritIDataVariablesContextInterface
        = new(
            DiagnosticIds.ContextClassMustInheritIDataVariablesContextInterface,
            "Variable context class must inherit IDataVariablesContext",
            "Variable context '{0}' must inherit IDataVariablesContext for code generation to run",
            GeneratorDiagnosticCategories.Syntax,
            DiagnosticSeverity.Warning,
            isEnabledByDefault: true);

    public static DiagnosticDescriptor ContextClassMustNotBeDeclaredStatic
       = new(
           DiagnosticIds.ContextClassMustNotBeDeclaredStatic,
           "Variable context class cannot be declared 'static'",
           "Variable context '{0}' cannot be a static class for code generation to run",
           GeneratorDiagnosticCategories.Syntax,
           DiagnosticSeverity.Warning,
           isEnabledByDefault: true);

    public static readonly DiagnosticDescriptor ContextClassVariableNameCreatesPropertyNameConflict
        = new(
            DiagnosticIds.ContextClassVariableNameCreatesPropertyNameConflict,
            "Variable name conflicts with existing context variable property",
            "Context class variable '{0}' conflicts with existing property '{2}' for variable '{1}'",
            GeneratorDiagnosticCategories.Validation,
            DiagnosticSeverity.Error,
            isEnabledByDefault: true);

    public static DiagnosticDescriptor DeprecatingVariableNotFound
        = new(
            DiagnosticIds.TelemetryVariablesDeprecatingVariableNotFound,
            "Deprecating variable not found",
            "Variable '{0}' is deprecated but deprecating variable '{1}' was not found",
            GeneratorDiagnosticCategories.Validation,
            DiagnosticSeverity.Warning,
            isEnabledByDefault: true);

    public static readonly DiagnosticDescriptor DescriptorClassAlreadyExistsInAssembly
        = new(
            DiagnosticIds.DescriptorClassAlreadyExistsInAssembly,
            "GenerateDataVariableDescriptorsAttribute may decorate only one class in an assembly",
            "'{0}' is decorated with GenerateDataVariableDescriptorsAttribute but a descriptor class is already defined by '{1}'. An assembly may only have one descriptor class.",
            GeneratorDiagnosticCategories.Syntax,
            DiagnosticSeverity.Error,
            isEnabledByDefault: true);

    public static DiagnosticDescriptor DescriptorClassMustBeDeclaredPartial
        = new(
            DiagnosticIds.DescriptorClassMustBeDeclaredPartial,
            "Variable descriptors class must be declared 'partial'",
            "Variable descriptors class '{0}' must be declared 'partial'",
            GeneratorDiagnosticCategories.Syntax,
            DiagnosticSeverity.Warning,
            isEnabledByDefault: true);

    public static DiagnosticDescriptor DescriptorClassMustBeDeclaredStatic
        = new(
            DiagnosticIds.DescriptorClassMustBeDeclaredStatic,
            "Variable descriptors class must be declared 'static'",
            "Variable descriptors class '{0}' must be declared 'static' for code generation to run",
            GeneratorDiagnosticCategories.Syntax,
            DiagnosticSeverity.Warning,
            isEnabledByDefault: true);

    public static readonly DiagnosticDescriptor DescriptorNameConflictsWithExistingVariable
        = new(
            DiagnosticIds.DescriptorNameConflictsWithExistingVariable,
            "Variable descriptor name conflict",
            "Variable '{0}' has descriptor name '{1}' which conflicts with existing descriptor for variable '{2}'",
            GeneratorDiagnosticCategories.Validation,
            DiagnosticSeverity.Error,
            isEnabledByDefault: true);

    public static readonly DiagnosticDescriptor IncludedVariablesFileAlreadyIncludesVariable
        = new(
            DiagnosticIds.IncludedVariablesFile_VariableAlreadyIncluded,
            "Variable is already included",
            "Included variables file already includes variable '{0}': {1}",
            GeneratorDiagnosticCategories.Validation,
            DiagnosticSeverity.Warning,
            isEnabledByDefault: true);

    public static DiagnosticDescriptor IncludedVariablesFileContainsNoVariables
        = new(
            DiagnosticIds.IncludedVariablesFileContainsNoVariableNames,
            "Variable includes file does not specify any variables",
            "Variable includes file does not specify any variables: '{0}'",
            GeneratorDiagnosticCategories.Validation,
            DiagnosticSeverity.Warning,
            isEnabledByDefault: true);

    public static DiagnosticDescriptor IncludedVariablesFileContainsEmptyVariableName
        = new(
            DiagnosticIds.IncludedVariablesFile_ContainsEmptyVariableName,
            "Included variable name is empty",
            "Included variable name is empty",
            GeneratorDiagnosticCategories.Validation,
            DiagnosticSeverity.Warning,
            isEnabledByDefault: true);

    public static DiagnosticDescriptor IncludedVariablesFileNotFound
        = new(
            DiagnosticIds.IncludedVariables_FileNotFound,
            "Variable includes file not found",
            "Variable includes file was not found: '{0}'",
            GeneratorDiagnosticCategories.InputFiles,
            DiagnosticSeverity.Error,
            isEnabledByDefault: true);

    public static DiagnosticDescriptor TelemetryVariablesFileContainsNoVariables
        = new(
            DiagnosticIds.TelemetryVariablesFileContainsNoVariables,
            "No variables defined in telemetry variables file",
            "Telemetry variables file does not define any variables: '{0}'",
            GeneratorDiagnosticCategories.Validation,
            DiagnosticSeverity.Warning,
            isEnabledByDefault: true);

    public static readonly DiagnosticDescriptor TelemetryVariablesFileNotFound
        = new(
            DiagnosticIds.TelemetryVariablesFileNotFound,
            "Telemetry variables file not found",
            "Telemetry variables file was not found: '{0}'",
            GeneratorDiagnosticCategories.InputFiles,
            DiagnosticSeverity.Warning,
            isEnabledByDefault: true);

    public static readonly DiagnosticDescriptor TelemetryVariableAlreadyDefined
        = new(
            DiagnosticIds.TelemetryVariableAlreadyDefined,
            "Telemetry variable is already defined",
            "Telemetry variable '{0}' is already defined",
            GeneratorDiagnosticCategories.Validation,
            DiagnosticSeverity.Error,
            isEnabledByDefault: true);

    public static readonly DiagnosticDescriptor VariableClassConfiguredClassNameInUse
        = new(
            DiagnosticIds.VariableClassNameInUse,
            "Variable class name in use",
            "Variable class '{0}' will not be generated for variable '{1}' because the type name conflicts with variable class '{2}' for variable '{3}'",
            GeneratorDiagnosticCategories.Validation,
            DiagnosticSeverity.Error,
            isEnabledByDefault: true);

    public static readonly DiagnosticDescriptor VariableOptionsFileContainsDuplicateClassName
        = new(
            DiagnosticIds.VariableOptionsFileContainsDuplicateClassName,
            "Value for variable option ClassName is assigned to another variable",
            "Options for variable '{0}' defines a value for ClassName '{1}' which is used by variable '{2}'",
            GeneratorDiagnosticCategories.Validation,
            DiagnosticSeverity.Error,
            isEnabledByDefault: true);

    public static readonly DiagnosticDescriptor VariableOptionsFileContainsDuplicateKey
        = new(
            DiagnosticIds.VariableOptionsFileContainsDuplicateKey,
            "Variable options already defined",
            "Variable options file already defines a value for key '{0}'",
            GeneratorDiagnosticCategories.Validation,
            DiagnosticSeverity.Error,
            isEnabledByDefault: true);

    public static readonly DiagnosticDescriptor VariableOptionsFileContainsDuplicateVariableName
        = new(
            DiagnosticIds.VariableOptionsFileContainsDuplicateVariableName,
            "Value for variable option Name is assigned to another variable",
            "Options for variable '{0}' defines a value for Name '{1}' which is used by variable '{2}'",
            GeneratorDiagnosticCategories.Validation,
            DiagnosticSeverity.Error,
            isEnabledByDefault: true);
}
