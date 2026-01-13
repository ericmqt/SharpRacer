namespace SharpRacer.Commands.Chat;

/// <summary>
/// Defines methods for sending chat commands to the simulator.
/// </summary>
public interface IChatCommandClient
{
    /// <summary>
    /// Close the chat window.
    /// </summary>
    void CloseChat();

    /// <summary>
    /// Run a chat macro. Valid values are 1 through 15.
    /// </summary>
    /// <param name="macroId">The macro to execute.</param>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="macroId"/> is outside the range 1 through 15.</exception>
    void Macro(int macroId);

    /// <summary>
    /// Open a new chat window.
    /// </summary>
    void OpenChat();

    /// <summary>
    /// Initiate a reply to the last private chat message received.
    /// </summary>
    void ReplyToLastPrivateMessage();
}
