using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace SharpRacer.SourceGenerators.TelemetryVariables.InputModels;
public class IncludedVariableNameTests
{
    [Fact]
    public void Ctor_Test()
    {
        var location = Location.Create(
            "test.txt",
            new TextSpan(2, 3),
            new LinePositionSpan(new LinePosition(0, 2), new LinePosition(0, 3)));

        var name = "Lat";

        var includedVariableName = new IncludedVariableName(name, location);

        Assert.Equal(name, includedVariableName.Value);
        Assert.Equal(location, includedVariableName.SourceLocation);
    }

    [Fact]
    public void Ctor_ThrowOnNullVariableNameTest()
    {
        var location = Location.Create(
            "test.txt",
            new TextSpan(2, 3),
            new LinePositionSpan(new LinePosition(0, 2), new LinePosition(0, 3)));

        Assert.Throws<ArgumentException>(() => new IncludedVariableName(null!, location));
    }

    [Fact]
    public void Ctor_ThrowOnNullLocationTest()
    {
        Assert.Throws<ArgumentNullException>(() => new IncludedVariableName("test", null!));
    }

    [Fact]
    public void Equals_Test()
    {
        var location = Location.Create(
            "test.txt",
            new TextSpan(2, 3),
            new LinePositionSpan(new LinePosition(0, 2), new LinePosition(0, 3)));

        var includedVariable1 = new IncludedVariableName("Lat", location);
        var includedVariable2 = new IncludedVariableName("Lat", location);

        EquatableStructAssert.Equal(includedVariable1, includedVariable2);
    }

    [Fact]
    public void Equals_DefaultValueTest()
    {
        var location = Location.Create(
            "test.txt",
            new TextSpan(2, 3),
            new LinePositionSpan(new LinePosition(0, 2), new LinePosition(0, 3)));

        var includedVariable1 = new IncludedVariableName("Lat", location);

        EquatableStructAssert.NotEqual(includedVariable1, default);
    }

    [Theory]
    [MemberData(nameof(GetInequalityData))]
    public void Equals_InequalityTest(IncludedVariableName includedVariable1, IncludedVariableName includedVariable2)
    {
        EquatableStructAssert.NotEqual(includedVariable1, includedVariable2);
    }

    [Fact]
    public void EqualsObject_WrongObjectTypeTest()
    {
        var location = Location.Create(
            "test.txt",
            new TextSpan(2, 3),
            new LinePositionSpan(new LinePosition(0, 2), new LinePosition(0, 3)));

        var includedVariable1 = new IncludedVariableName("Lat", location);

        EquatableStructAssert.ObjectEqualsMethod(false, includedVariable1, location);
    }

    [Fact]
    public void ImplicitStringOperatorTest()
    {
        var includedVariable1 = new IncludedVariableName("Lat", Location.None);

        string name = includedVariable1;

        Assert.Equal("Lat", name);
        Assert.Equal(name, includedVariable1.Value);

        Assert.Equal(string.Empty, default(IncludedVariableName));
    }

    public static TheoryData<IncludedVariableName, IncludedVariableName> GetInequalityData()
    {
        var location = Location.Create(
            "test.txt",
            new TextSpan(2, 3),
            new LinePositionSpan(new LinePosition(0, 2), new LinePosition(0, 3)));

        return new TheoryData<IncludedVariableName, IncludedVariableName>()
        {
            // Value
            {
                new IncludedVariableName("Test1", location),
                new IncludedVariableName("Test2", location)
            },

            // Location
            {
                new IncludedVariableName("Test", location),
                new IncludedVariableName("Test", Location.None)
            }
        };
    }
}
