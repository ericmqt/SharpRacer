using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using SharpRacer.SourceGenerators.TelemetryVariables.Diagnostics;
using SharpRacer.SourceGenerators.TelemetryVariables.InputModels;
using SharpRacer.SourceGenerators.TelemetryVariables.Json;
using SharpRacer.SourceGenerators.TelemetryVariables.Pipeline;
using SharpRacer.SourceGenerators.TelemetryVariables.TestHelpers;
using SharpRacer.SourceGenerators.Testing.TelemetryVariables;

using VariableOptionsCollectionResult = (
    System.Collections.Immutable.ImmutableArray<SharpRacer.SourceGenerators.TelemetryVariables.InputModels.VariableOptions> Options,
    System.Collections.Immutable.ImmutableArray<Microsoft.CodeAnalysis.Diagnostic> Diagnostics);

using VariableOptionsFileResult = (
    SharpRacer.SourceGenerators.TelemetryVariables.InputModels.VariableOptionsFile File,
    Microsoft.CodeAnalysis.Diagnostic? Diagnostic);

namespace SharpRacer.SourceGenerators.TelemetryVariables;
public class VariableOptionsProviderTests
{
    [Fact]
    public void NoOptionsFileTest()
    {
        var variablesText = new VariableInfoDocumentBuilder()
            .AddScalar("SessionTime", VariableValueType.Double, "Seconds since session start", "s")
            .ToAdditionalTextFile(GeneratorConfigurationDefaults.VariableInfoFileName);

        var runResult = new VariablesGeneratorBuilder()
            .WithAdditionalText(variablesText)
            .Build()
            .RunGenerator();

        GeneratorAssert.TrackedStepExecuted(runResult, TrackingNames.VariableOptionsProvider_GetValueProvider);

        var variableOptionsResult = runResult.TrackedSteps[TrackingNames.VariableOptionsProvider_GetValueProvider].Single();

        var output = variableOptionsResult.Outputs.Single();
        var result = ((ImmutableArray<VariableOptions> Options, ImmutableArray<Diagnostic> Diagnostics))output.Value;

        Assert.Equal(IncrementalStepRunReason.New, output.Reason);
        Assert.Empty(result.Diagnostics);
        Assert.Empty(result.Options);
    }

    [Fact]
    public void AmbiguousVariableOptionsFileNameTest()
    {
        var variablesText = new VariableInfoDocumentBuilder()
            .AddScalar("SessionTime", VariableValueType.Double, "Seconds since session start", "s")
            .ToAdditionalTextFile(GeneratorConfigurationDefaults.VariableInfoFileName);

        var variableOptionsBuilder = new JsonVariableOptionsDocumentBuilder()
            .Add("SessionTime", new JsonVariableOptionsValue(null, null))
            .Add("SessionTime", new JsonVariableOptionsValue("SessionTick", null));

        var variableOptionsText1 = variableOptionsBuilder.ToAdditionalTextFile(GeneratorConfigurationDefaults.VariableOptionsFileName);
        var variableOptionsText2 = variableOptionsBuilder.ToAdditionalTextFile($"Assets\\{GeneratorConfigurationDefaults.VariableOptionsFileName}");

        var runResult = new VariablesGeneratorBuilder()
            .WithAdditionalText(variablesText)
            .WithAdditionalText(variableOptionsText1)
            .WithAdditionalText(variableOptionsText2)
            .Build()
            .RunGenerator();

        GeneratorAssert.NoException(runResult);
        GeneratorAssert.ContainsDiagnostic(runResult, DiagnosticIds.VariableOptionsFile_AmbiguousFileName);

        var stepResult = GeneratorAssert.TrackedStepExecuted(
            runResult, TrackingNames.VariableOptionsProvider_GetVariableOptionsFile)
            .Single();

        var stepOutput = stepResult.Outputs.Single();

        var result = (VariableOptionsFileResult)stepOutput.Value;

        Assert.Equal(default, result.File);
        Assert.NotNull(result.Diagnostic);
        Assert.Equal(DiagnosticIds.VariableOptionsFile_AmbiguousFileName, result.Diagnostic.Id);
    }

