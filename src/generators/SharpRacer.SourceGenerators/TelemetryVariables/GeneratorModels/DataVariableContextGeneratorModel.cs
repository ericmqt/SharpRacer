using Microsoft.CodeAnalysis;
using SharpRacer.SourceGenerators.TelemetryVariables.Pipeline;

namespace SharpRacer.SourceGenerators.TelemetryVariables.GeneratorModels;
internal class DataVariableContextGeneratorModel
{
    public DataVariableContextGeneratorModel(VariableContextClassGeneratorTarget contextGeneratorTarget)
    {
        ClassGeneratorTarget = contextGeneratorTarget ?? throw new ArgumentNullException(nameof(contextGeneratorTarget));

        // TODO: Change this to deserialized names
        VariableNamesFile = contextGeneratorTarget.IncludedVariableNamesFile;

        HasDataVariablesContextInterface = ClassGeneratorTarget
            .TargetClassSymbol
            .Interfaces
            .FirstOrDefault(x => string.Equals(x.Name, "IDataVariablesContext", StringComparison.Ordinal)) != null;
    }

    public VariableContextClassGeneratorTarget ClassGeneratorTarget { get; }
    public bool HasDataVariablesContextInterface { get; }
    public IncludedVariableNamesFile? VariableNamesFile { get; }

}
