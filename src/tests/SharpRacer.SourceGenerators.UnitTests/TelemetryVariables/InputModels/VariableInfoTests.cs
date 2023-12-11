using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using SharpRacer.SourceGenerators.TelemetryVariables.Json;

namespace SharpRacer.SourceGenerators.TelemetryVariables.InputModels;
public class VariableInfoTests
{
    [Fact]
    public void Ctor_Test()
    {
        var location = Location.Create(
            "test.txt",
            new TextSpan(2, 3),
            new LinePositionSpan(new LinePosition(0, 2), new LinePosition(0, 3)));

        var jsonVariable = new JsonVariableInfo(
            "Test",
            VariableValueType.Int,
            4,
            "Test variable",
            "test/s",
            true,
            false,
            null);

        var variableInfo = new VariableInfo(jsonVariable, location);

        Assert.Equal(jsonVariable.Name, variableInfo.Name);
        Assert.Equal(jsonVariable.ValueType, variableInfo.ValueType);
        Assert.Equal(jsonVariable.ValueCount, variableInfo.ValueCount);
        Assert.Equal(jsonVariable.Description, variableInfo.Description);
        Assert.Equal(jsonVariable.ValueUnit, jsonVariable.ValueUnit);
        Assert.Equal(jsonVariable.IsTimeSliceArray, variableInfo.IsTimeSliceArray);
        Assert.Equal(jsonVariable.IsDeprecated, variableInfo.IsDeprecated);
        Assert.Equal(jsonVariable.DeprecatedBy, variableInfo.DeprecatedBy);
        Assert.Equal(location, variableInfo.JsonLocation);
    }

    [Fact]
    public void Equals_Test()
    {
        var jsonVariable1 = new JsonVariableInfo(
            "Test",
            VariableValueType.Int,
            4,
            "Test variable",
            "test/s",
            true,
            false,
            null);

        var jsonVariable2 = new JsonVariableInfo(
            "Test",
            VariableValueType.Int,
            4,
            "Test variable",
            "test/s",
            true,
            false,
            null);

        var variable1Location = Location.Create(
            "test.txt",
            new TextSpan(2, 3),
            new LinePositionSpan(new LinePosition(0, 2), new LinePosition(0, 3)));

        var variable2Location = Location.Create(
            "test.txt",
            new TextSpan(2, 3),
            new LinePositionSpan(new LinePosition(0, 2), new LinePosition(0, 3)));

        var variable1 = new VariableInfo(jsonVariable1, variable1Location);
        var variable2 = new VariableInfo(jsonVariable2, variable2Location);

        Assert.True(variable1 == variable2);
        Assert.False(variable1 != variable2);
        Assert.True(variable1.Equals(variable2));
        Assert.Equal(variable1.GetHashCode(), variable2.GetHashCode());
    }

    [Fact]
    public void Equals_UnequalTest()
    {
        var jsonVariable1 = new JsonVariableInfo(
            "Test",
            VariableValueType.Int,
            4,
            "Test variable",
            "test/s",
            true,
            false,
            null);

        var jsonVariable2 = new JsonVariableInfo(
            "Test2",
            VariableValueType.Float,
            1,
            "Test variable",
            null,
            false,
            true,
            "Test3");

        var variable1Location = Location.Create(
            "test.txt",
            new TextSpan(2, 3),
            new LinePositionSpan(new LinePosition(0, 2), new LinePosition(0, 3)));

        var variable2Location = Location.Create(
            "test.txt",
            new TextSpan(9, 10),
            new LinePositionSpan(new LinePosition(0, 2), new LinePosition(0, 3)));

        var variable1 = new VariableInfo(jsonVariable1, variable1Location);
        var variable2 = new VariableInfo(jsonVariable2, variable2Location);

        Assert.False(variable1 == variable2);
        Assert.True(variable1 != variable2);
        Assert.False(variable1.Equals(variable2));
        Assert.NotEqual(variable1.GetHashCode(), variable2.GetHashCode());
    }
}
