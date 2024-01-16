using Microsoft.CodeAnalysis.Text;

namespace SharpRacer.SourceGenerators.TelemetryVariables.Json;
public class JsonVariableOptionsTests
{
    public static TheoryData<JsonVariableOptions, JsonVariableOptions> InequalityData => ModelInequalityData.JsonVariableOptionsData();

    [Fact]
    public void Ctor_Test()
    {
        var key = "Lat";
        var keySpan = new TextSpan(200, key.Length);
        var optionsValue = new JsonVariableOptionsValue("Latitude", "LatitudeVariable");
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
        var optionsValue = new JsonVariableOptionsValue("Latitude", "LatitudeVariable");
        var valueSpan = new TextSpan(keySpan.End + 10, 256);

        var options1 = new JsonVariableOptions(key, keySpan, optionsValue, valueSpan);
        var options2 = new JsonVariableOptions(key, keySpan, optionsValue, valueSpan);

        Assert.True(options1 == options2);
        Assert.False(options1 != options2);
        Assert.True(options1.Equals(options2));
        Assert.Equal(options1.GetHashCode(), options2.GetHashCode());
    }

    [Theory]
    [MemberData(nameof(InequalityData))]
    public void Equals_InequalityTest(JsonVariableOptions options1, JsonVariableOptions options2)
    {
        EquatableStructAssert.NotEqual(options1, options2);
    }

    [Fact]
    public void Equals_WrongObjectTypeTest()
    {
        var options1 = new JsonVariableOptions(
            "Lat",
            new TextSpan(200, 3),
            new JsonVariableOptionsValue("Latitude", "LatitudeVariable"),
            new TextSpan(220, 256));

        EquatableStructAssert.ObjectEqualsMethod(false, options1, int.MaxValue);
    }
}
