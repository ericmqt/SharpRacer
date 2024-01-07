using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using SharpRacer.SourceGenerators.TelemetryVariables.Diagnostics;
using SharpRacer.SourceGenerators.TelemetryVariables.InputModels;
using SharpRacer.SourceGenerators.TelemetryVariables.Pipeline;
using SharpRacer.SourceGenerators.TelemetryVariables.TestHelpers;
using SharpRacer.SourceGenerators.Testing.TelemetryVariables;

namespace SharpRacer.SourceGenerators.TelemetryVariables;
public class VariableInfoProviderTests
{
    [Fact]
    public void GetValueProvider_Test()
    {
        var variablesText = new VariableInfoDocumentBuilder()
            .AddScalar("SessionTime", VariableValueType.Double, "Seconds since session start", "s")
            .ToVariableInfoFile(GeneratorConfigurationDefaults.VariableInfoFileName);

        var runResult = new VariablesGeneratorBuilder()
            .WithAdditionalText(variablesText.File)
            .Build()
            .RunGenerator();

        GeneratorAssert.TrackedStepExecuted(runResult, TrackingNames.VariableInfoProvider_GetValueProvider);

        var variableInfoFileResult = runResult.TrackedSteps[TrackingNames.VariableInfoProvider_GetValueProvider].Single();

        var output = variableInfoFileResult.Outputs.Single();
        var pipelineResult = ((ImmutableArray<VariableInfo> Variables, ImmutableArray<Diagnostic> Diagnostics))output.Value;

        Assert.Equal(IncrementalStepRunReason.New, output.Reason);
        Assert.Empty(pipelineResult.Diagnostics);
        Assert.Single(pipelineResult.Variables);
    }

    [Fact]
    public void GetValueProvider_AlreadyDefinedVariableNotEmittedTest()
    {
        var variablesFile = new VariableInfoDocumentBuilder()
            .AddScalar("SessionTime", VariableValueType.Double, "Seconds since session start", "s")
            .AddScalar("Test", VariableValueType.Int, "Test variable", null)
            .AddScalar("Test", VariableValueType.Int, "Test variable", null)
            .ToAdditionalTextFile(GeneratorConfigurationDefaults.VariableInfoFileName);

        var runResult = new VariablesGeneratorBuilder()
           .WithAdditionalText(variablesFile)
           .Build()
           .RunGenerator();

        Assert.Null(runResult.Exception);

        GeneratorAssert.ContainsDiagnostic(runResult, DiagnosticIds.TelemetryVariableAlreadyDefined);
        GeneratorAssert.TrackedStepExecuted(runResult, TrackingNames.VariableInfoProvider_GetValueProvider);
        GeneratorAssert.TrackedStepNotExecuted(runResult, TrackingNames.VariableModelsValueProvider_GetVariableInfoProvider);

        var providerStep = runResult.TrackedSteps[TrackingNames.VariableInfoProvider_GetValueProvider].Single();
        var providerStepOutput = providerStep.Outputs.Single();

        var result = ((ImmutableArray<VariableInfo> Variables, ImmutableArray<Diagnostic> Diagnostics))providerStepOutput.Value;

        Assert.Single(result.Variables, x => x.Name == "SessionTime");
        Assert.Single(result.Variables, x => x.Name == "Test");
        Assert.Single(result.Diagnostics);
        Assert.Single(result.Diagnostics, x => x.Id == DiagnosticIds.TelemetryVariableAlreadyDefined);
    }

    [Fact]
    public void GetValueProvider_FileReadExceptionTest()
    {
        var runResult = new VariablesGeneratorBuilder()
            .WithAdditionalText(GeneratorConfigurationDefaults.VariableInfoFileName, "[{ \"Name\": \"SessionTime\", ")
            .Build()
            .RunGenerator();

        GeneratorAssert.TrackedStepExecuted(runResult, TrackingNames.VariableInfoProvider_GetValueProvider);
        //GeneratorAssert.TrackedStepExecuted(runResult, TrackingNames.VariableOptionsProvider_GetValueProvider);
        GeneratorAssert.TrackedStepNotExecuted(runResult, TrackingNames.VariableModelsValueProvider_GetVariableInfoProvider);

        GeneratorAssert.ContainsDiagnostic(runResult, DiagnosticIds.AdditionalText_FileReadException);

        var providerStep = runResult.TrackedSteps[TrackingNames.VariableInfoProvider_GetValueProvider].Single();
        var providerStepOutput = providerStep.Outputs.Single();

        var result = ((ImmutableArray<VariableInfo> Variables, ImmutableArray<Diagnostic> Diagnostics))providerStepOutput.Value;

        Assert.Single(result.Diagnostics);
        Assert.Single(result.Diagnostics, x => x.Id == DiagnosticIds.AdditionalText_FileReadException);
        Assert.Empty(result.Variables);
    }

