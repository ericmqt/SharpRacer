using SharpRacer.Telemetry;

namespace SharpRacer.Internal;

internal interface IConnectionTelemetryVariableInfoProvider : ITelemetryVariableInfoProvider
{
    void OnTelemetryVariablesActivated(ISimulatorConnection connection);
}
