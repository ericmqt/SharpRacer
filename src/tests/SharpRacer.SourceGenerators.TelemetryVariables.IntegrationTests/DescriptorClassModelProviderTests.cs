using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using SharpRacer.SourceGenerators.TelemetryVariables.Diagnostics;
using SharpRacer.SourceGenerators.TelemetryVariables.GeneratorModels;
using SharpRacer.SourceGenerators.TelemetryVariables.InputModels;
using SharpRacer.SourceGenerators.TelemetryVariables.Pipeline;
using SharpRacer.SourceGenerators.TelemetryVariables.TestHelpers;
using SharpRacer.SourceGenerators.Testing.TelemetryVariables;

namespace SharpRacer.SourceGenerators.TelemetryVariables;
public class DescriptorClassModelProviderTests
{
    [Fact]
    public void GetValueProvider_DiagnosticsForMultipleDescriptorClassesTest()
    {
        var descriptorClass1 = @"
using SharpRacer.Telemetry.Variables;
namespace Test.Assembly;
[GenerateDataVariableDescriptors]
public static partial class MyDescriptors1 { }";

        var descriptorClass2 = @"
using SharpRacer.Telemetry.Variables;
namespace Test.Assembly;
[GenerateDataVariableDescriptors]
public static partial class MyDescriptors2 { }";

        var variablesText = new JsonVariableInfoDocumentBuilder()
            .AddScalar("SessionTime", VariableValueType.Double, "Seconds since session start", "s")
            .ToAdditionalTextFile(GeneratorConfigurationDefaults.VariableInfoFileName);

        var testModel = new VariablesGeneratorBuilder()
            .WithAdditionalText(variablesText)
            .WithSyntaxTree(CSharpSyntaxTree.ParseText(descriptorClass1))
            .WithSyntaxTree(CSharpSyntaxTree.ParseText(descriptorClass2))
            .Build();

        var driver = testModel.GeneratorDriver.RunGenerators(testModel.Compilation);
        var runResult = driver.GetRunResult().Results.Single();

        var valueProviderStep = runResult.TrackedSteps[TrackingNames.DescriptorClassModelProvider_GetValueProvider].Single();

        Assert.Single(valueProviderStep.Outputs);

        var valueProviderResult = valueProviderStep.Outputs.Single();
        Assert.Equal(IncrementalStepRunReason.New, valueProviderResult.Reason);

        var pipelineValueResult = (PipelineValueResult<DescriptorClassModel>)valueProviderResult.Value;

        Assert.True(pipelineValueResult.HasValue);
        Assert.Single(pipelineValueResult.Diagnostics);
        Assert.Single(runResult.Diagnostics, x => x.Id == DiagnosticIds.DescriptorClass_AssemblyAlreadyContainsDescriptorClassTarget);
    }

    [Fact]
    public void GetValueProvider_Test()
    {
        var descriptorClass = @"
using SharpRacer.Telemetry.Variables;
namespace Test.Assembly;
[GenerateDataVariableDescriptors]
public static partial class MyDescriptors { }";

        var variablesText = new JsonVariableInfoDocumentBuilder()
            .AddScalar("SessionTime", VariableValueType.Double, "Seconds since session start", "s")
            .ToAdditionalTextFile(GeneratorConfigurationDefaults.VariableInfoFileName);

        var testModel = new VariablesGeneratorBuilder()
            .WithAdditionalText(variablesText)
            .WithSyntaxTree(CSharpSyntaxTree.ParseText(descriptorClass))
            .Build();

        var driver = testModel.GeneratorDriver.RunGenerators(testModel.Compilation);
        var runResult = driver.GetRunResult().Results.Single();

        var valueProviderStep = runResult.TrackedSteps[TrackingNames.DescriptorClassModelProvider_GetValueProvider].Single();

        Assert.Single(valueProviderStep.Outputs);

        var valueProviderResult = valueProviderStep.Outputs.Single();
        Assert.Equal(IncrementalStepRunReason.New, valueProviderResult.Reason);

        var pipelineValueResult = (PipelineValueResult<DescriptorClassModel>)valueProviderResult.Value;

        Assert.True(pipelineValueResult.HasValue);
        Assert.Empty(pipelineValueResult.Diagnostics);

        var classModel = pipelineValueResult.Value;

        Assert.Equal("Test.Assembly", classModel.TypeNamespace);
        Assert.Equal("MyDescriptors", classModel.TypeName);
        Assert.Single(classModel.DescriptorProperties);
    }
}
