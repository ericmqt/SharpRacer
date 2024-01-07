using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using SharpRacer.SourceGenerators.TelemetryVariables.Diagnostics;
using SharpRacer.SourceGenerators.TelemetryVariables.GeneratorModels;
using SharpRacer.SourceGenerators.TelemetryVariables.InputModels;

using CreateVariableModelsResult = (
    System.Collections.Immutable.ImmutableArray<SharpRacer.SourceGenerators.TelemetryVariables.GeneratorModels.VariableModel> Models,
    System.Collections.Immutable.ImmutableArray<Microsoft.CodeAnalysis.Diagnostic> Diagnostics);

using InputVariablesAndOptions = (
    System.Collections.Immutable.ImmutableArray<SharpRacer.SourceGenerators.TelemetryVariables.InputModels.VariableInfo> Variables,
    System.Collections.Immutable.ImmutableArray<SharpRacer.SourceGenerators.TelemetryVariables.InputModels.VariableOptions> Options);

namespace SharpRacer.SourceGenerators.TelemetryVariables.Pipeline;
internal static class VariableModelsValueProvider
{
    internal static IncrementalValueProvider<CreateVariableModelsResult> GetValueProvider(
        IncrementalValueProvider<ImmutableArray<VariableInfo>> variableInfoProvider,
        IncrementalValueProvider<ImmutableArray<VariableOptions>> variableOptionsProvider)
    {
        return variableInfoProvider.Combine(variableOptionsProvider)
            .Select<InputVariablesAndOptions, CreateVariableModelsResult>(
                static (item, cancellationToken) => CreateModels(item.Variables, item.Options, cancellationToken))
            .WithTrackingName(TrackingNames.VariableModelsValueProvider_GetValuesProvider);
    }

    private static CreateVariableModelsResult CreateModels(
        ImmutableArray<VariableInfo> variables,
        ImmutableArray<VariableOptions> variableOptions,
        CancellationToken cancellationToken)
    {
        var modelBuilder = ImmutableArray.CreateBuilder<VariableModel>(variables.Length);
        var diagnosticsBuilder = ImmutableArray.CreateBuilder<Diagnostic>();

        foreach (var variable in variables)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var model = new VariableModel(variable, variableOptions.FirstOrDefault(x => x.VariableKey.Equals(variable.Name)));

            if (model.IsDeprecated && model.DeprecatingVariableName != null)
            {
                var deprecatingModel = variables.FirstOrDefault(x => x.Name.Equals(model.DeprecatingVariableName));

                if (deprecatingModel == default)
                {
                    var diagnostic = GeneratorDiagnostics.DeprecatingVariableNotFound(
                        model.VariableName,
                        model.DeprecatingVariableName,
                        model.Options.ValueLocation);

                    diagnosticsBuilder.Add(diagnostic);
                }
            }

            modelBuilder.Add(model);
        }

        return (modelBuilder.ToImmutable(), diagnosticsBuilder.ToImmutable());
    }
}
