namespace SharpRacer;

/// <summary>
/// Provides data for the <see cref="ISimulatorConnection.StateChanged"/> event, which represents a change in the value of
/// <see cref="ISimulatorConnection.State"/>.
/// </summary>
public sealed class SimulatorConnectionStateChangedEventArgs : EventArgs
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SimulatorConnectionStateChangedEventArgs"/> class.
    /// </summary>
    /// <param name="newState"></param>
    /// <param name="oldState"></param>
    public SimulatorConnectionStateChangedEventArgs(SimulatorConnectionState newState, SimulatorConnectionState oldState)
    {
        NewState = newState;
        OldState = oldState;
    }

    /// <summary>
    /// Gets the <see cref="SimulatorConnectionState"/> value that represents the current connection state.
    /// </summary>
    public SimulatorConnectionState NewState { get; }

    /// <summary>
    /// Gets the <see cref="SimulatorConnectionState"/> value that represents the connection state prior to the event being raised.
    /// </summary>
    public SimulatorConnectionState OldState { get; }
}
