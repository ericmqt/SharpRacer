using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.Text;
using Microsoft.CodeAnalysis;
using SharpRacer.SourceGenerators.TelemetryVariables.Json;
using SharpRacer.SourceGenerators.TelemetryVariables.InputModels;

namespace SharpRacer.SourceGenerators.TelemetryVariables.GeneratorModels;
public class VariableClassGeneratorModelFactoryTests
{
    [Fact]
    public void Ctor_Test()
    {
        var generatorOptions = new VariableClassGeneratorOptions(
            isGeneratorEnabled: true,
            targetNamespace: "Test.Variables",
            ImmutableArray<DescriptorPropertyReference>.Empty);

        var factory = new VariableClassGeneratorModelFactory(generatorOptions, initialCapacity: 3);

        Assert.NotNull(factory);
    }

    [Fact]
    public void Add_Test()
    {
        var generatorOptions = new VariableClassGeneratorOptions(
            isGeneratorEnabled: true,
            targetNamespace: "Test.Variables",
            ImmutableArray<DescriptorPropertyReference>.Empty);

        var jsonVariable = new JsonVariableInfo(
            "Test",
            VariableValueType.Int,
            4,
            "Test variable",
            "test/s",
            true,
            false,
            null);

        var variableInfo = new VariableInfo(jsonVariable);

        var factory = new VariableClassGeneratorModelFactory(generatorOptions, initialCapacity: 3);

        factory.Add(new VariableModel(variableInfo, new VariableOptions(variableInfo.Name, "Test", "TestVariable")));

        var variableClasses = factory.Build();

        Assert.Single(variableClasses);
        Assert.Empty(variableClasses.First().Diagnostics);
    }

    [Fact]
    public void Add_DuplicateClassNameDiagnosticTest()
    {
        var generatorOptions = new VariableClassGeneratorOptions(
            isGeneratorEnabled: true,
            targetNamespace: "Test.Variables",
            ImmutableArray<DescriptorPropertyReference>.Empty);

        var jsonVariable1 = new JsonVariableInfo(
            "Lat",
            VariableValueType.Int,
            4,
            "Test variable",
            "test/s",
            true,
            false,
            null);

        var jsonVariable2 = new JsonVariableInfo(
            "Lon",
            VariableValueType.Int,
            1,
            "Test variable",
            "test/s",
            true,
            false,
            null);

        var variableModel1 = new VariableModel(
            new VariableInfo(jsonVariable1),
            new VariableOptions(jsonVariable1.Name, "Latitude", "LatitudeVariable"));

        var variableModel2 = new VariableModel(
            new VariableInfo(jsonVariable2),
            new VariableOptions(jsonVariable2.Name, "Longitude", "LatitudeVariable"));

        var factory = new VariableClassGeneratorModelFactory(generatorOptions, initialCapacity: 3);

        factory.Add(variableModel1);
        factory.Add(variableModel2);

        var variableClasses = factory.Build();

        Assert.Equal(2, variableClasses.Length);

        var lonVariable = variableClasses.Single(x => x.VariableName == "Lon");
        Assert.NotEmpty(lonVariable.Diagnostics);
        Assert.True(lonVariable.Diagnostics.HasErrors());
    }
}
