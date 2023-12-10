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

        Assert.True(fileName1.Equals(fileName2));
        Assert.True(fileName1 == fileName2);
        Assert.False(fileName1 != fileName2);
        Assert.Equal(fileName1.GetHashCode(), fileName2.GetHashCode());
    }

    [Fact]
    public void Equals_DefaultValueTest()
    {
        var fileName1 = new VariableInfoFileName("Foo.bar");

        Assert.False(fileName1.Equals(default));
        Assert.False(fileName1 == default);
        Assert.True(fileName1 != default);
        Assert.NotEqual(fileName1.GetHashCode(), default(VariableInfoFileName).GetHashCode());

        Assert.False(default(VariableInfoFileName).Equals(fileName1));
        Assert.False(default == fileName1);
        Assert.True(default != fileName1);
    }

    [Fact]
    public void Equals_ObjectTest()
    {
        var fileName1 = new VariableInfoFileName("Foo.bar");
        object fileName2 = new VariableInfoFileName("Foo.bar");
        object nonFileNameValue = 56;

        Assert.True(fileName1.Equals(fileName2));
        Assert.False(fileName1.Equals(nonFileNameValue));
    }

    [Fact]
    public void DefaultValueImplicitStringOperatorReturnsEmptyTest()
    {
        VariableInfoFileName fileName = default;

        Assert.Equal(string.Empty, fileName);
    }
}
