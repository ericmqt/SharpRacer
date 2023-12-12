using Microsoft.CodeAnalysis.CSharp;
using SharpRacer.SourceGenerators.TelemetryVariables.Diagnostics;
using SharpRacer.SourceGenerators.TelemetryVariables.InputModels;
using SharpRacer.SourceGenerators.TelemetryVariables.Pipeline;
using SharpRacer.SourceGenerators.TelemetryVariables.TestHelpers;

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

        var testModel = new VariablesGeneratorBuilder()
            .WithAdditionalText(GeneratorConfigurationDefaults.VariableInfoFileName, VariableInfoJson_SessionTime())
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

        var testModel = new VariablesGeneratorBuilder()
            .WithAdditionalText(GeneratorConfigurationDefaults.VariableInfoFileName, VariableInfoJson_SessionTime())
            .WithSyntaxTree(CSharpSyntaxTree.ParseText(descriptorClass1))
            .WithSyntaxTree(CSharpSyntaxTree.ParseText(descriptorClass2))
            .Build();

        var driver = testModel.GeneratorDriver.RunGenerators(testModel.Compilation);
        var runResult = driver.GetRunResult().Results.Single();

        var findTargetClassesResults = runResult.TrackedSteps[TrackingNames.DescriptorsGeneratorModelValueProvider_GetTargetClasses];

        Assert.Single(runResult.Diagnostics, x => x.Id == DiagnosticIds.DescriptorClass_AssemblyAlreadyContainsDescriptorClassTarget);
        Assert.Equal(2, findTargetClassesResults.Length);
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
