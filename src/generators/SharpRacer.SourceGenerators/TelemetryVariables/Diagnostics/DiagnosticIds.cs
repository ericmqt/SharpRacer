namespace SharpRacer.SourceGenerators.TelemetryVariables.Diagnostics;

internal static class DiagnosticIds
{
    public const string AdditionalText_ContentReadError = "SR1010";
    public const string AdditionalText_FileReadException = "SR1011";

    public const string TelemetryVariablesFileNotFound = "SR1100";
    public const string IncludedVariables_FileNotFound = "SR1101";

    // InputFiles: AmbiguousFileName
    public const string TelemetryVariablesFile_AmbiguousFileName = "SR1110";
    public const string IncludedVariables_AmbiguousFileName = "SR1111";
    public const string VariableOptionsFile_AmbiguousFileName = "SR1112";

    // Syntax: DescriptorClass
    public const string DescriptorClassMustBeDeclaredPartial = "SR2101";
    public const string DescriptorClassMustBeDeclaredStatic = "SR2102";
    public const string DescriptorClassAlreadyExistsInAssembly = "SR2103";

    // Syntax: ContextClass
    public const string ContextClassMustBeDeclaredPartial = "SR2201";
    public const string ContextClassMustNotBeDeclaredStatic = "SR2202";
    public const string ContextClassMustInheritIDataVariablesContextInterface = "SR2203";

    // Validation: TelemetryVariables
    public const string TelemetryVariablesFileContainsNoVariables = "SR3100";
    public const string TelemetryVariableAlreadyDefined = "SR3101";
    public const string TelemetryVariablesDeprecatingVariableNotFound = "SR3102";

    // Validation: VariableOptions
    public const string VariableOptionsFileContainsDuplicateKey = "SR3210";
    public const string VariableOptionsFileContainsDuplicateVariableName = "SR3211";
    public const string VariableOptionsFileContainsDuplicateClassName = "SR3212";

    // Validation: IncludedVariables
    public const string IncludedVariablesFileContainsNoVariableNames = "SR3300";
    public const string IncludedVariablesFile_VariableAlreadyIncluded = "SR3301";
    public const string IncludedVariablesFile_ContainsEmptyVariableName = "SR3302";

    // Validation: Descriptors
    public const string DescriptorNameConflictsWithExistingVariable = "SR3402";

    // Validation: ContextClasses
    public const string ContextClassIncludedVariableNotFound = "SR3502";
    public const string ContextClassVariableNameCreatesPropertyNameConflict = "SR3503";
    public const string ContextClassConfiguredPropertyNameConflict = "SR3504";

    // Validation: VariableClasses
    public const string VariableClassNameInUse = "SR3610";
}
