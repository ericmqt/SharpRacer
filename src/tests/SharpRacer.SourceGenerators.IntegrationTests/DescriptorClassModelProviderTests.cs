using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using SharpRacer.SourceGenerators.TelemetryVariables.Diagnostics;
using SharpRacer.SourceGenerators.TelemetryVariables.InputModels;
using SharpRacer.SourceGenerators.TelemetryVariables.Pipeline;
using SharpRacer.SourceGenerators.TelemetryVariables.TestHelpers;
using SharpRacer.SourceGenerators.Testing.TelemetryVariables;

using DescriptorClassModelResult = (
    SharpRacer.SourceGenerators.TelemetryVariables.GeneratorModels.DescriptorClassModel Model,
    System.Collections.Immutable.ImmutableArray<Microsoft.CodeAnalysis.Diagnostic> Diagnostics);

namespace SharpRacer.SourceGenerators.TelemetryVariables;
public class DescriptorClassModelProviderTests
{
    [Fact]
    public void GetValueProvider_Test()
    {
        var descriptorClass = @"
using SharpRacer.Telemetry.Variables;
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
    public void GetValueProvider_TargetClassIsNotPartialDiagnosticTest()
    {
        var descriptorClass = @"
using SharpRacer.Telemetry.Variables;
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
    public void GetValueProvider_TargetClassIsNotStaticDiagnosticTest()
    {
        var descriptorClass = @"
using SharpRacer.Telemetry.Variables;
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
