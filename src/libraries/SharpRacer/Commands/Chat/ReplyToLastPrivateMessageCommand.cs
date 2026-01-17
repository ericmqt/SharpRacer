using System.Diagnostics.CodeAnalysis;

namespace SharpRacer.Commands.Chat;

/// <summary>
/// Represents a simulator command that begins composing a reply to the most recent private message received.
/// </summary>
public readonly struct ReplyToLastPrivateMessageCommand : ISimulatorCommand<ReplyToLastPrivateMessageCommand>
{
    /// <summary>
    /// Initializes a new <see cref="ReplyToLastPrivateMessageCommand"/> instance.
    /// </summary>
    public ReplyToLastPrivateMessageCommand()
    {

    }

    /// <inheritdoc />
    public static ushort CommandId { get; } = (ushort)SimulatorCommandId.Chat;

    /// <inheritdoc />
    public readonly CommandMessage ToCommandMessage()
    {
        return new CommandMessage(CommandId, (ushort)ChatCommandType.ReplyToLastPrivateMessage);
    }

    /// <inheritdoc />
    public static ReplyToLastPrivateMessageCommand Parse(CommandMessage message)
    {
        CommandMessageParseException.ThrowIfCommandIdNotEqual(message, CommandId);

        var reader = new CommandMessageReader(ref message);

        ChatCommandType chatCommandType;

        try { chatCommandType = reader.ReadArg1<ChatCommandType>(); }
        catch (CommandMessageReaderException ex)
        {
            throw CommandMessageParseException.Arg1ReaderException(ex);
        }

        if (chatCommandType != ChatCommandType.ReplyToLastPrivateMessage)
        {
            throw CommandMessageParseException.InvalidArg1Value(ChatCommandType.ReplyToLastPrivateMessage, chatCommandType);
        }

        return new ReplyToLastPrivateMessageCommand();
    }

    /// <inheritdoc />
    public static bool TryParse(CommandMessage message, out ReplyToLastPrivateMessageCommand result)
    {
        if (message.CommandId != CommandId)
        {
            result = default;
            return false;
        }

        var reader = new CommandMessageReader(ref message);

        if (!reader.TryReadArg1<ChatCommandType>(out var chatCommandType) || chatCommandType != ChatCommandType.ReplyToLastPrivateMessage)
        {
            result = default;
            return false;
        }

        result = new ReplyToLastPrivateMessageCommand();
        return true;
    }

    /// <inheritdoc />
    public static implicit operator CommandMessage(ReplyToLastPrivateMessageCommand command)
    {
        return command.ToCommandMessage();
    }

    #region IEquatable Implementation

    /// <inheritdoc />
    public override readonly bool Equals([NotNullWhen(true)] object? obj)
    {
        return obj is ReplyToLastPrivateMessageCommand other && Equals(other);
    }

    /// <inheritdoc />
    public readonly bool Equals(ReplyToLastPrivateMessageCommand other)
    {
        return true;
    }

    /// <inheritdoc />
    public override readonly int GetHashCode()
    {
        return new HashCode().ToHashCode();
    }

    /// <inheritdoc />
    public static bool operator ==(ReplyToLastPrivateMessageCommand left, ReplyToLastPrivateMessageCommand right)
    {
        return left.Equals(right);
    }

    /// <inheritdoc />
    public static bool operator !=(ReplyToLastPrivateMessageCommand left, ReplyToLastPrivateMessageCommand right)
    {
        return !(left == right);
    }

    #endregion IEquatable Implementation
}
