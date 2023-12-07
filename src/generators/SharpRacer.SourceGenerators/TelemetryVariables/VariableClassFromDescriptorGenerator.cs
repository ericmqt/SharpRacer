using SharpRacer.SourceGenerators.TelemetryVariables.GeneratorModels;

namespace SharpRacer.SourceGenerators.TelemetryVariables;
internal class VariableClassFromDescriptorGenerator
{
    private readonly List<string> _usingNamespaces;

    public VariableClassFromDescriptorGenerator(VariableClassFromDescriptorGeneratorModel model)
    {
        Model = model ?? throw new ArgumentNullException(nameof(model));

        _usingNamespaces = new List<string>()
        {
            "System",
            "System.Collections.Generic",
            "System.Text",
            "SharpRacer.Telemetry.Variables"
        };
    }

    public VariableClassFromDescriptorGeneratorModel Model { get; }
}
