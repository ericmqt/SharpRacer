using SharpRacer.SourceGenerators.TelemetryVariables;
using SharpRacer.SourceGenerators.Testing.Configuration;
using SharpRacer.SourceGenerators.Testing.TelemetryVariables;

namespace SharpRacer.SourceGenerators.Extensions;
public class AnalyzerConfigOptionsExtensionsTests
{
    [Fact]
    public void GetMSBuildProperty_Test()
    {
        var options = new VariablesGeneratorGlobalOptions(
            new VariablesGeneratorGlobalOptionsValues
            {
                GenerateTelemetryVariableClasses = "true"
            });

        var result = options.GetMSBuildProperty(MSBuildProperties.GenerateVariableClassesKey);

        Assert.True(result.Exists);
        Assert.Equal("true", result.Value);
    }

    [Fact]
    public void GetMSBuildProperty_ThrowOnDefaultValue()
    {
        var config = new EmptyAnalyzerConfigOptions();
        Assert.Throws<ArgumentException>(() => config.GetMSBuildProperty(default));
    }
}
