using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using SharpRacer.SourceGenerators.TelemetryVariables.Diagnostics;
using SharpRacer.SourceGenerators.TelemetryVariables.InputModels;
using SharpRacer.SourceGenerators.TelemetryVariables.Pipeline;
using SharpRacer.SourceGenerators.TelemetryVariables.TestHelpers;
using SharpRacer.SourceGenerators.Testing.TelemetryVariables;

namespace SharpRacer.SourceGenerators.TelemetryVariables;
public class ContextClassInfoValuesProviderTests
{
    [Fact]
    public void Diagnostics_TargetClassDoesNotHaveIDataVariablesContextInterfaceTest()
    {
        var contextClassDefinition = @"using SharpRacer.Telemetry.Variables
namespace Test.Assembly;
[GenerateDataVariablesContext(""TelemetryVariables_VariableNames.json"")]
internal partial class TelemetryVariables { }";

        var variablesText = new JsonVariableInfoDocumentBuilder()
            .AddScalar("SessionTime", VariableValueType.Double, "Seconds since session start", "s")
            .ToAdditionalTextFile(GeneratorConfigurationDefaults.VariableInfoFileName);

        var testModel = new VariablesGeneratorBuilder()
            .WithAdditionalText(variablesText)
            .WithSyntaxTree(CSharpSyntaxTree.ParseText(contextClassDefinition))
            .Build();

        var driver = testModel.GeneratorDriver.RunGenerators(testModel.Compilation);
        var runResult = driver.GetRunResult().Results.Single();

        var contextClassResultsStep = runResult.TrackedSteps[TrackingNames.ContextClassInfoValuesProvider_GetContextClassResults].Single();

        Assert.Single(contextClassResultsStep.Inputs);
        Assert.Single(contextClassResultsStep.Outputs);

        var contextClassResultsValue = contextClassResultsStep.Outputs.Single();
        Assert.Equal(IncrementalStepRunReason.New, contextClassResultsValue.Reason);

        var contextClassResult = (ContextClassResult)contextClassResultsValue.Value;

        Assert.False(contextClassResult.IsValid);
        Assert.Single(contextClassResult.Diagnostics);
        Assert.Single(runResult.Diagnostics, x => x.Id == DiagnosticIds.ContextClassMustInheritIDataVariablesContextInterface);
    }

    [Fact]
    public void Diagnostics_TargetClassIsNotPartialTest()
    {
        var contextClassDefinition = @"using SharpRacer.Telemetry.Variables
namespace Test.Assembly;
[GenerateDataVariablesContext(""TelemetryVariables_VariableNames.json"")]
internal class TelemetryVariables : IDataVariablesContext { }";

        var variablesText = new JsonVariableInfoDocumentBuilder()
            .AddScalar("SessionTime", VariableValueType.Double, "Seconds since session start", "s")
            .ToAdditionalTextFile(GeneratorConfigurationDefaults.VariableInfoFileName);

        var testModel = new VariablesGeneratorBuilder()
            .WithAdditionalText(variablesText)
            .WithSyntaxTree(CSharpSyntaxTree.ParseText(contextClassDefinition))
            .Build();

        var driver = testModel.GeneratorDriver.RunGenerators(testModel.Compilation);
        var runResult = driver.GetRunResult().Results.Single();

        var contextClassResultsStep = runResult.TrackedSteps[TrackingNames.ContextClassInfoValuesProvider_GetContextClassResults].Single();

        Assert.Single(contextClassResultsStep.Inputs);
        Assert.Single(contextClassResultsStep.Outputs);

        var contextClassResultsValue = contextClassResultsStep.Outputs.Single();
        Assert.Equal(IncrementalStepRunReason.New, contextClassResultsValue.Reason);

        var contextClassResult = (ContextClassResult)contextClassResultsValue.Value;

        Assert.False(contextClassResult.IsValid);
        Assert.Single(contextClassResult.Diagnostics);
        Assert.Single(runResult.Diagnostics, x => x.Id == DiagnosticIds.ContextClassMustBeDeclaredPartial);
    }

    [Fact]
    public void Diagnostics_TargetClassIsStaticTest()
    {
        var contextClassDefinition = @"using SharpRacer.Telemetry.Variables
namespace Test.Assembly;
[GenerateDataVariablesContext(""TelemetryVariables_VariableNames.json"")]
internal static partial class TelemetryVariables : IDataVariablesContext { }";

        var variablesText = new JsonVariableInfoDocumentBuilder()
            .AddScalar("SessionTime", VariableValueType.Double, "Seconds since session start", "s")
            .ToAdditionalTextFile(GeneratorConfigurationDefaults.VariableInfoFileName);

        var testModel = new VariablesGeneratorBuilder()
            .WithAdditionalText(variablesText)
            .WithSyntaxTree(CSharpSyntaxTree.ParseText(contextClassDefinition))
            .Build();

        var driver = testModel.GeneratorDriver.RunGenerators(testModel.Compilation);
        var runResult = driver.GetRunResult().Results.Single();

        var contextClassResultsStep = runResult.TrackedSteps[TrackingNames.ContextClassInfoValuesProvider_GetContextClassResults].Single();

        Assert.Single(contextClassResultsStep.Inputs);
        Assert.Single(contextClassResultsStep.Outputs);

        var contextClassResultsValue = contextClassResultsStep.Outputs.Single();
        Assert.Equal(IncrementalStepRunReason.New, contextClassResultsValue.Reason);

        var contextClassResult = (ContextClassResult)contextClassResultsValue.Value;

        Assert.False(contextClassResult.IsValid);
        Assert.Single(contextClassResult.Diagnostics);
        Assert.Single(runResult.Diagnostics, x => x.Id == DiagnosticIds.ContextClassMustNotBeDeclaredStatic);
    }
}
