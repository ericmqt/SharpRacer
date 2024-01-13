using SharpRacer.SourceGenerators.TelemetryVariables.InputModels;

namespace SharpRacer.SourceGenerators.TelemetryVariables.GeneratorModels;
public class VariableModelTests
{
    public static TheoryData<VariableModel, VariableModel> InequalityData => ModelInequalityData.VariableModelData();

    [Fact]
    public void Ctor_Test()
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

        var variableOptions1 = new VariableOptions("Lat", "Latitude", "Latitude");

        var model = new VariableModel(variable1, variableOptions1);

        Assert.Equal(variableOptions1, model.Options);

        Assert.Equal("Lat", model.VariableName);
        Assert.Equal("Test variable", model.Description);
        Assert.Equal(4, model.ValueCount);
        Assert.Equal(VariableValueType.Int, model.ValueType);
        Assert.Equal("test/s", model.ValueUnit);

        Assert.Null(model.DeprecatingVariableName);
        Assert.False(model.IsDeprecated);
    }

    [Fact]
    public void Ctor_DefaultOptionsTest()
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

        var model = new VariableModel(variable1, default);

        Assert.Equal("Lat", model.VariableName);
        Assert.Equal(default, model.Options);
    }

    [Fact]
    public void Ctor_DeprecatedVariableTest()
    {
        var variable1 = new VariableInfo(
            "Lat",
            VariableValueType.Int,
            4,
            "Test variable",
            "test/s",
            true,
            true,
            "LatitudeEx");

        var variableOptions1 = new VariableOptions("Lat", "Latitude", "Latitude");

        var model = new VariableModel(variable1, variableOptions1);

        Assert.True(model.IsDeprecated);
        Assert.Equal("LatitudeEx", model.DeprecatingVariableName);
    }

    [Fact]
    public void Ctor_ThrowOnDefaultVariableInfoTest()
    {
        Assert.Throws<ArgumentException>(() => new VariableModel(default, new VariableOptions("Test", null, null)));
    }

    [Fact]
    public void DataVariableTypeArgument_ScalarValueTypeTest()
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

        var model = new VariableModel(variable, default);

        Assert.Equal("int", model.DataVariableTypeArgument().ToFullString());
    }

    [Fact]
    public void DescriptorPropertyName_DefaultOptionsTest()
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

        var model = new VariableModel(variable1, default);

        Assert.Equal("Lat", model.DescriptorPropertyName());
    }

    [Fact]
    public void DescriptorPropertyName_FromOptionsTest()
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

        var variableOptions1 = new VariableOptions("Lat", "Latitude", "Latitude");

        var model = new VariableModel(variable1, variableOptions1);

        Assert.Equal("Latitude", model.DescriptorPropertyName());
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

    [Theory]
    [MemberData(nameof(InequalityData))]
    public void Equals_InequalityTest(VariableModel model1, VariableModel model2)
    {
        EquatableStructAssert.NotEqual(model1, model2);
    }
}
