using System.Diagnostics.CodeAnalysis;

namespace SharpRacer.Commands.Camera;

/// <summary>
/// Represents a simulator command that directs the camera to target a specific car.
/// </summary>
public readonly struct TargetCarCommand : ISimulatorCommand<TargetCarCommand>
{
    private readonly ushort _carNumber;

    /// <summary>
    /// Initializes a new <see cref="TargetCarCommand"/> instance with the specified car number.
    /// </summary>
    /// <param name="carNumber">The number of the car to target.</param>
    /// <param name="cameraGroup">The camera group.</param>
    /// <param name="cameraIndex">The camera index.</param>
    public TargetCarCommand(CarNumber carNumber, int cameraGroup, int cameraIndex)
        : this(carNumber, CameraTargetMode.Driver, (ushort)cameraGroup, (ushort)cameraIndex)
    {

    }

    /// <summary>
    /// Initializes a new <see cref="TargetCarCommand"/> instance with the specified
    /// <see cref="CameraTargetMode">CameraTargetMode</see>.
    /// </summary>
    /// <param name="targetMode">The target mode.</param>
    /// <param name="cameraGroup">The camera group.</param>
    /// <param name="cameraIndex">The camera index.</param>
    public TargetCarCommand(CameraTargetMode targetMode, int cameraGroup, int cameraIndex)
        : this(CarNumber.None, targetMode, (ushort)cameraGroup, (ushort)cameraIndex)
    {

    }

    internal TargetCarCommand(CarNumber carNumber, CameraTargetMode targetMode, ushort cameraGroup, ushort cameraIndex)
    {
        if (carNumber == CarNumber.None && targetMode == CameraTargetMode.Driver)
        {
            throw new ArgumentException(
                $"'{nameof(targetMode)}' cannot be set to {nameof(CameraTargetMode.Driver)} when '{nameof(carNumber)}' is a default value.",
                nameof(targetMode));
        }

        if (carNumber != CarNumber.None && targetMode != CameraTargetMode.Driver)
        {
            throw new ArgumentException(
                $"'{nameof(targetMode)}' must be set to {nameof(CameraTargetMode.Driver)} when '{nameof(carNumber)}' is not a default value.",
                nameof(targetMode));
        }

        TargetMode = targetMode;
        CameraGroup = cameraGroup;
        CameraIndex = cameraIndex;

        _carNumber = (ushort)carNumber.Value;
    }

    /// <inheritdoc />
    public static ushort CommandId { get; } = (ushort)SimulatorCommandId.CameraTargetDriver;

    /// <summary>
    /// Gets the value indicating the targeted car if <see cref="TargetMode">TargetMode</see> is set to
    /// <see cref="CameraTargetMode.Driver"/>, otherwise this property returns <see cref="CarNumber.None"/>.
    /// </summary>
    public readonly CarNumber CarNumber => TargetMode == CameraTargetMode.Driver ? new CarNumber(_carNumber) : CarNumber.None;

    /// <summary>
    /// Gets the value indicating the camera group to use.
    /// </summary>
    public readonly ushort CameraGroup { get; }

    /// <summary>
    /// Gets the value indicating the index of the camera to use.
    /// </summary>
    public readonly ushort CameraIndex { get; }

    /// <summary>
    /// Gets the targeting mode to use.
    /// </summary>
    public readonly CameraTargetMode TargetMode { get; }

    /// <inheritdoc />
    public readonly CommandMessage ToCommandMessage()
    {
        if (TargetMode == CameraTargetMode.Driver)
        {
            return new CommandMessage(CommandId, (ushort)CarNumber.Value, CameraGroup, CameraIndex);
        }

        return new CommandMessage(CommandId, unchecked((ushort)TargetMode), CameraGroup, CameraIndex);
    }

    /// <inheritdoc />
    public static TargetCarCommand Parse(CommandMessage message)
    {
        CommandMessageParseException.ThrowIfCommandIdNotEqual(message, CommandId);

        var reader = new CommandMessageReader(ref message);

        CarNumber carNumber;
        CameraTargetMode targetMode;

        if ((short)message.Arg1 >= 0)
        {
            carNumber = new CarNumber(message.Arg1);
            targetMode = CameraTargetMode.Driver;
        }
        else
        {
            carNumber = CarNumber.None;

            try { targetMode = reader.ReadArg1<CameraTargetMode>(); }
            catch (CommandMessageReaderException ex)
            {
                throw CommandMessageParseException.Arg1ReaderException(ex);
            }
        }

        return new TargetCarCommand(carNumber, targetMode, message.Arg2, message.Arg3);
    }

    /// <inheritdoc />
    public static bool TryParse(CommandMessage message, out TargetCarCommand result)
    {
        if (message.CommandId != CommandId)
        {
            result = default;
            return false;
        }

        var reader = new CommandMessageReader(ref message);

        CarNumber carNumber;
        CameraTargetMode targetMode;

        if ((short)message.Arg1 >= 0)
        {
            carNumber = new CarNumber(message.Arg1);
            targetMode = CameraTargetMode.Driver;
        }
        else
        {
            carNumber = CarNumber.None;

            if (!reader.TryReadArg1(out targetMode))
            {
                result = default;
                return false;
            }
        }

        result = new TargetCarCommand(carNumber, targetMode, message.Arg2, message.Arg3);
        return true;
    }

    /// <inheritdoc />
    public static implicit operator CommandMessage(TargetCarCommand command)
    {
        return command.ToCommandMessage();
    }

    #region IEquatable Implementation

    /// <inheritdoc />
    public override readonly bool Equals([NotNullWhen(true)] object? obj)
    {
        return obj is TargetCarCommand command && Equals(command);
    }

    /// <inheritdoc />
    public readonly bool Equals(TargetCarCommand other)
    {
        return TargetMode == other.TargetMode &&
               CarNumber == other.CarNumber &&
               CameraGroup == other.CameraGroup &&
               CameraIndex == other.CameraIndex;
    }

    /// <inheritdoc />
    public override readonly int GetHashCode()
    {
        return HashCode.Combine(TargetMode, CarNumber, CameraGroup, CameraIndex);
    }

    /// <inheritdoc />
    public static bool operator ==(TargetCarCommand left, TargetCarCommand right)
    {
        return left.Equals(right);
    }

    /// <inheritdoc />
    public static bool operator !=(TargetCarCommand left, TargetCarCommand right)
    {
        return !(left == right);
    }

    #endregion IEquatable Implementation
}
