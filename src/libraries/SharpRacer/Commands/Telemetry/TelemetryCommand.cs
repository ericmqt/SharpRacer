using System.Diagnostics.CodeAnalysis;

namespace SharpRacer.Commands.Telemetry;

/// <summary>
/// Represents a simulator command that controls telemetry recording.
/// </summary>
public readonly struct TelemetryCommand : ISimulatorCommand<TelemetryCommand>
{
    /// <summary>
    /// Initializes a new <see cref="TelemetryCommand"/> with the specified <see cref="TelemetryCommandType">TelemetryCommandType</see>.
    /// </summary>
    public TelemetryCommand(TelemetryCommandType type)
    {
        Type = type;
    }

    /// <inheritdoc />
    public static ushort CommandId { get; } = (ushort)SimulatorCommandId.Telemetry;

    /// <summary>
    /// Gets a value that indicates the telemetry command to execute.
    /// </summary>
    public readonly TelemetryCommandType Type { get; }

    /// <inheritdoc />
    public readonly CommandMessage ToCommandMessage()
    {
        return new CommandMessage(CommandId, (ushort)Type);
    }

    /// <inheritdoc />
    public static TelemetryCommand Parse(CommandMessage message)
    {
        CommandMessageParseException.ThrowIfCommandIdNotEqual(message, CommandId);

        var reader = new CommandMessageReader(ref message);

        TelemetryCommandType telemetryCommandType;

        try { telemetryCommandType = reader.ReadArg1<TelemetryCommandType>(); }
        catch (CommandMessageReaderException ex)
        {
            throw CommandMessageParseException.Arg1ReaderException(ex);
        }

        return new TelemetryCommand(telemetryCommandType);
    }

    /// <inheritdoc />
    public static bool TryParse(CommandMessage message, out TelemetryCommand result)
    {
        if (message.CommandId != CommandId)
        {
            result = default;
            return false;
        }

        var reader = new CommandMessageReader(ref message);

        if (!reader.TryReadArg1<TelemetryCommandType>(out var telemetryCommandType))
        {
            result = default;
            return false;
        }

        result = new TelemetryCommand(telemetryCommandType);
        return true;
    }

    /// <inheritdoc />
    public static implicit operator CommandMessage(TelemetryCommand command)
    {
        return command.ToCommandMessage();
    }

    #region IEquatable Implementation

    /// <inheritdoc />
    public override readonly bool Equals([NotNullWhen(true)] object? obj)
    {
        return obj is TelemetryCommand command && Equals(command);
    }

    /// <inheritdoc />
    public readonly bool Equals(TelemetryCommand other)
    {
        return Type == other.Type;
    }

    /// <inheritdoc />
    public override readonly int GetHashCode()
    {
        return HashCode.Combine(Type);
    }

    /// <inheritdoc />
    public static bool operator ==(TelemetryCommand left, TelemetryCommand right)
    {
        return left.Equals(right);
    }

    /// <inheritdoc />
    public static bool operator !=(TelemetryCommand left, TelemetryCommand right)
    {
        return !(left == right);
    }

    #endregion IEquatable Implementation
}
