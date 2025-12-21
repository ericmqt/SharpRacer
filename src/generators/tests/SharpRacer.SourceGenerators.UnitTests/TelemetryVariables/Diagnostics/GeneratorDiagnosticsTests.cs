using System.Text.Json;
using SharpRacer.SourceGenerators.Testing;
using SharpRacer.SourceGenerators.Testing.Text;

namespace SharpRacer.SourceGenerators.TelemetryVariables.Diagnostics;
public class GeneratorDiagnosticsTests
{
    [Fact]
    public void AdditionalTextContentReadError_Test()
    {
        var file = new AdditionalTextFile("test.json", new JsonSourceText("{}"));
        var diagnostic = GeneratorDiagnostics.AdditionalTextContentReadError(file, location: null);

        Assert.Equal(DiagnosticIds.AdditionalText_ContentReadError, diagnostic.Id);
    }

    [Fact]
    public void AdditionalTextFileReadException_Test()
    {
        var file = new AdditionalTextFile("test.json", new JsonSourceText("{}"));
        var diagnostic = GeneratorDiagnostics.AdditionalTextFileReadException(file, new JsonException("test"), location: null);

        Assert.Equal(DiagnosticIds.AdditionalText_FileReadException, diagnostic.Id);
    }

    [Fact]
    public void AmbiguousIncludedVariablesFileName_Test()
    {
        var diagnostic = GeneratorDiagnostics.AmbiguousIncludedVariablesFileName("test.json");

        Assert.Equal(DiagnosticIds.IncludedVariables_AmbiguousFileName, diagnostic.Id);
    }

    [Fact]
    public void AmbiguousTelemetryVariablesFileName_Test()
    {
        var diagnostic = GeneratorDiagnostics.AmbiguousTelemetryVariablesFileName("test.json");

        Assert.Equal(DiagnosticIds.TelemetryVariablesFile_AmbiguousFileName, diagnostic.Id);
    }

    [Fact]
    public void AmbiguousVariableOptionsFileName_Test()
    {
        var diagnostic = GeneratorDiagnostics.AmbiguousVariableOptionsFileName("test.json");

        Assert.Equal(DiagnosticIds.VariableOptionsFile_AmbiguousFileName, diagnostic.Id);
    }

    [Fact]
    public void ContextClassConfiguredPropertyNameConflict_Test()
    {
        var diagnostic = GeneratorDiagnostics.ContextClassConfiguredPropertyNameConflict(
            "TestContext",
            "Test",
            "TestProperty",
            "TestEx",
            "TestProperty");

        Assert.Equal(DiagnosticIds.ContextClassConfiguredPropertyNameConflict, diagnostic.Id);
    }

    [Fact]
    public void ContextClassIncludedVariableNotFound_Test()
    {
        var diagnostic = GeneratorDiagnostics.ContextClassIncludedVariableNotFound("MyContext", "TestEx");

        Assert.Equal(DiagnosticIds.ContextClassIncludedVariableNotFound, diagnostic.Id);
    }

    [Fact]
    public void ContextClassMustBeDeclaredPartial_Test()
    {
        var diagnostic = GeneratorDiagnostics.ContextClassMustBeDeclaredPartial("MyContext");

        Assert.Equal(DiagnosticIds.ContextClassMustBeDeclaredPartial, diagnostic.Id);
    }

    [Fact]
    public void ContextClassMustInheritIDataVariablesContextInterface_Test()
    {
        var diagnostic = GeneratorDiagnostics.ContextClassMustInheritIDataVariablesContextInterface("MyContext");

        Assert.Equal(DiagnosticIds.ContextClassMustInheritITelemetryVariablesContextInterface, diagnostic.Id);
    }

    [Fact]
    public void ContextClassMustNotBeDeclaredStatic_Test()
    {
        var diagnostic = GeneratorDiagnostics.ContextClassMustNotBeDeclaredStatic("MyContext");

        Assert.Equal(DiagnosticIds.ContextClassMustNotBeDeclaredStatic, diagnostic.Id);
    }

    [Fact]
    public void ContextClassVariableNameCreatesPropertyNameConflict_Test()
    {
        var diagnostic = GeneratorDiagnostics.ContextClassVariableNameCreatesPropertyNameConflict(
            "TestContext", "Test", "TextEx", "TextProperty");

        Assert.Equal(DiagnosticIds.ContextClassVariableNameCreatesPropertyNameConflict, diagnostic.Id);
    }

    [Fact]
    public void DeprecatingVariableNotFound_Test()
    {
        var diagnostic = GeneratorDiagnostics.DeprecatingVariableNotFound("Test", "TestEx");

        Assert.Equal(DiagnosticIds.TelemetryVariablesDeprecatingVariableNotFound, diagnostic.Id);
    }

    [Fact]
    public void DescriptorClassAlreadyExistsInAssembly_Test()
    {
        var diagnostic = GeneratorDiagnostics.DescriptorClassAlreadyExistsInAssembly("TestDescriptors2", "TestDescriptors");

        Assert.Equal(DiagnosticIds.DescriptorClassAlreadyExistsInAssembly, diagnostic.Id);
    }

