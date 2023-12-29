﻿using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using SharpRacer.SourceGenerators.TelemetryVariables.Diagnostics;
using SharpRacer.SourceGenerators.TelemetryVariables.GeneratorModels;
using SharpRacer.SourceGenerators.TelemetryVariables.InputModels;

namespace SharpRacer.SourceGenerators.TelemetryVariables.Pipeline;
internal static class VariableModelsValueProvider
{
    internal static IncrementalValuesProvider<VariableModel> GetValueProvider(
        ref IncrementalGeneratorInitializationContext context,
        IncrementalValueProvider<GeneratorConfiguration> generatorConfiguration)
    {
        var variableOptionsProvider = GetVariableOptionsProvider(ref context, generatorConfiguration);
        context.ReportDiagnostics(variableOptionsProvider.Select(static (x, _) => x.Diagnostics));

        // Read VariableInfo collection from input file
        var variableInfoModelsResult = GetVariableInfoProvider(ref context, generatorConfiguration);
        context.ReportDiagnostics(variableInfoModelsResult.Select(static (x, _) => x.Diagnostics));

        var variableModelsResult = variableInfoModelsResult.Select(static (x, _) => x.Values)
            .Combine(variableOptionsProvider.Select(static (x, _) => x.Values))
            .Select(static (item, ct) => CreateVariableModels(item.Left, item.Right));

        context.ReportDiagnostics(variableModelsResult.Select(static (x, _) => x.Diagnostics));

        return variableModelsResult.SelectMany(static (result, _) => result.Values);
    }

    private static IncrementalValueProvider<PipelineValuesResult<VariableInfo>> GetVariableInfoProvider(
        ref IncrementalGeneratorInitializationContext context,
        IncrementalValueProvider<GeneratorConfiguration> generatorConfiguration)
    {
        // Variable info
        var variableInfoFile = VariableInfoFileProvider.GetValueProvider(context.AdditionalTextsProvider, generatorConfiguration);

        context.ReportDiagnostics(variableInfoFile.Select(static (x, _) => x.Diagnostics));

        return variableInfoFile.Select(static (input, ct) =>
        {
            if (!input.HasValue || input.HasErrors)
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
                ct.ThrowIfCancellationRequested();

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
        var variableOptionsFile = VariableOptionsFileProvider.GetValueProvider(context.AdditionalTextsProvider, generatorConfiguration);

        context.ReportDiagnostics(variableOptionsFile.Select(static (x, _) => x.Diagnostics));

        // Read VariableOptions values from input file
        return variableOptionsFile.Select(static (input, ct) =>
        {
            // Options file is optional, we can skip this
            if (!input.HasValue || input.HasErrors)
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
        }).WithTrackingName(TrackingNames.VariableModelsValueProvider_GetVariableOptionsProvider);
    }

    private static PipelineValuesResult<VariableModel> CreateVariableModels(ImmutableArray<VariableInfo> variables, ImmutableArray<VariableOptions> variableOptions)
    {
        var diagnosticsBuilder = ImmutableArray.CreateBuilder<Diagnostic>();

        var variableModels = variables
            .Select(x => new VariableModel(x, variableOptions.FirstOrDefault(x => x.VariableKey.Equals(x.Name))))
            .ToList();

        // Find deprecating variables
        for (int i = 0; i < variableModels.Count; i++)
        {
            var model = variableModels[i];

            if (model.VariableInfo.IsDeprecated && model.VariableInfo.DeprecatedBy != null)
            {
                var deprecatingModel = variableModels.FirstOrDefault(x => x.VariableName.Equals(model.VariableInfo.DeprecatedBy));

                if (deprecatingModel != default)
                {
                    variableModels[i] = model.WithDeprecatingVariable(deprecatingModel);
                }
                else
                {
                    var diagnostic = VariableModelDiagnostics.DeprecatingVariableNotFoundWarning(
                        model.VariableName,
                        model.VariableInfo.DeprecatedBy);

                    diagnosticsBuilder.Add(diagnostic);
                }
            }
        }

        return new PipelineValuesResult<VariableModel>(variableModels.ToImmutableArray(), diagnosticsBuilder.ToImmutable());
    }
}
