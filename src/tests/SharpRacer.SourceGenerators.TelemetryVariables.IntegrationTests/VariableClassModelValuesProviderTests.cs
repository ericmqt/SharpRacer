﻿using Microsoft.CodeAnalysis;
using SharpRacer.SourceGenerators.TelemetryVariables.Diagnostics;
using SharpRacer.SourceGenerators.TelemetryVariables.GeneratorModels;
using SharpRacer.SourceGenerators.TelemetryVariables.InputModels;
using SharpRacer.SourceGenerators.TelemetryVariables.Json;
using SharpRacer.SourceGenerators.TelemetryVariables.Pipeline;
using SharpRacer.SourceGenerators.TelemetryVariables.TestHelpers;
using SharpRacer.SourceGenerators.Testing.TelemetryVariables;

namespace SharpRacer.SourceGenerators.TelemetryVariables;
public class VariableClassModelValuesProviderTests
{
    [Fact]
    public void DuplicateClassNameDiagnosticTest()
    {
        var variablesText = new VariableInfoDocumentBuilder()
            .AddScalar("Lat", VariableValueType.Double, "GPS latitude", null)
            .AddScalar("Lon", VariableValueType.Double, "GPS longitude", null)
            .ToAdditionalTextFile(GeneratorConfigurationDefaults.VariableInfoFileName);

        var variableOptionsText = new JsonVariableOptionsDocumentBuilder()
            .Add("Lat", new JsonVariableOptionsValue("Latitude", null)) // produces LatitudeVariable via Name option
            .Add("Lon", new JsonVariableOptionsValue(null, "Latitude")) // produces LatitudeVariable via ClassName option
            .ToAdditionalTextFile(GeneratorConfigurationDefaults.VariableOptionsFileName);

        var generatorOptions = new VariablesGeneratorGlobalOptions(
            new VariablesGeneratorGlobalOptionsValues { GenerateTelemetryVariableClasses = "true" });

        var runResult = new VariablesGeneratorBuilder()
            .WithAdditionalText(variablesText)
            .WithAdditionalText(variableOptionsText)
            .ConfigureGlobalOptions(o => o.GenerateTelemetryVariableClasses = "true")
            .Build()
            .RunGenerator();

        var step = GeneratorAssert.TrackedStepExecuted(runResult, TrackingNames.VariableClassModelValuesProvider_GetValuesProvider).Single();
        GeneratorAssert.ContainsDiagnostic(runResult, DiagnosticIds.VariableClassNameInUse);

        var results = step.Outputs.Select(x => (VariableClassModel)x.Value).ToList();

        Assert.Equal(2, results.Count);
        Assert.Single(results, x => x.VariableName == "Lat");
        Assert.Single(results, x => x.VariableName == "Lon");

        Assert.Empty(results.Single(x => x.VariableName == "Lat").Diagnostics);
        Assert.Single(results.Single(x => x.VariableName == "Lon").Diagnostics);
        Assert.Single(results.Single(x => x.VariableName == "Lon").Diagnostics, x => x.Id == DiagnosticIds.VariableClassNameInUse);
    }
}
