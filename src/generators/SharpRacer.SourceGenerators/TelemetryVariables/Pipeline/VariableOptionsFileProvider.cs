using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using SharpRacer.SourceGenerators.TelemetryVariables.Diagnostics;
using SharpRacer.SourceGenerators.TelemetryVariables.InputModels;

namespace SharpRacer.SourceGenerators.TelemetryVariables.Pipeline;
internal static class VariableOptionsFileProvider
{
    public static IncrementalValueProvider<PipelineValueResult<VariableOptionsFile>> GetValueProvider(
        IncrementalValuesProvider<AdditionalText> additionalTextsProvider,
        IncrementalValueProvider<GeneratorConfiguration> generatorConfigurationProvider)
    {
        var variableOptionsFileName = generatorConfigurationProvider.Select(static (config, _) => config.VariableOptionsFileName);

        var variableOptionsTexts = additionalTextsProvider
            .Combine(variableOptionsFileName)
            .Where(static x => x.Right.IsMatch(x.Left))
            .Select(static (x, _) => x.Left)
            .Collect();

        return variableOptionsFileName.Combine(variableOptionsTexts)
            .Select(static (x, ct) => GetPipelineValueResult(x.Left, x.Right, ct))
            .WithTrackingName(TrackingNames.VariableOptionsFileProvider_GetValueProvider);
    }

    public static PipelineValueResult<VariableOptionsFile> GetPipelineValueResult(
        VariableOptionsFileName fileName, ImmutableArray<AdditionalText> additionalTexts, CancellationToken cancellationToken = default)
    {
        if (!TryFindVariableOptionsFile(fileName, additionalTexts, cancellationToken, out var optionsFile, out var diagnostic))
        {
            if (diagnostic != null)
            {
                return diagnostic!;
            }

            return new PipelineValueResult<VariableOptionsFile>();
        }

        return optionsFile;
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

        var file = additionalTexts.Single();

        var sourceText = file.GetText(cancellationToken);

        if (sourceText is null)
        {
            diagnostic = VariableOptionsDiagnostics.FileContentReadFailure(fileName);

            return false;
        }

        variableOptionsFile = new VariableOptionsFile(fileName, file, sourceText);
        diagnostic = null;

        return true;
    }
}
