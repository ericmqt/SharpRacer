using System.Diagnostics.CodeAnalysis;

namespace SharpRacer.Commands.Replay;

/// <summary>
/// Represents a simulator command that controls replay playback speed.
/// </summary>
public readonly struct SetPlaySpeedCommand : ISimulatorCommand<SetPlaySpeedCommand>
{
    /// <summary>
    /// Initializes a new <see cref="SetPlaySpeedCommand"/> instance with the specified playback speed.
    /// </summary>
    /// <param name="playSpeed">The playback rate.</param>
    public SetPlaySpeedCommand(ushort playSpeed)
        : this(playSpeed, isSlowMotion: false)
    {

    }

    /// <summary>
    /// Initializes a new <see cref="SetPlaySpeedCommand"/> instance with the specified playback speed.
    /// </summary>
    /// <param name="playSpeed">The playback rate.</param>
    /// <param name="isSlowMotion">
    /// When set to <see langword="true"/>, the playback speed is set to the multiplicative inverse of <paramref name="playSpeed"/>.
    /// </param>
    public SetPlaySpeedCommand(ushort playSpeed, bool isSlowMotion)
    {
        PlaySpeed = playSpeed;
        IsSlowMotion = isSlowMotion;
    }

    /// <inheritdoc />
    public static ushort CommandId { get; } = (ushort)SimulatorCommandId.ReplaySetPlaySpeed;

    /// <summary>
    /// Indicates if playback speed is less than 1x. When <see langword="true"/>, the playback speed is the multiplicative inverse of
    /// <see cref="PlaySpeed">PlaySpeed</see>.
    /// </summary>
    public readonly bool IsSlowMotion { get; }

    /// <summary>
    /// Gets the playback rate. If <see cref="IsSlowMotion">IsSlowMotion</see> is set to <see langword="true"/>, then the playback speed
    /// will be the multiplicative inverse of the returned value (1/x).
    /// </summary>
    public readonly ushort PlaySpeed { get; }

    /// <inheritdoc />
    public readonly CommandMessage ToCommandMessage()
    {
        return new CommandMessage(CommandId, PlaySpeed, IsSlowMotion);
    }

    /// <inheritdoc />
    public static SetPlaySpeedCommand Parse(CommandMessage message)
    {
        CommandMessageParseException.ThrowIfCommandIdNotEqual(message, CommandId);

        var reader = new CommandMessageReader(ref message);

        var playSpeed = reader.ReadArg1();
        var isSlowMotion = reader.ReadArg2Bool();

        return new SetPlaySpeedCommand(playSpeed, isSlowMotion);
    }

    /// <inheritdoc />
    public static bool TryParse(CommandMessage message, out SetPlaySpeedCommand result)
    {
        if (message.CommandId != CommandId)
        {
            result = default;
            return false;
        }

        var reader = new CommandMessageReader(ref message);

        var playSpeed = reader.ReadArg1();
        var isSlowMotion = reader.ReadArg2Bool();

        result = new SetPlaySpeedCommand(playSpeed, isSlowMotion);
        return true;
    }

    /// <inheritdoc />
    public static implicit operator CommandMessage(SetPlaySpeedCommand command)
    {
        return command.ToCommandMessage();
    }

    #region IEquatable Implementation

    /// <inheritdoc />
    public override readonly bool Equals([NotNullWhen(true)] object? obj)
    {
        return obj is SetPlaySpeedCommand command && Equals(command);
    }

    /// <inheritdoc />
    public readonly bool Equals(SetPlaySpeedCommand other)
    {
        return PlaySpeed == other.PlaySpeed && IsSlowMotion == other.IsSlowMotion;
    }

    /// <inheritdoc />
    public override readonly int GetHashCode()
    {
        return HashCode.Combine(PlaySpeed, IsSlowMotion);
    }

    /// <inheritdoc />
    public static bool operator ==(SetPlaySpeedCommand left, SetPlaySpeedCommand right)
    {
        return left.Equals(right);
    }

    /// <inheritdoc />
    public static bool operator !=(SetPlaySpeedCommand left, SetPlaySpeedCommand right)
    {
        return !(left == right);
    }

    #endregion IEquatable Implementation
}
