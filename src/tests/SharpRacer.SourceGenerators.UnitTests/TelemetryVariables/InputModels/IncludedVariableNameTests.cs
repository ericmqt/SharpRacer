using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using SharpRacer.SourceGenerators.TelemetryVariables.Diagnostics;

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
        Assert.False(includedVariableName.Diagnostics.IsDefault);
        Assert.True(includedVariableName.Diagnostics.IsEmpty);
    }

    [Fact]
    public void Ctor_WithDiagnosticsTest()
    {
        var location = Location.Create(
            "test.txt",
            new TextSpan(2, 3),
            new LinePositionSpan(new LinePosition(0, 2), new LinePosition(0, 3)));

        var name = "Lat";

        var diagnostic = IncludedVariablesDiagnostics.VariableAlreadyIncluded(name, location);

        var includedVariableName = new IncludedVariableName(name, location, [diagnostic]);

        Assert.Equal(name, includedVariableName.Value);
        Assert.Equal(location, includedVariableName.SourceLocation);
        Assert.False(includedVariableName.Diagnostics.IsDefault);
        Assert.False(includedVariableName.Diagnostics.IsEmpty);
        Assert.Single(includedVariableName.Diagnostics);
        Assert.Equal(diagnostic, includedVariableName.Diagnostics.Single());
    }

    [Fact]
    public void Ctor_ThrowOnNullOrEmptyVariableNameTest()
    {
        var location = Location.Create(
            "test.txt",
            new TextSpan(2, 3),
            new LinePositionSpan(new LinePosition(0, 2), new LinePosition(0, 3)));

        Assert.Throws<ArgumentException>(() => new IncludedVariableName(null!, location));
        Assert.Throws<ArgumentException>(() => new IncludedVariableName(null!, location, ImmutableArray<Diagnostic>.Empty));
        Assert.Throws<ArgumentException>(() => new IncludedVariableName(string.Empty, location));
        Assert.Throws<ArgumentException>(() => new IncludedVariableName(string.Empty, location, ImmutableArray<Diagnostic>.Empty));
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

        Assert.True(includedVariable1 == includedVariable2);
        Assert.False(includedVariable1 != includedVariable2);
        Assert.True(includedVariable1.Equals(includedVariable2));
        Assert.Equal(includedVariable1.GetHashCode(), includedVariable2.GetHashCode());
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

        Assert.False(includedVariable1 == default);
        Assert.True(includedVariable1 != default);
        Assert.False(includedVariable1.Equals(default));
        Assert.False(default == includedVariable1);
        Assert.True(default != includedVariable1);
        Assert.False(default(IncludedVariableName).Equals(includedVariable1));
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

        Assert.False(includedVariable1 == includedVariable2);
        Assert.True(includedVariable1 != includedVariable2);
        Assert.False(includedVariable1.Equals(includedVariable2));
        Assert.NotEqual(includedVariable1.GetHashCode(), includedVariable2.GetHashCode());
    }
}
