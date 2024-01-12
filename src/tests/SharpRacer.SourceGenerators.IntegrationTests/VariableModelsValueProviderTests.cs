using SharpRacer.SourceGenerators.TelemetryVariables.Diagnostics;
using SharpRacer.SourceGenerators.TelemetryVariables.InputModels;
using SharpRacer.SourceGenerators.TelemetryVariables.Pipeline;
using SharpRacer.SourceGenerators.TelemetryVariables.TestHelpers;
using SharpRacer.SourceGenerators.Testing.TelemetryVariables;

using CreateVariableModelsResult = (
    System.Collections.Immutable.ImmutableArray<SharpRacer.SourceGenerators.TelemetryVariables.GeneratorModels.VariableModel> Models,
    System.Collections.Immutable.ImmutableArray<Microsoft.CodeAnalysis.Diagnostic> Diagnostics);

namespace SharpRacer.SourceGenerators.TelemetryVariables;
public class VariableModelsValueProviderTests
{
    [Fact]
    public void DeprecatingVariableNotFound_Test()
    {
        var variablesText = new VariableInfoDocumentBuilder()
           .AddScalar("Lat", VariableValueType.Double, "GPS latitude", null)
           .AddScalar("Lon", VariableValueType.Double, "GPS longitude", null, deprecatedBy: "Longitude2")
           .ToAdditionalTextFile(GeneratorConfigurationDefaults.VariableInfoFileName);

        var runResult = new VariablesGeneratorBuilder()
            .WithAdditionalText(variablesText)
            .Build()
            .RunGenerator();

        GeneratorAssert.NoException(runResult);
        GeneratorAssert.ContainsDiagnostic(runResult, DiagnosticIds.TelemetryVariablesDeprecatingVariableNotFound);

        var stepResult = GeneratorAssert.TrackedStepExecuted(runResult, TrackingNames.VariableModelsValueProvider_GetValuesProvider).Single();
        var stepOutput = stepResult.Outputs.Single();

        var result = (CreateVariableModelsResult)stepOutput.Value;

        var latModel = Assert.Single(result.Models, x => x.VariableName == "Lat");
        var lonModel = Assert.Single(result.Models, x => x.VariableName == "Lon");

        Assert.Equal("Longitude2", lonModel.DeprecatingVariableName);
    }

    [Fact]
    public void EmptyInputs_Test()
    {
        var runResult = new VariablesGeneratorBuilder()
            .Build()
            .RunGenerator();

        GeneratorAssert.NoException(runResult);
        GeneratorAssert.ContainsDiagnostic(runResult, DiagnosticIds.TelemetryVariablesFileNotFound);

        GeneratorAssert.TrackedStepNotExecuted(runResult, TrackingNames.ContextClassModelValuesProvider_GetValuesProvider);
        GeneratorAssert.TrackedStepNotExecuted(runResult, TrackingNames.VariableClassModelValuesProvider_GetValuesProvider);
    }

    [Fact]
    public void Variables_NoOptionsTest()
    {
        var variablesText = new VariableInfoDocumentBuilder()
           .AddScalar("Lat", VariableValueType.Double, "GPS latitude", null)
           .AddScalar("Lon", VariableValueType.Double, "GPS longitude", null)
           .ToAdditionalTextFile(GeneratorConfigurationDefaults.VariableInfoFileName);

        var runResult = new VariablesGeneratorBuilder()
            .WithAdditionalText(variablesText)
            .Build()
            .RunGenerator();

        GeneratorAssert.NoException(runResult);
        GeneratorAssert.NoDiagnostics(runResult);

        var stepResult = GeneratorAssert.TrackedStepExecuted(runResult, TrackingNames.VariableModelsValueProvider_GetValuesProvider).Single();
        var stepOutput = stepResult.Outputs.Single();

        var result = (CreateVariableModelsResult)stepOutput.Value;

        var latModel = Assert.Single(result.Models, x => x.VariableName == "Lat");
        var lonModel = Assert.Single(result.Models, x => x.VariableName == "Lon");

        Assert.Equal(default, latModel.Options);
        Assert.Equal(default, lonModel.Options);
    }
}
