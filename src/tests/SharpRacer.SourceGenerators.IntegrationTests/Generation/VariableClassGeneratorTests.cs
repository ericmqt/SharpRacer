using Microsoft.CodeAnalysis;
using SharpRacer.SourceGenerators.TelemetryVariables.InputModels;
using SharpRacer.SourceGenerators.TelemetryVariables.Pipeline;
using SharpRacer.SourceGenerators.TelemetryVariables.TestHelpers;
using SharpRacer.SourceGenerators.Testing.TelemetryVariables;

namespace SharpRacer.SourceGenerators.TelemetryVariables.Generation;
public class VariableClassGeneratorTests
{
    [Fact]
    public void ModifiedVariableUpdatesSingleVariableClassTest()
    {
        var variablesText = new VariableInfoDocumentBuilder()
            .AddScalar("Lat", VariableValueType.Double, "GPS latitude", null)
            .AddScalar("Lon", VariableValueType.Double, "GPS longitude", null)
            .ToAdditionalTextFile(GeneratorConfigurationDefaults.VariableInfoFileName);

        var generatorModel = new VariablesGeneratorBuilder()
            .WithAdditionalText(variablesText)
            .ConfigureGlobalOptions(o => o.GenerateTelemetryVariableClasses = "true")
            .Build();

        var driver = generatorModel.GeneratorDriver.RunGenerators(generatorModel.Compilation);

        var initialRunResult = driver.GetRunResult().Results.Single();

        GeneratorAssert.NoException(initialRunResult);
        GeneratorAssert.NoDiagnostics(initialRunResult);

        var classModelResults = initialRunResult.TrackedOutputSteps["SourceOutput"]
            .Where(x => x.Inputs.Length == 1 && x.Inputs.Any(y => y.Source.Name == TrackingNames.VariableClassModelValuesProvider_GetValuesProvider))
            .ToList();

        Assert.Equal(2, classModelResults.Count);

        var classModelOutputs = classModelResults.Select(x => x.Outputs.Single()).ToList();

        Assert.True(classModelOutputs.All(x => x.Reason == IncrementalStepRunReason.New));

        // Mutate by removing a variable
        var newVariablesText = new VariableInfoDocumentBuilder()
            .AddScalar("Latitude", VariableValueType.Double, "GPS latitude", null)
            .AddScalar("Lon", VariableValueType.Double, "GPS longitude", null)
            .ToAdditionalTextFile(GeneratorConfigurationDefaults.VariableInfoFileName);

        var mutatedDriver = driver.ReplaceAdditionalText(variablesText, newVariablesText);

        var mutatedRunResult = mutatedDriver.RunGenerators(generatorModel.Compilation).GetRunResult().Results.Single();

        GeneratorAssert.NoException(mutatedRunResult);
        GeneratorAssert.NoDiagnostics(mutatedRunResult);

        var mutatedClassModelResults = mutatedRunResult.TrackedOutputSteps["SourceOutput"]
            .Where(x => x.Inputs.Length == 1 && x.Inputs.Any(y => y.Source.Name == TrackingNames.VariableClassModelValuesProvider_GetValuesProvider))
            .ToList();

        Assert.Equal(2, mutatedClassModelResults.Count);

        var mutatedClassModelOutputs = mutatedClassModelResults.Select(x => x.Outputs.Single()).ToList();

        Assert.Single(mutatedClassModelOutputs.Select(x => x.Reason), x => x == IncrementalStepRunReason.Cached);
        Assert.Single(mutatedClassModelOutputs.Select(x => x.Reason), x => x == IncrementalStepRunReason.Modified);
    }
}
