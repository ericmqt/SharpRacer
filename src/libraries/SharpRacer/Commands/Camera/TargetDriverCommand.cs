using System.Diagnostics.CodeAnalysis;

namespace SharpRacer.Commands.Camera;

/// <summary>
/// Represents a simulator command that directs the camera to target a specific driver.
/// </summary>
public readonly struct TargetDriverCommand : ISimulatorCommand<TargetDriverCommand>
{
    private readonly ushort _driverNumber;

    /// <summary>
    /// Initializes a new <see cref="TargetDriverCommand"/> instance with the specified
    /// <see cref="CameraTargetMode">CameraTargetMode</see>.
    /// </summary>
    /// <param name="targetMode">The target mode.</param>
    /// <param name="cameraGroup">The camera group.</param>
    /// <param name="cameraIndex">The camera index.</param>
    public TargetDriverCommand(CameraTargetMode targetMode, int cameraGroup, int cameraIndex)
        : this(targetMode, 0, (ushort)cameraGroup, (ushort)cameraIndex)
    {

    }

    /// <summary>
    /// Initializes a new <see cref="TargetDriverCommand"/> instance to target the car driven by the specified driver.
    /// </summary>
    /// <param name="driverNumber">The number of the target driver.</param>
    /// <param name="cameraGroup">The camera group.</param>
    /// <param name="cameraIndex">The camera index.</param>
    public TargetDriverCommand(int driverNumber, int cameraGroup, int cameraIndex)
        : this(CameraTargetMode.Driver, (ushort)driverNumber, (ushort)cameraGroup, (ushort)cameraIndex)
    {

    }

    internal TargetDriverCommand(CameraTargetMode targetMode, ushort driverNumber, ushort cameraGroup, ushort cameraIndex)
    {
        if (driverNumber > 0 && targetMode != CameraTargetMode.Driver)
        {
            throw new ArgumentException(
                $"'{nameof(driverNumber)}' must be zero when '{nameof(targetMode)}' is not set to {nameof(CameraTargetMode.Driver)}.",
                nameof(driverNumber));
        }

        TargetMode = targetMode;
        CameraGroup = cameraGroup;
        CameraIndex = cameraIndex;

        _driverNumber = targetMode == CameraTargetMode.Driver ? driverNumber : (ushort)0;
    }

    /// <inheritdoc />
    public static ushort CommandId { get; } = (ushort)SimulatorCommandId.CameraTargetDriver;

    /// <summary>
    /// Gets the value indicating the targeted driver number if <see cref="TargetMode">TargetMode</see> is set to
    /// <see cref="CameraTargetMode.Driver"/>, otherwise this property returns zero.
    /// </summary>
    public readonly ushort DriverNumber => TargetMode == CameraTargetMode.Driver ? _driverNumber : (ushort)0;

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
            return new CommandMessage(CommandId, DriverNumber, CameraGroup, CameraIndex);
        }

        return new CommandMessage(CommandId, unchecked((ushort)TargetMode), CameraGroup, CameraIndex);
    }

    /// <inheritdoc />
    public static TargetDriverCommand Parse(CommandMessage message)
    {
        CommandMessageParseException.ThrowIfCommandIdNotEqual(message, CommandId);

        var reader = new CommandMessageReader(ref message);

        var targetMode = CameraTargetMode.Driver;
        ushort driverNumber = message.Arg1;

        if ((short)message.Arg1 < 0)
        {
            driverNumber = 0;

            try { targetMode = reader.ReadArg1<CameraTargetMode>(); }
            catch (CommandMessageReaderException ex)
            {
                throw CommandMessageParseException.Arg1ReaderException(ex);
            }
        }

        return new TargetDriverCommand(targetMode, driverNumber, message.Arg2, message.Arg3);
    }

    /// <inheritdoc />
    public static bool TryParse(CommandMessage message, out TargetDriverCommand result)
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

        result = new TargetDriverCommand(targetMode, driverNumber, message.Arg2, message.Arg3);
        return true;
    }

    /// <inheritdoc />
    public static implicit operator CommandMessage(TargetDriverCommand command)
    {
        return command.ToCommandMessage();
    }

    #region IEquatable Implementation

    /// <inheritdoc />
    public override readonly bool Equals([NotNullWhen(true)] object? obj)
    {
        return obj is TargetDriverCommand command && Equals(command);
    }

    /// <inheritdoc />
    public readonly bool Equals(TargetDriverCommand other)
    {
        return TargetMode == other.TargetMode &&
               _driverNumber == other._driverNumber &&
               CameraGroup == other.CameraGroup &&
               CameraIndex == other.CameraIndex;
    }

    /// <inheritdoc />
    public override readonly int GetHashCode()
    {
        return HashCode.Combine(TargetMode, _driverNumber, CameraGroup, CameraIndex);
    }

    /// <inheritdoc />
    public static bool operator ==(TargetDriverCommand left, TargetDriverCommand right)
    {
        return left.Equals(right);
    }

    /// <inheritdoc />
    public static bool operator !=(TargetDriverCommand left, TargetDriverCommand right)
    {
        return !(left == right);
    }

    #endregion IEquatable Implementation
}
