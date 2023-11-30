using Microsoft.CodeAnalysis;

namespace SharpRacer.SourceGenerators.TelemetryVariables.Diagnostics;

internal static class VariablesFileDiagnostics
{
    private static readonly DiagnosticDescriptor _FileNotFound
        = new DiagnosticDescriptor(
            "SR1100",
            "Telemetry variables file was not found",
            "Telemetry variables file was not found: '{0}'",
            GeneratorDiagnostics.DefaultCategory,
            DiagnosticSeverity.Error,
            isEnabledByDefault: true);

    private static readonly DiagnosticDescriptor _FileContentsReadFailure
        = new DiagnosticDescriptor(
            "SR1101",
            "Failed to read contents of telemetry variables file",
            "Unspecified error occurred while reading contents of telemetry variables file '{0}'",
            GeneratorDiagnostics.DefaultCategory,
            DiagnosticSeverity.Error,
            isEnabledByDefault: true);

    private static readonly DiagnosticDescriptor _FileReadException
        = new DiagnosticDescriptor(
            "SR1102",
            "Exception thrown while parsing telemetry variables file",
            "{0} was thrown while parsing telemetry variables file '{1}': {2}",
            GeneratorDiagnostics.DefaultCategory,
            DiagnosticSeverity.Error,
            isEnabledByDefault: true);

    private static readonly DiagnosticDescriptor _MultipleMatchingFiles
        = new DiagnosticDescriptor(
            "SR1103",
            "Multiple telemetry variables files found",
            "Multiple telemetry variables files were found with file name '{0}'",
            GeneratorDiagnostics.DefaultCategory,
            DiagnosticSeverity.Error,
            isEnabledByDefault: true);

    private static readonly DiagnosticDescriptor _ZeroVariablesInFile
        = new DiagnosticDescriptor(
            "SR1104",
            "Telemetry variables file contains zero variables",
            "Telemetry variables file contains zero variables: '{0}'",
            GeneratorDiagnostics.DefaultCategory,
            DiagnosticSeverity.Warning,
            isEnabledByDefault: true);

    private static readonly DiagnosticDescriptor _DuplicateVariable
        = new DiagnosticDescriptor(
            "SR1105",
            "Telemetry variables file has multiple variables of the same name",
            "Telemetry variables file has more than one entry for variable '{0}': '{1}'",
            GeneratorDiagnostics.DefaultCategory,
            DiagnosticSeverity.Error,
            isEnabledByDefault: true);

    public static Diagnostic DuplicateVariable(string variableName, string variablesFileName, Location? location = null)
    {
        return Diagnostic.Create(_DuplicateVariable, location, variableName, variablesFileName);
    }

    public static Diagnostic DuplicateVariable(string variableName, AdditionalText variablesText, Location? location = null)
    {
        return DuplicateVariable(variableName, variablesText.Path, location);
    }

    public static Diagnostic FileNotFound(string variablesFileName, Location? location = null)
    {
        return Diagnostic.Create(_FileNotFound, location, variablesFileName);
    }

    public static Diagnostic FileContentReadFailure(string variablesFileName, Location? location = null)
    {
        return Diagnostic.Create(_FileContentsReadFailure, location, variablesFileName);
    }

    public static Diagnostic FileContentReadFailure(AdditionalText text, Location? location = null)
    {
        return FileContentReadFailure(text.Path, location);
    }

    public static Diagnostic FileReadException(string variablesFileName, Exception exception, Location? location = null)
    {
        return Diagnostic.Create(_FileReadException, location, exception.GetType(), variablesFileName, exception?.Message);
    }

    public static Diagnostic FileReadException(AdditionalText text, Exception exception, Location? location = null)
    {
        return FileReadException(text.Path, exception, location);
    }

    public static Diagnostic MultipleMatchingFiles(string variablesFileName, Location? location = null)
    {
        return Diagnostic.Create(_MultipleMatchingFiles, location, variablesFileName);
    }

    public static Diagnostic WarnZeroVariablesInFile(string variablesFileName, Location? location = null)
    {
        return Diagnostic.Create(_ZeroVariablesInFile, location, variablesFileName);
    }

    public static Diagnostic WarnZeroVariablesInFile(AdditionalText text, Location? location = null)
    {
        return WarnZeroVariablesInFile(text.Path, location);
    }
}