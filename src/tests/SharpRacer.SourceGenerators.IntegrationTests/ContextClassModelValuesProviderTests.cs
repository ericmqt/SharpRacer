using SharpRacer.SourceGenerators.TelemetryVariables.Diagnostics;
using SharpRacer.SourceGenerators.TelemetryVariables.InputModels;
using SharpRacer.SourceGenerators.TelemetryVariables.Pipeline;
using SharpRacer.SourceGenerators.TelemetryVariables.TestHelpers;
using SharpRacer.SourceGenerators.Testing.TelemetryVariables;
using ContextClassModelsResult = (
    SharpRacer.SourceGenerators.TelemetryVariables.GeneratorModels.ContextClassModel Model,
    System.Collections.Immutable.ImmutableArray<Microsoft.CodeAnalysis.Diagnostic> Diagnostics);

namespace SharpRacer.SourceGenerators.TelemetryVariables;
public class ContextClassModelValuesProviderTests
{
    [Fact]
    public void GetValuesProvider_NoContextsTest()
    {
        var variablesText = new VariableInfoDocumentBuilder()
           .AddScalar("SessionTime", VariableValueType.Double, "Seconds since session start", "s")
           .AddScalar("SessionTick", VariableValueType.Int, "Current update number", null)
           .ToAdditionalTextFile(GeneratorConfigurationDefaults.VariableInfoFileName);

        var runResult = new VariablesGeneratorBuilder()
            .WithAdditionalText(variablesText)
            .Build()
            .RunGenerator();

        GeneratorAssert.NoException(runResult);
        GeneratorAssert.NoDiagnostics(runResult);
        GeneratorAssert.TrackedStepNotExecuted(runResult, TrackingNames.ContextClassModelValuesProvider_GetValuesProvider);
    }

    [Fact]
    public void GetValuesProvider_ReturnsDefaultModelValueOnContextClassInfoErrorTest()
    {
        var contextClassDefinition = @"using SharpRacer.Telemetry.Variables
namespace Test.Assembly;
[GenerateDataVariablesContext]
internal class TelemetryVariables { }";

        var variablesText = new VariableInfoDocumentBuilder()
            .AddScalar("SessionTime", VariableValueType.Double, "Seconds since session start", "s")
            .AddScalar("SessionTick", VariableValueType.Int, "Current update number", null)
            .ToAdditionalTextFile(GeneratorConfigurationDefaults.VariableInfoFileName);

        var runResult = new VariablesGeneratorBuilder()
            .WithAdditionalText(variablesText)
            .WithCSharpSyntaxTree(contextClassDefinition)
            .Build()
            .RunGenerator();

        GeneratorAssert.NoException(runResult);

        var stepResult = GeneratorAssert.TrackedStepExecuted(runResult, TrackingNames.ContextClassModelValuesProvider_GetValuesProvider).Single();
        var stepOutput = stepResult.Outputs.Single();

        var result = (ContextClassModelsResult)stepOutput.Value;

        Assert.Equal(default, result.Model);
        Assert.NotEmpty(result.Diagnostics);
    }

    [Fact]
    public void GetValuesProvider_SingleContextTest()
    {
        var contextClassDefinition = @"using SharpRacer.Telemetry.Variables
namespace Test.Assembly;
[GenerateDataVariablesContext]
internal partial class TelemetryVariables : IDataVariablesContext { }";

        var variablesText = new VariableInfoDocumentBuilder()
            .AddScalar("SessionTime", VariableValueType.Double, "Seconds since session start", "s")
            .AddScalar("SessionTick", VariableValueType.Int, "Current update number", null)
            .ToAdditionalTextFile(GeneratorConfigurationDefaults.VariableInfoFileName);

        var runResult = new VariablesGeneratorBuilder()
            .WithAdditionalText(variablesText)
            .WithCSharpSyntaxTree(contextClassDefinition)
            .Build()
            .RunGenerator();

        GeneratorAssert.NoException(runResult);
        GeneratorAssert.NoDiagnostics(runResult);

        var stepResult = GeneratorAssert.TrackedStepExecuted(runResult, TrackingNames.ContextClassModelValuesProvider_GetValuesProvider).Single();
        var stepOutput = stepResult.Outputs.Single();

        var result = (ContextClassModelsResult)stepOutput.Value;

        Assert.Equal("Test.Assembly", result.Model.ClassNamespace);
        Assert.Equal("TelemetryVariables", result.Model.ClassName);
        Assert.Empty(result.Diagnostics);
        Assert.Equal(2, result.Model.Variables.Length);
    }

