using System.Diagnostics.CodeAnalysis;

namespace SharpRacer.Commands.PitService;

/// <summary>
/// Represents a simulator command that requests fuel be added during the next pit stop, optionally specifying the amount of fuel to add.
/// </summary>
public readonly struct AddFuelCommand : ISimulatorCommand<AddFuelCommand>
{
    /// <summary>
    /// Initializes a new <see cref="AddFuelCommand"/> instance without specifying the amount of fuel to add.
    /// </summary>
    public AddFuelCommand()
    {

    }

    /// <summary>
    /// Initializes a new <see cref="AddFuelCommand"/> instance with the specified number of liters of fuel to request during the next
    /// pit stop.
    /// </summary>
    /// <param name="fuelQuantityLiters">The amount of fuel to add, in liters.</param>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="fuelQuantityLiters"/> is negative.</exception>
    public AddFuelCommand(int fuelQuantityLiters)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(fuelQuantityLiters);

        FuelQuantity = fuelQuantityLiters;
    }

    /// <inheritdoc />
    public static ushort CommandId { get; } = (ushort)SimulatorCommandId.PitCommand;

    /// <summary>
    /// Gets the amount of fuel to add, in liters, during the next pit stop.
    /// </summary>
    public readonly int FuelQuantity { get; }

    /// <inheritdoc />
    public readonly CommandMessage ToCommandMessage()
    {
        return new CommandMessage(CommandId, (ushort)PitServiceCommandType.AddFuel, FuelQuantity);
    }

    /// <inheritdoc />
    public static AddFuelCommand Parse(CommandMessage message)
    {
        CommandMessageParseException.ThrowIfCommandIdNotEqual(message, CommandId);

        var reader = new CommandMessageReader(ref message);

        PitServiceCommandType serviceCommandType;

        try { serviceCommandType = reader.ReadArg1<PitServiceCommandType>(); }
        catch (CommandMessageReaderException ex)
        {
            throw CommandMessageParseException.Arg1ReaderException(ex);
        }

        if (serviceCommandType != PitServiceCommandType.AddFuel)
        {
            throw new CommandMessageParseException($"Read invalid {nameof(PitServiceCommandType)} value: {serviceCommandType}.");
        }

        var fuelQuantity = reader.ReadArg2Int();

        if (fuelQuantity < 0)
        {
            throw new CommandMessageParseException(
                $"Read invalid fuel quantity value: {fuelQuantity}. Fuel quantity must be greater than or equal to zero.");
        }

        return new AddFuelCommand(fuelQuantity);
    }

    /// <inheritdoc />
    public static bool TryParse(CommandMessage message, out AddFuelCommand result)
    {
        if (message.CommandId != CommandId)
        {
            result = default;
            return false;
        }

        var reader = new CommandMessageReader(ref message);

        if (!reader.TryReadArg1<PitServiceCommandType>(out var service) || service != PitServiceCommandType.AddFuel)
        {
            result = default;
            return false;
        }

        var fuelQuantity = reader.ReadArg2Int();

        if (fuelQuantity < 0)
        {
            result = default;
            return false;
        }

        result = new AddFuelCommand(fuelQuantity);
        return true;
    }

    /// <inheritdoc />
    public static implicit operator CommandMessage(AddFuelCommand command)
    {
        return command.ToCommandMessage();
    }

    #region IEquatable Implementation

    /// <inheritdoc />
    public override readonly bool Equals([NotNullWhen(true)] object? obj)
    {
        return obj is AddFuelCommand command && Equals(command);
    }

    /// <inheritdoc />
    public readonly bool Equals(AddFuelCommand other)
    {
        return FuelQuantity == other.FuelQuantity;
    }

    /// <inheritdoc />
    public override readonly int GetHashCode()
    {
        return HashCode.Combine(FuelQuantity);
    }

    /// <inheritdoc />
    public static bool operator ==(AddFuelCommand left, AddFuelCommand right)
    {
        return left.Equals(right);
    }

    /// <inheritdoc />
    public static bool operator !=(AddFuelCommand left, AddFuelCommand right)
    {
        return !(left == right);
    }

    #endregion IEquatable Implementation
}
