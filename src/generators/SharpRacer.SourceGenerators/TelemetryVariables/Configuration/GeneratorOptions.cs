using SharpRacer.SourceGenerators.TelemetryVariables.InputModels;

namespace SharpRacer.SourceGenerators.TelemetryVariables.Configuration;
internal class GeneratorOptions
{
    public VariablesGeneratorOptions VariablesGenerator { get; set; } = new VariablesGeneratorOptions();
    public Dictionary<string, VariableOptions> VariableOptions { get; set; } = new Dictionary<string, VariableOptions>();
}
