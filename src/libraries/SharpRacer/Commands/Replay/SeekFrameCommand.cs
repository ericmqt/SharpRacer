using System.Diagnostics.CodeAnalysis;

namespace SharpRacer.Commands.Replay;

/// <summary>
/// Represents a simulator command that seeks to a specific frame relative to an origin point.
/// </summary>
public readonly struct SeekFrameCommand : ISimulatorCommand<SeekFrameCommand>
{
    /// <summary>
    /// Initializes a new <see cref="SeekFrameCommand"/> instance to seek to the specified frame relative to the current position.
    /// </summary>
    /// <param name="frame">The frame to seek, relative to the current position.</param>
    public SeekFrameCommand(int frame)
        : this(frame, ReplaySeekOrigin.Current)
    {

    }

    /// <summary>
    /// Initializes a new <see cref="SeekFrameCommand"/> instance to seek to the specified frame number relative to the seek origin.
    /// </summary>
    /// <param name="frame">
    /// The frame to seek relative to <paramref name="seekOrigin"/>.
    /// </param>
    /// <param name="seekOrigin">
    /// Specifies the beginning, the end, or the current position as a reference point for <paramref name="frame"/>.
    /// </param>
    public SeekFrameCommand(int frame, ReplaySeekOrigin seekOrigin)
    {
        if (seekOrigin == ReplaySeekOrigin.Begin && frame < 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(frame),
                $"'{nameof(frame)}' must be greater than or equal to zero when '{nameof(seekOrigin)}' is {ReplaySeekOrigin.Begin}.");
        }

        if (seekOrigin == ReplaySeekOrigin.End && frame > 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(frame),
                $"'{nameof(frame)}' must be less than or equal to zero when '{nameof(seekOrigin)}' is {ReplaySeekOrigin.End}.");
        }

        Frame = frame;
        SeekOrigin = seekOrigin;
    }

    /// <inheritdoc />
    public static ushort CommandId { get; } = (ushort)SimulatorCommandId.ReplaySetPlayPosition;

    /// <summary>
    /// Gets the frame to seek, relative to <see cref="SeekOrigin">SeekOrigin</see>.
    /// </summary>
    public readonly int Frame { get; }

    /// <summary>
    /// The point from which to begin seeking for the configured frame number.
    /// </summary>
    public readonly ReplaySeekOrigin SeekOrigin { get; }

    /// <inheritdoc />
    public readonly CommandMessage ToCommandMessage()
    {
        return new CommandMessage(CommandId, (ushort)SeekOrigin, Frame);
    }

    /// <inheritdoc />
    public static SeekFrameCommand Parse(CommandMessage message)
    {
        CommandMessageParseException.ThrowIfCommandIdNotEqual(message, CommandId);

        var reader = new CommandMessageReader(ref message);

        ReplaySeekOrigin seekOrigin;

        try { seekOrigin = reader.ReadArg1<ReplaySeekOrigin>(); }
        catch (CommandMessageReaderException ex)
        {
            throw CommandMessageParseException.Arg1ReaderException(ex);
        }

        var frame = reader.ReadArg2Int();

        if (seekOrigin == ReplaySeekOrigin.Begin && frame < 0)
        {
            throw new CommandMessageParseException(
                $"Invalid frame value ({frame}) for {nameof(SeekOrigin)} {seekOrigin}: Frame must be greater than or equal to zero.");
        }

        if (seekOrigin == ReplaySeekOrigin.End && frame > 0)
        {
            throw new CommandMessageParseException(
                $"Invalid frame value ({frame}) for {nameof(SeekOrigin)} {seekOrigin}: Frame must be less than or equal to zero.");
        }

        return new SeekFrameCommand(frame, seekOrigin);
    }

    /// <inheritdoc />
    public static bool TryParse(CommandMessage message, out SeekFrameCommand result)
    {
        result = default;

        if (message.CommandId != CommandId)
        {
            return false;
        }

        var reader = new CommandMessageReader(ref message);

        if (!reader.TryReadArg1<ReplaySeekOrigin>(out var seekOrigin))
        {
            return false;
        }

        var frame = reader.ReadArg2Int();

        if (seekOrigin == ReplaySeekOrigin.Begin && frame < 0)
        {
            return false;
        }
        else if (seekOrigin == ReplaySeekOrigin.End && frame > 0)
        {
            return false;
        }

        result = new SeekFrameCommand(frame, seekOrigin);
        return true;
    }

    /// <inheritdoc />
    public static implicit operator CommandMessage(SeekFrameCommand command)
    {
        return command.ToCommandMessage();
    }

    #region IEquatable Implementation

    /// <inheritdoc />
    public override readonly bool Equals([NotNullWhen(true)] object? obj)
    {
        return obj is SeekFrameCommand command && Equals(command);
    }

    /// <inheritdoc />
    public readonly bool Equals(SeekFrameCommand other)
    {
        return Frame == other.Frame && SeekOrigin == other.SeekOrigin;
    }

    /// <inheritdoc />
    public override readonly int GetHashCode()
    {
        return HashCode.Combine(Frame, SeekOrigin);
    }

    /// <inheritdoc />
    public static bool operator ==(SeekFrameCommand left, SeekFrameCommand right)
    {
        return left.Equals(right);
    }

    /// <inheritdoc />
    public static bool operator !=(SeekFrameCommand left, SeekFrameCommand right)
    {
        return !(left == right);
    }

    #endregion IEquatable Implementation
}
