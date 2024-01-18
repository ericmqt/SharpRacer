using SharpRacer.SourceGenerators.TelemetryVariables.Diagnostics;
using SharpRacer.SourceGenerators.Testing;
using SharpRacer.SourceGenerators.Testing.Text;

namespace SharpRacer.SourceGenerators.TelemetryVariables.InputModels;
public class IncludedVariablesFileTests
{
    [Fact]
    public void Ctor_Test()
    {
        var additionalText = new AdditionalTextFile("IncludedVariables.json", "[ \"Test\", \"Test2\" ]");
        var sourceText = additionalText.GetText();
        var fileName = new IncludedVariablesFileName("IncludedVariables.json");

        var includedVariablesFile = new IncludedVariablesFile(
            fileName,
            additionalText,
            sourceText!);

        Assert.Equal(fileName, includedVariablesFile.FileName);
        Assert.Equal(additionalText, includedVariablesFile.File);
        Assert.Equal(sourceText, includedVariablesFile.SourceText);
    }

    [Fact]
    public void Ctor_ThrowOnNullAdditionalTextOrSourceTextTest()
    {
        var additionalText = new AdditionalTextFile("IncludedVariables.json", "[ \"Test\", \"Test2\" ]");
        var sourceText = additionalText.GetText()!;

        Assert.Throws<ArgumentNullException>(() => new IncludedVariablesFile(default, null!, sourceText));
        Assert.Throws<ArgumentNullException>(() => new IncludedVariablesFile(default, additionalText, null!));
    }

    [Fact]
    public void Equals_Test()
    {
        var additionalText = new AdditionalTextFile("IncludedVariables.json", "[ \"Test\", \"Test2\" ]");
        var sourceText = additionalText.GetText();
        var fileName = new IncludedVariablesFileName("IncludedVariables.json");

        var includedVariablesFile1 = new IncludedVariablesFile(
            fileName,
            additionalText,
            sourceText!);

        var includedVariablesFile2 = new IncludedVariablesFile(
            fileName,
            additionalText,
            sourceText!);

        EquatableStructAssert.Equal(includedVariablesFile1, includedVariablesFile2);
    }

    [Fact]
    public void Equals_DefaultValueTest()
    {
        var additionalText = new AdditionalTextFile("IncludedVariables.json", "[ \"Test\", \"Test2\" ]");
        var sourceText = additionalText.GetText();
        var fileName = new IncludedVariablesFileName("IncludedVariables.json");

        var includedVariablesFile1 = new IncludedVariablesFile(
            fileName,
            additionalText,
            sourceText!);

        EquatableStructAssert.NotEqual(includedVariablesFile1, default);
    }

    [Theory]
    [MemberData(nameof(GetInequalityData))]
    public void Equals_InequalityTest(IncludedVariablesFile file1, IncludedVariablesFile file2)
    {
        EquatableStructAssert.NotEqual(file1, file2);
    }

    [Fact]
    public void EqualsObject_WrongObjectTypeTest()
    {
        var additionalText = new AdditionalTextFile("IncludedVariables.json", "[ \"Test\", \"Test2\" ]");
        var sourceText = additionalText.GetText();
        var fileName = new IncludedVariablesFileName("IncludedVariables.json");

        var includedVariablesFile1 = new IncludedVariablesFile(
            fileName,
            additionalText,
            sourceText!);

        EquatableStructAssert.ObjectEqualsMethod(false, includedVariablesFile1, int.MaxValue);
    }

    [Fact]
    public void ReadJson_Test()
    {
        var additionalText = new AdditionalTextFile("IncludedVariables.json", "[ \"Test\", \"Test2\" ]");
        var sourceText = additionalText.GetText()!;
        var fileName = new IncludedVariablesFileName("IncludedVariables.json");

        var includedVariablesFile = new IncludedVariablesFile(
            fileName,
            additionalText,
            sourceText);

        var names = includedVariablesFile.ReadJson(default, out var readDiagnostic);

        Assert.Null(readDiagnostic);
        Assert.Equal(2, names.Length);
        Assert.Equal("Test", names.First().Value);
        Assert.Equal("Test2", names.Last().Value);
    }

    [Fact]
    public void ReadJson_DefaultTest()
    {
        var file = default(IncludedVariablesFile);

        var results = file.ReadJson(default, out var readDiagnostic);

        Assert.Empty(results);
        Assert.Null(readDiagnostic);
    }

    [Fact]
    public void ReadJson_NoIncludedVariableNamesWarningDiagnosticTest()
    {
        var additionalText = new AdditionalTextFile("IncludedVariables.json", "[ ]");
        var sourceText = additionalText.GetText()!;
        var fileName = new IncludedVariablesFileName("IncludedVariables.json");

        var includedVariablesFile = new IncludedVariablesFile(
            fileName,
            additionalText,
            sourceText);

        var names = includedVariablesFile.ReadJson(default, out var readDiagnostic);

        Assert.NotNull(readDiagnostic);
        Assert.Equal(DiagnosticIds.IncludedVariablesFileContainsNoVariableNames, readDiagnostic.Id);
        Assert.Empty(names);
    }

    [Fact]
    public void ReadJson_JsonExceptionDiagnosticTest()
    {
        var additionalText = new AdditionalTextFile("IncludedVariables.json", "!@#43!@35");
        var sourceText = additionalText.GetText()!;
        var fileName = new IncludedVariablesFileName("IncludedVariables.json");

        var includedVariablesFile = new IncludedVariablesFile(
            fileName,
            additionalText,
            sourceText);

        var names = includedVariablesFile.ReadJson(default, out var readDiagnostic);

        Assert.NotNull(readDiagnostic);
        Assert.Equal(DiagnosticIds.AdditionalText_FileReadException, readDiagnostic.Id);
        Assert.Empty(names);
    }

    [Fact]
    public void ReadJson_ExceptionDiagnosticTest()
    {
        var additionalText = new AdditionalTextFile("IncludedVariables.json", "!@#43!@35");
        var fileName = new IncludedVariablesFileName("IncludedVariables.json");

        var includedVariablesFile = new IncludedVariablesFile(
            fileName,
            additionalText,
            new NullSourceText());

        var names = includedVariablesFile.ReadJson(default, out var readDiagnostic);

        Assert.NotNull(readDiagnostic);
        Assert.Equal(DiagnosticIds.AdditionalText_FileReadException, readDiagnostic.Id);
        Assert.Empty(names);
    }

    public static TheoryData<IncludedVariablesFile, IncludedVariablesFile> GetInequalityData()
    {
        // IncludedVariablesFile only determines equality via the FileName and File properties

        var additionalText1 = new AdditionalTextFile("IncludedVariables1.json", "[ \"Test\", \"Test2\" ]");
        var sourceText1 = additionalText1.GetText()!;

        var additionalText2 = new AdditionalTextFile("IncludedVariables2.json", "[ \"Test\", \"Test2\" ]");
        var sourceText2 = additionalText1.GetText()!;

        var fileName1 = new IncludedVariablesFileName("IncludedVariables1.json");
        var fileName2 = new IncludedVariablesFileName("IncludedVariables2.json");

        return new TheoryData<IncludedVariablesFile, IncludedVariablesFile>()
        {
            // FileName
            {
                new IncludedVariablesFile(fileName1, additionalText1, sourceText1),
                new IncludedVariablesFile(fileName2, additionalText1, sourceText1)
            },

            // File
            {
                new IncludedVariablesFile(fileName1, additionalText1, sourceText1),
                new IncludedVariablesFile(fileName1, additionalText2, sourceText2)
            }
        };
    }
}
