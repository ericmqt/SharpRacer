namespace SharpRacer.SourceGenerators.TelemetryVariables.InputModels;
public class GeneratorConfigurationTests
{
    public static TheoryData<GeneratorConfiguration, GeneratorConfiguration> InequalityData => GetInequalityData();

    [Fact]
    public void Ctor_Test()
    {
        var variableInfoFileName = new VariableInfoFileName("Foo.bar");
        var variableOptionsFileName = new VariableOptionsFileName("Bar.baz");
        var generateVariableClasses = true;
        var variableClassesNamespace = "Test.Assembly";

        var config = new GeneratorConfiguration(
            variableInfoFileName,
            variableOptionsFileName,
            generateVariableClasses,
            variableClassesNamespace);

        Assert.Equal(variableInfoFileName, config.VariableInfoFileName);
        Assert.Equal(variableOptionsFileName, config.VariableOptionsFileName);
        Assert.Equal(generateVariableClasses, config.GenerateVariableClasses);
        Assert.Equal(variableClassesNamespace, config.VariableClassesNamespace);
    }

    [Fact]
    public void Ctor_ThrowOnDefaultVariableInfoFileNameTest()
    {
        Assert.Throws<ArgumentException>(() =>
            new GeneratorConfiguration(
                variableInfoFileName: default,
                variableOptionsFileName: new VariableOptionsFileName("Foo.bar"),
                generateVariableClasses: true,
                variableClassesNamespace: "Test.Assembly"));
    }

    [Fact]
    public void Ctor_ThrowOnDefaultVariableOptionsFileNameTest()
    {
        Assert.Throws<ArgumentException>(() =>
            new GeneratorConfiguration(
                variableInfoFileName: new VariableInfoFileName("Foo.bar"),
                variableOptionsFileName: default,
                generateVariableClasses: true,
                variableClassesNamespace: "Test.Assembly"));
    }

    [Fact]
    public void Ctor_ThrowOnNullOrEmptyVariableClassesNamespaceTest()
    {
        var variableInfoFileName = new VariableInfoFileName("Foo.bar");
        var variableOptionsFileName = new VariableOptionsFileName("Bar.baz");
        var generateVariableClasses = true;

        Assert.Throws<ArgumentException>(() =>
            new GeneratorConfiguration(
                variableInfoFileName,
                variableOptionsFileName,
                generateVariableClasses,
                variableClassesNamespace: null!));

        Assert.Throws<ArgumentException>(() =>
            new GeneratorConfiguration(
                variableInfoFileName,
                variableOptionsFileName,
                generateVariableClasses,
                variableClassesNamespace: string.Empty));
    }

    [Fact]
    public void Equals_Test()
    {
        var config1 = new GeneratorConfiguration(
            variableInfoFileName: new VariableInfoFileName("Foo.bar"),
            variableOptionsFileName: new VariableOptionsFileName("Bar.baz"),
            generateVariableClasses: true,
            variableClassesNamespace: "Test.Assembly");

        var config2 = new GeneratorConfiguration(
            variableInfoFileName: new VariableInfoFileName("Foo.bar"),
            variableOptionsFileName: new VariableOptionsFileName("Bar.baz"),
            generateVariableClasses: true,
            variableClassesNamespace: "Test.Assembly");

        EquatableStructAssert.Equal(config1, config2);
    }

    [Fact]
    public void Equals_DefaultValueTest()
    {
        var config1 = new GeneratorConfiguration(
            variableInfoFileName: new VariableInfoFileName("Foo.bar"),
            variableOptionsFileName: new VariableOptionsFileName("Bar.baz"),
            generateVariableClasses: true,
            variableClassesNamespace: "Test.Assembly");

        EquatableStructAssert.NotEqual(config1, default);
    }

    [Theory]
    [MemberData(nameof(InequalityData))]
    public void Equals_InequalityTest(GeneratorConfiguration config1, GeneratorConfiguration config2)
    {
        EquatableStructAssert.NotEqual(config1, config2);
    }

    [Fact]
    public void EqualsObject_WrongObjectTypeTest()
    {
        var config1 = new GeneratorConfiguration(
            variableInfoFileName: new VariableInfoFileName("Foo.bar"),
            variableOptionsFileName: new VariableOptionsFileName("Bar.baz"),
            generateVariableClasses: true,
            variableClassesNamespace: "Test.Assembly");

        EquatableStructAssert.ObjectEqualsMethod(false, config1, int.MaxValue);
    }

    private static TheoryData<GeneratorConfiguration, GeneratorConfiguration> GetInequalityData()
    {
        return new TheoryData<GeneratorConfiguration, GeneratorConfiguration>()
        {
            // VariableInfoFileName
            {
                new GeneratorConfiguration(new VariableInfoFileName("test.json"), new VariableOptionsFileName("options.json"), true, "Test.App"),
                new GeneratorConfiguration(new VariableInfoFileName("variables.json"), new VariableOptionsFileName("options.json"), true, "Test.App")
            },

            // VariableOptionsFileName
            {
                new GeneratorConfiguration(new VariableInfoFileName("test.json"), new VariableOptionsFileName("options1.json"), true, "Test.App"),
                new GeneratorConfiguration(new VariableInfoFileName("test.json"), new VariableOptionsFileName("options2.json"), true, "Test.App")
            },

            // GenerateVariableClasses
            {
                new GeneratorConfiguration(new VariableInfoFileName("test.json"), new VariableOptionsFileName("options.json"), true, "Test.App"),
                new GeneratorConfiguration(new VariableInfoFileName("test.json"), new VariableOptionsFileName("options.json"), false, "Test.App")
            },

            // VariableClassesNamespace
            {
                new GeneratorConfiguration(new VariableInfoFileName("test.json"), new VariableOptionsFileName("options.json"), true, "Test.App1"),
                new GeneratorConfiguration(new VariableInfoFileName("test.json"), new VariableOptionsFileName("options.json"), true, "Test.App2")
            }
        };
    }
}
