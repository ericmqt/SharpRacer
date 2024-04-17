using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using SharpRacer.SourceGenerators.TelemetryVariables.Diagnostics;
using SharpRacer.SourceGenerators.TelemetryVariables.InputModels;
using SharpRacer.SourceGenerators.TelemetryVariables.Json;
using SharpRacer.SourceGenerators.TelemetryVariables.TestHelpers;
using SharpRacer.SourceGenerators.Testing.TelemetryVariables;

using DescriptorClassModelResult = (
    SharpRacer.SourceGenerators.TelemetryVariables.GeneratorModels.DescriptorClassModel Model,
    System.Collections.Immutable.ImmutableArray<Microsoft.CodeAnalysis.Diagnostic> Diagnostics);

using DescriptorPropertiesResult = (
    System.Collections.Immutable.ImmutableArray<SharpRacer.SourceGenerators.TelemetryVariables.GeneratorModels.DescriptorPropertyModel> Properties,
    System.Collections.Immutable.ImmutableArray<Microsoft.CodeAnalysis.Diagnostic> Diagnostics);

namespace SharpRacer.SourceGenerators.TelemetryVariables.Pipeline;
public class DescriptorClassModelProviderTests
{
    [Fact]
    public void GetValueProvider_Test()
    {
        var descriptorClass = @"
using SharpRacer.Telemetry;
namespace Test.Assembly;
[GenerateDataVariableDescriptors]
public static partial class MyDescriptors { }";

        var variablesText = new VariableInfoDocumentBuilder()
            .AddScalar("SessionTime", VariableValueType.Double, "Seconds since session start", "s")
            .ToAdditionalTextFile(GeneratorConfigurationDefaults.VariableInfoFileName);

        var runResult = new VariablesGeneratorBuilder()
            .WithAdditionalText(variablesText)
            .WithCSharpSyntaxTree(descriptorClass)
            .Build()
            .RunGenerator();

        var valueProviderStep = GeneratorAssert.TrackedStepExecuted(
            runResult, TrackingNames.DescriptorClassModelProvider_GetValueProvider)
            .Single();

        Assert.Single(valueProviderStep.Outputs);

        var valueProviderResult = valueProviderStep.Outputs.Single();
        Assert.Equal(IncrementalStepRunReason.New, valueProviderResult.Reason);

        var result = (DescriptorClassModelResult)valueProviderResult.Value;

        Assert.NotEqual(default, result.Model);
        Assert.Empty(result.Diagnostics);

        var classModel = result.Model;

        Assert.Equal("Test.Assembly", classModel.TypeNamespace);
        Assert.Equal("MyDescriptors", classModel.TypeName);
        Assert.Single(classModel.DescriptorProperties);
    }

    [Fact]
    public void DescriptorNameConflictsWithExistingVariableTest()
    {
        var descriptorClass = @"
using SharpRacer.Telemetry;
namespace Test.Assembly;
[GenerateDataVariableDescriptors]
public static partial class MyDescriptors { }";

        var variablesText = new VariableInfoDocumentBuilder()
            .AddScalar("SessionTime", VariableValueType.Double, "Seconds since session start", "s")
            .AddScalar("SessionTick", VariableValueType.Int, "Current update number", null)
            .ToAdditionalTextFile(GeneratorConfigurationDefaults.VariableInfoFileName);

        var variableOptionsText = new JsonVariableOptionsDocumentBuilder()
            .Add("SessionTime", new JsonVariableOptionsValue(null, null))
            .Add("SessionTick", new JsonVariableOptionsValue("SessionTime", null)) // produces SessionTime property conflict
            .ToAdditionalTextFile(GeneratorConfigurationDefaults.VariableOptionsFileName);

        var runResult = new VariablesGeneratorBuilder()
            .WithAdditionalText(variablesText)
            .WithAdditionalText(variableOptionsText)
            .WithCSharpSyntaxTree(descriptorClass)
            .Build()
            .RunGenerator();

        GeneratorAssert.NoException(runResult);
        GeneratorAssert.ContainsDiagnostic(runResult, DiagnosticIds.DescriptorNameConflictsWithExistingVariable);

        var stepResult = GeneratorAssert.TrackedStepExecuted(
            runResult, TrackingNames.DescriptorClassModelProvider_GetDescriptorProperties)
            .Single();

        var stepOutput = stepResult.Outputs.Single();

        var result = (DescriptorPropertiesResult)stepOutput.Value;

        Assert.Single(result.Properties, x => x.VariableName == "SessionTime");
        Assert.Single(result.Diagnostics, x => x.Id == DiagnosticIds.DescriptorNameConflictsWithExistingVariable);
    }