    [Fact]
    public void GetVariableInfoFile_WithConfiguredFileName_Test()
    {
        var variablesText = new VariableInfoDocumentBuilder()
            .AddScalar("SessionTime", VariableValueType.Double, "Seconds since session start", "s")
            .ToVariableInfoFile("MyVariables.json");

        var runResult = new VariablesGeneratorBuilder()
            .ConfigureGlobalOptions(options => options.VariableInfoFileName = "MyVariables.json")
            .WithAdditionalText(variablesText.File)
            .Build()
            .RunGenerator();

        GeneratorAssert.TrackedStepExecuted(runResult, TrackingNames.VariableInfoProvider_GetVariableInfoFile);

        var variableInfoFileResult = runResult.TrackedSteps[TrackingNames.VariableInfoProvider_GetVariableInfoFile].Single();

        var output = variableInfoFileResult.Outputs.Single();
        var result = ((VariableInfoFile File, Diagnostic? Diagnostic))output.Value;

        Assert.Equal(IncrementalStepRunReason.New, output.Reason);
        Assert.Null(result.Diagnostic);
        Assert.NotEqual(default, result.File);
        Assert.Equal(variablesText.SourceText.ToString(), result.File.SourceText.ToString());
    }

    [Fact]
    public void GetVariableInfoFile_AmbiguousFileNameDiagnostic_Test()
    {
        var variablesText1 = new VariableInfoDocumentBuilder()
            .AddScalar("SessionTime", VariableValueType.Double, "Seconds since session start", "s")
            .ToAdditionalTextFile(GeneratorConfigurationDefaults.VariableInfoFileName);

        var variablesText2 = new VariableInfoDocumentBuilder()
            .AddScalar("SessionTime", VariableValueType.Double, "Seconds since session start", "s")
            .ToAdditionalTextFile($"Assets\\{GeneratorConfigurationDefaults.VariableInfoFileName}");

        var runResult = new VariablesGeneratorBuilder()
            .WithAdditionalText(variablesText1)
            .WithAdditionalText(variablesText2)
            .Build()
            .RunGenerator();

        GeneratorAssert.TrackedStepExecuted(runResult, TrackingNames.VariableInfoProvider_GetVariableInfoFile);

        GeneratorAssert.ContainsDiagnostic(runResult, DiagnosticIds.TelemetryVariablesFile_AmbiguousFileName);

        var modelStep = runResult.TrackedSteps[TrackingNames.VariableInfoProvider_GetVariableInfoFile].Single();
        var modelStepOutput = modelStep.Outputs.Single();

        var result = ((VariableInfoFile File, Diagnostic? Diagnostic))modelStepOutput.Value;

        Assert.Equal(default, result.File);
        Assert.NotNull(result.Diagnostic);
        Assert.Equal(DiagnosticIds.TelemetryVariablesFile_AmbiguousFileName, result.Diagnostic.Id);
    }

    [Fact]
    public void GetVariableInfoFile_FileNotFoundDiagnostic_Test()
    {
        // Create variables file with non-default file name
        var variablesText = new VariableInfoDocumentBuilder()
            .AddScalar("SessionTime", VariableValueType.Double, "Seconds since session start", "s")
            .ToAdditionalTextFile("MyVariables.json");

        var runResult = new VariablesGeneratorBuilder()
            .WithAdditionalText(variablesText)
            .Build()
            .RunGenerator();

        GeneratorAssert.TrackedStepExecuted(runResult, TrackingNames.VariableInfoProvider_GetVariableInfoFile);
        GeneratorAssert.TrackedStepNotExecuted(runResult, TrackingNames.VariableModelsValueProvider_GetVariableInfoProvider);

        GeneratorAssert.ContainsDiagnostic(runResult, DiagnosticIds.TelemetryVariablesFileNotFound);

        var variableInfoFileResult = runResult.TrackedSteps[TrackingNames.VariableInfoProvider_GetVariableInfoFile].Single();

        var output = variableInfoFileResult.Outputs.Single();
        var result = ((VariableInfoFile File, Diagnostic? Diagnostic))output.Value;

        Assert.Equal(default, result.File);
        Assert.NotNull(result.Diagnostic);
        Assert.Equal(DiagnosticIds.TelemetryVariablesFileNotFound, result.Diagnostic.Id);
    }
}
