using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using SharpRacer.SourceGenerators.TelemetryVariables.GeneratorModels;
using SharpRacer.SourceGenerators.TelemetryVariables.InputModels;

namespace SharpRacer.SourceGenerators.TelemetryVariables.Pipeline;
internal static class VariableModelsValueProvider
{
    internal static IncrementalValuesProvider<VariableModel> Get(
        ref IncrementalGeneratorInitializationContext context,
        IncrementalValueProvider<GeneratorConfiguration> generatorConfiguration)
    {
        var variableOptionsProvider = GetVariableOptionsProvider(ref context, generatorConfiguration);
        context.ReportDiagnostics(variableOptionsProvider.Select(static (x, _) => x.Diagnostics));

        // Read VariableInfo collection from input file
        var variableInfoModelsResult = GetVariableInfoProvider(ref context, generatorConfiguration);
        context.ReportDiagnostics(variableInfoModelsResult.Select(static (x, _) => x.Diagnostics));

        // Build VariableModel
        var variableModelsResult = variableInfoModelsResult.Select(static (x, _) => x.Values)
            .Combine(variableOptionsProvider.Select(static (x, _) => x.Values))
            .Select(static (input, ct) =>
            {
                var diagnosticsBuilder = ImmutableArray.CreateBuilder<Diagnostic>();

                var models = VariableModelFactory.CreateArray(
                    input.Left,
                    input.Right,
                    diagnosticsBuilder,
                    ct);

                return new PipelineValuesResult<VariableModel>(models, diagnosticsBuilder.ToImmutable());
            });

        context.ReportDiagnostics(variableModelsResult.Select(static (x, _) => x.Diagnostics));

        return variableModelsResult.SelectMany(static (x, _) => x.Values).WithTrackingName("VariableModelsValueProvider_Get");
    }

    private static IncrementalValueProvider<PipelineValuesResult<VariableInfo>> GetVariableInfoProvider(
        ref IncrementalGeneratorInitializationContext context,
        IncrementalValueProvider<GeneratorConfiguration> generatorConfiguration)
    {
        // Variable info
        var variableInfoFile = context.AdditionalTextsProvider.GetVariableInfoFile(generatorConfiguration);

        context.ReportDiagnostics(variableInfoFile.Select(static (x, _) => x.Diagnostics));

        return variableInfoFile.Select(static (input, ct) =>
        {
            if (input.IsDefaultOrEmpty || input.HasErrors)
            {
                return new PipelineValuesResult<VariableInfo>();
            }

            var jsonVariables = input.Value.Read(ct, out var readDiagnostic);
            var diagnosticBuilder = ImmutableArray.CreateBuilder<Diagnostic>();

            if (readDiagnostic != null)
            {
                if (readDiagnostic.IsError())
                {
                    return new PipelineValuesResult<VariableInfo>(readDiagnostic);
                }
                else
                {
                    diagnosticBuilder.Add(readDiagnostic);
                }
            }

            var factory = new VariableInfoFactory(input.Value, jsonVariables.Length);

            foreach (var jsonVariable in jsonVariables)
            {
                // NOTE: Models with errors are not added to the constructed collection but will return diagnostics
                factory.TryAdd(jsonVariable, out var variableDiagnostics);

                diagnosticBuilder.AddRange(variableDiagnostics);
            }

            return new PipelineValuesResult<VariableInfo>(factory.Build(), diagnosticBuilder.ToImmutable());
        }).WithTrackingName("GetVariableInfoProvider");
    }

    private static IncrementalValueProvider<PipelineValuesResult<VariableOptions>> GetVariableOptionsProvider(
        ref IncrementalGeneratorInitializationContext context,
        IncrementalValueProvider<GeneratorConfiguration> generatorConfiguration)
    {
        var variableOptionsFile = context.AdditionalTextsProvider.GetVariableOptionsFile(generatorConfiguration);

        context.ReportDiagnostics(variableOptionsFile.Select(static (x, _) => x.Diagnostics));

        // Read VariableOptions values from input file
        return variableOptionsFile.Select(static (input, ct) =>
        {
            // Options file is optional, we can skip this
            if (input.IsDefaultOrEmpty || input.HasErrors)
            {
                return new PipelineValuesResult<VariableOptions>();
            }

            var jsonVariableOptions = input.Value.Read(ct, out var readDiagnostic);
            var diagnosticsBuilder = ImmutableArray.CreateBuilder<Diagnostic>();

            if (readDiagnostic != null)
            {
                if (readDiagnostic.IsError())
                {
                    return new PipelineValuesResult<VariableOptions>(readDiagnostic);
                }
                else
                {
                    diagnosticsBuilder.Add(readDiagnostic);
                }
            }

            var factory = new VariableOptionsFactory(input.Value, jsonVariableOptions.Length);

            foreach (var jsonVariableOption in jsonVariableOptions)
            {
                factory.TryAdd(jsonVariableOption, out var optionsDiagnostics);

                diagnosticsBuilder.AddRange(optionsDiagnostics);
            }

            return new PipelineValuesResult<VariableOptions>(factory.Build(), diagnosticsBuilder.ToImmutable());
        });
    }
}
