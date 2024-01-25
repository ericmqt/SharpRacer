using System.Runtime.Versioning;
using SharpRacer.Interop;

namespace SharpRacer.Commands;

/// <summary>
/// Provides methods for controlling telemetry recording in the simulator.
/// </summary>
/// <remarks>
/// These commands may be issued at any time, however telemetry is only recorded when the driver is in the car.
/// </remarks>
[SupportedOSPlatform("windows5.1.2600")]
public static class TelemetryCommands
{
    /// <summary>
    /// Write the current telemetry file to disk and begin a new telemetry recording.
    /// </summary>
    public static void Restart()
    {
        BroadcastMessage.Send(SimulatorCommandId.TelemetryCommand, (ushort)TelemetryCommandType.Restart);
    }

    /// <summary>
    /// Start telemetry recording.
    /// </summary>
    public static void Start()
    {
        BroadcastMessage.Send(SimulatorCommandId.TelemetryCommand, (ushort)TelemetryCommandType.Start);
    }

    /// <summary>
    /// Stop telemetry recording.
    /// </summary>
    public static void Stop()
    {
        BroadcastMessage.Send(SimulatorCommandId.TelemetryCommand, (ushort)TelemetryCommandType.Stop);
    }
}
