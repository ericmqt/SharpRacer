using Microsoft.CodeAnalysis;

namespace SharpRacer.SourceGenerators.TelemetryVariables.Diagnostics;
internal static class VariableOptionsDiagnostics
{
    private const string _Category = "SharpRacer.VariableOptions";

    public static Diagnostic AmbiguousFileName(string fileName, Location? location = null)
    {
        return Diagnostic.Create(_AmbiguousFileName, location, fileName);
    }

    public static Diagnostic DuplicateContextPropertyName(string variableKey, string contextPropertyName, string duplicatedVariableKey, Location? location = null)
    {
        return Diagnostic.Create(_DuplicateContextPropertyName, location, variableKey, contextPropertyName, duplicatedVariableKey);
    }

    public static Diagnostic DuplicateDescriptorName(string variableKey, string contextPropertyName, string duplicatedVariableKey, Location? location = null)
    {
        return Diagnostic.Create(_DuplicateContextPropertyName, location, variableKey, contextPropertyName, duplicatedVariableKey);
    }

    public static Diagnostic DuplicateKey(string key, Location? location = null)
    {
        return Diagnostic.Create(_DuplicateKey, location, key);
    }

    public static Diagnostic DuplicateVariableName(string variableKey, string contextPropertyName, string duplicatedVariableKey, Location? location = null)
    {
        return Diagnostic.Create(_DuplicateVariableName, location, variableKey, contextPropertyName, duplicatedVariableKey);
    }

    public static Diagnostic FileContentReadFailure(string fileName, Location? location = null)
    {
        return Diagnostic.Create(_FileContentReadFailure, location, fileName);
    }

    public static Diagnostic FileReadException(string fileName, Exception exception, Location? location = null)
    {
        return Diagnostic.Create(_FileReadException, location, exception.GetType(), fileName, exception.Message);
    }

    public static Diagnostic FileReadException(string fileName, Type exceptionType, string message, Location? location = null)
    {
        return Diagnostic.Create(_FileReadException, location, exceptionType, fileName, message);
    }

    private static readonly DiagnosticDescriptor _AmbiguousFileName
        = new(
            "SR2201",
            "More than one telemetry variables options file found matching the expected file name",
            "Telemetry variables options filename '{0}' matches multiple files",
            _Category,
            DiagnosticSeverity.Error,
            isEnabledByDefault: true);

    private static readonly DiagnosticDescriptor _FileContentReadFailure
        = new(
            "SR2202",
            "Error reading telemetry variables options file",
            "Error reading telemetry variables options file '{0}'",
            _Category,
            DiagnosticSeverity.Error,
            isEnabledByDefault: true);

    private static readonly DiagnosticDescriptor _FileReadException
        = new(
            "SR2203",
            "Exception thrown while parsing telemetry variables options file",
            "'{0}' thrown while parsing telemetry variables options file '{1}': {2}",
            _Category,
            DiagnosticSeverity.Error,
            isEnabledByDefault: true);

    private static readonly DiagnosticDescriptor _DuplicateKey
        = new(
            "SR2210",
            "Variable options already defined",
            "Variable options file already defines a value for key '{0}'",
            _Category,
            DiagnosticSeverity.Error,
            isEnabledByDefault: true);

    private static readonly DiagnosticDescriptor _DuplicateVariableName
        = new(
            "SR2211",
            "Value for variable option Name is assigned to another variable",
            "Options for variable '{0}' defines a value for Name '{1}' which is used by variable '{2}'",
            _Category,
            DiagnosticSeverity.Error,
            isEnabledByDefault: true);

    private static readonly DiagnosticDescriptor _DuplicateContextPropertyName
        = new(
            "SR2212",
            "Value for variable option ContextPropertyName is assigned to another variable",
            "Options for variable '{0}' defines a value for ContextPropertyName '{1}' which is used by variable '{2}'",
            _Category,
            DiagnosticSeverity.Error,
            isEnabledByDefault: true);

    private static readonly DiagnosticDescriptor _DuplicateDescriptorName
        = new(
            "SR2213",
            "Value for variable option DescriptorName is assigned to another variable",
            "Options for variable '{0}' defines a value for DescriptorName '{1}' which is used by variable '{2}'",
            _Category,
            DiagnosticSeverity.Error,
            isEnabledByDefault: true);
}
