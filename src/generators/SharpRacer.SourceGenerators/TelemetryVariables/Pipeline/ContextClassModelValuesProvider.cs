using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using SharpRacer.SourceGenerators.TelemetryVariables.Diagnostics;
using SharpRacer.SourceGenerators.TelemetryVariables.GeneratorModels;
using ContextClassInfoIncludedVariables = (
    SharpRacer.SourceGenerators.TelemetryVariables.InputModels.ContextClassInfo ClassInfo,
    System.Collections.Immutable.ImmutableArray<SharpRacer.SourceGenerators.TelemetryVariables.GeneratorModels.ContextVariableModel> IncludedVariables,
    System.Collections.Immutable.ImmutableArray<Microsoft.CodeAnalysis.Diagnostic> Diagnostics);
using ContextClassInfoValuesProviderResult = (
    SharpRacer.SourceGenerators.TelemetryVariables.InputModels.ContextClassInfo ClassInfo,
    System.Collections.Immutable.ImmutableArray<Microsoft.CodeAnalysis.Diagnostic> Diagnostics);
using ContextClassModelsResult = (
    SharpRacer.SourceGenerators.TelemetryVariables.GeneratorModels.ContextClassModel Model,
    System.Collections.Immutable.ImmutableArray<Microsoft.CodeAnalysis.Diagnostic> Diagnostics);
using ContextVariablePrototype = (
    SharpRacer.SourceGenerators.TelemetryVariables.GeneratorModels.VariableModel VariableModel,
    SharpRacer.SourceGenerators.TelemetryVariables.GeneratorModels.DescriptorPropertyReference? DescriptorPropertyReference,
    SharpRacer.SourceGenerators.TelemetryVariables.GeneratorModels.VariableClassReference? VariableClassReference);
using DescriptorAndVariableClassReferences = (
    System.Collections.Immutable.ImmutableArray<SharpRacer.SourceGenerators.TelemetryVariables.GeneratorModels.DescriptorPropertyReference> DescriptorPropertyReferences,
    System.Collections.Immutable.ImmutableArray<SharpRacer.SourceGenerators.TelemetryVariables.GeneratorModels.VariableClassReference> VariableClassReferences);

namespace SharpRacer.SourceGenerators.TelemetryVariables.Pipeline;
internal static class ContextClassModelValuesProvider
{
    public static IncrementalValuesProvider<ContextClassModelsResult> GetValuesProvider(
        IncrementalValuesProvider<ContextClassInfoValuesProviderResult> contextClassInfos,
        IncrementalValuesProvider<VariableModel> variableModels,
        IncrementalValueProvider<ImmutableArray<DescriptorPropertyReference>> descriptorReferences,
        IncrementalValueProvider<ImmutableArray<VariableClassReference>> variableClassReferences)
    {
        var contextVariablePrototypes = ContextVariablePrototypes(
            variableModels,
            descriptorReferences.Combine(variableClassReferences));

        return contextClassInfos.Combine(contextVariablePrototypes.Collect())
            .Select(ContextClassIncludedVariables)
            .Select(static (input, cancellationToken) =>
            {
                if (input.ClassInfo == default)
                {
                    return (default, input.Diagnostics);
                }

                var model = new ContextClassModel(input.ClassInfo, input.IncludedVariables);

                return (model, input.Diagnostics);
            })
            .WithTrackingName(TrackingNames.ContextClassModelValuesProvider_GetValuesProvider);
    }

    private static ContextClassInfoIncludedVariables ContextClassIncludedVariables(
        (ContextClassInfoValuesProviderResult, ImmutableArray<ContextVariablePrototype>) input, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var contextClassInfoValuesProviderResult = input.Item1;
        var variablePrototypes = input.Item2;

        // Forward existing diagnostics to our output
        var diagnosticsBuilder = ImmutableArray.CreateBuilder<Diagnostic>();
        diagnosticsBuilder.AddRange(contextClassInfoValuesProviderResult.Diagnostics);

        if (contextClassInfoValuesProviderResult.ClassInfo == default)
        {
            return (contextClassInfoValuesProviderResult.ClassInfo, ImmutableArray<ContextVariableModel>.Empty, diagnosticsBuilder.ToImmutable());
        }

        var contextClassInfo = contextClassInfoValuesProviderResult.ClassInfo;

        var factory = new ContextVariableModelFactory();

        if (contextClassInfo.IncludedVariables.IncludeAll())
        {
            foreach (var prototype in variablePrototypes)
            {
                factory.TryAdd(
                    prototype.VariableModel,
                    prototype.DescriptorPropertyReference,
                    prototype.VariableClassReference,
                    out var modelDiagnostics);

                diagnosticsBuilder.AddRange(modelDiagnostics);
            }

            return (contextClassInfo, factory.Build(), diagnosticsBuilder.ToImmutable());
        }

        // Include by name
        foreach (var variableName in contextClassInfo.IncludedVariables.VariableNames)
        {
            var prototype = variablePrototypes.FirstOrDefault(x => x.VariableModel.VariableName.Equals(variableName, StringComparison.Ordinal));

            if (prototype != default)
            {
                factory.TryAdd(
                    prototype.VariableModel,
                    prototype.DescriptorPropertyReference,
                    prototype.VariableClassReference,
                    out var modelDiagnostics);

                diagnosticsBuilder.AddRange(modelDiagnostics);
            }
            else
            {
                var diagnostic = GeneratorDiagnostics.ContextClassIncludedVariableNotFound(
                    contextClassInfo.ToFullyQualifiedName(),
                    variableName,
                    contextClassInfo.GeneratorAttributeLocation);

                diagnosticsBuilder.Add(diagnostic);
            }
        }

        return (contextClassInfo, factory.Build(), diagnosticsBuilder.ToImmutable());
    }

    private static IncrementalValuesProvider<ContextVariablePrototype> ContextVariablePrototypes(
        IncrementalValuesProvider<VariableModel> variableModels,
        IncrementalValueProvider<DescriptorAndVariableClassReferences> descriptorAndVariableClassReferences)
    {
        return variableModels.Combine(descriptorAndVariableClassReferences)
            .Select(static (item, cancellationToken) =>
            {
                cancellationToken.ThrowIfCancellationRequested();

                DescriptorPropertyReference? descriptorReference = null;
                VariableClassReference? variableClassReference = null;

                if (item.Right.DescriptorPropertyReferences.Any(x => x.VariableName.Equals(item.Left.VariableName)))
                {
                    descriptorReference = item.Right.DescriptorPropertyReferences.FirstOrDefault(
                        x => x.VariableName.Equals(item.Left.VariableName));
                }

                if (item.Right.VariableClassReferences.Any(x => x.VariableName.Equals(item.Left.VariableName)))
                {
                    variableClassReference = item.Right.VariableClassReferences.FirstOrDefault(
                        x => x.VariableName.Equals(item.Left.VariableName));
                }

                return (item.Left, descriptorReference, variableClassReference);
            });
    }
}