    [Fact]
    public void MultipleDescriptorClassesDiagnosticTest()
    {
        var descriptorClass1 = @"
using SharpRacer.Telemetry;
namespace Test.Assembly;
[GenerateDataVariableDescriptors]
public static partial class MyDescriptors1 { }";

        var descriptorClass2 = @"
using SharpRacer.Telemetry;
namespace Test.Assembly;
[GenerateDataVariableDescriptors]
public static partial class MyDescriptors2 { }";

        var variablesText = new VariableInfoDocumentBuilder()
            .AddScalar("SessionTime", VariableValueType.Double, "Seconds since session start", "s")
            .ToAdditionalTextFile(GeneratorConfigurationDefaults.VariableInfoFileName);

        var runResult = new VariablesGeneratorBuilder()
            .WithAdditionalText(variablesText)
            .WithSyntaxTree(CSharpSyntaxTree.ParseText(descriptorClass1))
            .WithSyntaxTree(CSharpSyntaxTree.ParseText(descriptorClass2))
            .Build()
            .RunGenerator();

        var valueProviderStep = GeneratorAssert.TrackedStepExecuted(
            runResult, TrackingNames.DescriptorClassModelProvider_GetValueProvider)
            .Single();

        Assert.Single(valueProviderStep.Outputs);

        var valueProviderResult = valueProviderStep.Outputs.Single();
        Assert.Equal(IncrementalStepRunReason.New, valueProviderResult.Reason);

        var result = (DescriptorClassModelResult)valueProviderResult.Value;

        Assert.NotEqual(default, result.Model);
        Assert.Single(result.Diagnostics);
        Assert.Single(runResult.Diagnostics, x => x.Id == DiagnosticIds.DescriptorClassAlreadyExistsInAssembly);
    }

    [Fact]
    public void TargetClassIsNotPartialDiagnosticTest()
    {
        var descriptorClass = @"
using SharpRacer.Telemetry;
namespace Test.Assembly;
[GenerateDataVariableDescriptors]
public static class MyDescriptors { }";

        var variablesText = new VariableInfoDocumentBuilder()
            .AddScalar("SessionTime", VariableValueType.Double, "Seconds since session start", "s")
            .ToAdditionalTextFile(GeneratorConfigurationDefaults.VariableInfoFileName);

        var runResult = new VariablesGeneratorBuilder()
            .WithAdditionalText(variablesText)
            .WithCSharpSyntaxTree(descriptorClass)
            .Build()
            .RunGenerator();

        var valueProviderStep = GeneratorAssert.TrackedStepExecuted(
            runResult, TrackingNames.DescriptorClassModelProvider_GetValueProvider)
            .Single();

        GeneratorAssert.ContainsDiagnostic(runResult, DiagnosticIds.DescriptorClassMustBeDeclaredPartial);

        var valueProviderResult = valueProviderStep.Outputs.Single();

        var result = (DescriptorClassModelResult)valueProviderResult.Value;

        Assert.Equal(default, result.Model);
        Assert.Single(result.Diagnostics);
        Assert.Single(result.Diagnostics, x => x.Id == DiagnosticIds.DescriptorClassMustBeDeclaredPartial);
    }

    [Fact]
    public void TargetClassIsNotStaticDiagnosticTest()
    {
        var descriptorClass = @"
using SharpRacer.Telemetry;
namespace Test.Assembly;
[GenerateDataVariableDescriptors]
public partial class MyDescriptors { }";

        var variablesText = new VariableInfoDocumentBuilder()
            .AddScalar("SessionTime", VariableValueType.Double, "Seconds since session start", "s")
            .ToAdditionalTextFile(GeneratorConfigurationDefaults.VariableInfoFileName);

        var runResult = new VariablesGeneratorBuilder()
            .WithAdditionalText(variablesText)
            .WithCSharpSyntaxTree(descriptorClass)
            .Build()
            .RunGenerator();

        var valueProviderStep = GeneratorAssert.TrackedStepExecuted(
            runResult, TrackingNames.DescriptorClassModelProvider_GetValueProvider)
            .Single();

        GeneratorAssert.ContainsDiagnostic(runResult, DiagnosticIds.DescriptorClassMustBeDeclaredStatic);

        var valueProviderResult = valueProviderStep.Outputs.Single();

        var result = (DescriptorClassModelResult)valueProviderResult.Value;

        Assert.Equal(default, result.Model);
        Assert.Single(result.Diagnostics);
        Assert.Single(result.Diagnostics, x => x.Id == DiagnosticIds.DescriptorClassMustBeDeclaredStatic);
    }
}
