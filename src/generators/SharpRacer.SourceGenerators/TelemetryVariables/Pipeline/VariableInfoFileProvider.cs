using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using SharpRacer.SourceGenerators.TelemetryVariables.Diagnostics;
using SharpRacer.SourceGenerators.TelemetryVariables.InputModels;

namespace SharpRacer.SourceGenerators.TelemetryVariables.Pipeline;
internal static class VariableInfoFileProvider
{
    public static IncrementalValueProvider<PipelineValueResult<VariableInfoFile>> GetValueProvider(
        IncrementalValuesProvider<AdditionalText> additionalTextsProvider,
        IncrementalValueProvider<GeneratorConfiguration> generatorConfigurationProvider)
    {
        var variableInfoFileName = generatorConfigurationProvider.Select(static (config, _) => config.VariableInfoFileName);

        var variableInfoTexts = additionalTextsProvider
            .Combine(variableInfoFileName)
            .Where(static x => x.Right.IsMatch(x.Left))
            .Select(static (x, _) => x.Left)
            .Collect();

        return variableInfoFileName.Combine(variableInfoTexts)
            .Select(static (x, ct) => GetPipelineValueResult(x.Left, x.Right, ct))
            .WithTrackingName(TrackingNames.VariableInfoFileProvider_GetValueProvider);
    }

    public static PipelineValueResult<VariableInfoFile> GetPipelineValueResult(
        VariableInfoFileName fileName,
        ImmutableArray<AdditionalText> additionalTexts,
        CancellationToken cancellationToken = default)
    {
        if (!TryFindVariableInfoFile(fileName, additionalTexts, cancellationToken, out var variableInfoFile, out var diagnostic))
        {
            return diagnostic!;
        }

        return variableInfoFile;
    }

    public static bool TryFindVariableInfoFile(
        VariableInfoFileName fileName,
        ImmutableArray<AdditionalText> additionalTexts,
        CancellationToken cancellationToken,
        out VariableInfoFile variableInfoFile,
        out Diagnostic? diagnostic)
    {
        cancellationToken.ThrowIfCancellationRequested();

        variableInfoFile = default;

        if (!additionalTexts.Any())
        {
            diagnostic = GeneratorDiagnostics.TelemetryVariablesFileNotFound(fileName);

            return false;
        }

        if (additionalTexts.Length > 1)
        {
            diagnostic = GeneratorDiagnostics.AmbiguousTelemetryVariablesFileName(fileName);

            return false;
        }

        var file = additionalTexts.Single();

        var sourceText = file.GetText(cancellationToken);

        if (sourceText is null)
        {
            diagnostic = GeneratorDiagnostics.AdditionalTextContentReadError(file);

            return false;
        }

        variableInfoFile = new VariableInfoFile(fileName, file, sourceText);
        diagnostic = null;

        return variableInfoFile != default;
    }
}
