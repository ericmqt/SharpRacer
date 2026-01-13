using System.Runtime.Versioning;
using SharpRacer.Interop;
using Windows.Win32;

namespace SharpRacer.Commands.Interop;

/// <summary>
/// Facilitates sending commands to the simulator via the SendNotifyMessage Win32 API.
/// </summary>
public class SimulatorCommandSink : ISimulatorCommandSink
{
    /// <summary>
    /// Initializes a new <see cref="SimulatorCommandSink"/> instance using the default message sink.
    /// </summary>
    [SupportedOSPlatform(PInvoke.MinOSPlatform)]
    public SimulatorCommandSink()
        : this(SimulatorNotifyMessageSink.Instance)
    {

    }

    /// <summary>
    /// Initializes a new <see cref="SimulatorCommandSink"/> instance using the specified message sink.
    /// </summary>
    /// <param name="messageSink">The <see cref="ISimulatorNotifyMessageSink"/> used to send messages to the simulator.</param>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="messageSink"/> is <see langword="null"/>.
    /// </exception>
    public SimulatorCommandSink(ISimulatorNotifyMessageSink messageSink)
    {
        MessageSink = messageSink ?? throw new ArgumentNullException(nameof(messageSink));
    }

    /// <summary>
    /// Gets the default <see cref="SimulatorCommandSink"/> instance.
    /// </summary>
    [SupportedOSPlatform(PInvoke.MinOSPlatform)]
    public static ISimulatorCommandSink Instance { get; } = new SimulatorCommandSink();

    internal ISimulatorNotifyMessageSink MessageSink { get; }

    /// <inheritdoc />
    public void Send<TCommand>(TCommand command) where TCommand : ISimulatorCommand<TCommand>
    {
        Send(command.ToCommandMessage());
    }

    /// <inheritdoc />
    public void Send(CommandMessage commandMessage)
    {
        MessageSink.Send(commandMessage.ToNotifyMessage());
    }
}
