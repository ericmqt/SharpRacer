using System.Diagnostics.CodeAnalysis;

namespace SharpRacer.Commands.Chat;

/// <summary>
/// Represents a simulator command that opens the chat window.
/// </summary>
public readonly struct OpenChatCommand : ISimulatorCommand<OpenChatCommand>
{
    /// <summary>
    /// Initializes a new <see cref="OpenChatCommand"/> instance.
    /// </summary>
    public OpenChatCommand()
    {

    }

    /// <inheritdoc />
    public static ushort CommandId { get; } = (ushort)SimulatorCommandId.Chat;

    /// <inheritdoc />
    public readonly CommandMessage ToCommandMessage()
    {
        return new CommandMessage(CommandId, (ushort)ChatCommandType.OpenChat);
    }

    /// <inheritdoc />
    public static OpenChatCommand Parse(CommandMessage message)
    {
        CommandMessageParseException.ThrowIfCommandIdNotEqual(message, CommandId);

        var reader = new CommandMessageReader(ref message);

        ChatCommandType chatCommandType;

        try { chatCommandType = reader.ReadArg1<ChatCommandType>(); }
        catch (CommandMessageReaderException ex)
        {
            throw CommandMessageParseException.Arg1ReaderException(ex);
        }

        if (chatCommandType != ChatCommandType.OpenChat)
        {
            throw CommandMessageParseException.InvalidArg1Value(ChatCommandType.OpenChat, chatCommandType);
        }

        return new OpenChatCommand();
    }

    /// <inheritdoc />
    public static bool TryParse(CommandMessage message, out OpenChatCommand result)
    {
        if (message.CommandId != CommandId)
        {
            result = default;
            return false;
        }

        var reader = new CommandMessageReader(ref message);

        if (!reader.TryReadArg1<ChatCommandType>(out var chatCommandType) || chatCommandType != ChatCommandType.OpenChat)
        {
            result = default;
            return false;
        }

        result = new OpenChatCommand();
        return true;
    }

    /// <inheritdoc />
    public static implicit operator CommandMessage(OpenChatCommand command)
    {
        return command.ToCommandMessage();
    }

    #region IEquatable Implementation

    /// <inheritdoc />
    public override readonly bool Equals([NotNullWhen(true)] object? obj)
    {
        return obj is OpenChatCommand other && Equals(other);
    }

    /// <inheritdoc />
    public readonly bool Equals(OpenChatCommand other)
    {
        return true;
    }

    /// <inheritdoc />
    public override readonly int GetHashCode()
    {
        return new HashCode().ToHashCode();
    }

    /// <inheritdoc />
    public static bool operator ==(OpenChatCommand left, OpenChatCommand right)
    {
        return left.Equals(right);
    }

    /// <inheritdoc />
    public static bool operator !=(OpenChatCommand left, OpenChatCommand right)
    {
        return !(left == right);
    }

    #endregion IEquatable Implementation
}
