using SharpRacer.SourceGenerators.TelemetryVariables.Diagnostics;
using SharpRacer.SourceGenerators.Testing.TelemetryVariables;

namespace SharpRacer.SourceGenerators.TelemetryVariables.InputModels;
public class VariableInfoFactoryTests
{
    [Fact]
    public void Ctor_Test()
    {
        var file = new JsonVariableInfoDocumentBuilder()
            .AddScalar("Test", VariableValueType.Int, "Test variable", null)
            .ToVariableInfoFile("Variables.json");

        var factory = new VariableInfoFactory(file, 1);

        Assert.NotNull(factory);
    }

    [Fact]
    public void TryAdd_DefaultJsonVariableInfoValueTest()
    {
        var file = new JsonVariableInfoDocumentBuilder()
            .AddScalar("Test", VariableValueType.Int, "Test variable", null)
            .ToVariableInfoFile("Variables.json");

        var factory = new VariableInfoFactory(file, 2);

        var addResult = factory.TryAdd(default, out var diagnostics);
        Assert.False(addResult);
        Assert.Empty(diagnostics);
    }

    [Fact]
    public void TryAdd_VariableAlreadyDefinedDiagnosticTest()
    {
        var file = new JsonVariableInfoDocumentBuilder()
            .AddScalar("Test", VariableValueType.Int, "Test variable", null)
            .ToVariableInfoFile("Variables.json");

        var factory = new VariableInfoFactory(file, 2);

        var jsonVariables = file.Read(default, out var readDiagnostic);
        Assert.Null(readDiagnostic);

        var testVariable = jsonVariables.First();
        var testVariable2 = jsonVariables.First();

        var addTestVariableResult = factory.TryAdd(testVariable, out var testVariableDiagnostics);
        Assert.True(addTestVariableResult);
        Assert.Empty(testVariableDiagnostics);

        var addTestVariable2Result = factory.TryAdd(testVariable2, out testVariableDiagnostics);

        Assert.False(addTestVariable2Result);
        Assert.Single(testVariableDiagnostics);

        var diagnostic = testVariableDiagnostics.First();
        Assert.Equal(DiagnosticIds.VariableInfo_VariableAlreadyDefined, diagnostic.Id);
    }
}
