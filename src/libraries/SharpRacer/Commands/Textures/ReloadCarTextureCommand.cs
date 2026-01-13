using System.Diagnostics.CodeAnalysis;

namespace SharpRacer.Commands.Textures;

/// <summary>
/// Represents a simulator command that reloads the texture of a specific car.
/// </summary>
public readonly struct ReloadCarTextureCommand : ISimulatorCommand<ReloadCarTextureCommand>
{
    /// <summary>
    /// Initializes a new <see cref="ReloadCarTextureCommand"/> instance to reload the textures for all cars.
    /// </summary>
    public ReloadCarTextureCommand()
        : this((ushort)0)
    {

    }

    /// <summary>
    /// Initializes a new <see cref="ReloadCarTextureCommand"/> instance to reload the texture of a car by index.
    /// </summary>
    /// <param name="carIndex">The index of the car whose texture will be reloaded.</param>
    public ReloadCarTextureCommand(int carIndex)
        : this((ushort)carIndex)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(carIndex);
    }

    private ReloadCarTextureCommand(ushort carIndex)
    {
        CarIndex = carIndex;
    }

    /// <inheritdoc />
    public static ushort CommandId { get; } = (ushort)SimulatorCommandId.ReloadTextures;

    /// <summary>
    /// Gets the index of the car whose texture will be reloaded when the command is executed or returns zero if the textures for all cars
    /// will be reloaded.
    /// </summary>
    public readonly ushort CarIndex { get; }

    /// <inheritdoc />
    public readonly CommandMessage ToCommandMessage()
    {
        return new CommandMessage(CommandId, CarIndex);
    }

    /// <inheritdoc />
    public static ReloadCarTextureCommand Parse(CommandMessage message)
    {
        CommandMessageParseException.ThrowIfCommandIdNotEqual(message, CommandId);

        return new ReloadCarTextureCommand(message.Arg1);
    }

    /// <inheritdoc />
    public static bool TryParse(CommandMessage message, out ReloadCarTextureCommand result)
    {
        if (message.CommandId != CommandId)
        {
            result = default;
            return false;
        }

        result = new ReloadCarTextureCommand(message.Arg1);
        return true;
    }

    /// <inheritdoc />
    public static implicit operator CommandMessage(ReloadCarTextureCommand command)
    {
        return command.ToCommandMessage();
    }

    #region IEquatable Implementation

    /// <inheritdoc />
    public override readonly bool Equals([NotNullWhen(true)] object? obj)
    {
        return obj is ReloadCarTextureCommand command && Equals(command);
    }

    /// <inheritdoc />
    public readonly bool Equals(ReloadCarTextureCommand other)
    {
        return CarIndex == other.CarIndex;
    }

    /// <inheritdoc />
    public override readonly int GetHashCode()
    {
        return HashCode.Combine(CarIndex);
    }

    /// <inheritdoc />
    public static bool operator ==(ReloadCarTextureCommand left, ReloadCarTextureCommand right)
    {
        return left.Equals(right);
    }

    /// <inheritdoc />
    public static bool operator !=(ReloadCarTextureCommand left, ReloadCarTextureCommand right)
    {
        return !(left == right);
    }

    #endregion IEquatable Implementation
}
