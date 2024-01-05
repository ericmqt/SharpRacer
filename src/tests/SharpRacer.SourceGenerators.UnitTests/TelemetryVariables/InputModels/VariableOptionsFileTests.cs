using SharpRacer.SourceGenerators.TelemetryVariables.Diagnostics;
using SharpRacer.SourceGenerators.Testing;

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
    public void Equals_Test()
    {
        var fileName = new VariableOptionsFileName("Foo.bar");
        var additionalText = new AdditionalTextFile("Foo.bar", "Hello, world!");
        var sourceText = additionalText.GetText()!;

        var variableOptionsFile1 = new VariableOptionsFile(fileName, additionalText, sourceText);
        var variableOptionsFile2 = new VariableOptionsFile(fileName, additionalText, sourceText);

        Assert.True(variableOptionsFile1 == variableOptionsFile2);
        Assert.False(variableOptionsFile1 != variableOptionsFile2);
        Assert.True(variableOptionsFile1.Equals(variableOptionsFile2));
        Assert.Equal(variableOptionsFile1.GetHashCode(), variableOptionsFile2.GetHashCode());
    }

    [Fact]
    public void Equals_DefaultValueTest()
    {
        var fileName = new VariableOptionsFileName("Foo.bar");
        var additionalText = new AdditionalTextFile("Foo.bar", "Hello, world!");
        var sourceText = additionalText.GetText()!;

        var variableOptionsFile = new VariableOptionsFile(fileName, additionalText, sourceText);

        Assert.False(variableOptionsFile == default);
        Assert.True(variableOptionsFile != default);
        Assert.False(variableOptionsFile.Equals(default));
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
}
