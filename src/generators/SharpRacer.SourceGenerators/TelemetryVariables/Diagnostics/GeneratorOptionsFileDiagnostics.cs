using Microsoft.CodeAnalysis;

namespace SharpRacer.SourceGenerators.TelemetryVariables.Diagnostics;

internal static class GeneratorOptionsFileDiagnostics
{
    private static readonly DiagnosticDescriptor _FileContentReadFailure
       = new DiagnosticDescriptor(
           "SR1000",
           "Failed to read contents of generator configuration file",
           "Unspecified error occurred while reading contents of generator configuration file '{0}'",
           GeneratorDiagnostics.DefaultCategory,
           DiagnosticSeverity.Error,
           isEnabledByDefault: true);

    private static readonly DiagnosticDescriptor _FileReadException
        = new DiagnosticDescriptor(
            "SR1001",
            "Exception thrown while parsing generator configuration file",
            "{0} was thrown while parsing generator configuration file '{0}': {1}",
            GeneratorDiagnostics.DefaultCategory,
            DiagnosticSeverity.Error,
            isEnabledByDefault: true);

    private static readonly DiagnosticDescriptor _MultipleConfigurationFilesFound
        = new DiagnosticDescriptor(
            "SR1002",
            "Multiple source generator configuration files found",
            "Multiple source generator configuration files were found with file name '{0}'",
            GeneratorDiagnostics.DefaultCategory,
            DiagnosticSeverity.Error,
            isEnabledByDefault: true);

    public static Diagnostic FileContentReadFailure(string configurationFileName, Location? location = null)
    {
        return Diagnostic.Create(_FileContentReadFailure, location, configurationFileName);
    }

    public static Diagnostic FileContentReadFailure(AdditionalText text, Location? location = null)
    {
        return FileContentReadFailure(text.Path, location);
    }

    public static Diagnostic FileReadException(string configurationFileName, Exception exception, Location? location = null)
    {
        return Diagnostic.Create(_FileReadException, location, exception.GetType(), configurationFileName, exception.Message);
    }

    public static Diagnostic FileReadException(AdditionalText text, Exception exception, Location? location = null)
    {
        return FileReadException(text.Path, exception, location);
    }

    public static Diagnostic MultipleFilesFound(string configurationFileName, Location? location = null)
    {
        return Diagnostic.Create(_MultipleConfigurationFilesFound, location, configurationFileName);
    }
}