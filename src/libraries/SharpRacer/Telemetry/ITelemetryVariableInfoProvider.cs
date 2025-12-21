namespace SharpRacer.Telemetry;

/// <summary>
/// Provides a collection of <see cref="TelemetryVariableInfo"/> objects from a telemetry data source.
/// </summary>
public interface ITelemetryVariableInfoProvider
{
    /// <summary>
    /// Gets a collection of <see cref="TelemetryVariableInfo"/> objects representing the telemetry variables exposed by the data source.
    /// </summary>
    IEnumerable<TelemetryVariableInfo> Variables { get; }

    /// <summary>
    /// Registers a callback that is executed if and when the specified telemetry variable becomes available.
    /// </summary>
    /// <param name="variableName">The name of the variable.</param>
    /// <param name="callback">The action to execute when the telemetry variable becomes available.</param>
    void NotifyTelemetryVariableActivated(string variableName, Action<TelemetryVariableInfo> callback);
}
