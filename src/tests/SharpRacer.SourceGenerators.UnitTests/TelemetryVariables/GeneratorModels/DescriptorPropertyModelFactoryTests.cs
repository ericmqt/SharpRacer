using SharpRacer.SourceGenerators.TelemetryVariables.Diagnostics;
using SharpRacer.SourceGenerators.TelemetryVariables.InputModels;

namespace SharpRacer.SourceGenerators.TelemetryVariables.GeneratorModels;
public class DescriptorPropertyModelFactoryTests
{
    [Fact]
    public void TryAdd_Test()
    {
        var variable1 = new VariableInfo(
            "Lat",
            VariableValueType.Int,
            4,
            "Test variable",
            "test/s",
            true,
            false,
            null);

        var variable2 = new VariableInfo(
            "Lon",
            VariableValueType.Int,
            1,
            "Test variable",
            "test/s",
            true,
            false,
            null);

        var variableOptions1 = new VariableOptions("Lat", "Latitude", "Latitude");
        var variableOptions2 = new VariableOptions("Lon", "Longitude", "Longitude");

        var variableModel1 = new VariableModel(variable1, variableOptions1);
        var variableModel2 = new VariableModel(variable2, variableOptions2);

        var factory = new DescriptorPropertyModelFactory();

        Assert.True(factory.TryAdd(variableModel1, out var diagnostic));
        Assert.Null(diagnostic);

        Assert.True(factory.TryAdd(variableModel2, out diagnostic));
        Assert.Null(diagnostic);

        var results = factory.Build();
        Assert.Equal(2, results.Length);
        Assert.Single(results, x => x.VariableModel == variableModel1);
        Assert.Single(results, x => x.VariableModel == variableModel2);
    }

    [Fact]
    public void TryAdd_DefaultValueModelTest()
    {
        var factory = new DescriptorPropertyModelFactory();

        var result = factory.TryAdd(default, out var diagnostic);

        Assert.False(result);
        Assert.Null(diagnostic);

        var models = factory.Build();

        Assert.Empty(models);
    }

    [Fact]
    public void TryAdd_ConflictingPropertyNameDiagnosticTest()
    {
        var variable1 = new VariableInfo(
            "Lat",
            VariableValueType.Int,
            4,
            "Test variable",
            "test/s",
            true,
            false,
            null);

        var variable2 = new VariableInfo(
            "Lon",
            VariableValueType.Int,
            1,
            "Test variable",
            "test/s",
            true,
            false,
            null);

        var variableOptions1 = new VariableOptions("Lat", "Latitude", "Latitude");
        var variableOptions2 = new VariableOptions("Lon", "Latitude", "Longitude");

        var variableModel1 = new VariableModel(variable1, variableOptions1);
        var variableModel2 = new VariableModel(variable2, variableOptions2);

        var factory = new DescriptorPropertyModelFactory();

        Assert.True(factory.TryAdd(variableModel1, out var diagnostic));
        Assert.Null(diagnostic);

        Assert.False(factory.TryAdd(variableModel2, out diagnostic));
        Assert.NotNull(diagnostic);
        Assert.Equal(DiagnosticIds.DescriptorNameConflictsWithExistingVariable, diagnostic.Id);

        var results = factory.Build();

        Assert.Single(results);
        Assert.Single(results, x => x.VariableModel == variableModel1);
    }
}
