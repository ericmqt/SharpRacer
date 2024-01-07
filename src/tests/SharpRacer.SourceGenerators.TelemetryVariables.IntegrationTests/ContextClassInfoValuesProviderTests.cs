using Microsoft.CodeAnalysis;
using SharpRacer.SourceGenerators.TelemetryVariables.Diagnostics;
using SharpRacer.SourceGenerators.TelemetryVariables.InputModels;
using SharpRacer.SourceGenerators.TelemetryVariables.Pipeline;
using SharpRacer.SourceGenerators.TelemetryVariables.TestHelpers;
using SharpRacer.SourceGenerators.Testing.TelemetryVariables;

namespace SharpRacer.SourceGenerators.TelemetryVariables;
public class ContextClassInfoValuesProviderTests
{
    [Fact]
    public void GetValuesProvider_Test()
    {
        var contextClassDefinition = @"using SharpRacer.Telemetry.Variables
namespace Test.Assembly;
[GenerateDataVariablesContext(""TelemetryVariables_VariableNames.json"")]
internal partial class TelemetryVariables : IDataVariablesContext { }";

        var variablesText = new VariableInfoDocumentBuilder()
            .AddScalar("SessionTime", VariableValueType.Double, "Seconds since session start", "s")
            .ToAdditionalTextFile(GeneratorConfigurationDefaults.VariableInfoFileName);

        var includedVariables1 = IncludedVariablesDocumentBuilder.FromNames("SessionTime")
            .ToAdditionalTextFile("TelemetryVariables_VariableNames.json");

        var runResult = new VariablesGeneratorBuilder()
            .WithAdditionalText(variablesText)
            .WithAdditionalText(includedVariables1)
            .WithCSharpSyntaxTree(contextClassDefinition)
            .Build()
            .RunGenerator();

        GeneratorAssert.TrackedStepExecuted(runResult, TrackingNames.ContextClassInfoValuesProvider_GetValuesProvider);
        GeneratorAssert.NoDiagnostics(runResult);

        var stepResult = runResult.TrackedSteps[TrackingNames.ContextClassInfoValuesProvider_GetValuesProvider].Single();
        var stepOutput = stepResult.Outputs.Single();

        var result = (ContextClassInfo)stepOutput.Value;

        Assert.Equal("Test.Assembly", result.ClassNamespace);
        Assert.Equal("TelemetryVariables", result.ClassName);
        Assert.NotNull(result.GeneratorAttributeLocation);
        Assert.Equal("TelemetryVariables_VariableNames.json", result.IncludedVariablesFileName);
        Assert.Single(result.IncludedVariables.VariableNames);
        Assert.Single(result.IncludedVariables.VariableNames, x => x.Value == "SessionTime");
    }

    [Fact]
    public void Diagnostics_IncludedVariablesAdditionalTextFileReadExceptionTest()
    {
        var contextClassDefinition = @"using SharpRacer.Telemetry.Variables
namespace Test.Assembly;
[GenerateDataVariablesContext(""TelemetryVariables_VariableNames.json"")]
internal partial class TelemetryVariables : IDataVariablesContext { }";

        var variablesText = new VariableInfoDocumentBuilder()
            .AddScalar("SessionTime", VariableValueType.Double, "Seconds since session start", "s")
            .ToAdditionalTextFile(GeneratorConfigurationDefaults.VariableInfoFileName);

        var runResult = new VariablesGeneratorBuilder()
            .WithAdditionalText(variablesText)
            .WithAdditionalText("TelemetryVariables_VariableNames.json", "[ \"TestVar")
            .WithCSharpSyntaxTree(contextClassDefinition)
            .Build()
            .RunGenerator();

        GeneratorAssert.TrackedStepExecuted(runResult, TrackingNames.ContextClassInfoValuesProvider_ContextClassesWithIncludedVariables);
        GeneratorAssert.TrackedStepNotExecuted(runResult, TrackingNames.ContextClassInfoValuesProvider_GetValuesProvider);
        GeneratorAssert.ContainsDiagnostic(runResult, DiagnosticIds.AdditionalText_FileReadException);

        var stepResult = runResult.TrackedSteps[TrackingNames.ContextClassInfoValuesProvider_ContextClassesWithIncludedVariables].Single();
        var stepOutput = stepResult.Outputs.Single();

        var result = (PipelineValueResult<ContextClassInfo>)stepOutput.Value;

        Assert.True(result.HasErrors);
        Assert.False(result.HasValue);
        Assert.Single(result.Diagnostics);
        Assert.Single(result.Diagnostics, x => x.Id == DiagnosticIds.AdditionalText_FileReadException);
    }

