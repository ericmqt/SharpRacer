using SharpRacer.SourceGenerators.TelemetryVariables.Diagnostics;
using SharpRacer.SourceGenerators.TelemetryVariables.InputModels;
using SharpRacer.SourceGenerators.TelemetryVariables.Json;
using SharpRacer.SourceGenerators.TelemetryVariables.TestHelpers;
using SharpRacer.SourceGenerators.Testing.TelemetryVariables;
using ContextClassInfoIncludedVariables = (
    SharpRacer.SourceGenerators.TelemetryVariables.InputModels.ContextClassInfo ClassInfo,
    System.Collections.Immutable.ImmutableArray<SharpRacer.SourceGenerators.TelemetryVariables.GeneratorModels.ContextVariableModel> IncludedVariables,
    System.Collections.Immutable.ImmutableArray<Microsoft.CodeAnalysis.Diagnostic> Diagnostics);
using ContextClassModelsResult = (
    SharpRacer.SourceGenerators.TelemetryVariables.GeneratorModels.ContextClassModel Model,
    System.Collections.Immutable.ImmutableArray<Microsoft.CodeAnalysis.Diagnostic> Diagnostics);

namespace SharpRacer.SourceGenerators.TelemetryVariables.Pipeline;

public class ContextClassModelValuesProviderTests
{
    [Fact]
    public void NoContextsTest()
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
    public void SingleContextTest()
    {
        var contextClassDefinition = @"using SharpRacer.Telemetry;
namespace Test.Assembly;
[GenerateTelemetryVariablesContext]
internal partial class TelemetryVariables : ITelemetryVariablesContext { }";

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
    public void SingleContextWithIncludesTest()
    {
        var contextClassDefinition = @"using SharpRacer.Telemetry;
namespace Test.Assembly;
[GenerateTelemetryVariablesContext(""Assets\\TelemetryVariables_VariableNames.json"")]
internal partial class TelemetryVariables : ITelemetryVariablesContext { }";

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
    public void ConfiguredPropertyNameConflictTest()
    {
        var contextClassDefinition = @"using SharpRacer.Telemetry;
namespace Test.Assembly;
[GenerateTelemetryVariablesContext(""TelemetryVariables_VariableNames.json"")]
internal partial class TelemetryVariables : ITelemetryVariablesContext { }";

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
            .WithCSharpSyntaxTree(contextClassDefinition)
            .Build()
            .RunGenerator();

        GeneratorAssert.NoException(runResult);
        GeneratorAssert.ContainsDiagnostic(runResult, DiagnosticIds.ContextClassConfiguredPropertyNameConflict);

        var stepResult = GeneratorAssert.TrackedStepExecuted(
            runResult, TrackingNames.ContextClassModelValuesProvider_ContextClassIncludedVariables).Single();

        GeneratorAssert.TrackedStepExecuted(runResult, TrackingNames.ContextClassModelValuesProvider_GetValuesProvider);

        var stepOutput = stepResult.Outputs.Single();

        var result = (ContextClassInfoIncludedVariables)stepOutput.Value;

        Assert.Single(result.IncludedVariables, x => x.VariableModel.VariableName == "SessionTime");
        Assert.DoesNotContain("SessionTick", result.IncludedVariables.Select(x => x.VariableModel.VariableName));
        Assert.Single(result.Diagnostics, x => x.Id == DiagnosticIds.ContextClassConfiguredPropertyNameConflict);
    }

    [Fact]
    public void DefaultModelValueOnContextClassInfoErrorTest()
    {
        var contextClassDefinition = @"using SharpRacer.Telemetry;
namespace Test.Assembly;
[GenerateTelemetryVariablesContext]
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
    public void IncludedVariableNotFoundTest()
    {
        var contextClassDefinition = @"using SharpRacer.Telemetry;
namespace Test.Assembly;
[GenerateTelemetryVariablesContext(""TelemetryVariables_VariableNames.json"")]
internal partial class TelemetryVariables : ITelemetryVariablesContext { }";

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

    [Fact]
    public void VariableNameCreatesPropertyNameConflictTest()
    {
        var contextClassDefinition = @"using SharpRacer.Telemetry;
namespace Test.Assembly;
[GenerateTelemetryVariablesContext(""TelemetryVariables_VariableNames.json"")]
internal partial class TelemetryVariables : ITelemetryVariablesContext { }";

        var variablesText = new VariableInfoDocumentBuilder()
            .AddScalar("SessionTime", VariableValueType.Double, "Seconds since session start", "s")
            .AddScalar("SessionTick", VariableValueType.Int, "Current update number", null)
            .ToAdditionalTextFile(GeneratorConfigurationDefaults.VariableInfoFileName);

        var variableOptionsText = new JsonVariableOptionsDocumentBuilder()
            .Add("SessionTime", new JsonVariableOptionsValue("SessionTick", null))
            .Add("SessionTick", new JsonVariableOptionsValue(null, null)) // produces SessionTime property conflict
            .ToAdditionalTextFile(GeneratorConfigurationDefaults.VariableOptionsFileName);

        var runResult = new VariablesGeneratorBuilder()
            .WithAdditionalText(variablesText)
            .WithAdditionalText(variableOptionsText)
            .WithCSharpSyntaxTree(contextClassDefinition)
            .Build()
            .RunGenerator();

        GeneratorAssert.NoException(runResult);
        GeneratorAssert.ContainsDiagnostic(runResult, DiagnosticIds.ContextClassVariableNameCreatesPropertyNameConflict);

        var stepResult = GeneratorAssert.TrackedStepExecuted(
            runResult, TrackingNames.ContextClassModelValuesProvider_ContextClassIncludedVariables).Single();

        GeneratorAssert.TrackedStepExecuted(runResult, TrackingNames.ContextClassModelValuesProvider_GetValuesProvider);

        var stepOutput = stepResult.Outputs.Single();

        var result = (ContextClassInfoIncludedVariables)stepOutput.Value;

        Assert.Single(result.IncludedVariables, x => x.VariableModel.VariableName == "SessionTime");
        Assert.DoesNotContain("SessionTick", result.IncludedVariables.Select(x => x.VariableModel.VariableName));
        Assert.Single(result.Diagnostics, x => x.Id == DiagnosticIds.ContextClassVariableNameCreatesPropertyNameConflict);
    }
}