    [Fact]
    public void DescriptorClassMustBeDeclaredPartial_Test()
    {
        var diagnostic = GeneratorDiagnostics.DescriptorClassMustBeDeclaredPartial("MyDescriptors");

        Assert.Equal(DiagnosticIds.DescriptorClassMustBeDeclaredPartial, diagnostic.Id);
    }

    [Fact]
    public void DescriptorClassMustBeDeclaredStatic_Test()
    {
        var diagnostic = GeneratorDiagnostics.DescriptorClassMustBeDeclaredStatic("MyDescriptors");

        Assert.Equal(DiagnosticIds.DescriptorClassMustBeDeclaredStatic, diagnostic.Id);
    }

    [Fact]
    public void DescriptorNameConflictsWithExistingVariable_Test()
    {
        var diagnostic = GeneratorDiagnostics.DescriptorNameConflictsWithExistingVariable("Test", "Test", "TestEx");

        Assert.Equal(DiagnosticIds.DescriptorNameConflictsWithExistingVariable, diagnostic.Id);
    }

    [Fact]
    public void IncludedVariablesFileAlreadyIncludesVariable_Test()
    {
        var diagnostic = GeneratorDiagnostics.IncludedVariablesFileAlreadyIncludesVariable("Test", "test.json");

        Assert.Equal(DiagnosticIds.IncludedVariablesFile_VariableAlreadyIncluded, diagnostic.Id);
    }

    [Fact]
    public void IncludedVariablesFileContainsEmptyVariableName_Test()
    {
        var diagnostic = GeneratorDiagnostics.IncludedVariablesFileContainsEmptyVariableName();

        Assert.Equal(DiagnosticIds.IncludedVariablesFile_ContainsEmptyVariableName, diagnostic.Id);
    }

    [Fact]
    public void IncludedVariablesFileContainsNoVariables_Test()
    {
        var file = new AdditionalTextFile("test.json", new JsonSourceText("{}"));
        var diagnostic = GeneratorDiagnostics.IncludedVariablesFileContainsNoVariables(file);

        Assert.Equal(DiagnosticIds.IncludedVariablesFileContainsNoVariableNames, diagnostic.Id);
    }

    [Fact]
    public void IncludedVariablesFileNotFound_Test()
    {
        var diagnostic = GeneratorDiagnostics.IncludedVariablesFileNotFound("test.json");

        Assert.Equal(DiagnosticIds.IncludedVariables_FileNotFound, diagnostic.Id);
    }

    [Fact]
    public void TelemetryVariablesFileContainsNoVariables_Test()
    {
        var diagnostic = GeneratorDiagnostics.TelemetryVariablesFileContainsNoVariables("test.json");

        Assert.Equal(DiagnosticIds.TelemetryVariablesFileContainsNoVariables, diagnostic.Id);
    }

    [Fact]
    public void TelemetryVariablesFileNotFound_Test()
    {
        var diagnostic = GeneratorDiagnostics.TelemetryVariablesFileNotFound("test.json");

        Assert.Equal(DiagnosticIds.TelemetryVariablesFileNotFound, diagnostic.Id);
    }

    [Fact]
    public void TelemetryVariableAlreadyDefined_Test()
    {
        var diagnostic = GeneratorDiagnostics.TelemetryVariableAlreadyDefined("Test");

        Assert.Equal(DiagnosticIds.TelemetryVariableAlreadyDefined, diagnostic.Id);
    }

    [Fact]
    public void VariableClassConfiguredClassNameInUse_Test()
    {
        var diagnostic = GeneratorDiagnostics.VariableClassConfiguredClassNameInUse("TestVariable", "Test", "TestVariable", "TestEx");

        Assert.Equal(DiagnosticIds.VariableClassNameInUse, diagnostic.Id);
    }

    [Fact]
    public void VariableOptionsFileContainsDuplicateClassName_Test()
    {
        var diagnostic = GeneratorDiagnostics.VariableOptionsFileContainsDuplicateClassName("Test", "TestVariable", "TestEx");

        Assert.Equal(DiagnosticIds.VariableOptionsFileContainsDuplicateClassName, diagnostic.Id);
    }

    [Fact]
    public void VariableOptionsFileContainsDuplicateKey_Test()
    {
        var diagnostic = GeneratorDiagnostics.VariableOptionsFileContainsDuplicateKey("Test");

        Assert.Equal(DiagnosticIds.VariableOptionsFileContainsDuplicateKey, diagnostic.Id);
    }

    [Fact]
    public void VariableOptionsFileContainsDuplicateVariableName_Test()
    {
        var diagnostic = GeneratorDiagnostics.VariableOptionsFileContainsDuplicateVariableName("Test", "TestProperty", "TestEx");

        Assert.Equal(DiagnosticIds.VariableOptionsFileContainsDuplicateVariableName, diagnostic.Id);
    }
}
