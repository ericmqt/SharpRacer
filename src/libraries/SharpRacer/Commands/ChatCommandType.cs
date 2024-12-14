namespace SharpRacer.Commands;

/// <summary>
/// Defines chat command types.
/// </summary>
/// <remarks>
/// See irsdk_ChatCommandMode in the iRacing SDK.
/// </remarks>
public enum ChatCommandType : ushort
{
    /// <summary>
    /// Invoke a chat macro.
    /// </summary>
    Macro = 0,

    /// <summary>
    /// Open the chat window.
    /// </summary>
    OpenChat = 1,

    /// <summary>
    /// Reply to the last private message received.
    /// </summary>
    ReplyToLastPrivateMessage = 2,

    /// <summary>
    /// Close the chat window.
    /// </summary>
    CloseChat = 3
}
