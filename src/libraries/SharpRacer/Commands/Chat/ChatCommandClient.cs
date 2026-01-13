using System.Runtime.Versioning;
using SharpRacer.Commands.Interop;
using Windows.Win32;

namespace SharpRacer.Commands.Chat;

/// <summary>
/// Provides methods for sending chat commands to the simulator.
/// </summary>
public sealed class ChatCommandClient : CommandClientBase, IChatCommandClient
{
    /// <summary>
    /// Initializes a new <see cref="ChatCommandClient"/> instance.
    /// </summary>
    [SupportedOSPlatform(PInvoke.MinOSPlatform)]
    public ChatCommandClient()
        : this(SimulatorCommandSink.Instance)
    {

    }

    /// <summary>
    /// Initializes a new <see cref="ChatCommandClient"/> instance with the specified command sink.
    /// </summary>
    /// <param name="commandSink">The <see cref="ISimulatorCommandSink"/> to use for sending commands to the simulator.</param>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="commandSink"/> is <see langword="null"/>.
    /// </exception>
    public ChatCommandClient(ISimulatorCommandSink commandSink)
        : base(commandSink)
    {

    }

    /// <inheritdoc />
    public void CloseChat()
    {
        Send(new CloseChatCommand());
    }

    /// <inheritdoc />
    public void Macro(int macroId)
    {
        Send(new MacroCommand(macroId));
    }

    /// <inheritdoc />
    public void OpenChat()
    {
        Send(new OpenChatCommand());
    }

    /// <inheritdoc />
    public void ReplyToLastPrivateMessage()
    {
        Send(new ReplyToLastPrivateMessageCommand());
    }
}
