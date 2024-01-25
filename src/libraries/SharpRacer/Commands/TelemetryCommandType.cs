namespace SharpRacer.Commands;
public enum TelemetryCommandType : ushort
{
    /// <summary>
    /// Stop telemetry recording.
    /// </summary>
    Stop = 0,

    /// <summary>
    /// Start telemetry recording.
    /// </summary>
    Start = 1,

    /// <summary>
    /// Write the current telemetry file to disk and begin a new telemetry recording.
    /// </summary>
    Restart = 2
}
