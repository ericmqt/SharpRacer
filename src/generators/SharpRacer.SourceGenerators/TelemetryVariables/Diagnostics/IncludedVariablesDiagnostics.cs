using Microsoft.CodeAnalysis;

namespace SharpRacer.SourceGenerators.TelemetryVariables.Diagnostics;
internal static class IncludedVariablesDiagnostics
{
    private const string _Category = "SharpRacer.IncludedVariables";

    private static readonly DiagnosticDescriptor _FileNotFound
        = new(
            "SR2300",
            "Variable includes file not found",
            "Variable includes file was not found: '{0}'",
            _Category,
            DiagnosticSeverity.Error,
            isEnabledByDefault: true);

    private static readonly DiagnosticDescriptor _AmbiguousFileName
        = new(
            "SR2301",
            "More than one variable includes file found matching the expected file name",
            "Variable includes filename '{0}' matches multiple files",
            _Category,
            DiagnosticSeverity.Error,
            isEnabledByDefault: true);

    private static readonly DiagnosticDescriptor _FileContentReadFailure
        = new(
            "SR2302",
            "Error reading variable includes file",
            "Error reading variable includes file '{0}'",
            _Category,
            DiagnosticSeverity.Error,
            isEnabledByDefault: true);

    private static readonly DiagnosticDescriptor _FileReadException
        = new(
            "SR2303",
            "Exception thrown while parsing telemetry variable includes file",
            "'{0}' thrown while parsing telemetry variable includes file '{1}': {2}",
            _Category,
            DiagnosticSeverity.Error,
            isEnabledByDefault: true);

    private static readonly DiagnosticDescriptor _NoIncludedVariableNames
        = new(
            "SR2304",
            "Variable includes file does not specify any variables",
            "Variable includes file does not specify any variables: '{0}'",
            _Category,
            DiagnosticSeverity.Warning,
            isEnabledByDefault: true);

    private static readonly DiagnosticDescriptor _EmptyVariableName
        = new(
            "SR2305",
            "Included variable name is empty",
            "Included variable name is empty",
            _Category,
            DiagnosticSeverity.Warning,
            isEnabledByDefault: true);

    private static readonly DiagnosticDescriptor _VariableAlreadyIncluded
        = new(
            "SR2306",
            "Variable is already included",
            "Variable '{0}' is already included",
            _Category,
            DiagnosticSeverity.Warning,
            isEnabledByDefault: true);

    public static Diagnostic AmbiguousFileName(string fileName, Location? location = null)
    {
        return Diagnostic.Create(_AmbiguousFileName, location, fileName);
    }

    public static Diagnostic EmptyVariableName(Location location)
    {
        return Diagnostic.Create(_EmptyVariableName, location);
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

    public static Diagnostic NoIncludedVariableNamesWarning(string fileName, Location? location = null)
    {
        return Diagnostic.Create(_NoIncludedVariableNames, location, fileName);
    }

    public static Diagnostic VariableAlreadyIncluded(string variableName, Location location)
    {
        return Diagnostic.Create(_VariableAlreadyIncluded, location, variableName);
    }
}
