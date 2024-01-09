using SharpRacer.SourceGenerators.TelemetryVariables.Diagnostics;
using SharpRacer.SourceGenerators.Testing;

namespace SharpRacer.SourceGenerators.TelemetryVariables.InputModels;
public class VariableInfoFileTests
{
    [Fact]
    public void Ctor_Test()
    {
        var fileName = new VariableInfoFileName("Foo.bar");
        var additionalText = new AdditionalTextFile("Foo.bar", "Hello, world!");
        var sourceText = additionalText.GetText()!;

        var variableInfoFile = new VariableInfoFile(fileName, additionalText, sourceText);

        Assert.Equal(fileName, variableInfoFile.FileName);
        Assert.Equal(additionalText, variableInfoFile.File);
        Assert.Equal(sourceText, variableInfoFile.SourceText);
    }

    [Fact]
    public void Equals_Test()
    {
        var fileName = new VariableInfoFileName("Foo.bar");
        var additionalText = new AdditionalTextFile("Foo.bar", "Hello, world!");
        var sourceText = additionalText.GetText()!;

        var variableInfoFile1 = new VariableInfoFile(fileName, additionalText, sourceText);
        var variableInfoFile2 = new VariableInfoFile(fileName, additionalText, sourceText);

        EquatableStructAssert.Equal(variableInfoFile1, variableInfoFile2);
    }

    [Fact]
    public void Equals_DefaultValueTest()
    {
        var fileName = new VariableInfoFileName("Foo.bar");
        var additionalText = new AdditionalTextFile("Foo.bar", "Hello, world!");
        var sourceText = additionalText.GetText()!;

        var variableInfoFile = new VariableInfoFile(fileName, additionalText, sourceText);

        EquatableStructAssert.NotEqual(variableInfoFile, default);
    }

    [Fact]
    public void Read_Test()
    {
        var fileName = new VariableInfoFileName("Foo.bar");
        var additionalText = new AdditionalTextFile(
            "Foo.bar",
            @"[{
  ""DeprecatedBy"": null,
  ""Description"": ""Seconds since session start"",
  ""IsDeprecated"": false,
  ""IsTimeSliceArray"": false,
  ""Name"": ""SessionTime"",
  ""ValueCount"": 1,
  ""ValueType"": ""Double"",
  ""ValueUnit"": ""s""
}]");
        var sourceText = additionalText.GetText()!;

        var variableInfoFile = new VariableInfoFile(fileName, additionalText, sourceText);

        var result = variableInfoFile.Read(default, out var diagnostic);

        Assert.False(result.IsDefault);
        Assert.False(result.IsEmpty);
        Assert.Null(diagnostic);
        Assert.Single(result);
        Assert.Equal("SessionTime", result.First().Name);
    }

    [Fact]
    public void Read_FileReadExceptionDiagnosticOnInvalidJsonTest()
    {
        var fileName = new VariableInfoFileName("Foo.bar");
        var additionalText = new AdditionalTextFile("Foo.bar", "Hello, world!");
        var sourceText = additionalText.GetText()!;

        var variableInfoFile = new VariableInfoFile(fileName, additionalText, sourceText);

        var result = variableInfoFile.Read(default, out var diagnostic);

        Assert.False(result.IsDefault);
        Assert.True(result.IsEmpty);
        Assert.NotNull(diagnostic);
        Assert.Equal(DiagnosticIds.AdditionalText_FileReadException, diagnostic.Id);
    }

    [Fact]
    public void Read_EmptyCollectionAndDiagnosticWarningOnNoVariablesTest()
    {
        var fileName = new VariableInfoFileName("Foo.bar");
        var additionalText = new AdditionalTextFile("Foo.bar", "[]");
        var sourceText = additionalText.GetText()!;

        var variableInfoFile = new VariableInfoFile(fileName, additionalText, sourceText);
        var result = variableInfoFile.Read(default, out var diagnostic);

        Assert.False(result.IsDefault);
        Assert.True(result.IsEmpty);
        Assert.NotNull(diagnostic);
        Assert.Equal(DiagnosticIds.TelemetryVariablesFileContainsNoVariables, diagnostic.Id);
    }
}
