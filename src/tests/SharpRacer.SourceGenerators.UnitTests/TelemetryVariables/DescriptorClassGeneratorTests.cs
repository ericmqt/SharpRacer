namespace SharpRacer.SourceGenerators.TelemetryVariables;
public class DescriptorClassGeneratorTests
{
    [Fact]
    public void Ctor_ThrowOnNullModelTest()
    {
        Assert.Throws<ArgumentNullException>(() => new DescriptorClassGenerator(null!));
    }
}
