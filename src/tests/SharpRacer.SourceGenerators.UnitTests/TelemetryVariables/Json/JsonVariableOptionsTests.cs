using Microsoft.CodeAnalysis.Text;

namespace SharpRacer.SourceGenerators.TelemetryVariables.Json;
public class JsonVariableOptionsTests
{
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
    public void Equals_Test()
    {
        var key = "Lat";
        var keySpan = new TextSpan(200, key.Length);
        var optionsValue = new JsonVariableOptionsValue("Latitude", "LatitudeVariable");
        var valueSpan = new TextSpan(keySpan.End + 10, 256);

        var options1 = new JsonVariableOptions(key, keySpan, optionsValue, valueSpan);
        var options2 = new JsonVariableOptions(key, keySpan, optionsValue, valueSpan);

        EquatableStructAssert.Equal(options1, options2);
    }

    [Theory]
    [MemberData(nameof(GetInequalityData))]
    public void Equals_InequalityTest(JsonVariableOptions options1, JsonVariableOptions options2)
    {
        EquatableStructAssert.NotEqual(options1, options2);
    }

    [Fact]
    public void EqualsObject_WrongObjectTypeTest()
    {
        var options1 = new JsonVariableOptions(
            "Lat",
            new TextSpan(200, 3),
            new JsonVariableOptionsValue("Latitude", "LatitudeVariable"),
            new TextSpan(220, 256));

        EquatableStructAssert.ObjectEqualsMethod(false, options1, int.MaxValue);
    }

    public static TheoryData<JsonVariableOptions, JsonVariableOptions> GetInequalityData()
    {
        var optionsValue1 = new JsonVariableOptionsValue("Test1", "TestClass");
        var optionsValue2 = new JsonVariableOptionsValue("Test2", "TestClass");

        var keySpan1 = new TextSpan(200, "Key1".Length);
        var keySpan2 = new TextSpan(400, "Key2".Length);

        var valueSpan1 = new TextSpan(250, 100);
        var valueSpan2 = new TextSpan(450, 100);

        return new TheoryData<JsonVariableOptions, JsonVariableOptions>()
        {
            // Key
            {
                new JsonVariableOptions("Key1", keySpan1, optionsValue1, valueSpan1),
                new JsonVariableOptions("Key2", keySpan1, optionsValue1, valueSpan1)
            },

            // Key span
            {
                new JsonVariableOptions("Key1", keySpan1, optionsValue1, valueSpan1),
                new JsonVariableOptions("Key1", keySpan2, optionsValue1, valueSpan1)
            },

            // Value
            {
                new JsonVariableOptions("Key1", keySpan1, optionsValue1, valueSpan1),
                new JsonVariableOptions("Key1", keySpan1, optionsValue2, valueSpan1)
            },

            // Value span
            {
                new JsonVariableOptions("Key1", keySpan1, optionsValue1, valueSpan1),
                new JsonVariableOptions("Key1", keySpan1, optionsValue1, valueSpan2)
            },
        };
    }
}