    [Fact]
    public void Diagnostics_IncludedVariablesAmbiguousFileNameTest()
    {
        var contextClassDefinition = @"using SharpRacer.Telemetry.Variables
namespace Test.Assembly;
[GenerateDataVariablesContext(""TelemetryVariables_VariableNames.json"")]
internal partial class TelemetryVariables : IDataVariablesContext { }";

        var variablesText = new VariableInfoDocumentBuilder()
            .AddScalar("SessionTime", VariableValueType.Double, "Seconds since session start", "s")
            .ToAdditionalTextFile(GeneratorConfigurationDefaults.VariableInfoFileName);

        var includedVariables1 = IncludedVariablesDocumentBuilder.FromNames("SessionTime")
            .ToAdditionalTextFile("TelemetryVariables_VariableNames.json");

        var includedVariables2 = IncludedVariablesDocumentBuilder.FromNames("SessionTime")
            .ToAdditionalTextFile("Assets\\TelemetryVariables_VariableNames.json");

        var runResult = new VariablesGeneratorBuilder()
            .WithAdditionalText(variablesText)
            .WithAdditionalText(includedVariables1)
            .WithAdditionalText(includedVariables2)
            .WithCSharpSyntaxTree(contextClassDefinition)
            .Build()
            .RunGenerator();

        GeneratorAssert.TrackedStepExecuted(runResult, TrackingNames.ContextClassInfoValuesProvider_FindIncludedVariablesFiles);
        GeneratorAssert.TrackedStepNotExecuted(runResult, TrackingNames.ContextClassInfoValuesProvider_GetValuesProvider);
        GeneratorAssert.ContainsDiagnostic(runResult, DiagnosticIds.IncludedVariables_AmbiguousFileName);

        var stepResult = runResult.TrackedSteps[TrackingNames.ContextClassInfoValuesProvider_FindIncludedVariablesFiles].Single();

        Assert.NotNull(stepResult);
        Assert.Single(stepResult.Inputs);
        Assert.Single(stepResult.Outputs);

        var stepOutput = stepResult.Outputs.Single();

        var result = (PipelineValueResult<(ContextClassInfo, IncludedVariablesFile)>)stepOutput.Value;

        Assert.True(result.HasErrors);
        Assert.False(result.HasValue);
        Assert.Single(result.Diagnostics);
        Assert.Single(result.Diagnostics, x => x.Id == DiagnosticIds.IncludedVariables_AmbiguousFileName);
    }

    [Fact]
    public void Diagnostics_IncludedVariablesFileNotFoundTest()
    {
        var contextClassDefinition = @"using SharpRacer.Telemetry.Variables
namespace Test.Assembly;
[GenerateDataVariablesContext(""TelemetryVariables_VariableNames.json"")]
internal partial class TelemetryVariables : IDataVariablesContext { }";

        var variablesText = new VariableInfoDocumentBuilder()
            .AddScalar("SessionTime", VariableValueType.Double, "Seconds since session start", "s")
            .ToAdditionalTextFile(GeneratorConfigurationDefaults.VariableInfoFileName);

        var runResult = new VariablesGeneratorBuilder()
            .WithAdditionalText(variablesText)
            .WithCSharpSyntaxTree(contextClassDefinition)
            .Build()
            .RunGenerator();

        GeneratorAssert.TrackedStepExecuted(runResult, TrackingNames.ContextClassInfoValuesProvider_FindIncludedVariablesFiles);
        GeneratorAssert.TrackedStepNotExecuted(runResult, TrackingNames.ContextClassInfoValuesProvider_GetValuesProvider);
        GeneratorAssert.ContainsDiagnostic(runResult, DiagnosticIds.IncludedVariables_FileNotFound);

        var stepResult = runResult.TrackedSteps[TrackingNames.ContextClassInfoValuesProvider_FindIncludedVariablesFiles].Single();

        Assert.NotNull(stepResult);
        Assert.Single(stepResult.Inputs);
        Assert.Single(stepResult.Outputs);

        var stepOutput = stepResult.Outputs.Single();
        Assert.Equal(IncrementalStepRunReason.New, stepOutput.Reason);

        var result = (PipelineValueResult<(ContextClassInfo, IncludedVariablesFile)>)stepOutput.Value;

        Assert.True(result.HasErrors);
        Assert.False(result.HasValue);
        Assert.Single(result.Diagnostics);
        Assert.Single(result.Diagnostics, x => x.Id == DiagnosticIds.IncludedVariables_FileNotFound);
    }

