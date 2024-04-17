using SharpRacer.SourceGenerators.Testing;

namespace SharpRacer.SourceGenerators.TelemetryVariables.InputModels;
public class IncludedVariablesFileNameTests
{
    [Fact]
    public void Ctor_Test()
    {
        var fileName = new IncludedVariablesFileName("Foo.bar");

        string fileNameValue = fileName;

        Assert.Equal(fileNameValue, fileName, StringComparer.Ordinal);
    }

    [Fact]
    public void Ctor_ThrowOnNullOrEmptyFileNameTest()
    {
        Assert.Throws<ArgumentException>(() => new IncludedVariablesFileName(null!));
        Assert.Throws<ArgumentException>(() => new IncludedVariablesFileName(string.Empty));
    }

    [Fact]
    public void CreateOrGetDefault_Test()
    {
        var fileNameString = "test.json";
        var includedFileName = IncludedVariablesFileName.CreateOrGetDefault(fileNameString);

        Assert.NotEqual(default, includedFileName);
        Assert.Equal(fileNameString, includedFileName);
    }

    [Fact]
    public void CreateOrGetDefault_ReturnDefaultOnNullOrEmptyArgTest()
    {
        var includedFileName = IncludedVariablesFileName.CreateOrGetDefault(null);

        Assert.Equal(default, includedFileName);
        Assert.Equal(string.Empty, includedFileName);

        includedFileName = IncludedVariablesFileName.CreateOrGetDefault(string.Empty);

        Assert.Equal(default, includedFileName);
        Assert.Equal(string.Empty, includedFileName);
    }

    [Fact]
    public void IsMatch_Test()
    {
        var matchingFile = new AdditionalTextFile("Foo.bar", "Hello, world!");
        var matchingFileWithDirectory = new AdditionalTextFile("src/tests/Foo.bar", "Hello, world!");
        var nonMatchingFile = new AdditionalTextFile("Bar.baz", "Goodbye, world!");
        var nonMatchingFileWithDirectory = new AdditionalTextFile("src/tests/Bar.baz", "Goodbye, world!");

        var fileName = new IncludedVariablesFileName("Foo.bar");

        Assert.True(fileName.IsMatch(matchingFile));
        Assert.True(fileName.IsMatch(matchingFileWithDirectory));
        Assert.False(fileName.IsMatch(nonMatchingFile));
        Assert.False(fileName.IsMatch(nonMatchingFileWithDirectory));
    }

    [Fact]
    public void IsMatch_DefaultValueTest()
    {
        var matchingFile = new AdditionalTextFile("Foo.bar", "Hello, world!");

        var fileName = default(IncludedVariablesFileName);

        Assert.False(fileName.IsMatch(matchingFile));
    }

    [Fact]
    public void Equals_Test()
    {
        var fileName1 = new IncludedVariablesFileName("Foo.bar");
        var fileName2 = new IncludedVariablesFileName("Foo.bar");

        EquatableStructAssert.Equal(fileName1, fileName2);
    }

    [Fact]
    public void Equals_DefaultValueTest()
    {
        var fileName1 = new IncludedVariablesFileName("Foo.bar");

        EquatableStructAssert.NotEqual(fileName1, default);
    }

    [Fact]
    public void Equals_InequalityTest()
    {
        var fileName1 = new IncludedVariablesFileName("Foo1.bar");
        var fileName2 = new IncludedVariablesFileName("Foo2.bar");

        EquatableStructAssert.NotEqual(fileName1, fileName2);
    }

    [Fact]
    public void EqualsObject_WrongObjectTypeTest()
    {
        var fileName1 = new IncludedVariablesFileName("Foo.bar");

        EquatableStructAssert.ObjectEqualsMethod(false, fileName1, int.MaxValue);
    }

    [Fact]
    public void ImplicitConversionOperator_String_Test()
    {
        var fileNameValue = "Foo.bar";

        var includedVariablesFileName = new IncludedVariablesFileName(fileNameValue);
        string fileNameString = includedVariablesFileName;

        Assert.Equal(fileNameValue, fileNameString);
    }

    [Fact]
    public void ImplicitConversionOperator_String_DefaultValueReturnsEmptyTest()
    {
        IncludedVariablesFileName fileName = default;

        Assert.Equal(string.Empty, fileName);
    }
}
