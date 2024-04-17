using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using SharpRacer.SourceGenerators.TelemetryVariables.Json;

namespace SharpRacer.SourceGenerators.TelemetryVariables.InputModels;
public class VariableOptionsTests
{
    [Fact]
    public void Ctor_VariableKeyTest()
    {
        var options = new VariableOptions("Test");

        Assert.Equal("Test", options.VariableKey);
        Assert.Equal(Location.None, options.VariableKeyLocation);
        Assert.Equal(Location.None, options.ValueLocation);
        Assert.Null(options.Name);
        Assert.Null(options.ClassName);
    }

    [Fact]
    public void Ctor_KeyAndOptionsValuesWithLocationsTest()
    {
        var keyLocation = Location.Create(
            "test.txt",
            new TextSpan(1, 6),
            new LinePositionSpan(new LinePosition(0, 2), new LinePosition(0, 3)));

        var valueLocation = Location.Create(
            "test.txt",
            new TextSpan(10, 80),
            new LinePositionSpan(new LinePosition(1, 5), new LinePosition(12, 33)));

        var options = new VariableOptions("TestVar", "Test", "TestClass", keyLocation, valueLocation);

        Assert.Equal("TestVar", options.VariableKey);
        Assert.Equal("Test", options.Name);
        Assert.Equal("TestClass", options.ClassName);
        Assert.Equal(keyLocation, options.VariableKeyLocation);
        Assert.Equal(valueLocation, options.ValueLocation);
    }

    [Fact]
    public void Ctor_ThrowOnNullOrEmptyVariableKeyArgTest()
    {
        Assert.Throws<ArgumentException>(() => new VariableOptions(variableKey: null!));
        Assert.Throws<ArgumentException>(() => new VariableOptions(variableKey: string.Empty));
    }

    [Fact]
    public void Equals_Test()
    {
        var jsonOptions = new JsonVariableOptions(
            "Test",
            new TextSpan(4, 234),
            new JsonVariableOptionsValue(
                "TestVar",
                "TestVariable"),
            new TextSpan(256, 312));

        var keyLocation = Location.Create(
            "test.txt",
            jsonOptions.KeySpan,
            new LinePositionSpan(new LinePosition(0, 2), new LinePosition(0, 3)));

        var valueLocation = Location.Create(
            "test.txt",
            jsonOptions.ValueSpan,
            new LinePositionSpan(new LinePosition(0, 2), new LinePosition(0, 3)));

        var variableOptions1 = new VariableOptions(jsonOptions, keyLocation, valueLocation);
        var variableOptions2 = new VariableOptions(jsonOptions, keyLocation, valueLocation);

        EquatableStructAssert.Equal(variableOptions1, variableOptions2);
    }

    [Fact]
    public void Equals_DefaultValueTest()
    {
        var jsonOptions = new JsonVariableOptions(
            "Test",
            new TextSpan(4, 234),
            new JsonVariableOptionsValue(
                "TestVar",
                "TestVariable"),
            new TextSpan(256, 312));

        var keyLocation = Location.Create(
            "test.txt",
            jsonOptions.KeySpan,
            new LinePositionSpan(new LinePosition(0, 2), new LinePosition(0, 3)));

        var valueLocation = Location.Create(
            "test.txt",
            jsonOptions.ValueSpan,
            new LinePositionSpan(new LinePosition(0, 2), new LinePosition(0, 3)));

        var variableOptions = new VariableOptions(jsonOptions, keyLocation, valueLocation);

        EquatableStructAssert.NotEqual(variableOptions, default);
    }

    [Theory]
    [MemberData(nameof(GetInequalityData))]
    public void Equals_InequalityTest(VariableOptions options1, VariableOptions options2)
    {
        EquatableStructAssert.NotEqual(options1, options2);
    }

    [Fact]
    public void Equals_ParameterlessCtor_DefaultTest()
    {
        var variableOptions = new VariableOptions();

        EquatableStructAssert.Equal(variableOptions, default);
    }

    [Fact]
    public void EqualsObject_WrongObjectTypeTest()
    {
        var jsonOptions = new JsonVariableOptions(
            "Test",
            new TextSpan(4, 234),
            new JsonVariableOptionsValue(
                "TestVar",
                "TestVariable"),
            new TextSpan(256, 312));

        var keyLocation = Location.Create(
            "test.txt",
            jsonOptions.KeySpan,
            new LinePositionSpan(new LinePosition(0, 2), new LinePosition(0, 3)));

        var valueLocation = Location.Create(
            "test.txt",
            jsonOptions.ValueSpan,
            new LinePositionSpan(new LinePosition(0, 2), new LinePosition(0, 3)));

        var variableOptions = new VariableOptions(jsonOptions, keyLocation, valueLocation);

        EquatableStructAssert.ObjectEqualsMethod(false, variableOptions, valueLocation);
    }

    public static TheoryData<VariableOptions, VariableOptions> GetInequalityData()
    {
        var keyLocation = Location.Create(
            "test.txt",
            new TextSpan(1, 6),
            new LinePositionSpan(new LinePosition(0, 2), new LinePosition(0, 3)));

        var valueLocation = Location.Create(
            "test.txt",
            new TextSpan(10, 80),
            new LinePositionSpan(new LinePosition(1, 5), new LinePosition(12, 33)));

        return new TheoryData<VariableOptions, VariableOptions>()
        {
            // VariableKey
            {
                new VariableOptions("TestVar1", "Test", "TestClass", Location.None, Location.None),
                new VariableOptions("TestVar2", "Test", "TestClass", Location.None, Location.None)
            },

            // Name
            {
                new VariableOptions("TestVar", "Test1", "TestClass", Location.None, Location.None),
                new VariableOptions("TestVar", "Test2", "TestClass", Location.None, Location.None)
            },

            // ClassName
            {
                new VariableOptions("TestVar", "Test", "TestClass1", Location.None, Location.None),
                new VariableOptions("TestVar", "Test", "TestClass2", Location.None, Location.None)
            },

            // VariableKeyLocation
            {
                new VariableOptions("TestVar", "Test", "TestClass", Location.None, Location.None),
                new VariableOptions("TestVar", "Test", "TestClass", keyLocation, Location.None)
            },

            // ValueLocation
            {
                new VariableOptions("TestVar", "Test", "TestClass", Location.None, Location.None),
                new VariableOptions("TestVar", "Test", "TestClass", Location.None, valueLocation)
            }
        };
    }
}
