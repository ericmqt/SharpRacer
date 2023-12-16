using SharpRacer.SourceGenerators.Testing.Configuration;

namespace SharpRacer.SourceGenerators.Extensions;
public class AnalyzerConfigOptionsExtensionsTests
{
    [Fact]
    public void GetMSBuildProperty_ThrowOnDefaultValue()
    {
        var config = new EmptyAnalyzerConfigOptions();
        Assert.Throws<ArgumentException>(() => config.GetMSBuildProperty(default));
    }
}
