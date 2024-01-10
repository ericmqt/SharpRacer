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
    public void TryAdd_Test()
    {
        var additionalText = new AdditionalTextFile("IncludedVariables.json", "[ \"Test\", \"Test2\" ]");

        var includedVariablesFile = new IncludedVariablesFile(
            new IncludedVariablesFileName("IncludedVariables.json"),
            additionalText,
            additionalText.GetText()!);

        var factory = new IncludedVariableNameFactory(includedVariablesFile);

        var result = factory.TryAdd(new IncludedVariableNameValue("Test", new TextSpan(4, 4)), out var diagnostics);
        Assert.True(result);

        var includedNames = factory.Build();

        Assert.Single(includedNames);
        Assert.Equal("Test", includedNames.First().Value);
        Assert.False(diagnostics.IsDefault);
        Assert.Empty(diagnostics);
    }

    [Fact]
    public void TryAdd_EmptyNameDiagnosticTest()
    {
        var additionalText = new AdditionalTextFile("IncludedVariables.json", "[ \"Test\", \"Test2\" ]");

        var includedVariablesFile = new IncludedVariablesFile(
            new IncludedVariablesFileName("IncludedVariables.json"),
            additionalText,
            additionalText.GetText()!);

        var factory = new IncludedVariableNameFactory(includedVariablesFile);

        Assert.False(factory.TryAdd(new IncludedVariableNameValue(string.Empty, new TextSpan(4, 4)), out var diagnostics));

        Assert.Single(diagnostics);
        Assert.All(diagnostics, x => Assert.False(x.IsError()));
        Assert.Single(diagnostics, x => x.Id == DiagnosticIds.IncludedVariablesFile_ContainsEmptyVariableName);

        var includedNames = factory.Build();
        Assert.Empty(includedNames);
    }

    [Fact]
    public void TryAdd_VariableAlreadyIncludedDiagnosticTest()
    {
        var additionalText = new AdditionalTextFile("IncludedVariables.json", "[ \"Test\", \"Test2\" ]");

        var includedVariablesFile = new IncludedVariablesFile(
            new IncludedVariablesFileName("IncludedVariables.json"),
            additionalText,
            additionalText.GetText()!);

        var factory = new IncludedVariableNameFactory(includedVariablesFile);

        Assert.True(factory.TryAdd(new IncludedVariableNameValue("Test", new TextSpan(4, 4)), out var diagnostics1));
        Assert.False(factory.TryAdd(new IncludedVariableNameValue("Test", new TextSpan(4, 4)), out var diagnostics2));

        Assert.False(diagnostics1.IsDefault);
        Assert.Empty(diagnostics1);

        Assert.False(diagnostics2.IsDefault);
        Assert.Single(diagnostics2);
        Assert.Single(diagnostics2, x => x.Id == DiagnosticIds.IncludedVariablesFile_VariableAlreadyIncluded);

        var includedNames = factory.Build();
        Assert.Single(includedNames);
    }
}