    [Fact]
    public void ContainsDuplicateKeyTest()
    {
        var variablesText = new VariableInfoDocumentBuilder()
            .AddScalar("SessionTime", VariableValueType.Double, "Seconds since session start", "s")
            .ToAdditionalTextFile(GeneratorConfigurationDefaults.VariableInfoFileName);

        var variableOptionsText = new JsonVariableOptionsDocumentBuilder()
            .Add("SessionTime", new JsonVariableOptionsValue(null, null))
            .Add("SessionTime", new JsonVariableOptionsValue("SessionTick", null))
            .ToAdditionalTextFile(GeneratorConfigurationDefaults.VariableOptionsFileName);

        var runResult = new VariablesGeneratorBuilder()
            .WithAdditionalText(variablesText)
            .WithAdditionalText(variableOptionsText)
            .Build()
            .RunGenerator();

        GeneratorAssert.NoException(runResult);
        GeneratorAssert.ContainsDiagnostic(runResult, DiagnosticIds.VariableOptionsFileContainsDuplicateKey);

        var stepResult = GeneratorAssert.TrackedStepExecuted(
            runResult, TrackingNames.VariableOptionsProvider_GetValueProvider)
            .Single();

        var stepOutput = stepResult.Outputs.Single();

        var result = (VariableOptionsCollectionResult)stepOutput.Value;

        Assert.Single(result.Diagnostics, x => x.Id == DiagnosticIds.VariableOptionsFileContainsDuplicateKey);
        Assert.Single(result.Options);

        // Ensure the duplicated option wasn't applied to the original
        Assert.Single(result.Options, x => x.VariableKey == "SessionTime" && x.Name == null);
    }

    [Fact]
    public void ContainsDuplicateClassNameTest()
    {
        var variablesText = new VariableInfoDocumentBuilder()
            .AddScalar("SessionTime", VariableValueType.Double, "Seconds since session start", "s")
            .AddScalar("SessionTick", VariableValueType.Int, "Current update number", null)
            .ToAdditionalTextFile(GeneratorConfigurationDefaults.VariableInfoFileName);

        var variableOptionsText = new JsonVariableOptionsDocumentBuilder()
            .Add("SessionTime", new JsonVariableOptionsValue(null, "SessionTimeVariable"))
            .Add("SessionTick", new JsonVariableOptionsValue(null, "SessionTimeVariable"))
            .ToAdditionalTextFile(GeneratorConfigurationDefaults.VariableOptionsFileName);

        var runResult = new VariablesGeneratorBuilder()
            .WithAdditionalText(variablesText)
            .WithAdditionalText(variableOptionsText)
            .Build()
            .RunGenerator();

        GeneratorAssert.NoException(runResult);
        GeneratorAssert.ContainsDiagnostic(runResult, DiagnosticIds.VariableOptionsFileContainsDuplicateClassName);

        var stepResult = GeneratorAssert.TrackedStepExecuted(
            runResult, TrackingNames.VariableOptionsProvider_GetValueProvider)
            .Single();

        var stepOutput = stepResult.Outputs.Single();

        var result = (VariableOptionsCollectionResult)stepOutput.Value;

        Assert.Single(result.Diagnostics, x => x.Id == DiagnosticIds.VariableOptionsFileContainsDuplicateClassName);
        Assert.Single(result.Options);

        // Ensure the duplicated option wasn't applied to the original
        Assert.Single(result.Options, x => x.ClassName == "SessionTimeVariable");
    }

    [Fact]
    public void ContainsDuplicateVariableNameTest()
    {
        var variablesText = new VariableInfoDocumentBuilder()
            .AddScalar("SessionTime", VariableValueType.Double, "Seconds since session start", "s")
            .AddScalar("SessionTick", VariableValueType.Int, "Current update number", null)
            .ToAdditionalTextFile(GeneratorConfigurationDefaults.VariableInfoFileName);

        var variableOptionsText = new JsonVariableOptionsDocumentBuilder()
            .Add("SessionTime", new JsonVariableOptionsValue("SessionTime", null))
            .Add("SessionTick", new JsonVariableOptionsValue("SessionTime", null))
            .ToAdditionalTextFile(GeneratorConfigurationDefaults.VariableOptionsFileName);

        var runResult = new VariablesGeneratorBuilder()
            .WithAdditionalText(variablesText)
            .WithAdditionalText(variableOptionsText)
            .Build()
            .RunGenerator();

        GeneratorAssert.NoException(runResult);
        GeneratorAssert.ContainsDiagnostic(runResult, DiagnosticIds.VariableOptionsFileContainsDuplicateVariableName);

        var stepResult = GeneratorAssert.TrackedStepExecuted(
            runResult, TrackingNames.VariableOptionsProvider_GetValueProvider)
            .Single();

        var stepOutput = stepResult.Outputs.Single();

        var result = (VariableOptionsCollectionResult)stepOutput.Value;

        Assert.Single(result.Diagnostics, x => x.Id == DiagnosticIds.VariableOptionsFileContainsDuplicateVariableName);
        Assert.Single(result.Options);
        Assert.Single(result.Options, x => x.Name == "SessionTime");
    }
}
