using Microsoft.CodeAnalysis;
using SharpRacer.SourceGenerators.TelemetryVariables.Diagnostics;
using SharpRacer.SourceGenerators.TelemetryVariables.InputModels;
using SharpRacer.SourceGenerators.TelemetryVariables.Pipeline;
using SharpRacer.SourceGenerators.TelemetryVariables.TestHelpers;

namespace SharpRacer.SourceGenerators.TelemetryVariables;
public class VariableInfoFileProviderTests
{
    [Fact]
    public void FindVariableInfoFile_Test()
    {
        var testModel = new VariablesGeneratorBuilder()
            .WithAdditionalText(GeneratorConfigurationDefaults.VariableInfoFileName, VariableInfoJson_SessionTime())
            .Build();

        var driver = testModel.GeneratorDriver.RunGenerators(testModel.Compilation);
        var runResult = driver.GetRunResult().Results.Single();

        var variableInfoFileResult = runResult.TrackedSteps[TrackingNames.VariableInfoFileProvider_GetValueProvider].Single();

        var output = variableInfoFileResult.Outputs.Single();
        var pipelineResult = (PipelineValueResult<VariableInfoFile>)output.Value;

        Assert.Equal(IncrementalStepRunReason.New, output.Reason);
        Assert.Empty(pipelineResult.Diagnostics);
        Assert.False(pipelineResult.HasErrors);
        Assert.Equal(VariableInfoJson_SessionTime(), pipelineResult.Value.SourceText.ToString());
    }

    [Fact]
    public void FindVariableInfoFile_ConfiguredFileName_Test()
    {
        var testModel = new VariablesGeneratorBuilder()
            .ConfigureGlobalOptions(options => options.VariableInfoFileName = "MyVariables.json")
            .WithAdditionalText("MyVariables.json", VariableInfoJson_SessionTime())
            .Build();

        var driver = testModel.GeneratorDriver.RunGenerators(testModel.Compilation);
        var runResult = driver.GetRunResult().Results.Single();

        var variableInfoFileResult = runResult.TrackedSteps[TrackingNames.VariableInfoFileProvider_GetValueProvider].Single();

        var output = variableInfoFileResult.Outputs.Single();
        var pipelineResult = (PipelineValueResult<VariableInfoFile>)output.Value;

        Assert.Equal(IncrementalStepRunReason.New, output.Reason);
        Assert.Empty(pipelineResult.Diagnostics);
        Assert.False(pipelineResult.HasErrors);
        Assert.Equal(VariableInfoJson_SessionTime(), pipelineResult.Value.SourceText.ToString());
    }

    [Fact]
    public void FindVariableInfoFile_FileNotFound_Test()
    {
        var testModel = new VariablesGeneratorBuilder()
            .WithAdditionalText("MyVariables.json", VariableInfoJson_SessionTime())
            .Build();

        var driver = testModel.GeneratorDriver.RunGenerators(testModel.Compilation);
        var runResult = driver.GetRunResult().Results.Single();

        var variableInfoFileResult = runResult.TrackedSteps[TrackingNames.VariableInfoFileProvider_GetValueProvider].Single();

        var output = variableInfoFileResult.Outputs.Single();
        var pipelineResult = (PipelineValueResult<VariableInfoFile>)output.Value;

        Assert.True(pipelineResult.HasErrors);
        Assert.True(runResult.Diagnostics.Any(x => x.Id == DiagnosticIds.VariableInfo_FileNotFound));
        Assert.Equal(default, pipelineResult.Value);
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
