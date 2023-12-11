using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using SharpRacer.SourceGenerators.TelemetryVariables.Diagnostics;
using SharpRacer.SourceGenerators.TelemetryVariables.InputModels;

namespace SharpRacer.SourceGenerators.TelemetryVariables.Pipeline;
internal static class IncludedVariablesFileValuesProvider
{
    public static IncrementalValuesProvider<PipelineValueResult<(ClassWithGeneratorAttribute, IncludedVariablesFile)>> GetValuesProvider(
        IncrementalValuesProvider<(ClassWithGeneratorAttribute, IncludedVariablesFileName)> contextClassesProvider,
        IncrementalValuesProvider<AdditionalText> additionalTextsProvider)
    {
        return contextClassesProvider.Combine(additionalTextsProvider.Collect())
            .Select(static (item, ct) => GetPipelineValueResult(item.Left.Item1, item.Left.Item2, item.Right, ct))
            .WithTrackingName(TrackingNames.IncludedVariablesFileValuesProvider_GetValuesProvider);
    }

    public static PipelineValueResult<(ClassWithGeneratorAttribute, IncludedVariablesFile)> GetPipelineValueResult(
        ClassWithGeneratorAttribute contextClassData,
        IncludedVariablesFileName fileName,
        ImmutableArray<AdditionalText> additionalTexts,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (!TryFindIncludedVariablesFile(fileName, additionalTexts, cancellationToken, out var includedVariablesFile, out var diagnostic))
        {
            return new PipelineValueResult<(ClassWithGeneratorAttribute, IncludedVariablesFile)>(
                (contextClassData, default),
                diagnostic!);
        }

        return (contextClassData, includedVariablesFile);
    }

    public static bool TryFindIncludedVariablesFile(
        IncludedVariablesFileName fileName,
        ImmutableArray<AdditionalText> additionalTexts,
        CancellationToken cancellationToken,
        out IncludedVariablesFile includedVariablesFile,
        out Diagnostic? diagnostic)
    {
        cancellationToken.ThrowIfCancellationRequested();

        includedVariablesFile = default;

        if (!additionalTexts.Any())
        {
            diagnostic = IncludedVariablesDiagnostics.FileNotFound(fileName);
            return false;
        }

        var matches = additionalTexts.Where(fileName.IsMatch);

        if (matches.Count() > 1)
        {
            diagnostic = IncludedVariablesDiagnostics.AmbiguousFileName(fileName);
            return false;
        }

        var file = matches.Single();
        var sourceText = file.GetText(cancellationToken);

        if (sourceText is null)
        {
            diagnostic = IncludedVariablesDiagnostics.FileContentReadFailure(fileName);
            return false;
        }

        includedVariablesFile = new IncludedVariablesFile(fileName, file, sourceText);
        diagnostic = null;

        return true;
    }
}
