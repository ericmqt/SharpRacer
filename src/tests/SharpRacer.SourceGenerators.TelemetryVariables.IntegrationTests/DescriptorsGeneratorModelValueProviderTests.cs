using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using SharpRacer.SourceGenerators.TelemetryVariables.Diagnostics;
using SharpRacer.SourceGenerators.TelemetryVariables.GeneratorModels;
using SharpRacer.SourceGenerators.TelemetryVariables.InputModels;
using SharpRacer.SourceGenerators.TelemetryVariables.Pipeline;
using SharpRacer.SourceGenerators.TelemetryVariables.TestHelpers;
using SharpRacer.SourceGenerators.Testing.TelemetryVariables;

namespace SharpRacer.SourceGenerators.TelemetryVariables;
public class DescriptorsGeneratorModelValueProviderTests
{
    [Fact]
    public void GetTargetClasses_SingleDescriptorClass_Test()
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

        var findTargetClassesRunResult = runResult.TrackedSteps[TrackingNames.DescriptorsGeneratorModelValueProvider_GetTargetClasses].Single();

        var output = findTargetClassesRunResult.Outputs.Single();

        var targetClass = (ClassWithGeneratorAttribute)output.Value;

        Assert.Equal("MyDescriptors", targetClass.ClassSymbol.Name);
    }

    [Fact]
    public void GetTargetClasses_DiagnosticsForMultipleDescriptorClassesTest()
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

        var findTargetClassesResults = runResult.TrackedSteps[TrackingNames.DescriptorsGeneratorModelValueProvider_GetTargetClasses];

        Assert.Single(runResult.Diagnostics, x => x.Id == DiagnosticIds.DescriptorClass_AssemblyAlreadyContainsDescriptorClassTarget);
        Assert.Equal(2, findTargetClassesResults.Length);
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

        var valueProviderResult = runResult.TrackedSteps[TrackingNames.DescriptorsGeneratorModelValueProvider_GetValueProvider].Single();

        Assert.Single(valueProviderResult.Outputs);
        var descriptorClassGenOutput = valueProviderResult.Outputs.Single();

        Assert.Equal(IncrementalStepRunReason.New, descriptorClassGenOutput.Reason);
        var descriptorClassValue = (DescriptorsGeneratorModel)descriptorClassGenOutput.Value;

        Assert.NotNull(descriptorClassValue.GeneratorModel);
        Assert.Equal("Test.Assembly", descriptorClassValue.GeneratorModel.TypeNamespace);
        Assert.Equal("MyDescriptors", descriptorClassValue.GeneratorModel.TypeName);
        Assert.Single(descriptorClassValue.GeneratorModel.DescriptorProperties);
        Assert.Single(descriptorClassValue.DescriptorPropertyReferences);
    }
}
