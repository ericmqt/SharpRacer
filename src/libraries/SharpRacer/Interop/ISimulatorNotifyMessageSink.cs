namespace SharpRacer.Interop;

/// <summary>
/// Defines methods for sending messages to the simulator via the SendNotifyMessage Win32 API.
/// </summary>
public interface ISimulatorNotifyMessageSink
{
    /// <summary>
    /// Sends the specified message to the simulator.
    /// </summary>
    /// <param name="message">The message to send.</param>
    /// <exception cref="System.ComponentModel.Win32Exception">The SendNotifyMessage Win32 API invocation failed.</exception>
    void Send(SimulatorNotifyMessageData message);
}
