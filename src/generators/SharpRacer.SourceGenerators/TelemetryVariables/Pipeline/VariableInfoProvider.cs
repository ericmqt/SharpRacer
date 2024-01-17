using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using SharpRacer.SourceGenerators.TelemetryVariables.Diagnostics;
using SharpRacer.SourceGenerators.TelemetryVariables.InputModels;
using VariableInfoCollectionResult = (
    System.Collections.Immutable.ImmutableArray<SharpRacer.SourceGenerators.TelemetryVariables.InputModels.VariableInfo> Variables,
    System.Collections.Immutable.ImmutableArray<Microsoft.CodeAnalysis.Diagnostic> Diagnostics);
using VariableInfoFileNameAndTexts = (
    SharpRacer.SourceGenerators.TelemetryVariables.InputModels.VariableInfoFileName FileName,
    System.Collections.Immutable.ImmutableArray<Microsoft.CodeAnalysis.AdditionalText> AdditionalTexts);
using VariableInfoFileResult = (
    SharpRacer.SourceGenerators.TelemetryVariables.InputModels.VariableInfoFile File,
    Microsoft.CodeAnalysis.Diagnostic? Diagnostic);

namespace SharpRacer.SourceGenerators.TelemetryVariables.Pipeline;
internal static class VariableInfoProvider
{
    public static IncrementalValueProvider<VariableInfoCollectionResult> GetValueProvider(
        IncrementalValuesProvider<AdditionalText> additionalTextsProvider,
        IncrementalValueProvider<GeneratorConfiguration> generatorConfigurationProvider)
    {
        var variableInfoFile = GetVariableInfoFile(additionalTextsProvider, generatorConfigurationProvider);

        return variableInfoFile.Select<VariableInfoFileResult, VariableInfoCollectionResult>(
            static (providerValue, ct) =>
            {
                if (providerValue.Diagnostic != null)
                {
                    return (ImmutableArray<VariableInfo>.Empty, ImmutableArray.Create(providerValue.Diagnostic));
                }

                var diagnosticsBuilder = ImmutableArray.CreateBuilder<Diagnostic>();

                // Deserialize variables from file
                var variables = providerValue.File.Read(ct, out var readDiagnostic);

                if (readDiagnostic != null)
                {
                    if (readDiagnostic.IsError())
                    {
                        return (ImmutableArray<VariableInfo>.Empty, ImmutableArray.Create(readDiagnostic));
                    }

                    // Non-errors are OK
                    diagnosticsBuilder.Add(readDiagnostic);
                }

                var factory = new VariableInfoFactory(providerValue.File, variables.Length);

                foreach (var sourceVariable in variables)
                {
                    ct.ThrowIfCancellationRequested();

                    // NOTE: Models with errors are not added to the constructed collection but will return diagnostics
                    factory.TryAdd(sourceVariable, out var variableDiagnostics);

                    diagnosticsBuilder.AddRange(variableDiagnostics);
                }

                return (factory.Build(), diagnosticsBuilder.ToImmutable());
            })
            .WithTrackingName(TrackingNames.VariableInfoProvider_GetValueProvider);
    }

    private static IncrementalValueProvider<VariableInfoFileResult> GetVariableInfoFile(
        IncrementalValuesProvider<AdditionalText> additionalTextsProvider,
        IncrementalValueProvider<GeneratorConfiguration> generatorConfigurationProvider)
    {
        var variableInfoFileName = generatorConfigurationProvider.Select(static (config, _) => config.VariableInfoFileName);

        var variableInfoTexts = additionalTextsProvider
            .Combine(variableInfoFileName)
            .Where(static x => x.Right.IsMatch(x.Left))
            .Select(static (x, _) => x.Left)
            .Collect();

        return variableInfoFileName
            .Combine(variableInfoTexts)
            .Select<VariableInfoFileNameAndTexts, VariableInfoFileResult>(
                static (input, cancellationToken) =>
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    if (!input.AdditionalTexts.Any())
                    {
                        return (default(VariableInfoFile), GeneratorDiagnostics.TelemetryVariablesFileNotFound(input.FileName));
                    }

                    if (input.AdditionalTexts.Length > 1)
                    {
                        return (default(VariableInfoFile), GeneratorDiagnostics.AmbiguousTelemetryVariablesFileName(input.FileName));
                    }

                    var file = input.AdditionalTexts.Single();

                    var sourceText = file.GetText(cancellationToken);

                    if (sourceText is null)
                    {
                        return (default(VariableInfoFile), GeneratorDiagnostics.AdditionalTextContentReadError(file));
                    }

                    return (new VariableInfoFile(input.FileName, file, sourceText), null);
                })
            .WithTrackingName(TrackingNames.VariableInfoProvider_GetVariableInfoFile);
    }
}
