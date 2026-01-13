using System.Diagnostics.CodeAnalysis;

namespace SharpRacer.Commands.PitService;

/// <summary>
/// Represents a simulator command that requests a tire be changed during the next pit stop, optionally specifying the tire pressure.
/// </summary>
public readonly struct ChangeTireCommand : ISimulatorCommand<ChangeTireCommand>
{
    /// <summary>
    /// Initializes a new <see cref="ChangeTireCommand"/> instance with the specified tire to change, making no changes to tire
    /// pressure.
    /// </summary>
    /// <param name="tire">The tire to change during the next pit service.</param>
    public ChangeTireCommand(TireChangeTarget tire)
        : this(tire, 0)
    {

    }

    /// <summary>
    /// Initializes a new <see cref="ChangeTireCommand"/> instance with the specified tire to change and the tire pressure to use.
    /// </summary>
    /// <param name="tire">The tire to change during the next pit service.</param>
    /// <param name="pressureKPa">The tire pressure, in kilo-Pascals, to use for the new tire.</param>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="pressureKPa"/> is negative.</exception>
    public ChangeTireCommand(TireChangeTarget tire, int pressureKPa)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(pressureKPa);

        Tire = tire;
        PressureKPa = pressureKPa;
    }

    /// <inheritdoc />
    public static ushort CommandId { get; } = (ushort)SimulatorCommandId.PitCommand;

    /// <summary>
    /// Gets the tire pressure, in kilo-Pascals, to use for the new tire. This property returns zero if no tire pressure was specified.
    /// </summary>
    public readonly int PressureKPa { get; }

    /// <summary>
    /// Gets the tire to change.
    /// </summary>
    public readonly TireChangeTarget Tire { get; }

    /// <inheritdoc />
    public readonly CommandMessage ToCommandMessage()
    {
        var tireChangeArg = (ushort)((ushort)PitServiceCommandType.TireChange + (byte)Tire);

        return new CommandMessage(CommandId, tireChangeArg, PressureKPa);
    }

    /// <inheritdoc />
    public static ChangeTireCommand Parse(CommandMessage message)
    {
        CommandMessageParseException.ThrowIfCommandIdNotEqual(message, CommandId);

        var reader = new CommandMessageReader(ref message);

        const ushort tireChangeBase = (ushort)PitServiceCommandType.TireChange;

        var tire = (TireChangeTarget)(reader.ReadArg1() - tireChangeBase);

        if (!Enum.IsDefined(typeof(TireChangeTarget), tire))
        {
            throw new CommandMessageParseException(
                $"'{(int)tire}' is not a valid {nameof(TireChangeTarget)} value.");
        }

        var pressure = reader.ReadArg2Int();

        if (pressure < 0)
        {
            throw new CommandMessageParseException(
                $"Read invalid tire pressure value: {pressure}. Tire pressure must be greater than or equal to zero.");
        }

        return new ChangeTireCommand(tire, pressure);
    }

    /// <inheritdoc />
    public static bool TryParse(CommandMessage message, out ChangeTireCommand result)
    {
        if (message.CommandId != CommandId)
        {
            result = default;
            return false;
        }

        var reader = new CommandMessageReader(ref message);

        const ushort tireChangeBase = (ushort)PitServiceCommandType.TireChange;

        var tire = (TireChangeTarget)(reader.ReadArg1() - tireChangeBase);

        if (!Enum.IsDefined(typeof(TireChangeTarget), tire))
        {
            result = default;
            return false;
        }

        var pressure = reader.ReadArg2Int();

        if (pressure < 0)
        {
            result = default;
            return false;
        }

        result = new ChangeTireCommand(tire, pressure);
        return true;
    }

    /// <inheritdoc />
    public static implicit operator CommandMessage(ChangeTireCommand command)
    {
        return command.ToCommandMessage();
    }

    #region IEquatable Implementation

    /// <inheritdoc />
    public override readonly bool Equals([NotNullWhen(true)] object? obj)
    {
        return obj is ChangeTireCommand command && Equals(command);
    }

    /// <inheritdoc />
    public readonly bool Equals(ChangeTireCommand other)
    {
        return Tire == other.Tire && PressureKPa == other.PressureKPa;
    }

    /// <inheritdoc />
    public override readonly int GetHashCode()
    {
        return HashCode.Combine(Tire, PressureKPa);
    }

    /// <inheritdoc />
    public static bool operator ==(ChangeTireCommand left, ChangeTireCommand right)
    {
        return left.Equals(right);
    }

    /// <inheritdoc />
    public static bool operator !=(ChangeTireCommand left, ChangeTireCommand right)
    {
        return !(left == right);
    }

    #endregion IEquatable Implementation
}
