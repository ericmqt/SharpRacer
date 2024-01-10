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

        var name = "Lat";

        var includedVariable1 = new IncludedVariableName(name, location);
        var includedVariable2 = new IncludedVariableName(name, location);

        EquatableStructAssert.Equal(includedVariable1, includedVariable2);
    }

    [Fact]
    public void Equals_DefaultValueTest()
    {
        var location = Location.Create(
            "test.txt",
            new TextSpan(2, 3),
            new LinePositionSpan(new LinePosition(0, 2), new LinePosition(0, 3)));

        var name = "Lat";

        var includedVariable1 = new IncludedVariableName(name, location);

        EquatableStructAssert.NotEqual(includedVariable1, default);
    }

    [Fact]
    public void Equals_UnequalTest()
    {
        var location = Location.Create(
            "test.txt",
            new TextSpan(2, 3),
            new LinePositionSpan(new LinePosition(0, 2), new LinePosition(0, 3)));

        var includedVariable1 = new IncludedVariableName("Lat", location);
        var includedVariable2 = new IncludedVariableName("Lon", location);

        EquatableStructAssert.NotEqual(includedVariable1, includedVariable2);
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
}
