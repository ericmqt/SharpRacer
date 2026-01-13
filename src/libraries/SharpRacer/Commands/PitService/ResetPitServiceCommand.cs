using System.Diagnostics.CodeAnalysis;

namespace SharpRacer.Commands.PitService;

/// <summary>
/// Represents a simulator command that clears or resets requested pit services, equivalent to unchecking the relevant checkbox in the pit
/// services black box.
/// </summary>
public readonly struct ResetPitServiceCommand : ISimulatorCommand<ResetPitServiceCommand>
{
    /// <summary>
    /// Initializes a new <see cref="ResetPitServiceCommand"/> instance to reset the specified pit service option.
    /// </summary>
    /// <param name="service"></param>
    public ResetPitServiceCommand(PitServiceResetType service)
    {
        Service = service;
    }

    /// <inheritdoc />
    public static ushort CommandId { get; } = (ushort)SimulatorCommandId.PitCommand;

    /// <summary>
    /// Gets a value indicating the pit service that will be reset when the command is executed.
    /// </summary>
    public readonly PitServiceResetType Service { get; }

    /// <inheritdoc />
    public readonly CommandMessage ToCommandMessage()
    {
        return new CommandMessage(CommandId, (ushort)Service);
    }

    /// <inheritdoc />
    public static ResetPitServiceCommand Parse(CommandMessage message)
    {
        CommandMessageParseException.ThrowIfCommandIdNotEqual(message, CommandId);

        PitServiceResetType service;

        var reader = new CommandMessageReader(ref message);

        try { service = reader.ReadArg1<PitServiceResetType>(); }
        catch (CommandMessageReaderException ex)
        {
            throw CommandMessageParseException.Arg1ReaderException(ex);
        }

        return new ResetPitServiceCommand(service);
    }

    /// <inheritdoc />
    public static bool TryParse(CommandMessage message, out ResetPitServiceCommand result)
    {
        if (message.CommandId != CommandId)
        {
            result = default;
            return false;
        }

        var reader = new CommandMessageReader(ref message);

        if (!reader.TryReadArg1<PitServiceResetType>(out var service))
        {
            result = default;
            return false;
        }

        result = new ResetPitServiceCommand(service);
        return true;
    }

    /// <inheritdoc />
    public static implicit operator CommandMessage(ResetPitServiceCommand command)
    {
        return command.ToCommandMessage();
    }

    #region IEquatable Implementation

    /// <inheritdoc />
    public override readonly bool Equals([NotNullWhen(true)] object? obj)
    {
        return obj is ResetPitServiceCommand command && Equals(command);
    }

    /// <inheritdoc />
    public readonly bool Equals(ResetPitServiceCommand other)
    {
        return Service == other.Service;
    }

    /// <inheritdoc />
    public override readonly int GetHashCode()
    {
        return HashCode.Combine(Service);
    }

    /// <inheritdoc />
    public static bool operator ==(ResetPitServiceCommand left, ResetPitServiceCommand right)
    {
        return left.Equals(right);
    }

    /// <inheritdoc />
    public static bool operator !=(ResetPitServiceCommand left, ResetPitServiceCommand right)
    {
        return !(left == right);
    }

    #endregion IEquatable Implementation
}
