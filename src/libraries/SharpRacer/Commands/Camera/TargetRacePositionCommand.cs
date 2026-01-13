using System.Diagnostics.CodeAnalysis;

namespace SharpRacer.Commands.Camera;

/// <summary>
/// Represents a simulator command that directs the camera to target a car in a specific race position.
/// </summary>
public readonly struct TargetRacePositionCommand : ISimulatorCommand<TargetRacePositionCommand>
{
    private readonly ushort _carPosition;

    /// <summary>
    /// Initializes a new <see cref="TargetRacePositionCommand"/> instance with the specified
    /// <see cref="CameraTargetMode">CameraTargetMode</see>.
    /// </summary>
    /// <param name="targetMode">The target mode.</param>
    /// <param name="cameraGroup">The camera group.</param>
    /// <param name="cameraIndex">The camera index.</param>
    public TargetRacePositionCommand(CameraTargetMode targetMode, int cameraGroup, int cameraIndex)
        : this(targetMode, 0, (ushort)cameraGroup, (ushort)cameraIndex)
    {

    }

    /// <summary>
    /// Initializes a new <see cref="TargetRacePositionCommand"/> instance to target the car in the specified race position.
    /// </summary>
    /// <param name="carPosition">The position of the target car.</param>
    /// <param name="cameraGroup">The camera group.</param>
    /// <param name="cameraIndex">The camera index.</param>
    public TargetRacePositionCommand(int carPosition, int cameraGroup, int cameraIndex)
        : this(CameraTargetMode.Driver, (ushort)carPosition, (ushort)cameraGroup, (ushort)cameraIndex)
    {

    }

    internal TargetRacePositionCommand(CameraTargetMode targetMode, ushort carPosition, ushort cameraGroup, ushort cameraIndex)
    {
        if (carPosition > 0 && targetMode != CameraTargetMode.Driver)
        {
            throw new ArgumentException(
                $"'{nameof(carPosition)}' must be zero when '{nameof(targetMode)}' is not set to {nameof(CameraTargetMode.Driver)}.",
                nameof(carPosition));
        }

        TargetMode = targetMode;
        CameraGroup = cameraGroup;
        CameraIndex = cameraIndex;

        _carPosition = targetMode == CameraTargetMode.Driver ? carPosition : (ushort)0;
    }

    /// <inheritdoc />
    public static ushort CommandId { get; } = (ushort)SimulatorCommandId.CameraSwitchPosition;

    /// <summary>
    /// Gets the value indicating the camera group to use.
    /// </summary>
    public readonly ushort CameraGroup { get; }

    /// <summary>
    /// Gets the value indicating the index of the camera to use.
    /// </summary>
    public readonly ushort CameraIndex { get; }

    /// <summary>
    /// Gets the value representing the position of the targeted car if <see cref="TargetMode">TargetMode</see> is set to
    /// <see cref="CameraTargetMode.Driver"/>, otherwise this property returns zero.
    /// </summary>
    public readonly ushort Position => TargetMode == CameraTargetMode.Driver ? _carPosition : (ushort)0;

    /// <summary>
    /// Gets the targeting mode to use.
    /// </summary>
    public readonly CameraTargetMode TargetMode { get; }

    /// <inheritdoc />
    public readonly CommandMessage ToCommandMessage()
    {
        if (TargetMode == CameraTargetMode.Driver)
        {
            return new CommandMessage(CommandId, Position, CameraGroup, CameraIndex);
        }

        return new CommandMessage(CommandId, unchecked((ushort)TargetMode), CameraGroup, CameraIndex);
    }

    /// <inheritdoc />
    public static TargetRacePositionCommand Parse(CommandMessage message)
    {
        CommandMessageParseException.ThrowIfCommandIdNotEqual(message, CommandId);

        var reader = new CommandMessageReader(ref message);

        var targetMode = CameraTargetMode.Driver;
        ushort racePosition = message.Arg1;

        if ((short)message.Arg1 < 0)
        {
            racePosition = 0;

            try { targetMode = reader.ReadArg1<CameraTargetMode>(); }
            catch (CommandMessageReaderException ex)
            {
                throw CommandMessageParseException.Arg1ReaderException(ex);
            }
        }

        return new TargetRacePositionCommand(targetMode, racePosition, message.Arg2, message.Arg3);
    }

    /// <inheritdoc />
    public static bool TryParse(CommandMessage message, out TargetRacePositionCommand result)
    {
        if (message.CommandId != CommandId)
        {
            result = default;
            return false;
        }

        var reader = new CommandMessageReader(ref message);

        var targetMode = CameraTargetMode.Driver;
        ushort driverNumber = message.Arg1;

        if ((short)message.Arg1 < 0)
        {
            driverNumber = 0;

            if (!reader.TryReadArg1(out targetMode))
            {
                result = default;
                return false;
            }
        }

        result = new TargetRacePositionCommand(targetMode, driverNumber, message.Arg2, message.Arg3);
        return true;
    }

    /// <inheritdoc />
    public static implicit operator CommandMessage(TargetRacePositionCommand command)
    {
        return command.ToCommandMessage();
    }

    #region IEquatable Implementation

    /// <inheritdoc />
    public override readonly bool Equals([NotNullWhen(true)] object? obj)
    {
        return obj is TargetRacePositionCommand command && Equals(command);
    }

    /// <inheritdoc />
    public readonly bool Equals(TargetRacePositionCommand other)
    {
        return TargetMode == other.TargetMode &&
               _carPosition == other._carPosition &&
               CameraGroup == other.CameraGroup &&
               CameraIndex == other.CameraIndex;
    }

    /// <inheritdoc />
    public override readonly int GetHashCode()
    {
        return HashCode.Combine(_carPosition, CameraGroup, CameraIndex, TargetMode);
    }

    /// <inheritdoc />
    public static bool operator ==(TargetRacePositionCommand left, TargetRacePositionCommand right)
    {
        return left.Equals(right);
    }

    /// <inheritdoc />
    public static bool operator !=(TargetRacePositionCommand left, TargetRacePositionCommand right)
    {
        return !(left == right);
    }

    #endregion IEquatable Implementation
}
