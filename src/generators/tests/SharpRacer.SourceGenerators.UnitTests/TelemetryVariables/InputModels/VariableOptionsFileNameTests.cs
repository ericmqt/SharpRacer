using SharpRacer.SourceGenerators.Testing;

namespace SharpRacer.SourceGenerators.TelemetryVariables.InputModels;
public class VariableOptionsFileNameTests
{
    [Fact]
    public void Ctor_Test()
    {
        var fileName = new VariableOptionsFileName("Foo.bar");

        string fileNameValue = fileName;

        Assert.Equal(fileNameValue, fileName, StringComparer.Ordinal);
    }

    [Fact]
    public void Ctor_ThrowOnNullOrEmptyFileNameTest()
    {
        Assert.Throws<ArgumentException>(() => new VariableOptionsFileName(null!));
        Assert.Throws<ArgumentException>(() => new VariableOptionsFileName(string.Empty));
    }

    [Fact]
    public void IsMatch_Test()
    {
        var matchingFile = new AdditionalTextFile("Foo.bar", "Hello, world!");
        var matchingFileWithDirectory = new AdditionalTextFile("src/tests/Foo.bar", "Hello, world!");
        var nonMatchingFile = new AdditionalTextFile("Bar.baz", "Goodbye, world!");
        var nonMatchingFileWithDirectory = new AdditionalTextFile("src/tests/Bar.baz", "Goodbye, world!");

        var fileName = new VariableOptionsFileName("Foo.bar");

        Assert.True(fileName.IsMatch(matchingFile));
        Assert.True(fileName.IsMatch(matchingFileWithDirectory));
        Assert.False(fileName.IsMatch(nonMatchingFile));
        Assert.False(fileName.IsMatch(nonMatchingFileWithDirectory));
    }

    [Fact]
    public void IsMatch_DefaultValueReturnsFalseTest()
    {
        var file = new AdditionalTextFile("Bar.baz", "Goodbye, world!");

        var fileName = default(VariableOptionsFileName);

        Assert.False(fileName.IsMatch(file));
    }

    [Fact]
    public void Equals_Test()
    {
        var fileName1 = new VariableOptionsFileName("Foo.bar");
        var fileName2 = new VariableOptionsFileName("Foo.bar");

        EquatableStructAssert.Equal(fileName1, fileName2);
    }

    [Fact]
    public void Equals_DefaultValueTest()
    {
        var fileName1 = new VariableOptionsFileName("Foo.bar");

        EquatableStructAssert.NotEqual(fileName1, default);
    }

    [Fact]
    public void Equals_InequalityTest()
    {
        var fileName1 = new VariableOptionsFileName("Foo.bar");
        var fileName2 = new VariableOptionsFileName("Bar.foo");

        EquatableStructAssert.NotEqual(fileName1, fileName2);
    }

    [Fact]
    public void EqualsObject_WrongObjectTypeTest()
    {
        var fileName1 = new VariableOptionsFileName("Foo.bar");
        object nonFileNameValue = 56;

        EquatableStructAssert.ObjectEqualsMethod(false, fileName1, nonFileNameValue);
    }

    [Fact]
    public void ToString_Test()
    {
        var fileNameString = "Foo.bar";
        var optionsFileName = new VariableOptionsFileName(fileNameString);

        Assert.Equal(fileNameString, optionsFileName.ToString());
    }

    [Fact]
    public void ToString_DefaultValueTest()
    {
        Assert.Equal(string.Empty, default(VariableOptionsFileName).ToString());
    }

    [Fact]
    public void ImplicitConversionOperator_String_Test()
    {
        var fileNameValue = "Foo.bar";

        var variableOptionsFileName = new VariableOptionsFileName(fileNameValue);
        string fileNameString = variableOptionsFileName;

        Assert.Equal(fileNameValue, fileNameString);
    }

    [Fact]
    public void ImplicitConversionOperator_String_DefaultValueReturnsEmptyTest()
    {
        VariableOptionsFileName fileName = default;

        Assert.Equal(string.Empty, fileName);
    }
}
