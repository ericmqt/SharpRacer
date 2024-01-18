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
    public void Equals_Test()
    {
        var generatorOptions1 = new VariableClassGeneratorOptions(true, "Test.Variables");
        var generatorOptions2 = new VariableClassGeneratorOptions(true, "Test.Variables");

        EquatableStructAssert.Equal(generatorOptions1, generatorOptions2);
    }

    [Fact]
    public void Equals_DefaultValueTest()
    {
        var generatorOptions1 = new VariableClassGeneratorOptions(true, "Test.Variables");
        EquatableStructAssert.NotEqual(generatorOptions1, default);
    }

    [Theory]
    [MemberData(nameof(GetInequalityData))]
    public void Equals_InequalityTest(VariableClassGeneratorOptions options1, VariableClassGeneratorOptions options2)
    {
        EquatableStructAssert.NotEqual(options1, options2);
    }

    [Fact]
    public void EqualsObject_WrongObjectTypeTest()
    {
        var generatorOptions1 = new VariableClassGeneratorOptions(true, "Test.Variables");

        EquatableStructAssert.ObjectEqualsMethod(false, generatorOptions1, int.MaxValue);
    }

    public static TheoryData<VariableClassGeneratorOptions, VariableClassGeneratorOptions> GetInequalityData()
    {
        return new TheoryData<VariableClassGeneratorOptions, VariableClassGeneratorOptions>()
        {
            // IsGeneratorEnabled
            {
                new VariableClassGeneratorOptions(true, "Test.Variables"),
                new VariableClassGeneratorOptions(false, "Test.Variables")
            },

            // Target namespace
            {
                new VariableClassGeneratorOptions(true, "Test.Variables1"),
                new VariableClassGeneratorOptions(true, "Test.Variables2")
            }
        };
    }
}
