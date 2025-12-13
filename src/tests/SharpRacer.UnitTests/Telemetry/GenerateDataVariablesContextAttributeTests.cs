namespace SharpRacer.Telemetry;
public class GenerateDataVariablesContextAttributeTests
{
    [Fact]
    public void Ctor_Test()
    {
        var attr = new GenerateDataVariablesContextAttribute();

        Assert.Null(attr.IncludedVariableNamesFile);
    }

    [Fact]
    public void Ctor_IncludedVariableNamesFileNameTest()
    {
        string fileName = "variables.json";

        var attr = new GenerateDataVariablesContextAttribute(fileName);

        Assert.Equal(fileName, attr.IncludedVariableNamesFile);
    }

    [Fact]
    public void Ctor_ThrowIfNullOrEmptyFileNameTest()
    {
        Assert.Throws<ArgumentException>(() => new GenerateDataVariablesContextAttribute(string.Empty));
        Assert.Throws<ArgumentNullException>(() => new GenerateDataVariablesContextAttribute(null!));
    }
}
