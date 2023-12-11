namespace SharpRacer.SourceGenerators.TelemetryVariables.Json;
public class JsonVariableOptionsValueTests
{
    [Fact]
    public void Ctor_Test()
    {
        var name = "Latitude";
        var descriptorName = "LatitudeDescriptor";
        var contextPropertyName = "LatitudeVariable";

        var optionsValue = new JsonVariableOptionsValue(name, descriptorName, contextPropertyName);

        Assert.Equal(name, optionsValue.Name);
        Assert.Equal(descriptorName, optionsValue.DescriptorName);
        Assert.Equal(contextPropertyName, optionsValue.ContextPropertyName);
    }

    [Fact]
    public void Equals_Test()
    {
        var name = "Latitude";
        var descriptorName = "LatitudeDescriptor";
        var contextPropertyName = "LatitudeVariable";

        var optionsValue1 = new JsonVariableOptionsValue(name, descriptorName, contextPropertyName);
        var optionsValue2 = new JsonVariableOptionsValue(name, descriptorName, contextPropertyName);

        Assert.True(optionsValue1 == optionsValue2);
        Assert.False(optionsValue1 != optionsValue2);
        Assert.True(optionsValue1.Equals(optionsValue2));
        Assert.Equal(optionsValue1.GetHashCode(), optionsValue2.GetHashCode());
    }

    [Fact]
    public void Equals_UnequalTest()
    {
        var options1 = new JsonVariableOptionsValue("Latitude", "LatitudeDescriptor", "LatitudeVariable");
        var options2 = new JsonVariableOptionsValue("SessionTime", "SessionTimeDescriptor", "SessionTimeVariable");

        Assert.False(options1 == options2);
        Assert.True(options1 != options2);
        Assert.False(options1.Equals(options2));
        Assert.NotEqual(options1.GetHashCode(), options2.GetHashCode());
    }
}
