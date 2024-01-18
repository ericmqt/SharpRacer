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
    }

    [Fact]
    public void Equals_DefaultValueTest()
    {
        var variable1 = CreateTestVariable();

        EquatableStructAssert.NotEqual(variable1, default);
    }

    [Theory]
    [MemberData(nameof(GetInequalityData))]
    public void Equals_InequalityTest(VariableInfo variable1, VariableInfo variable2)
    {
        EquatableStructAssert.NotEqual(variable1, variable2);
    }

    [Fact]
    public void EqualsObject_WrongObjectTypeTest()
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
    }

    public static TheoryData<VariableInfo, VariableInfo> GetInequalityData()
    {
        var fakeLocation = Location.Create(
            "test.json",
            new TextSpan(10, 20),
            new LinePositionSpan(new LinePosition(1, 2), new LinePosition(4, 5)));

        return new TheoryData<VariableInfo, VariableInfo>()
        {
            // Name
            { CreateTestVariable(name: "Test1"), CreateTestVariable(name: "Test2") },

            // Value type
            { CreateTestVariable(valueType: VariableValueType.Byte), CreateTestVariable(valueType: VariableValueType.Int) },

            // Value count
            { CreateTestVariable(valueCount: 1), CreateTestVariable(valueCount: 10) },

            // Description
            { CreateTestVariable(description: "one description"), CreateTestVariable(description: "other description") },

            // Value unit
            { CreateTestVariable(valueUnit: "unit/s"), CreateTestVariable(valueUnit: "test/s") },

            // Timeslice array
            { CreateTestVariable(isTimeSliceArray: true), CreateTestVariable(isTimeSliceArray: false) },

            // Deprecated
            { CreateTestVariable(isDeprecated: true), CreateTestVariable(isDeprecated: false) },

            // Deprecated by
            { CreateTestVariable(isDeprecated: true, deprecatedBy: "TestEx"), CreateTestVariable(isDeprecated: true, deprecatedBy: "TestEx2") },

            // JsonSpan
            { CreateTestVariable().WithJsonSpan(new TextSpan(1, 2)), CreateTestVariable().WithJsonSpan(new TextSpan(100, 300)) },

            // JsonLocation
            { CreateTestVariable().WithJsonLocation(Location.None), CreateTestVariable().WithJsonLocation(fakeLocation) }
        };
    }

    private static VariableInfo CreateTestVariable(
        string name = "Test",
        VariableValueType valueType = VariableValueType.Int,
        int valueCount = 1,
        string description = "Test description",
        string? valueUnit = "unit/s",
        bool isTimeSliceArray = false,
        bool isDeprecated = false,
        string? deprecatedBy = null)
    {
        return new VariableInfo(name, valueType, valueCount, description, valueUnit, isTimeSliceArray, isDeprecated, deprecatedBy);
    }
}
