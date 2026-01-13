using System.Diagnostics.CodeAnalysis;

namespace SharpRacer.Commands.Camera;

/// <summary>
/// Represents a simulator command that sets the camera state.
/// </summary>
public readonly struct SetCameraStateCommand : ISimulatorCommand<SetCameraStateCommand>
{
    /// <summary>
    /// Initializes a new <see cref="SetCameraStateCommand"/> instance with the specified <see cref="CameraState">CameraState</see>.
    /// </summary>
    /// <param name="cameraState">The <see cref="CameraState">CameraState</see> value to send to the simulator.</param>
    public SetCameraStateCommand(CameraState cameraState)
    {
        State = cameraState;
    }

    /// <inheritdoc />
    public static ushort CommandId { get; } = (ushort)SimulatorCommandId.CameraSetState;

    /// <summary>
    /// Gets the value that specifies the desired state of the simulator camera tool.
    /// </summary>
    public readonly CameraState State { get; }

    /// <inheritdoc />
    public readonly CommandMessage ToCommandMessage()
    {
        return new CommandMessage(CommandId, (ushort)State);
    }

    /// <inheritdoc />
    public static SetCameraStateCommand Parse(CommandMessage message)
    {
        CommandMessageParseException.ThrowIfCommandIdNotEqual(message, CommandId);

        var reader = new CommandMessageReader(ref message);

        var state = reader.ReadArg1Flags<CameraState>();

        return new SetCameraStateCommand(state);
    }

    /// <inheritdoc />
    public static bool TryParse(CommandMessage message, out SetCameraStateCommand result)
    {
        if (message.CommandId != CommandId)
        {
            result = default;
            return false;
        }

        var reader = new CommandMessageReader(ref message);

        var state = reader.ReadArg1Flags<CameraState>();

        result = new SetCameraStateCommand(state);
        return true;
    }

    /// <inheritdoc />
    public static implicit operator CommandMessage(SetCameraStateCommand command)
    {
        return command.ToCommandMessage();
    }

    #region IEquatable Implementation

    /// <inheritdoc />
    public override readonly bool Equals([NotNullWhen(true)] object? obj)
    {
        return obj is SetCameraStateCommand command && Equals(command);
    }

    /// <inheritdoc />
    public readonly bool Equals(SetCameraStateCommand other)
    {
        return State == other.State;
    }

    /// <inheritdoc />
    public override readonly int GetHashCode()
    {
        return HashCode.Combine(State);
    }

    /// <inheritdoc />
    public static bool operator ==(SetCameraStateCommand left, SetCameraStateCommand right)
    {
        return left.Equals(right);
    }

    /// <inheritdoc />
    public static bool operator !=(SetCameraStateCommand left, SetCameraStateCommand right)
    {
        return !(left == right);
    }

    #endregion IEquatable Implementation
}
