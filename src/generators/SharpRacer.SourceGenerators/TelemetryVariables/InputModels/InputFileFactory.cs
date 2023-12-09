using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using SharpRacer.SourceGenerators.TelemetryVariables.Diagnostics;

namespace SharpRacer.SourceGenerators.TelemetryVariables.InputModels;
internal static class InputFileFactory
{
    public static IncludedVariablesFile IncludedVariablesFile(
        IncludedVariablesFileName fileName,
        AdditionalText additionalText,
        CancellationToken cancellationToken,
        out Diagnostic? diagnostic)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var sourceText = additionalText.GetText(cancellationToken);

        if (sourceText is null)
        {
            diagnostic = IncludedVariablesDiagnostics.FileContentReadFailure(fileName);

            return default;
        }

        diagnostic = null;

        return new IncludedVariablesFile(fileName, additionalText, sourceText);
    }

    public static IncludedVariablesFile IncludedVariablesFile(
        IncludedVariablesFileName fileName,
        ImmutableArray<AdditionalText> additionalTexts,
        CancellationToken cancellationToken,
        out Diagnostic? diagnostic)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var matches = additionalTexts.Where(fileName.IsMatch);

        if (!matches.Any())
        {
            // FileNotFound diagnostic
            diagnostic = IncludedVariablesDiagnostics.FileNotFound(fileName);

            return default;
        }

        if (matches.Count() > 1)
        {
            diagnostic = IncludedVariablesDiagnostics.AmbiguousFileName(fileName);

            return default;
        }

        return IncludedVariablesFile(fileName, matches.Single(), cancellationToken, out diagnostic);
    }
}
