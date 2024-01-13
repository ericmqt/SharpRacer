using SharpRacer.SourceGenerators.TelemetryVariables.Diagnostics;
using SharpRacer.SourceGenerators.TelemetryVariables.InputModels;

namespace SharpRacer.SourceGenerators.TelemetryVariables.GeneratorModels;
public class ContextVariableModelFactoryTests
{
    [Fact]
    public void TryAdd_Test()
    {
        var variable = new VariableInfo(
            "Lat",
            VariableValueType.Int,
            1,
            "Test variable",
            "test/s",
            true,
            true,
            "LatitudeEx");

        var variableModel = new VariableModel(variable, default);

        var factory = new ContextVariableModelFactory();

        Assert.True(factory.TryAdd(variableModel, null, null, out var diagnostics));
        Assert.Empty(diagnostics);

        var models = factory.Build();

        Assert.Single(models);
        Assert.Single(models, x => x.VariableModel == variableModel);
    }

    [Fact]
    public void TryAdd_ConfiguredPropertyNameConflictTest()
    {
        var variable1 = new VariableInfo(
            "Lat",
            VariableValueType.Int,
            1,
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

        var factory = new ContextVariableModelFactory();

        Assert.True(factory.TryAdd(variableModel1, null, null, out var diagnostics));
        Assert.Empty(diagnostics);

        Assert.False(factory.TryAdd(variableModel2, null, null, out diagnostics));
        Assert.Single(diagnostics);
        Assert.Single(diagnostics, x => x.Id == DiagnosticIds.ContextClassConfiguredPropertyNameConflict);

        var models = factory.Build();

        Assert.Single(models);
        Assert.Single(models, x => x.VariableModel == variableModel1);
    }

    [Fact]
    public void TryAdd_VariableNameCreatesPropertyNameConflictTest()
    {
        var variable1 = new VariableInfo(
            "Lat",
            VariableValueType.Int,
            1,
            "Test variable",
            "test/s",
            true,
            false,
            null);

        var variable2 = new VariableInfo(
            "Lat",
            VariableValueType.Int,
            1,
            "Test variable",
            "test/s",
            true,
            false,
            null);

        var variableModel1 = new VariableModel(variable1, default);
        var variableModel2 = new VariableModel(variable2, default);

        var factory = new ContextVariableModelFactory();

        Assert.True(factory.TryAdd(variableModel1, null, null, out var diagnostics));
        Assert.Empty(diagnostics);

        Assert.False(factory.TryAdd(variableModel2, null, null, out diagnostics));
        Assert.Single(diagnostics);
        Assert.Single(diagnostics, x => x.Id == DiagnosticIds.ContextClassVariableNameCreatesPropertyNameConflict);

        var models = factory.Build();

        Assert.Single(models);
        Assert.Single(models, x => x.VariableModel == variableModel1);
    }

    [Fact]
    public void TryAdd_DefaultValueModelTest()
    {
        var factory = new ContextVariableModelFactory();

        var result = factory.TryAdd(default, null, null, out var diagnostics);

        Assert.False(result);
        Assert.Empty(diagnostics);

        var models = factory.Build();

        Assert.Empty(models);
    }
}
