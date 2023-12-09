using Microsoft.CodeAnalysis;

namespace SharpRacer.SourceGenerators.TelemetryVariables.Diagnostics;
internal static class VariableInfoDiagnostics
{
    private const string _Category = "SharpRacer.VariableInfo";

    private static readonly DiagnosticDescriptor _FileNotFound
        = new(
            DiagnosticIds.VariableInfo_FileNotFound,
            "Telemetry variables file not found",
            "Telemetry variables file was not found: '{0}'",
            _Category,
            DiagnosticSeverity.Error,
            isEnabledByDefault: true);

    private static readonly DiagnosticDescriptor _AmbiguousFileName
        = new(
            DiagnosticIds.VariableInfo_AmbiguousFileName,
            "More than one telemetry variables file found matching the expected file name",
            "Telemetry variables filename '{0}' matches multiple files",
            _Category,
            DiagnosticSeverity.Error,
            isEnabledByDefault: true);

    private static readonly DiagnosticDescriptor _FileContentReadFailure
        = new(
            DiagnosticIds.VariableInfo_FileContentReadFailure,
            "Error reading telemetry variables file", "Error reading telemetry variables file '{0}'",
            _Category,
            DiagnosticSeverity.Error,
            isEnabledByDefault: true);

    private static readonly DiagnosticDescriptor _FileReadException
        = new(
            DiagnosticIds.VariableInfo_FileReadException,
            "Exception thrown while parsing telemetry variables file",
            "'{0}' thrown while parsing telemetry variables file '{1}': {2}",
            _Category,
            DiagnosticSeverity.Error,
            isEnabledByDefault: true);

    private static readonly DiagnosticDescriptor _NoVariablesDefinedInFile
        = new(
            DiagnosticIds.VariableInfo_NoVariablesDefinedInFile,
            "No variables defined in telemetry variables file",
            "Telemetry variables file does not define any variables: '{0}'",
            _Category,
            DiagnosticSeverity.Warning,
            isEnabledByDefault: true);

    private static readonly DiagnosticDescriptor _VariableAlreadyDefined
        = new(
            DiagnosticIds.VariableInfo_VariableAlreadyDefined,
            "Telemetry variable is already defined",
            "Telemetry variable '{0}' is already defined",
            _Category,
            DiagnosticSeverity.Error,
            isEnabledByDefault: true);

    public static Diagnostic AmbiguousFileName(string fileName, Location? location = null)
    {
        return Diagnostic.Create(_AmbiguousFileName, location, fileName);
    }

    public static Diagnostic FileContentReadFailure(string fileName, Location? location = null)
    {
        return Diagnostic.Create(_FileContentReadFailure, location, fileName);
    }

    public static Diagnostic FileNotFound(string fileName, Location? location = null)
    {
        return Diagnostic.Create(_FileNotFound, location, fileName);
    }

    public static Diagnostic FileReadException(string fileName, Exception exception, Location? location = null)
    {
        return Diagnostic.Create(_FileReadException, location, exception.GetType(), fileName, exception.Message);
    }

    public static Diagnostic FileReadException(string fileName, Type exceptionType, string message, Location? location = null)
    {
        return Diagnostic.Create(_FileReadException, location, exceptionType, fileName, message);
    }

    public static Diagnostic NoVariablesDefinedInFile(string fileName, Location? location = null)
    {
        return Diagnostic.Create(_NoVariablesDefinedInFile, location, fileName);
    }

    public static Diagnostic VariableAlreadyDefined(string variableName, Location? location = null)
    {
        return Diagnostic.Create(_VariableAlreadyDefined, location, variableName);
    }

    private static DiagnosticDescriptor ErrorDescriptor(string id, string title, string messageFormat)
    {
        return CreateDescriptor(id, title, messageFormat, DiagnosticSeverity.Error);
    }

    private static DiagnosticDescriptor WarningDescriptor(string id, string title, string messageFormat)
    {
        return CreateDescriptor(id, title, messageFormat, DiagnosticSeverity.Warning);
    }

    private static DiagnosticDescriptor CreateDescriptor(string id, string title, string messageFormat, DiagnosticSeverity severity)
    {
        return new DiagnosticDescriptor(id, title, messageFormat, _Category, severity, isEnabledByDefault: true);
    }
}
