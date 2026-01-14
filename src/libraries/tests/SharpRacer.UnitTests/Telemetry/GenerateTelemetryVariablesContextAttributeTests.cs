namespace SharpRacer.Telemetry;

public class GenerateTelemetryVariablesContextAttributeTests
{
    [Fact]
    public void Ctor_Test()
    {
        var attr = new GenerateTelemetryVariablesContextAttribute();

        Assert.Null(attr.IncludedVariableNamesFile);
    }

    [Fact]
    public void Ctor_IncludedVariableNamesFileNameTest()
    {
        string fileName = "variables.json";

        var attr = new GenerateTelemetryVariablesContextAttribute(fileName);

        Assert.Equal(fileName, attr.IncludedVariableNamesFile);
    }

    [Fact]
    public void Ctor_ThrowIfNullOrEmptyFileNameTest()
    {
        Assert.Throws<ArgumentException>(() => new GenerateTelemetryVariablesContextAttribute(string.Empty));
        Assert.Throws<ArgumentNullException>(() => new GenerateTelemetryVariablesContextAttribute(null!));
    }
}
