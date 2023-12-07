using Microsoft.CodeAnalysis;
using SharpRacer.SourceGenerators.TelemetryVariables.InputModels;

namespace SharpRacer.SourceGenerators.TelemetryVariables.Pipeline;
internal static class PipelineExtensions
{
    public static IncrementalValueProvider<PipelineValueResult<VariableInfoFile>> GetVariableInfoFile(
        this IncrementalValuesProvider<AdditionalText> additionalTextsProvider,
        IncrementalValueProvider<GeneratorConfiguration> generatorConfigurationProvider)
    {
        var variableInfoFileName = generatorConfigurationProvider.Select(static (config, _) => config.VariableInfoFileName);

        var variableInfoTexts = additionalTextsProvider
            .Combine(variableInfoFileName)
            .Where(static x => x.Right.IsMatch(x.Left))
            .Select(static (x, _) => x.Left)
            .Collect();

        return variableInfoFileName.Combine(variableInfoTexts)
            .Select(static (x, ct) =>
            {
                var result = InputFileFactory.VariableInfoFile(x.Left, x.Right, ct, out var diagnostic);

                if (diagnostic != null)
                {
                    return new PipelineValueResult<VariableInfoFile>(diagnostic);
                }

                return new PipelineValueResult<VariableInfoFile>(result);
            });
    }

    public static IncrementalValueProvider<PipelineValueResult<VariableOptionsFile>> GetVariableOptionsFile(
        this IncrementalValuesProvider<AdditionalText> additionalTextsProvider,
        IncrementalValueProvider<GeneratorConfiguration> generatorConfigurationProvider)
    {
        var variableOptionsFileName = generatorConfigurationProvider.Select(static (config, _) => config.VariableOptionsFileName);

        var variableOptionsTexts = additionalTextsProvider
            .Combine(variableOptionsFileName)
            .Where(static x => x.Right.IsMatch(x.Left))
            .Select(static (x, _) => x.Left)
            .Collect();

        return variableOptionsFileName.Combine(variableOptionsTexts)
            .Select(static (x, ct) =>
            {
                var result = InputFileFactory.VariableOptionsFile(x.Left, x.Right, ct, out var diagnostic);

                if (diagnostic != null)
                {
                    return new PipelineValueResult<VariableOptionsFile>(diagnostic);
                }

                return new PipelineValueResult<VariableOptionsFile>(result);
            });
    }
}
