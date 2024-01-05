using Microsoft.CodeAnalysis.Text;
using SharpRacer.SourceGenerators.TelemetryVariables.Diagnostics;
using SharpRacer.SourceGenerators.Testing;

namespace SharpRacer.SourceGenerators.TelemetryVariables.InputModels;
public class IncludedVariableNameFactoryTests
{
    [Fact]
    public void Ctor_Test()
    {
        var additionalText = new AdditionalTextFile("IncludedVariables.json", "[ \"Test\", \"Test2\" ]");

        var includedVariablesFile = new IncludedVariablesFile(
            new IncludedVariablesFileName("IncludedVariables.json"),
            additionalText,
            additionalText.GetText()!);

        var factory = new IncludedVariableNameFactory(includedVariablesFile);

        Assert.NotNull(factory);
    }

    [Fact]
    public void Add_Test()
    {
        var additionalText = new AdditionalTextFile("IncludedVariables.json", "[ \"Test\", \"Test2\" ]");

        var includedVariablesFile = new IncludedVariablesFile(
            new IncludedVariablesFileName("IncludedVariables.json"),
            additionalText,
            additionalText.GetText()!);

        var factory = new IncludedVariableNameFactory(includedVariablesFile);

        factory.Add(new IncludedVariableNameValue("Test", new TextSpan(4, 4)));

        var includedNames = factory.Build();

        Assert.Single(includedNames);
        Assert.Equal("Test", includedNames.First().Value);
    }

    [Fact]
    public void Add_EmptyNameDiagnosticTest()
    {
        var additionalText = new AdditionalTextFile("IncludedVariables.json", "[ \"Test\", \"Test2\" ]");

        var includedVariablesFile = new IncludedVariablesFile(
            new IncludedVariablesFileName("IncludedVariables.json"),
            additionalText,
            additionalText.GetText()!);

        var factory = new IncludedVariableNameFactory(includedVariablesFile);

        factory.Add(new IncludedVariableNameValue(string.Empty, new TextSpan(4, 4)));

        var includedNames = factory.Build();
        Assert.Single(includedNames);

        // Check we got the empty name warning diagnostic
        Assert.False(includedNames.First().Diagnostics.HasErrors());
        var diagnostic = includedNames.First().Diagnostics.First();
        Assert.Equal(DiagnosticIds.IncludedVariablesFile_ContainsEmptyVariableName, diagnostic.Id);
    }

    [Fact]
    public void Add_VariableAlreadyIncludedDiagnosticTest()
    {
        var additionalText = new AdditionalTextFile("IncludedVariables.json", "[ \"Test\", \"Test2\" ]");

        var includedVariablesFile = new IncludedVariablesFile(
            new IncludedVariablesFileName("IncludedVariables.json"),
            additionalText,
            additionalText.GetText()!);

        var factory = new IncludedVariableNameFactory(includedVariablesFile);

        factory.Add(new IncludedVariableNameValue("Test", new TextSpan(4, 4)));
        factory.Add(new IncludedVariableNameValue("Test", new TextSpan(4, 4)));

        var includedNames = factory.Build();
        Assert.Equal(2, includedNames.Length);

        var duplicatedName = includedNames.Last();
        Assert.NotEmpty(duplicatedName.Diagnostics);

        var diagnostic = duplicatedName.Diagnostics.First();
        Assert.Equal(DiagnosticIds.IncludedVariablesFile_VariableAlreadyIncluded, diagnostic.Id);
    }
}
