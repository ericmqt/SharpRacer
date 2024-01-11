using SharpRacer.SourceGenerators.TelemetryVariables.Diagnostics;
using SharpRacer.SourceGenerators.Testing.TelemetryVariables;

namespace SharpRacer.SourceGenerators.TelemetryVariables.InputModels;
public class VariableOptionsFactoryTests
{
    [Fact]
    public void Ctor_Test()
    {
        var optionsFile = new JsonVariableOptionsDocumentBuilder()
            .Add("Test", null, "TestVariable")
            .ToVariableOptionsFile("Options.json");

        var factory = new VariableOptionsFactory(optionsFile, 3);

        Assert.NotNull(factory);
    }

    [Fact]
    public void TryAdd_Test()
    {
        var optionsFile = new JsonVariableOptionsDocumentBuilder()
            .Add("Test", null, "TestVariable")
            .ToVariableOptionsFile("Options.json");

        var factory = new VariableOptionsFactory(optionsFile, 3);

        var inputOptions = optionsFile.Read(default, out var readDiagnostic);

        Assert.Null(readDiagnostic);

        var addResult = factory.TryAdd(inputOptions.First(), out var addDiagnostics);

        Assert.True(addResult);
        Assert.Empty(addDiagnostics);

        var results = factory.Build();

        Assert.Single(results);

        var outputOption = results.Single();

        Assert.Equal("Test", outputOption.VariableKey);
        Assert.Null(outputOption.Name);
        Assert.Equal("TestVariable", outputOption.ClassName);
    }

    [Fact]
    public void TryAdd_DefaultValueTest()
    {
        var optionsFile = new JsonVariableOptionsDocumentBuilder()
            .ToVariableOptionsFile("Options.json");

        var factory = new VariableOptionsFactory(optionsFile, 3);

        var addResult = factory.TryAdd(default, out var addDiagnostics);

        Assert.False(addResult);
        Assert.Empty(addDiagnostics);

        var results = factory.Build();
        Assert.Empty(results);
    }

    [Fact]
    public void TryAdd_DuplicateClassNameDiagnosticTest()
    {
        var optionsFile = new JsonVariableOptionsDocumentBuilder()
            .Add("Test", null, "TestVariable")
            .Add("Test2", null, "TestVariable")
            .ToVariableOptionsFile("Options.json");

        var factory = new VariableOptionsFactory(optionsFile, 3);

        var inputOptions = optionsFile.Read(default, out var readDiagnostic);

        Assert.Null(readDiagnostic);

        var addResult = factory.TryAdd(inputOptions[0], out var addDiagnostics);

        Assert.True(addResult);
        Assert.Empty(addDiagnostics);

        addResult = factory.TryAdd(inputOptions[1], out addDiagnostics);

        Assert.False(addResult);
        Assert.Single(addDiagnostics);

        var diagnostic = addDiagnostics.First();
        Assert.Equal(DiagnosticIds.VariableOptionsFileContainsDuplicateClassName, diagnostic.Id);
    }

    [Fact]
    public void TryAdd_DuplicateKeyDiagnosticTest()
    {
        var optionsFile = new JsonVariableOptionsDocumentBuilder()
            .Add("Test", null, "TestVariable")
            .Add("Test", null, null)
            .ToVariableOptionsFile("Options.json");

        var factory = new VariableOptionsFactory(optionsFile, 3);

        var inputOptions = optionsFile.Read(default, out var readDiagnostic);

        Assert.Null(readDiagnostic);

        var addResult = factory.TryAdd(inputOptions.First(), out var addDiagnostics);

        Assert.True(addResult);
        Assert.Empty(addDiagnostics);

        addResult = factory.TryAdd(inputOptions.Last(), out addDiagnostics);

        Assert.False(addResult);
        Assert.Single(addDiagnostics);

        var diagnostic = addDiagnostics.First();
        Assert.Equal(DiagnosticIds.VariableOptionsFileContainsDuplicateKey, diagnostic.Id);
    }

    [Fact]
    public void TryAdd_DuplicateVariableNameDiagnosticTest()
    {
        var optionsFile = new JsonVariableOptionsDocumentBuilder()
            .Add("Test", "TestVariable", "TestVariable")
            .Add("Test2", "TestVariable", "Test2Variable")
            .ToVariableOptionsFile("Options.json");

        var factory = new VariableOptionsFactory(optionsFile, 3);

        var inputOptions = optionsFile.Read(default, out var readDiagnostic);

        Assert.Null(readDiagnostic);

        var addResult = factory.TryAdd(inputOptions.First(), out var addDiagnostics);

        Assert.True(addResult);
        Assert.Empty(addDiagnostics);

        addResult = factory.TryAdd(inputOptions.Last(), out addDiagnostics);

        Assert.False(addResult);
        Assert.Single(addDiagnostics);

        var diagnostic = addDiagnostics.First();
        Assert.Equal(DiagnosticIds.VariableOptionsFileContainsDuplicateVariableName, diagnostic.Id);
    }


}
