using Microsoft.CodeAnalysis.Text;

namespace SharpRacer.SourceGenerators.TelemetryVariables.Json;
public class JsonVariableInfoTests
{
    [Fact]
    public void Ctor_Test()
    {
        var name = "Test";
        var valueType = VariableValueType.Int;
        int valueCount = 4;
        string description = "Test variable";
        string? unit = "test/s";
        bool isTimeSliceArray = true;
        bool isDeprecated = true;
        string? deprecatedBy = "TestEx";

        var variable = new JsonVariableInfo(
            name,
            valueType,
            valueCount,
            description,
            unit,
            isTimeSliceArray,
            isDeprecated,
            deprecatedBy);

        Assert.Equal(name, variable.Name);
        Assert.Equal(valueType, variable.ValueType);
        Assert.Equal(valueCount, variable.ValueCount);
        Assert.Equal(description, variable.Description);
        Assert.Equal(unit, variable.ValueUnit);
        Assert.Equal(isTimeSliceArray, variable.IsTimeSliceArray);
        Assert.Equal(isDeprecated, variable.IsDeprecated);
        Assert.Equal(deprecatedBy, variable.DeprecatedBy);
    }

    [Fact]
    public void Ctor_TextSpanTest()
    {
        var variable = new JsonVariableInfo(
            "Test",
            VariableValueType.Float,
            4,
            "Test variable",
            "test/s",
            false,
            false,
            null);

        var textSpan = new TextSpan(5, 20);

        var variable2 = new JsonVariableInfo(variable, textSpan);

        Assert.Equal(variable.Name, variable2.Name);
        Assert.Equal(variable.ValueType, variable2.ValueType);
        Assert.Equal(variable.ValueCount, variable2.ValueCount);
        Assert.Equal(variable.Description, variable2.Description);
        Assert.Equal(variable.ValueUnit, variable.ValueUnit);
        Assert.Equal(variable.IsTimeSliceArray, variable2.IsTimeSliceArray);
        Assert.Equal(variable.IsDeprecated, variable2.IsDeprecated);
        Assert.Equal(variable.DeprecatedBy, variable2.DeprecatedBy);
        Assert.Equal(textSpan, variable2.JsonSpan);
    }

    [Fact]
    public void Equals_Test()
    {
        var name = "Test";
        var valueType = VariableValueType.Int;
        int valueCount = 4;
        string description = "Test variable";
        string? unit = "test/s";
        bool isTimeSliceArray = true;
        bool isDeprecated = true;
        string? deprecatedBy = "TestEx";

        var variable1 = new JsonVariableInfo(
            name,
            valueType,
            valueCount,
            description,
            unit,
            isTimeSliceArray,
            isDeprecated,
            deprecatedBy);

        var variable2 = new JsonVariableInfo(
            name,
            valueType,
            valueCount,
            description,
            unit,
            isTimeSliceArray,
            isDeprecated,
            deprecatedBy);

        Assert.True(variable1 == variable2);
        Assert.False(variable1 != variable2);
        Assert.True(variable1.Equals(variable2));
        Assert.Equal(variable1.GetHashCode(), variable2.GetHashCode());
    }

    [Fact]
    public void Equals_TextSpanTest()
    {
        var variable = new JsonVariableInfo(
            "Test",
            VariableValueType.Float,
            4,
            "Test variable",
            "test/s",
            false,
            false,
            null);

        var textSpan = new TextSpan(5, 20);

        var variable1 = new JsonVariableInfo(variable, textSpan);
        var variable2 = new JsonVariableInfo(variable, textSpan);

        Assert.True(variable1 == variable2);
        Assert.False(variable1 != variable2);
        Assert.True(variable1.Equals(variable2));
        Assert.Equal(variable1.GetHashCode(), variable2.GetHashCode());
    }
}
