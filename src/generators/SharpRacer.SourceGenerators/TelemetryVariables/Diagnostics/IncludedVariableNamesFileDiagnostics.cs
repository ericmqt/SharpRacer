using Microsoft.CodeAnalysis;

namespace SharpRacer.SourceGenerators.TelemetryVariables.Diagnostics;

internal static class IncludedVariableNamesFileDiagnostics
{
    private static readonly DiagnosticDescriptor _IncludedVariablesFileNotFound
        = new DiagnosticDescriptor(
            "SR1200",
            "Input file for variables context was not found",
            "Input file for variables context {0} was not found: {1}",
            GeneratorDiagnostics.DefaultCategory,
            DiagnosticSeverity.Error,
            isEnabledByDefault: true);

    private static readonly DiagnosticDescriptor _AmbiguousFileName
        = new DiagnosticDescriptor(
            "SR1201",
            "Variables context input file name is ambiguous",
            "Input file name for variables context {0} matches multiple files: {1}",
            GeneratorDiagnostics.DefaultCategory,
            DiagnosticSeverity.Error,
            isEnabledByDefault: true);

    private static readonly DiagnosticDescriptor _FileContentReadFailure
        = new DiagnosticDescriptor(
            "SR1202",
            "Failed to read variable names file for telemetry variables context",
            "Failed to read variable names file for telemetry variables context {0}: '{1}'",
            GeneratorDiagnostics.DefaultCategory,
            DiagnosticSeverity.Error,
            isEnabledByDefault: true);

    private static readonly DiagnosticDescriptor _FileReadException
        = new DiagnosticDescriptor(
            "SR1203",
            "Exception thrown while parsing telemetry variables context file",
            "{0} was thrown while parsing telemetry variables context file '{1}': {2}",
            GeneratorDiagnostics.DefaultCategory,
            DiagnosticSeverity.Error,
            isEnabledByDefault: true);

    public static Diagnostic AmbiguousFileName(string contextTypeName, string inputFileName, Location? location = null)
    {
        return Diagnostic.Create(_AmbiguousFileName, location, contextTypeName, inputFileName);
    }

    public static Diagnostic FileNotFound(string contextTypeName, string inputFileName, Location? location = null)
    {
        return Diagnostic.Create(_IncludedVariablesFileNotFound, location, contextTypeName, inputFileName);
    }

    public static Diagnostic FileContentReadFailure(string contextTypeName, string inputFileName, Location? location = null)
    {
        return Diagnostic.Create(_FileContentReadFailure, location, contextTypeName, inputFileName);
    }
    public static Diagnostic FileContentReadFailure(string contextTypeName, AdditionalText additionalText, Location? location = null)
    {
        return FileContentReadFailure(contextTypeName, additionalText.Path, location);
    }

    public static Diagnostic FileReadException(string contextTypeName, string fileName, Exception exception, Location? location = null)
    {
        return Diagnostic.Create(_FileReadException, location, exception.GetType(), contextTypeName, fileName);
    }

    public static Diagnostic FileReadException(string contextTypeName, AdditionalText additionalText, Exception exception, Location? location = null)
    {
        return FileReadException(contextTypeName, additionalText.Path, exception, location);
    }
}