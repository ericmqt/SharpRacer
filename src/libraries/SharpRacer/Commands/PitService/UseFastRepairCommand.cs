using System.Diagnostics.CodeAnalysis;

namespace SharpRacer.Commands.PitService;

/// <summary>
/// Represents a simulator command that requests the use of a fast repair, if one is available, during the next pit stop.
/// </summary>
public readonly struct UseFastRepairCommand : ISimulatorCommand<UseFastRepairCommand>
{
    /// <summary>
    /// Initializes a new <see cref="UseFastRepairCommand"/> instance.
    /// </summary>
    public UseFastRepairCommand()
    {

    }

    /// <inheritdoc />
    public static ushort CommandId { get; } = (ushort)SimulatorCommandId.PitCommand;

    /// <inheritdoc />
    public readonly CommandMessage ToCommandMessage()
    {
        return new CommandMessage(CommandId, (ushort)PitServiceCommandType.FastRepair);
    }

    /// <inheritdoc />
    public static UseFastRepairCommand Parse(CommandMessage message)
    {
        CommandMessageParseException.ThrowIfCommandIdNotEqual(message, CommandId);

        var reader = new CommandMessageReader(ref message);

        PitServiceCommandType pitServiceType;

        try { pitServiceType = reader.ReadArg1<PitServiceCommandType>(); }
        catch (CommandMessageReaderException ex)
        {
            throw CommandMessageParseException.Arg1ReaderException(ex);
        }

        if (pitServiceType != PitServiceCommandType.FastRepair)
        {
            throw new CommandMessageParseException($"Read invalid {nameof(PitServiceCommandType)} value: {pitServiceType}.");
        }

        return new UseFastRepairCommand();
    }

    /// <inheritdoc />
    public static bool TryParse(CommandMessage message, out UseFastRepairCommand result)
    {
        if (message.CommandId != CommandId)
        {
            result = default;
            return false;
        }

        var reader = new CommandMessageReader(ref message);

        if (!reader.TryReadArg1<PitServiceCommandType>(out var service) || service != PitServiceCommandType.FastRepair)
        {
            result = default;
            return false;
        }

        result = new UseFastRepairCommand();
        return true;
    }

    /// <inheritdoc />
    public static implicit operator CommandMessage(UseFastRepairCommand command)
    {
        return command.ToCommandMessage();
    }

    #region IEquatable Implementation

    /// <inheritdoc />
    public override readonly bool Equals([NotNullWhen(true)] object? obj)
    {
        return obj is UseFastRepairCommand command && Equals(command);
    }

    /// <inheritdoc />
    public readonly bool Equals(UseFastRepairCommand other)
    {
        return true;
    }

    /// <inheritdoc />
    public override readonly int GetHashCode()
    {
        return new HashCode().ToHashCode();
    }

    /// <inheritdoc />
    public static bool operator ==(UseFastRepairCommand left, UseFastRepairCommand right)
    {
        return left.Equals(right);
    }

    /// <inheritdoc />
    public static bool operator !=(UseFastRepairCommand left, UseFastRepairCommand right)
    {
        return !(left == right);
    }

    #endregion IEquatable Implementation
}
