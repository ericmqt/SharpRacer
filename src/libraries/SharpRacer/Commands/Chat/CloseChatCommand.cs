using System.Diagnostics.CodeAnalysis;

namespace SharpRacer.Commands.Chat;

/// <summary>
/// Represents a simulator command that closes the chat window.
/// </summary>
public readonly struct CloseChatCommand : ISimulatorCommand<CloseChatCommand>
{
    /// <summary>
    /// Initializes a new <see cref="CloseChatCommand"/> instance.
    /// </summary>
    public CloseChatCommand()
    {

    }

    /// <inheritdoc />
    public static ushort CommandId { get; } = (ushort)SimulatorCommandId.Chat;

    /// <inheritdoc />
    public readonly CommandMessage ToCommandMessage()
    {
        return new CommandMessage(CommandId, (ushort)ChatCommandType.CloseChat);
    }

    /// <inheritdoc />
    public static CloseChatCommand Parse(CommandMessage message)
    {
        CommandMessageParseException.ThrowIfCommandIdNotEqual(message, CommandId);

        var reader = new CommandMessageReader(ref message);

        ChatCommandType chatCommandType;

        try { chatCommandType = reader.ReadArg1<ChatCommandType>(); }
        catch (CommandMessageReaderException ex)
        {
            throw CommandMessageParseException.Arg1ReaderException(ex);
        }

        if (chatCommandType != ChatCommandType.CloseChat)
        {
            throw CommandMessageParseException.InvalidArg1Value(ChatCommandType.CloseChat, chatCommandType);
        }

        return new CloseChatCommand();
    }

    /// <inheritdoc />
    public static bool TryParse(CommandMessage message, out CloseChatCommand result)
    {
        if (message.CommandId != CommandId)
        {
            result = default;
            return false;
        }

        var reader = new CommandMessageReader(ref message);

        if (!reader.TryReadArg1<ChatCommandType>(out var chatCommandType) || chatCommandType != ChatCommandType.CloseChat)
        {
            result = default;
            return false;
        }

        result = new CloseChatCommand();
        return true;
    }

    /// <inheritdoc />
    public static implicit operator CommandMessage(CloseChatCommand command)
    {
        return command.ToCommandMessage();
    }

    #region IEquatable Implementation

    /// <inheritdoc />
    public override readonly bool Equals([NotNullWhen(true)] object? obj)
    {
        return obj is CloseChatCommand other && Equals(other);
    }

    /// <inheritdoc />
    public readonly bool Equals(CloseChatCommand other)
    {
        return true;
    }

    /// <inheritdoc />
    public override readonly int GetHashCode()
    {
        return new HashCode().ToHashCode();
    }

    /// <inheritdoc />
    public static bool operator ==(CloseChatCommand left, CloseChatCommand right)
    {
        return left.Equals(right);
    }

    /// <inheritdoc />
    public static bool operator !=(CloseChatCommand left, CloseChatCommand right)
    {
        return !(left == right);
    }

    #endregion IEquatable Implementation
}
