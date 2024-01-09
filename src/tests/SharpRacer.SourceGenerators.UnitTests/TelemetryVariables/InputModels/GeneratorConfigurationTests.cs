namespace SharpRacer.SourceGenerators.TelemetryVariables.InputModels;
public class GeneratorConfigurationTests
{
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
        EquatableStructAssert.NotEqual(config1, default);
        EquatableStructAssert.ObjectEqualsMethod(false, config1, DateTime.MinValue);
    }
}
