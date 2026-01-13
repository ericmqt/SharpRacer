using System.Diagnostics.CodeAnalysis;

namespace SharpRacer.Commands.VideoCapture;

/// <summary>
/// Represents a simulator command that controls video capture functions.
/// </summary>
public readonly struct VideoCaptureCommand : ISimulatorCommand<VideoCaptureCommand>
{
    /// <summary>
    /// Initializes a new <see cref="VideoCaptureCommand"/> with the specified <see cref="VideoCaptureCommandType">VideoCaptureCommandType</see>.
    /// </summary>
    /// <param name="type"></param>
    public VideoCaptureCommand(VideoCaptureCommandType type)
    {
        Type = type;
    }

    /// <inheritdoc />
    public static ushort CommandId { get; } = (ushort)SimulatorCommandId.VideoCapture;

    /// <summary>
    /// Gets a value that indicates the video capture command to execute.
    /// </summary>
    public readonly VideoCaptureCommandType Type { get; }

    /// <inheritdoc />
    public readonly CommandMessage ToCommandMessage()
    {
        return new CommandMessage(CommandId, (ushort)Type);
    }

    /// <inheritdoc />
    public static VideoCaptureCommand Parse(CommandMessage message)
    {
        CommandMessageParseException.ThrowIfCommandIdNotEqual(message, CommandId);

        var reader = new CommandMessageReader(ref message);

        VideoCaptureCommandType videoCaptureCommandType;

        try { videoCaptureCommandType = reader.ReadArg1<VideoCaptureCommandType>(); }
        catch (CommandMessageReaderException ex)
        {
            throw CommandMessageParseException.Arg1ReaderException(ex);
        }

        return new VideoCaptureCommand(videoCaptureCommandType);
    }

    /// <inheritdoc />
    public static bool TryParse(CommandMessage message, out VideoCaptureCommand result)
    {
        if (message.CommandId != CommandId)
        {
            result = default;
            return false;
        }

        var reader = new CommandMessageReader(ref message);

        if (!reader.TryReadArg1<VideoCaptureCommandType>(out var videoCaptureCommandType))
        {
            result = default;
            return false;
        }

        result = new VideoCaptureCommand(videoCaptureCommandType);
        return true;
    }

    /// <inheritdoc />
    public static implicit operator CommandMessage(VideoCaptureCommand command)
    {
        return command.ToCommandMessage();
    }

    #region IEquatable Implementation

    /// <inheritdoc />
    public override readonly bool Equals([NotNullWhen(true)] object? obj)
    {
        return obj is VideoCaptureCommand command && Equals(command);
    }

    /// <inheritdoc />
    public readonly bool Equals(VideoCaptureCommand other)
    {
        return Type == other.Type;
    }

    /// <inheritdoc />
    public override readonly int GetHashCode()
    {
        return HashCode.Combine(Type);
    }

    /// <inheritdoc />
    public static bool operator ==(VideoCaptureCommand left, VideoCaptureCommand right)
    {
        return left.Equals(right);
    }

    /// <inheritdoc />
    public static bool operator !=(VideoCaptureCommand left, VideoCaptureCommand right)
    {
        return !(left == right);
    }

    #endregion IEquatable Implementation
}
