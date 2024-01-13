using SharpRacer.SourceGenerators.TelemetryVariables.InputModels;

namespace SharpRacer.SourceGenerators.TelemetryVariables.GeneratorModels;
public class VariableClassGeneratorOptionsTests
{
    [Fact]
    public void Ctor_Test()
    {
        var isGeneratorEnabled = true;
        var targetNamespace = "Test.Variables";
        var classNameFormat = "{0}Variable";

        var generatorOptions = new VariableClassGeneratorOptions(isGeneratorEnabled, targetNamespace);

        Assert.Equal(isGeneratorEnabled, generatorOptions.IsGeneratorEnabled);
        Assert.Equal(targetNamespace, generatorOptions.TargetNamespace);
        Assert.Equal(classNameFormat, generatorOptions.ClassNameFormat);
    }

    [Fact]
    public void Ctor_DefaultTargetNamespaceTest()
    {
        var generatorOptions = new VariableClassGeneratorOptions(false, null!);

        Assert.Equal(GeneratorConfigurationDefaults.TelemetryVariableClassesNamespace, generatorOptions.TargetNamespace);
    }

    [Fact]
    public void FormatClassName_Test()
    {
        var generatorOptions = new VariableClassGeneratorOptions(true, "Test.Variables");

        Assert.Equal("SessionTickVariable", generatorOptions.FormatClassName("SessionTick"));
    }

    [Fact]
    public void Equals_IdenticalValuesTest()
    {
        var generatorOptions1 = new VariableClassGeneratorOptions(true, "Test.Variables");
        var generatorOptions2 = new VariableClassGeneratorOptions(true, "Test.Variables");

        EquatableStructAssert.Equal(generatorOptions1, generatorOptions2);
        EquatableStructAssert.NotEqual(generatorOptions1, default);
        EquatableStructAssert.ObjectEqualsMethod(false, generatorOptions1, int.MaxValue);
    }

    [Fact]
    public void Equals_DifferentGeneratorEnabledValuesTest()
    {
        var generatorOptions1 = new VariableClassGeneratorOptions(true, "Test.Variables");
        var generatorOptions2 = new VariableClassGeneratorOptions(false, "Test.Variables");

        EquatableStructAssert.NotEqual(generatorOptions1, generatorOptions2);
    }

    [Fact]
    public void Equals_DifferentTargetNamespacesTest()
    {
        var generatorOptions1 = new VariableClassGeneratorOptions(true, "Test.Variables");
        var generatorOptions2 = new VariableClassGeneratorOptions(true, "Test.Variables2");

        EquatableStructAssert.NotEqual(generatorOptions1, generatorOptions2);
    }
}
