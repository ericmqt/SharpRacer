using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using SharpRacer.SourceGenerators.TelemetryVariables.InputModels;

namespace SharpRacer.SourceGenerators.TelemetryVariables.Pipeline;
internal static class ContextClassIncludedVariableNamesValuesProvider
{
    public static IncrementalValuesProvider<PipelineValueResult<(ClassWithGeneratorAttribute, ImmutableArray<IncludedVariableName>)>> GetValuesProvider(
        IncrementalValuesProvider<(ClassWithGeneratorAttribute, IncludedVariablesFile)> contextClassesWithIncludedVariableFilesProvider)
    {
        return contextClassesWithIncludedVariableFilesProvider.Select(static (item, ct) => GetPipelineValueResult(item.Item1, item.Item2, ct))
            .WithTrackingName(TrackingNames.ContextClassIncludedVariableNamesValuesProvider);
    }

    public static IncrementalValuesProvider<Diagnostic> GetDiagnostics(
        IncrementalValuesProvider<PipelineValueResult<(ClassWithGeneratorAttribute, ImmutableArray<IncludedVariableName>)>> provider)
    {
        return provider.SelectMany(static (item, ct) =>
        {
            ct.ThrowIfCancellationRequested();

            var includedNameDiagnostics = item.Value.Item2.Where(x => x.Diagnostics.HasErrors()).SelectMany(x => x.Diagnostics);

            return item.Diagnostics.Where(x => x.IsError()).Concat(includedNameDiagnostics);
        });
    }

    public static PipelineValueResult<(ClassWithGeneratorAttribute, ImmutableArray<IncludedVariableName>)> GetPipelineValueResult(
        ClassWithGeneratorAttribute contextClass,
        IncludedVariablesFile includedVariablesFile,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (includedVariablesFile == default)
        {
            return (contextClass, ImmutableArray<IncludedVariableName>.Empty);
        }

        var includedVariableNameValues = includedVariablesFile.ReadJson(cancellationToken, out var readDiagnostic);

        if (readDiagnostic != null && readDiagnostic.IsError())
        {
            return readDiagnostic;
        }

        var factory = new IncludedVariableNameFactory(includedVariablesFile);

        foreach (var variableNameValue in includedVariableNameValues)
        {
            factory.Add(variableNameValue);
        }

        return (contextClass, factory.Build());
    }
}
