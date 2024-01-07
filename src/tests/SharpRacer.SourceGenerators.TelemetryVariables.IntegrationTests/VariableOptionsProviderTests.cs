using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using SharpRacer.SourceGenerators.TelemetryVariables.InputModels;
using SharpRacer.SourceGenerators.TelemetryVariables.Pipeline;
using SharpRacer.SourceGenerators.TelemetryVariables.TestHelpers;
using SharpRacer.SourceGenerators.Testing.TelemetryVariables;

namespace SharpRacer.SourceGenerators.TelemetryVariables;
public class VariableOptionsProviderTests
{
    [Fact]
    public void NoOptionsFile_Test()
    {
        var variablesText = new VariableInfoDocumentBuilder()
            .AddScalar("SessionTime", VariableValueType.Double, "Seconds since session start", "s")
            .ToAdditionalTextFile(GeneratorConfigurationDefaults.VariableInfoFileName);

        var runResult = new VariablesGeneratorBuilder()
            .WithAdditionalText(variablesText)
            .Build()
            .RunGenerator();

        GeneratorAssert.TrackedStepExecuted(runResult, TrackingNames.VariableOptionsProvider_GetValueProvider);

        var variableOptionsResult = runResult.TrackedSteps[TrackingNames.VariableOptionsProvider_GetValueProvider].Single();

        var output = variableOptionsResult.Outputs.Single();
        var result = ((ImmutableArray<VariableOptions> Options, ImmutableArray<Diagnostic> Diagnostics))output.Value;

        Assert.Equal(IncrementalStepRunReason.New, output.Reason);
        Assert.Empty(result.Diagnostics);
        Assert.Empty(result.Options);
    }
}
