using Microsoft.CodeAnalysis;
using SharpRacer.SourceGenerators.TelemetryVariables.InputModels;
using SharpRacer.SourceGenerators.TelemetryVariables.Pipeline;
using SharpRacer.SourceGenerators.TelemetryVariables.TestHelpers;

namespace SharpRacer.SourceGenerators.TelemetryVariables;
public class VariableOptionsProviderTests
{
    [Fact]
    public void NoOptionsFile_Test()
    {
        var testModel = new VariablesGeneratorBuilder()
            .WithAdditionalText(GeneratorConfigurationDefaults.VariableInfoFileName, VariableInfoJson_SessionTime())
            .Build();

        var driver = testModel.GeneratorDriver.RunGenerators(testModel.Compilation);
        var runResult = driver.GetRunResult().Results.Single();

        var variableOptionsResult = runResult.TrackedSteps[TrackingNames.VariableModelsValueProvider_GetVariableOptionsProvider].Single();

        var output = variableOptionsResult.Outputs.Single();
        var pipelineResult = (PipelineValuesResult<VariableOptions>)output.Value;

        Assert.Equal(IncrementalStepRunReason.New, output.Reason);
        Assert.False(pipelineResult.IsDefault);
        Assert.False(pipelineResult.Diagnostics.HasErrors());
        Assert.Empty(pipelineResult.Values);
    }

    private static string VariableInfoJson_SessionTime()
    {
        return @"[
  {
    ""DeprecatedBy"": null,
    ""Description"": ""Seconds since session start"",
    ""IsDeprecated"": false,
    ""IsTimeSliceArray"": false,
    ""Name"": ""SessionTime"",
    ""ValueCount"": 1,
    ""ValueType"": ""Double"",
    ""ValueUnit"": ""s""
  }
]";
    }
}
