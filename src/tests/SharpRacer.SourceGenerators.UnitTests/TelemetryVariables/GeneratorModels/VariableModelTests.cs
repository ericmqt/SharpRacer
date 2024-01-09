using SharpRacer.SourceGenerators.TelemetryVariables.InputModels;

namespace SharpRacer.SourceGenerators.TelemetryVariables.GeneratorModels;
public class VariableModelTests
{
    [Fact]
    public void Ctor_ThrowOnDefaultVariableInfoTest()
    {
        Assert.Throws<ArgumentException>(() => new VariableModel(default, new VariableOptions("Test", null, null)));
    }

    [Fact]
    public void Equals_Test()
    {
        var jsonVariable1 = new VariableInfo(
            "Lat",
            VariableValueType.Int,
            4,
            "Test variable",
            "test/s",
            true,
            false,
            null);

        var jsonVariable2 = new VariableInfo(
            "Lat",
            VariableValueType.Int,
            4,
            "Test variable",
            "test/s",
            true,
            false,
            null);

        var variableModel1 = new VariableModel(
            jsonVariable1,
            new VariableOptions(jsonVariable1.Name, "Latitude", "LatitudeVariable"));

        var variableModel2 = new VariableModel(
            jsonVariable2,
            new VariableOptions(jsonVariable2.Name, "Latitude", "LatitudeVariable"));

        EquatableStructAssert.Equal(variableModel1, variableModel2);
        EquatableStructAssert.NotEqual(variableModel1, default);
        EquatableStructAssert.ObjectEqualsMethod(false, variableModel1, jsonVariable2);
    }

    [Fact]
    public void Equals_UnequalTest()
    {
        var jsonVariable1 = new VariableInfo(
            "Lat",
            VariableValueType.Int,
            4,
            "Test variable",
            "test/s",
            true,
            false,
            null);

        var jsonVariable2 = new VariableInfo(
            "Lon",
            VariableValueType.Int,
            1,
            "Test variable",
            "test/s",
            true,
            false,
            null);

        var variableModel1 = new VariableModel(
            jsonVariable1,
            new VariableOptions(jsonVariable1.Name, "Latitude", "LatitudeVariable"));

        var variableModel2 = new VariableModel(
            jsonVariable2,
            new VariableOptions(jsonVariable2.Name, "Longitude", "LatitudeVariable"));

        EquatableStructAssert.NotEqual(variableModel1, variableModel2);
    }
}
