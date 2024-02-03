namespace SharpRacer;

/// <summary>
/// Describes the current state of a connection to the simulator.
/// </summary>
public enum SimulatorConnectionState
{
    /// <summary>
    /// The connection to the simulator is closed.
    /// </summary>
    Closed = 0,

    /// <summary>
    /// The connection object is connecting to the simulator.
    /// </summary>
    Connecting = 1,

    /// <summary>
    /// The connection to the simulator is open.
    /// </summary>
    Open = 2
}
