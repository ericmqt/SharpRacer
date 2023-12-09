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

    public static VariableInfoFile VariableInfoFile(
        VariableInfoFileName fileName,
        AdditionalText additionalText,
        CancellationToken cancellationToken,
        out Diagnostic? diagnostic)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var sourceText = additionalText.GetText(cancellationToken);

        if (sourceText is null)
        {
            diagnostic = VariableInfoDiagnostics.FileContentReadFailure(fileName);

            return default;
        }

        diagnostic = null;

        return new VariableInfoFile(fileName, additionalText, sourceText);
    }

    public static VariableInfoFile VariableInfoFile(
        VariableInfoFileName fileName,
        ImmutableArray<AdditionalText> additionalTexts,
        CancellationToken cancellationToken,
        out Diagnostic? diagnostic)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (!additionalTexts.Any())
        {
            diagnostic = VariableInfoDiagnostics.FileNotFound(fileName);

            return default;
        }

        if (additionalTexts.Length > 1)
        {
            diagnostic = VariableInfoDiagnostics.AmbiguousFileName(fileName);

            return default;
        }

        return VariableInfoFile(fileName, additionalTexts.Single(), cancellationToken, out diagnostic);
    }

    public static VariableOptionsFile VariableOptionsFile(
        VariableOptionsFileName fileName,
        AdditionalText additionalText,
        CancellationToken cancellationToken,
        out Diagnostic? diagnostic)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var sourceText = additionalText.GetText(cancellationToken);

        if (sourceText is null)
        {
            diagnostic = VariableOptionsDiagnostics.FileContentReadFailure(fileName);

            return default;
        }

        diagnostic = null;
        return new VariableOptionsFile(fileName, additionalText, sourceText);
    }

    public static bool TryFindVariableOptionsFile(
        VariableOptionsFileName fileName,
        ImmutableArray<AdditionalText> additionalTexts,
        CancellationToken cancellationToken,
        out VariableOptionsFile variableOptionsFile,
        out Diagnostic? diagnostic)
    {
        cancellationToken.ThrowIfCancellationRequested();

        variableOptionsFile = default;

        if (!additionalTexts.Any())
        {
            // Options file not required, so if we don't find one that's OK
            diagnostic = null;

            return false;
        }

        if (additionalTexts.Length > 1)
        {
            diagnostic = VariableOptionsDiagnostics.AmbiguousFileName(fileName);

            return false;
        }

        variableOptionsFile = VariableOptionsFile(fileName, additionalTexts.Single(), cancellationToken, out diagnostic);

        return variableOptionsFile != default;
    }
}
