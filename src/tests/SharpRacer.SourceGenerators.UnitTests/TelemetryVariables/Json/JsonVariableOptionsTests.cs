using Microsoft.CodeAnalysis.Text;

namespace SharpRacer.SourceGenerators.TelemetryVariables.Json;
public class JsonVariableOptionsTests
{
    [Fact]
    public void Ctor_Test()
    {
        var key = "Lat";
        var keySpan = new TextSpan(200, key.Length);
        var optionsValue = new JsonVariableOptionsValue("Latitude", "LatitudeDescriptor", "LatitudeVariable");
        var valueSpan = new TextSpan(keySpan.End + 10, 256);

        var options = new JsonVariableOptions(key, keySpan, optionsValue, valueSpan);

        Assert.Equal(key, options.Key);
        Assert.Equal(keySpan, options.KeySpan);
        Assert.Equal(optionsValue, options.Value);
        Assert.Equal(valueSpan, options.ValueSpan);
    }

    [Fact]
    public void Equals_EqualTest()
    {
        var key = "Lat";
        var keySpan = new TextSpan(200, key.Length);
        var optionsValue = new JsonVariableOptionsValue("Latitude", "LatitudeDescriptor", "LatitudeVariable");
        var valueSpan = new TextSpan(keySpan.End + 10, 256);

        var options1 = new JsonVariableOptions(key, keySpan, optionsValue, valueSpan);
        var options2 = new JsonVariableOptions(key, keySpan, optionsValue, valueSpan);

        Assert.True(options1 == options2);
        Assert.False(options1 != options2);
        Assert.True(options1.Equals(options2));
        Assert.Equal(options1.GetHashCode(), options2.GetHashCode());
    }

    [Fact]
    public void Equals_UnequalTest()
    {
        var options1 = new JsonVariableOptions(
            "Lat",
            new TextSpan(200, 3),
            new JsonVariableOptionsValue("Latitude", "LatitudeDescriptor", "LatitudeVariable"),
            new TextSpan(220, 256));

        var options2 = new JsonVariableOptions(
            "SessionTime",
            new TextSpan(200, 3),
            new JsonVariableOptionsValue("SessionTime", "SessionTimeDescriptor", "SessionTimeVariable"),
            new TextSpan(220, 256));

        Assert.False(options1 == options2);
        Assert.True(options1 != options2);
        Assert.False(options1.Equals(options2));
        Assert.NotEqual(options1.GetHashCode(), options2.GetHashCode());
    }
}
