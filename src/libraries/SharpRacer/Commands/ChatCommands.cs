using System.Runtime.Versioning;
using SharpRacer.Interop;

namespace SharpRacer.Commands;

[SupportedOSPlatform("windows5.1.2600")]
public static class ChatCommands
{
    /// <summary>
    /// Close the chat window.
    /// </summary>
    public static void Close()
    {
        SendCommand(ChatCommandType.CloseChat);
    }

    /// <summary>
    /// Run a chat macro. Valid values are 1 through 15.
    /// </summary>
    /// <param name="macroId">The macro to execute.</param>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="macroId"/> is outside the range 1 through 15.</exception>
    public static void Macro(int macroId)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(macroId, 1);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(macroId, 15);

        SendCommand(ChatCommandType.Macro, macroId);
    }

    /// <summary>
    /// Open a new chat window.
    /// </summary>
    public static void Open()
    {
        SendCommand(ChatCommandType.OpenChat);
    }

    /// <summary>
    /// Reply to the last private chat message received.
    /// </summary>
    public static void ReplyToLastPrivateMessage()
    {
        SendCommand(ChatCommandType.ReplyToLastPrivateMessage);
    }

    private static void SendCommand(ChatCommandType chatCommand)
    {
        BroadcastMessage.Send(SimulatorCommandId.ChatCommand, (ushort)chatCommand);
    }

    private static void SendCommand(ChatCommandType chatCommand, int arg1)
    {
        BroadcastMessage.Send(SimulatorCommandId.ChatCommand, (ushort)chatCommand, arg1);
    }
}
