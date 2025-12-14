using SharpRacer.Telemetry;

namespace SharpRacer.Internal;
internal interface IConnectionDataVariableInfoProvider : IDataVariableInfoProvider
{
    void OnDataVariablesActivated(ISimulatorConnection connection);
}
