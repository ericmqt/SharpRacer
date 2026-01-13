namespace SharpRacer.Commands.Interop;

/// <summary>
/// Defines methods for sending commands to the simulator.
/// </summary>
public interface ISimulatorCommandSink
{
    /// <summary>
    /// Sends the specified command message to the simulator.
    /// </summary>
    /// <param name="commandMessage">The command message to send to the simulator.</param>
    /// <exception cref="System.ComponentModel.Win32Exception">The SendNotifyMessage Win32 API invocation failed.</exception>
    void Send(CommandMessage commandMessage);

    /// <summary>
    /// Sends the specified command to the simulator.
    /// </summary>
    /// <typeparam name="TCommand">A type that implements <see cref="ISimulatorCommand{TSelf}"/>.</typeparam>
    /// <param name="command">The command to send to the simulator.</param>
    /// <exception cref="System.ComponentModel.Win32Exception">The SendNotifyMessage Win32 API invocation failed.</exception>
    void Send<TCommand>(TCommand command)
        where TCommand : ISimulatorCommand<TCommand>;
}
