using System.Collections.Immutable;

namespace SharpRacer.SourceGenerators.TelemetryVariables.GeneratorModels;
internal class DataVariableContextGeneratorModel
{
    public DataVariableContextGeneratorModel(ImmutableArray<VariableModel> variables)
    {
        Variables = variables;

        /*ClassGeneratorTarget = contextGeneratorTarget ?? throw new ArgumentNullException(nameof(contextGeneratorTarget));

        // TODO: Change this to deserialized names
        VariableNamesFile = contextGeneratorTarget.IncludedVariableNamesFile;

        HasDataVariablesContextInterface = ClassGeneratorTarget
            .TargetClassSymbol
            .Interfaces
            .FirstOrDefault(x => string.Equals(x.Name, "IDataVariablesContext", StringComparison.Ordinal)) != null;*/

    }

    public bool HasDataVariablesContextInterface { get; }
    public string TypeName { get; }
    public string TypeNamespace { get; }
    public ImmutableArray<VariableModel> Variables { get; }

}
