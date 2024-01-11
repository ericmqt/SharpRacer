using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using SharpRacer.SourceGenerators.TelemetryVariables.Json;

namespace SharpRacer.SourceGenerators.TelemetryVariables.InputModels;
public class VariableOptionsTests
{
    [Fact]
    public void Ctor_ThrowOnNullOrEmptyVariableKeyArgTest()
    {
        Assert.Throws<ArgumentException>(() => new VariableOptions(variableKey: null!));
        Assert.Throws<ArgumentException>(() => new VariableOptions(variableKey: string.Empty));
    }

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

        EquatableStructAssert.NotEqual(variableOptions, default);
        EquatableStructAssert.ObjectEqualsMethod(false, variableOptions, valueLocation);
    }

    [Fact]
    public void Equals_ParameterlessCtor_DefaultTest()
    {
        var variableOptions = new VariableOptions();

        EquatableStructAssert.Equal(variableOptions, default);
    }
}
