namespace SharpRacer.Commands.Telemetry;

/// <summary>
/// Defines methods for sending telemetry recording commands to the simulator.
/// </summary>
public interface ITelemetryCommandClient
{
    /// <summary>
    /// Write the current telemetry file to disk and begin a new telemetry recording.
    /// </summary>
    void Restart();

    /// <summary>
    /// Start telemetry recording.
    /// </summary>
    void Start();

    /// <summary>
    /// Stop telemetry recording.
    /// </summary>
    void Stop();
}
