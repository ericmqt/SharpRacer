namespace SharpRacer.SourceGenerators.TelemetryVariables.Diagnostics;

internal static class DiagnosticIds
{
    public const string DescriptorClass_AssemblyAlreadyContainsDescriptorClassTarget = "SR1010";

    public const string VariableClassNameInUse = "SR3301";

    public const string VariableInfo_FileNotFound = "SR2100";
    public const string VariableInfo_AmbiguousFileName = "SR2101";
    public const string VariableInfo_FileContentReadFailure = "SR2102";
    public const string VariableInfo_FileReadException = "SR2103";
    public const string VariableInfo_NoVariablesDefinedInFile = "SR2104";
    public const string VariableInfo_VariableAlreadyDefined = "SR2105";

    public const string VariableOptions_AmbiguousFileName = "SR2201";
    public const string VariableOptions_FileContentReadFailure = "SR2202";
    public const string VariableOptions_FileReadException = "SR2203";

    public const string VariableOptions_DuplicateKey = "SR2210";
    public const string VariableOptions_DuplicateVariableName = "SR2211";
    public const string VariableOptions_DuplicateClassName = "SR2212";
    public const string VariableOptions_DuplicateDescriptorName = "SR2213";

    public const string VariableModels_NameInUse = "SR3210";
    public const string VariableModels_ContextPropertyNameInUse = "SR3211";
    public const string VariableModels_DescriptorNameInUse = "SR3212";
    public const string VariableModels_ConfiguredNameResultsInContextPropertyNameCollision = "SR3213";
    public const string VariableModels_ConfiguredNameResultsInDescriptorNameCollision = "SR3214";
}
