using SharpRacer.Telemetry.Variables;

namespace SourceGeneratorDebugApp;

[GenerateDataVariablesContext("TelemetryVariables_VariableNames.json")]
internal partial class TelemetryVariables
{
    /*public TelemetryVariables(IDataVariableInfoProvider variableProvider)
    {
        ArgumentNullException.ThrowIfNull(variableProvider);

        DataVariableFactory factory = new(variableProvider);

        var myVar = factory.CreateScalar<int>("Variable");
    }*/
}
