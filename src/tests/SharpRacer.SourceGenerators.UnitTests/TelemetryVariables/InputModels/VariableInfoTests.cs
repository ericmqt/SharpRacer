using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace SharpRacer.SourceGenerators.TelemetryVariables.InputModels;
public class VariableInfoTests
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

        var span = new TextSpan(2, 3);
        var location = Location.Create(
            "test.txt",
            span,
            new LinePositionSpan(new LinePosition(0, 2), new LinePosition(0, 3)));

        var variableInfo = new VariableInfo(
            name,
            valueType,
            valueCount,
            description,
            unit,
            isTimeSliceArray,
            isDeprecated,
            deprecatedBy,
            span,
            location);

        Assert.Equal(name, variableInfo.Name);
        Assert.Equal(valueType, variableInfo.ValueType);
        Assert.Equal(valueCount, variableInfo.ValueCount);
        Assert.Equal(description, variableInfo.Description);
        Assert.Equal(unit, variableInfo.ValueUnit);
        Assert.Equal(isTimeSliceArray, variableInfo.IsTimeSliceArray);
        Assert.Equal(isDeprecated, variableInfo.IsDeprecated);
        Assert.Equal(deprecatedBy, variableInfo.DeprecatedBy);

        Assert.Equal(span, variableInfo.JsonSpan);
        Assert.Equal(location, variableInfo.JsonLocation);
    }

    [Fact]
    public void Equals_Test()
    {
        var variable1Span = new TextSpan(2, 3);
        var variable2Span = new TextSpan(2, 3);

        var variable1Location = Location.Create(
            "test.txt",
            variable1Span,
            new LinePositionSpan(new LinePosition(0, 2), new LinePosition(0, 3)));

        var variable2Location = Location.Create(
            "test.txt",
            variable2Span,
            new LinePositionSpan(new LinePosition(0, 2), new LinePosition(0, 3)));

        var variable1 = new VariableInfo(
            "Test",
            VariableValueType.Int,
            4,
            "Test variable",
            "test/s",
            true,
            false,
            null,
            variable1Span,
            variable1Location);

        var variable2 = new VariableInfo(
            "Test",
            VariableValueType.Int,
            4,
            "Test variable",
            "test/s",
            true,
            false,
            null,
            variable2Span,
            variable2Location);

        EquatableStructAssert.Equal(variable1, variable2);
        EquatableStructAssert.Equal(variable2, variable1);
        EquatableStructAssert.NotEqual(variable1, default);
    }

    [Fact]
    public void Equals_UnequalTest()
    {
        var variable1Span = new TextSpan(2, 3);
        var variable2Span = new TextSpan(9, 10);

        var variable1Location = Location.Create(
            "test.txt",
            variable1Span,
            new LinePositionSpan(new LinePosition(0, 2), new LinePosition(0, 3)));

        var variable2Location = Location.Create(
            "test.txt",
            variable2Span,
            new LinePositionSpan(new LinePosition(0, 2), new LinePosition(0, 3)));

        var variable1 = new VariableInfo(
            "Test",
            VariableValueType.Int,
            4,
            "Test variable",
            "test/s",
            true,
            false,
            null,
            variable1Span,
            variable1Location);

        var variable2 = new VariableInfo(
            "Test2",
            VariableValueType.Float,
            1,
            "Test variable",
            null,
            false,
            true,
            "Test3",
            variable2Span,
            variable2Location);

        EquatableStructAssert.NotEqual(variable1, variable2);
        EquatableStructAssert.NotEqual(variable2, variable1);
    }

    [Fact]
    public void Equals_WrongTypeObjectTest()
    {
        var name = "Test";
        var valueType = VariableValueType.Int;
        int valueCount = 4;
        string description = "Test variable";
        string? unit = "test/s";
        bool isTimeSliceArray = true;
        bool isDeprecated = true;
        string? deprecatedBy = "TestEx";

        var span = new TextSpan(2, 3);
        var location = Location.Create(
            "test.txt",
            span,
            new LinePositionSpan(new LinePosition(0, 2), new LinePosition(0, 3)));

        var variableInfo = new VariableInfo(
            name,
            valueType,
            valueCount,
            description,
            unit,
            isTimeSliceArray,
            isDeprecated,
            deprecatedBy,
            span,
            location);

        EquatableStructAssert.ObjectEqualsMethod(false, variableInfo, DateTime.MinValue);
        EquatableStructAssert.ObjectEqualsMethod(false, default(VariableInfo), DateTime.MinValue);
    }
}
