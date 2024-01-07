using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using SharpRacer.SourceGenerators.TelemetryVariables.Diagnostics;
using SharpRacer.SourceGenerators.TelemetryVariables.InputModels;
using VariableOptionsCollectionResult = (
    System.Collections.Immutable.ImmutableArray<SharpRacer.SourceGenerators.TelemetryVariables.InputModels.VariableOptions> Options,
    System.Collections.Immutable.ImmutableArray<Microsoft.CodeAnalysis.Diagnostic> Diagnostics);
using VariableOptionsFileNameAndTexts = (
    SharpRacer.SourceGenerators.TelemetryVariables.InputModels.VariableOptionsFileName FileName,
    System.Collections.Immutable.ImmutableArray<Microsoft.CodeAnalysis.AdditionalText> AdditionalTexts);

using VariableOptionsFileResult = (
    SharpRacer.SourceGenerators.TelemetryVariables.InputModels.VariableOptionsFile File,
    Microsoft.CodeAnalysis.Diagnostic? Diagnostic);

namespace SharpRacer.SourceGenerators.TelemetryVariables.Pipeline;
internal static class VariableOptionsProvider
{
    public static IncrementalValueProvider<VariableOptionsCollectionResult> GetValueProvider(
        IncrementalValuesProvider<AdditionalText> additionalTextsProvider,
        IncrementalValueProvider<GeneratorConfiguration> generatorConfigurationProvider)
    {
        var variableOptionsFile = GetVariableOptionsFile(additionalTextsProvider, generatorConfigurationProvider);

        return variableOptionsFile.Select<VariableOptionsFileResult, VariableOptionsCollectionResult>(
            static (providerValue, cancellationToken) =>
            {
                if (providerValue.Diagnostic != null)
                {
                    return (ImmutableArray<VariableOptions>.Empty, ImmutableArray.Create(providerValue.Diagnostic));
                }

                if (providerValue.File == default)
                {
                    return (ImmutableArray<VariableOptions>.Empty, ImmutableArray<Diagnostic>.Empty);
                }

                var diagnosticsBuilder = ImmutableArray.CreateBuilder<Diagnostic>();

                // Deserialize options from file
                var variableOptions = providerValue.File.Read(cancellationToken, out var readDiagnostic);

                if (readDiagnostic != null)
                {
                    if (readDiagnostic.IsError())
                    {
                        return (ImmutableArray<VariableOptions>.Empty, ImmutableArray.Create(readDiagnostic));
                    }

                    // Non-errors are OK
                    diagnosticsBuilder.Add(readDiagnostic);
                }

                var factory = new VariableOptionsFactory(providerValue.File, variableOptions.Length);

                foreach (var sourceOption in variableOptions)
                {
                    factory.TryAdd(sourceOption, out var optionsDiagnostics);

                    diagnosticsBuilder.AddRange(optionsDiagnostics);
                }

                return (factory.Build(), diagnosticsBuilder.ToImmutable());
            })
            .WithTrackingName(TrackingNames.VariableOptionsProvider_GetValueProvider);
    }

    private static IncrementalValueProvider<VariableOptionsFileResult> GetVariableOptionsFile(
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
            .Select<VariableOptionsFileNameAndTexts, VariableOptionsFileResult>(
                static (input, cancellationToken) =>
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    if (!input.AdditionalTexts.Any())
                    {
                        // Options file not required, so if we don't find one that's OK

                        return (default, null);
                    }

                    if (input.AdditionalTexts.Length > 1)
                    {
                        return (default, GeneratorDiagnostics.AmbiguousVariableOptionsFileName(input.FileName));
                    }

                    var file = input.AdditionalTexts.Single();

                    var sourceText = file.GetText(cancellationToken);

                    if (sourceText is null)
                    {
                        return (default, GeneratorDiagnostics.AdditionalTextContentReadError(file));
                    }

                    return (new VariableOptionsFile(input.FileName, file, sourceText), null);
                })
            .WithTrackingName(TrackingNames.VariableOptionsProvider_GetVariableOptionsFile);
    }
}