    [Fact]
    public void Diagnostics_TargetClassDoesNotHaveIDataVariablesContextInterfaceTest()
    {
        var contextClassDefinition = @"using SharpRacer.Telemetry.Variables
namespace Test.Assembly;
[GenerateDataVariablesContext(""TelemetryVariables_VariableNames.json"")]
internal partial class TelemetryVariables { }";

        var variablesText = new VariableInfoDocumentBuilder()
            .AddScalar("SessionTime", VariableValueType.Double, "Seconds since session start", "s")
            .ToAdditionalTextFile(GeneratorConfigurationDefaults.VariableInfoFileName);

        var runResult = new VariablesGeneratorBuilder()
            .WithAdditionalText(variablesText)
            .WithCSharpSyntaxTree(contextClassDefinition)
            .Build()
            .RunGenerator();

        GeneratorAssert.TrackedStepExecuted(runResult, TrackingNames.ContextClassInfoValuesProvider_GetContextClassResults);
        GeneratorAssert.TrackedStepNotExecuted(runResult, TrackingNames.ContextClassInfoValuesProvider_GetValuesProvider);
        GeneratorAssert.ContainsDiagnostic(runResult, DiagnosticIds.ContextClassMustInheritIDataVariablesContextInterface);

        var contextClassResultsStep = runResult.TrackedSteps[TrackingNames.ContextClassInfoValuesProvider_GetContextClassResults].Single();

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

        var variablesText = new VariableInfoDocumentBuilder()
            .AddScalar("SessionTime", VariableValueType.Double, "Seconds since session start", "s")
            .ToAdditionalTextFile(GeneratorConfigurationDefaults.VariableInfoFileName);

        var runResult = new VariablesGeneratorBuilder()
            .WithAdditionalText(variablesText)
            .WithCSharpSyntaxTree(contextClassDefinition)
            .Build()
            .RunGenerator();

        GeneratorAssert.TrackedStepExecuted(runResult, TrackingNames.ContextClassInfoValuesProvider_GetContextClassResults);
        GeneratorAssert.TrackedStepNotExecuted(runResult, TrackingNames.ContextClassInfoValuesProvider_GetValuesProvider);
        GeneratorAssert.ContainsDiagnostic(runResult, DiagnosticIds.ContextClassMustBeDeclaredPartial);

        var contextClassResultsStep = runResult.TrackedSteps[TrackingNames.ContextClassInfoValuesProvider_GetContextClassResults].Single();

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

        var variablesText = new VariableInfoDocumentBuilder()
            .AddScalar("SessionTime", VariableValueType.Double, "Seconds since session start", "s")
            .ToAdditionalTextFile(GeneratorConfigurationDefaults.VariableInfoFileName);

        var runResult = new VariablesGeneratorBuilder()
            .WithAdditionalText(variablesText)
            .WithCSharpSyntaxTree(contextClassDefinition)
            .Build()
            .RunGenerator();

        GeneratorAssert.TrackedStepExecuted(runResult, TrackingNames.ContextClassInfoValuesProvider_GetContextClassResults);
        GeneratorAssert.TrackedStepNotExecuted(runResult, TrackingNames.ContextClassInfoValuesProvider_GetValuesProvider);
        GeneratorAssert.ContainsDiagnostic(runResult, DiagnosticIds.ContextClassMustNotBeDeclaredStatic);

        var contextClassResultsStep = runResult.TrackedSteps[TrackingNames.ContextClassInfoValuesProvider_GetContextClassResults].Single();

        var contextClassResultsValue = contextClassResultsStep.Outputs.Single();
        Assert.Equal(IncrementalStepRunReason.New, contextClassResultsValue.Reason);

        var contextClassResult = (ContextClassResult)contextClassResultsValue.Value;

        Assert.False(contextClassResult.IsValid);
        Assert.Single(contextClassResult.Diagnostics);
        Assert.Single(contextClassResult.Diagnostics, x => x.Id == DiagnosticIds.ContextClassMustNotBeDeclaredStatic);
    }
}
