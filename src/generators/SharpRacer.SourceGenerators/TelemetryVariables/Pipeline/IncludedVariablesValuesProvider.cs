using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using SharpRacer.SourceGenerators.TelemetryVariables.InputModels;

namespace SharpRacer.SourceGenerators.TelemetryVariables.Pipeline;
internal static class IncludedVariablesValuesProvider
{
    public static IncrementalValuesProvider<(ClassWithGeneratorAttribute, IncludedVariables)> GetValuesProvider(
        IncrementalValuesProvider<PipelineValueResult<(ClassWithGeneratorAttribute, ImmutableArray<IncludedVariableName>)>> contextClassesWithIncludedVariableNamesProvider)
    {
        return contextClassesWithIncludedVariableNamesProvider.Select(static (item, ct) =>
        {
            var validIncludeNames = item.Value.Item2.Where(x => !x.Diagnostics.HasErrors()).ToImmutableArray();

            var includedVariables = new IncludedVariables(validIncludeNames);

            return (item.Value.Item1, includedVariables);
        }).WithTrackingName(TrackingNames.IncludedVariablesValuesProvider);
    }
}
