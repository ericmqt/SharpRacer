namespace SharpRacer.SourceGenerators.TelemetryVariables.Json;
public class JsonVariableOptionsValueTests
{
    [Fact]
    public void Ctor_Test()
    {
        var name = "Latitude";
        var className = "LatitudeVariable";

        var optionsValue = new JsonVariableOptionsValue(name, className);

        Assert.Equal(name, optionsValue.Name);
        Assert.Equal(className, optionsValue.ClassName);
    }

    [Fact]
    public void Equals_Test()
    {
        var name = "Latitude";
        var className = "LatitudeVariable";

        var optionsValue1 = new JsonVariableOptionsValue(name, className);
        var optionsValue2 = new JsonVariableOptionsValue(name, className);

        EquatableStructAssert.Equal(optionsValue1, optionsValue2);
    }

    [Fact]
    public void Equals_DefaultValueTest()
    {
        var optionsValue1 = new JsonVariableOptionsValue("Latitude", "LatitudeVariable");

        EquatableStructAssert.NotEqual(optionsValue1, default);
    }

    [Theory]
    [MemberData(nameof(GetInequalityData))]
    public void Equals_InequalityTest(JsonVariableOptionsValue options1, JsonVariableOptionsValue options2)
    {
        EquatableStructAssert.NotEqual(options1, options2);
    }

    [Fact]
    public void EqualsObject_WrongObjectTypeTest()
    {
        var options1 = new JsonVariableOptionsValue("Latitude", "LatitudeVariable");

        EquatableStructAssert.ObjectEqualsMethod(false, options1, int.MaxValue);
    }

    public static TheoryData<JsonVariableOptionsValue, JsonVariableOptionsValue> GetInequalityData()
    {
        return new TheoryData<JsonVariableOptionsValue, JsonVariableOptionsValue>()
        {
            // Name
            {
                new JsonVariableOptionsValue("Test1", "TestClass"),
                new JsonVariableOptionsValue("Test2", "TestClass")
            },

            // Class name
            {
                new JsonVariableOptionsValue("Test", "TestClass1"),
                new JsonVariableOptionsValue("Test", "TestClass2")
            }
        };
    }
}