    [Fact]
    public void GetValuesProvider_SingleContextWithIncludesTest()
    {
        var contextClassDefinition = @"using SharpRacer.Telemetry.Variables
namespace Test.Assembly;
[GenerateDataVariablesContext(""Assets\\TelemetryVariables_VariableNames.json"")]
internal partial class TelemetryVariables : IDataVariablesContext { }";

        var variablesText = new VariableInfoDocumentBuilder()
            .AddScalar("SessionTime", VariableValueType.Double, "Seconds since session start", "s")
            .AddScalar("SessionTick", VariableValueType.Int, "Current update number", null)
            .ToAdditionalTextFile(GeneratorConfigurationDefaults.VariableInfoFileName);

        var includedVariables1 = IncludedVariablesDocumentBuilder.FromNames("SessionTime")
            .ToAdditionalTextFile("Assets\\TelemetryVariables_VariableNames.json");

        var runResult = new VariablesGeneratorBuilder()
            .WithAdditionalText(variablesText)
            .WithAdditionalText(includedVariables1)
            .WithCSharpSyntaxTree(contextClassDefinition)
            .Build()
            .RunGenerator();

        GeneratorAssert.NoException(runResult);
        GeneratorAssert.NoDiagnostics(runResult);

        var stepResult = GeneratorAssert.TrackedStepExecuted(runResult, TrackingNames.ContextClassModelValuesProvider_GetValuesProvider).Single();
        var stepOutput = stepResult.Outputs.Single();

        var result = (ContextClassModelsResult)stepOutput.Value;

        Assert.Equal("Test.Assembly", result.Model.ClassNamespace);
        Assert.Equal("TelemetryVariables", result.Model.ClassName);
        Assert.Empty(result.Diagnostics);
        Assert.Single(result.Model.Variables);
        Assert.Single(result.Model.Variables, x => x.VariableModel.VariableName == "SessionTime");
    }

    [Fact]
    public void GetValuesProvider_IncludedVariableNotFoundTest()
    {
        var contextClassDefinition = @"using SharpRacer.Telemetry.Variables
namespace Test.Assembly;
[GenerateDataVariablesContext(""TelemetryVariables_VariableNames.json"")]
internal partial class TelemetryVariables : IDataVariablesContext { }";

        var variablesText = new VariableInfoDocumentBuilder()
            .AddScalar("SessionTime", VariableValueType.Double, "Seconds since session start", "s")
            .AddScalar("SessionTick", VariableValueType.Int, "Current update number", null)
            .ToAdditionalTextFile(GeneratorConfigurationDefaults.VariableInfoFileName);

        var includedVariables1 = IncludedVariablesDocumentBuilder.FromNames("NonExistentVariable")
            .ToAdditionalTextFile("TelemetryVariables_VariableNames.json");

        var runResult = new VariablesGeneratorBuilder()
            .WithAdditionalText(variablesText)
            .WithAdditionalText(includedVariables1)
            .WithCSharpSyntaxTree(contextClassDefinition)
            .Build()
            .RunGenerator();

        GeneratorAssert.NoException(runResult);
        GeneratorAssert.ContainsDiagnostic(runResult, DiagnosticIds.ContextClassIncludedVariableNotFound);
    }
}
