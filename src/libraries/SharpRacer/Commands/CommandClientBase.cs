using SharpRacer.Commands.Interop;

namespace SharpRacer.Commands;

/// <summary>
/// Provides a base class for simulator command clients.
/// </summary>
public abstract class CommandClientBase
{
    /// <summary>
    /// Initializes a new <see cref="CommandClientBase"/> instance with the specified command sink.
    /// </summary>
    /// <param name="commandSink">The <see cref="ISimulatorCommandSink"/> to use for sending commands to the simulator.</param>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="commandSink"/> is <see langword="null"/>.
    /// </exception>
    protected CommandClientBase(ISimulatorCommandSink commandSink)
    {
        CommandSink = commandSink ?? throw new ArgumentNullException(nameof(commandSink));
    }

    /// <summary>
    /// Gets the <see cref="ISimulatorCommandSink"/> used to send commands to the simulator.
    /// </summary>
    protected internal ISimulatorCommandSink CommandSink { get; }

    /// <summary>
    /// Sends the command to the simulator.
    /// </summary>
    /// <typeparam name="TCommand">A type that implements <see cref="ISimulatorCommand{TSelf}"/>.</typeparam>
    /// <param name="command">The command to send to the simulator.</param>
    protected internal void Send<TCommand>(TCommand command)
        where TCommand : ISimulatorCommand<TCommand>
    {
        CommandSink.Send(command);
    }
}
