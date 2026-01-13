using System.Diagnostics.CodeAnalysis;

namespace SharpRacer.Commands.Replay;

/// <summary>
/// Represents a simulator command that erases the replay tape.
/// </summary>
public readonly struct EraseReplayTapeCommand : ISimulatorCommand<EraseReplayTapeCommand>
{
    /// <summary>
    /// Initializes a new <see cref="EraseReplayTapeCommand"/> instance.
    /// </summary>
    public EraseReplayTapeCommand()
    {

    }

    /// <inheritdoc />
    public static ushort CommandId { get; } = (ushort)SimulatorCommandId.ReplaySetState;

    /// <inheritdoc />
    public readonly CommandMessage ToCommandMessage()
    {
        return new CommandMessage(CommandId, 0);
    }

    /// <inheritdoc />
    public static EraseReplayTapeCommand Parse(CommandMessage message)
    {
        CommandMessageParseException.ThrowIfCommandIdNotEqual(message, CommandId);

        return new EraseReplayTapeCommand();
    }

    /// <inheritdoc />
    public static bool TryParse(CommandMessage message, out EraseReplayTapeCommand result)
    {
        if (message.CommandId != CommandId)
        {
            result = default;
            return false;
        }

        result = new EraseReplayTapeCommand();
        return true;
    }

    /// <inheritdoc />
    public static implicit operator CommandMessage(EraseReplayTapeCommand command)
    {
        return command.ToCommandMessage();
    }

    #region IEquatable Implementation

    /// <inheritdoc />
    public override readonly bool Equals([NotNullWhen(true)] object? obj)
    {
        return obj is EraseReplayTapeCommand command && Equals(command);
    }

    /// <inheritdoc />
    public readonly bool Equals(EraseReplayTapeCommand other)
    {
        return true;
    }

    /// <inheritdoc />
    public override readonly int GetHashCode()
    {
        return new HashCode().ToHashCode();
    }

    /// <inheritdoc />
    public static bool operator ==(EraseReplayTapeCommand left, EraseReplayTapeCommand right)
    {
        return left.Equals(right);
    }

    /// <inheritdoc />
    public static bool operator !=(EraseReplayTapeCommand left, EraseReplayTapeCommand right)
    {
        return !(left == right);
    }

    #endregion IEquatable Implementation
}
