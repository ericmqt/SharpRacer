using System.Diagnostics.CodeAnalysis;

namespace SharpRacer.Commands.ForceFeedback;

/// <summary>
/// Represents a simulator command that configures the maximum force to apply to the steering wheel when generating force-feedback.
/// </summary>
public readonly struct SetMaxForceCommand : ISimulatorCommand<SetMaxForceCommand>
{
    /// <summary>
    /// Initializes a new <see cref="SetMaxForceCommand"/> instance with the specified maximum force in Newton-meters.
    /// </summary>
    /// <param name="maxForceNm">The maximum force in Newton-meters.</param>
    public SetMaxForceCommand(float maxForceNm)
    {
        MaxForceNm = maxForceNm;
    }

    /// <inheritdoc />
    public static ushort CommandId { get; } = (ushort)SimulatorCommandId.ForceFeedbackCommand;

    /// <summary>
    /// The maximum force in Newton-meters.
    /// </summary>
    public readonly float MaxForceNm { get; }

    /// <inheritdoc />
    public readonly CommandMessage ToCommandMessage()
    {
        return new CommandMessage(CommandId, 0, MaxForceNm);
    }

    /// <inheritdoc />
    public static SetMaxForceCommand Parse(CommandMessage message)
    {
        CommandMessageParseException.ThrowIfCommandIdNotEqual(message, CommandId);

        var reader = new CommandMessageReader(ref message);

        return new SetMaxForceCommand(reader.ReadArg2Float());
    }

    /// <inheritdoc />
    public static bool TryParse(CommandMessage message, out SetMaxForceCommand result)
    {
        if (message.CommandId != CommandId)
        {
            result = default;
            return false;
        }

        var reader = new CommandMessageReader(ref message);

        result = new SetMaxForceCommand(reader.ReadArg2Float());
        return true;
    }

    /// <inheritdoc />
    public static implicit operator CommandMessage(SetMaxForceCommand command)
    {
        return command.ToCommandMessage();
    }

    #region IEquatable Implementation

    /// <inheritdoc />
    public override readonly bool Equals([NotNullWhen(true)] object? obj)
    {
        return obj is SetMaxForceCommand command && Equals(command);
    }

    /// <inheritdoc />
    public readonly bool Equals(SetMaxForceCommand other)
    {
        return MaxForceNm == other.MaxForceNm;
    }

    /// <inheritdoc />
    public override readonly int GetHashCode()
    {
        return HashCode.Combine(MaxForceNm);
    }

    /// <inheritdoc />
    public static bool operator ==(SetMaxForceCommand left, SetMaxForceCommand right)
    {
        return left.Equals(right);
    }

    /// <inheritdoc />
    public static bool operator !=(SetMaxForceCommand left, SetMaxForceCommand right)
    {
        return !(left == right);
    }

    #endregion IEquatable Implementation
}
