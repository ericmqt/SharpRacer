using System.Diagnostics.CodeAnalysis;

namespace SharpRacer.Commands.PitService;

/// <summary>
/// Represents a simulator command that requests the use of a fast repair, if one is available, during the next pit stop.
/// </summary>
public readonly struct UseWindscreenTearOffCommand : ISimulatorCommand<UseWindscreenTearOffCommand>
{
    /// <summary>
    /// Initializes a new <see cref="UseWindscreenTearOffCommand"/> instance.
    /// </summary>
    public UseWindscreenTearOffCommand()
    {

    }

    /// <inheritdoc />
    public static ushort CommandId { get; } = (ushort)SimulatorCommandId.PitService;

    /// <inheritdoc />
    public readonly CommandMessage ToCommandMessage()
    {
        return new CommandMessage(CommandId, (ushort)PitServiceCommandType.WindscreenTearOff);
    }

    /// <inheritdoc />
    public static UseWindscreenTearOffCommand Parse(CommandMessage message)
    {
        CommandMessageParseException.ThrowIfCommandIdNotEqual(message, CommandId);

        var reader = new CommandMessageReader(ref message);

        PitServiceCommandType pitServiceType;

        try { pitServiceType = reader.ReadArg1<PitServiceCommandType>(); }
        catch (CommandMessageReaderException ex)
        {
            throw CommandMessageParseException.Arg1ReaderException(ex);
        }

        if (pitServiceType != PitServiceCommandType.WindscreenTearOff)
        {
            throw new CommandMessageParseException($"Read invalid {nameof(PitServiceCommandType)} value: {pitServiceType}.");
        }

        return new UseWindscreenTearOffCommand();
    }

    /// <inheritdoc />
    public static bool TryParse(CommandMessage message, out UseWindscreenTearOffCommand result)
    {
        if (message.CommandId != CommandId)
        {
            result = default;
            return false;
        }

        var reader = new CommandMessageReader(ref message);

        if (!reader.TryReadArg1<PitServiceCommandType>(out var service) || service != PitServiceCommandType.WindscreenTearOff)
        {
            result = default;
            return false;
        }

        result = new UseWindscreenTearOffCommand();
        return true;
    }

    /// <inheritdoc />
    public static implicit operator CommandMessage(UseWindscreenTearOffCommand command)
    {
        return command.ToCommandMessage();
    }

    #region IEquatable Implementation

    /// <inheritdoc />
    public override readonly bool Equals([NotNullWhen(true)] object? obj)
    {
        return obj is UseWindscreenTearOffCommand command && Equals(command);
    }

    /// <inheritdoc />
    public readonly bool Equals(UseWindscreenTearOffCommand other)
    {
        return true;
    }

    /// <inheritdoc />
    public override readonly int GetHashCode()
    {
        return new HashCode().ToHashCode();
    }

    /// <inheritdoc />
    public static bool operator ==(UseWindscreenTearOffCommand left, UseWindscreenTearOffCommand right)
    {
        return left.Equals(right);
    }

    /// <inheritdoc />
    public static bool operator !=(UseWindscreenTearOffCommand left, UseWindscreenTearOffCommand right)
    {
        return !(left == right);
    }

    #endregion IEquatable Implementation
}
