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

        Assert.True(optionsValue1 == optionsValue2);
        Assert.False(optionsValue1 != optionsValue2);
        Assert.True(optionsValue1.Equals(optionsValue2));
        Assert.Equal(optionsValue1.GetHashCode(), optionsValue2.GetHashCode());
    }

    [Fact]
    public void Equals_UnequalTest()
    {
        var options1 = new JsonVariableOptionsValue("Latitude", "LatitudeVariable");
        var options2 = new JsonVariableOptionsValue("SessionTime", "SessionTimeVariable");

        Assert.False(options1 == options2);
        Assert.True(options1 != options2);
        Assert.False(options1.Equals(options2));
        Assert.NotEqual(options1.GetHashCode(), options2.GetHashCode());
    }
}
