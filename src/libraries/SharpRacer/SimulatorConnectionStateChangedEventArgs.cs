namespace SharpRacer;
public sealed class SimulatorConnectionStateChangedEventArgs : EventArgs
{
    public SimulatorConnectionStateChangedEventArgs(SimulatorConnectionState newState, SimulatorConnectionState oldState)
    {
        NewState = newState;
        OldState = oldState;
    }

    public SimulatorConnectionState NewState { get; }
    public SimulatorConnectionState OldState { get; }
}
