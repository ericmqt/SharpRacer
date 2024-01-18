using SharpRacer.SourceGenerators.TelemetryVariables.Diagnostics;
using SharpRacer.SourceGenerators.Testing;
using SharpRacer.SourceGenerators.Testing.Text;

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
    public void Ctor_ThrowOnNullArgsTest()
    {
        var fileName = new VariableInfoFileName("Foo.bar");
        var additionalText = new AdditionalTextFile("Foo.bar", "Hello, world!");
        var sourceText = additionalText.GetText()!;

        Assert.Throws<ArgumentNullException>(() => new VariableInfoFile(fileName, null!, sourceText));
        Assert.Throws<ArgumentNullException>(() => new VariableInfoFile(fileName, additionalText, null!));
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

    [Theory]
    [MemberData(nameof(GetInequalityData))]
    public void Equals_InequalityTest(VariableInfoFile file1, VariableInfoFile file2)
    {
        EquatableStructAssert.NotEqual(file1, file2);
    }

    [Fact]
    public void EqualsObject_WrongObjectTypeTest()
    {
        var fileName = new VariableInfoFileName("Foo.bar");
        var additionalText = new AdditionalTextFile("Foo.bar", "Hello, world!");
        var sourceText = additionalText.GetText()!;

        var variableInfoFile = new VariableInfoFile(fileName, additionalText, sourceText);

        EquatableStructAssert.ObjectEqualsMethod(false, variableInfoFile, sourceText);
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
    public void Read_FileReadExceptionDiagnosticOnSourceTextReturnNullStringTest()
    {
        var fileName = new VariableInfoFileName("Foo.bar");
        var additionalText = new AdditionalTextFile("Foo.bar", "Hello, world!");
        var sourceText = new NullSourceText();

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

    public static TheoryData<VariableInfoFile, VariableInfoFile> GetInequalityData()
    {
        // IncludedVariablesFile only determines equality via the FileName and File properties

        var additionalText1 = new AdditionalTextFile("Variables1.json", "[ \"Test\", \"Test2\" ]");
        var sourceText1 = additionalText1.GetText()!;

        var additionalText2 = new AdditionalTextFile("Variables2.json", "[ \"Test\", \"Test2\" ]");
        var sourceText2 = additionalText1.GetText()!;

        var fileName1 = new VariableInfoFileName("Variables1.json");
        var fileName2 = new VariableInfoFileName("Variables2.json");

        return new TheoryData<VariableInfoFile, VariableInfoFile>()
        {
            // FileName
            {
                new VariableInfoFile(fileName1, additionalText1, sourceText1),
                new VariableInfoFile(fileName2, additionalText1, sourceText1)
            },

            // File
            {
                new VariableInfoFile(fileName1, additionalText1, sourceText1),
                new VariableInfoFile(fileName1, additionalText2, sourceText2)
            }
        };
    }
}
