using SharpRacer.SourceGenerators.TelemetryVariables.Diagnostics;
using SharpRacer.SourceGenerators.Testing;
using SharpRacer.SourceGenerators.Testing.Text;

namespace SharpRacer.SourceGenerators.TelemetryVariables.InputModels;
public class VariableOptionsFileTests
{
    [Fact]
    public void Ctor_Test()
    {
        var fileName = new VariableOptionsFileName("Foo.bar");
        var additionalText = new AdditionalTextFile("Foo.bar", "Hello, world!");
        var sourceText = additionalText.GetText()!;

        var variableOptionsFile = new VariableOptionsFile(fileName, additionalText, sourceText);

        Assert.Equal(fileName, variableOptionsFile.FileName);
        Assert.Equal(additionalText, variableOptionsFile.File);
        Assert.Equal(sourceText, variableOptionsFile.SourceText);
    }

    [Fact]
    public void Ctor_ThrowOnNullArgsTest()
    {
        var fileName = new VariableOptionsFileName("Foo.bar");
        var additionalText = new AdditionalTextFile("Foo.bar", "Hello, world!");
        var sourceText = additionalText.GetText()!;

        Assert.Throws<ArgumentNullException>(() => new VariableOptionsFile(fileName, null!, sourceText));
        Assert.Throws<ArgumentNullException>(() => new VariableOptionsFile(fileName, additionalText, null!));
    }

    [Fact]
    public void Equals_Test()
    {
        var fileName = new VariableOptionsFileName("Foo.bar");
        var additionalText = new AdditionalTextFile("Foo.bar", "Hello, world!");
        var sourceText = additionalText.GetText()!;

        var variableOptionsFile1 = new VariableOptionsFile(fileName, additionalText, sourceText);
        var variableOptionsFile2 = new VariableOptionsFile(fileName, additionalText, sourceText);

        EquatableStructAssert.Equal(variableOptionsFile1, variableOptionsFile2);
    }

    [Fact]
    public void Equals_DefaultValueTest()
    {
        var fileName = new VariableOptionsFileName("Foo.bar");
        var additionalText = new AdditionalTextFile("Foo.bar", "Hello, world!");
        var sourceText = additionalText.GetText()!;

        var variableOptionsFile = new VariableOptionsFile(fileName, additionalText, sourceText);

        EquatableStructAssert.NotEqual(variableOptionsFile, default);
    }

    [Theory]
    [MemberData(nameof(GetInequalityData))]
    public void Equals_InequalityTest(VariableOptionsFile file1, VariableOptionsFile file2)
    {
        EquatableStructAssert.NotEqual(file1, file2);
    }

    [Fact]
    public void EqualsObject_WrongObjectTypeTest()
    {
        var fileName = new VariableOptionsFileName("Foo.bar");
        var additionalText = new AdditionalTextFile("Foo.bar", "Hello, world!");
        var sourceText = additionalText.GetText()!;

        var variableOptionsFile = new VariableOptionsFile(fileName, additionalText, sourceText);

        EquatableStructAssert.ObjectEqualsMethod(false, variableOptionsFile, additionalText);
    }

    [Fact]
    public void Read_Test()
    {
        var fileName = new VariableOptionsFileName("Foo.bar");
        var additionalText = new AdditionalTextFile(
            "Foo.bar",
            @"{""ChanLatency"": {
    ""ContextPropertyName"": ""ChannelLatency"",
    ""DescriptorName"": ""ChannelLatency"",
    ""Name"": ""ChannelLatency""
  }}");
        var sourceText = additionalText.GetText()!;

        var variableOptionsFile = new VariableOptionsFile(fileName, additionalText, sourceText);
        var result = variableOptionsFile.Read(default, out var diagnostic);

        Assert.False(result.IsDefault);
        Assert.False(result.IsEmpty);
        Assert.Null(diagnostic);
        Assert.Single(result);
        Assert.Equal("ChanLatency", result.First().Key);
        Assert.Equal("ChannelLatency", result.First().Value.Name);
    }

    [Fact]
    public void Read_FileReadExceptionDiagnosticOnInvalidJsonTest()
    {
        var fileName = new VariableOptionsFileName("Foo.bar");
        var additionalText = new AdditionalTextFile("Foo.bar", "Hello, world!");
        var sourceText = additionalText.GetText()!;

        var variableOptionsFile = new VariableOptionsFile(fileName, additionalText, sourceText);

        var result = variableOptionsFile.Read(default, out var diagnostic);

        Assert.False(result.IsDefault);
        Assert.True(result.IsEmpty);
        Assert.NotNull(diagnostic);
        Assert.Equal(DiagnosticIds.AdditionalText_FileReadException, diagnostic.Id);
    }

    [Fact]
    public void Read_FileReadExceptionDiagnosticOnSourceTextReturnNullStringTest()
    {
        var fileName = new VariableOptionsFileName("Foo.bar");
        var additionalText = new AdditionalTextFile("Foo.bar", "Hello, world!");
        var sourceText = new NullSourceText();

        var variableOptionsFile = new VariableOptionsFile(fileName, additionalText, sourceText);

        var result = variableOptionsFile.Read(default, out var diagnostic);

        Assert.False(result.IsDefault);
        Assert.True(result.IsEmpty);
        Assert.NotNull(diagnostic);
        Assert.Equal(DiagnosticIds.AdditionalText_FileReadException, diagnostic.Id);
    }

    public static TheoryData<VariableOptionsFile, VariableOptionsFile> GetInequalityData()
    {
        // IncludedVariablesFile only determines equality via the FileName and File properties

        var additionalText1 = new AdditionalTextFile("Options1.json", "[ \"Test\", \"Test2\" ]");
        var sourceText1 = additionalText1.GetText()!;

        var additionalText2 = new AdditionalTextFile("Options2.json", "[ \"Test\", \"Test2\" ]");
        var sourceText2 = additionalText1.GetText()!;

        var fileName1 = new VariableOptionsFileName("Options1.json");
        var fileName2 = new VariableOptionsFileName("Options2.json");

        return new TheoryData<VariableOptionsFile, VariableOptionsFile>()
        {
            // FileName
            {
                new VariableOptionsFile(fileName1, additionalText1, sourceText1),
                new VariableOptionsFile(fileName2, additionalText1, sourceText1)
            },

            // File
            {
                new VariableOptionsFile(fileName1, additionalText1, sourceText1),
                new VariableOptionsFile(fileName1, additionalText2, sourceText2)
            }
        };
    }
}
