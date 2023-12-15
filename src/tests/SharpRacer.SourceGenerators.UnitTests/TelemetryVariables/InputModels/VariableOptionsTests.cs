using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using SharpRacer.SourceGenerators.TelemetryVariables.Json;

namespace SharpRacer.SourceGenerators.TelemetryVariables.InputModels;
public class VariableOptionsTests
{
    [Fact]
    public void Equals_DefaultTest()
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

        Assert.False(variableOptions == default);
        Assert.True(variableOptions != default);
        Assert.False(variableOptions.Equals(default));
        Assert.NotEqual(variableOptions.GetHashCode(), default(VariableOptions).GetHashCode());
    }

    [Fact]
    public void Equals_ParameterlessCtor_DefaultTest()
    {
        var variableOptions = new VariableOptions();

        Assert.True(variableOptions == default);
        Assert.False(variableOptions != default);
        Assert.True(variableOptions.Equals(default));
        Assert.Equal(variableOptions.GetHashCode(), default(VariableOptions).GetHashCode());
    }
}
