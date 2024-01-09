using SharpRacer.SourceGenerators.Testing;

namespace SharpRacer.SourceGenerators.TelemetryVariables.InputModels;
public class VariableInfoFileNameTests
{
    [Fact]
    public void Ctor_Test()
    {
        var fileName = new VariableInfoFileName("Foo.bar");

        string fileNameValue = fileName;

        Assert.Equal(fileNameValue, fileName, StringComparer.Ordinal);
    }

    [Fact]
    public void Ctor_ThrowOnNullOrEmptyFileNameTest()
    {
        Assert.Throws<ArgumentException>(() => new VariableInfoFileName(null!));
        Assert.Throws<ArgumentException>(() => new VariableInfoFileName(string.Empty));
    }

    [Fact]
    public void IsMatch_Test()
    {
        var matchingFile = new AdditionalTextFile("Foo.bar", "Hello, world!");
        var matchingFileWithDirectory = new AdditionalTextFile("src/tests/Foo.bar", "Hello, world!");
        var nonMatchingFile = new AdditionalTextFile("Bar.baz", "Goodbye, world!");
        var nonMatchingFileWithDirectory = new AdditionalTextFile("src/tests/Bar.baz", "Goodbye, world!");

        var fileName = new VariableInfoFileName("Foo.bar");

        Assert.True(fileName.IsMatch(matchingFile));
        Assert.True(fileName.IsMatch(matchingFileWithDirectory));
        Assert.False(fileName.IsMatch(nonMatchingFile));
        Assert.False(fileName.IsMatch(nonMatchingFileWithDirectory));
    }

    [Fact]
    public void Equals_Test()
    {
        var fileName1 = new VariableInfoFileName("Foo.bar");
        var fileName2 = new VariableInfoFileName("Foo.bar");

        EquatableStructAssert.Equal(fileName1, fileName2);
        EquatableStructAssert.ObjectEqualsMethod(false, fileName1, int.MinValue);
    }

    [Fact]
    public void Equals_DefaultValueTest()
    {
        var fileName1 = new VariableInfoFileName("Foo.bar");

        EquatableStructAssert.NotEqual(fileName1, default);
    }

    [Fact]
    public void Equals_ObjectTest()
    {
        var fileName1 = new VariableInfoFileName("Foo.bar");
        object nonFileNameValue = 56;

        EquatableStructAssert.ObjectEqualsMethod(false, fileName1, nonFileNameValue);
    }

    [Fact]
    public void DefaultValueImplicitStringOperatorReturnsEmptyTest()
    {
        VariableInfoFileName fileName = default;

        Assert.Equal(string.Empty, fileName);
    }
}
