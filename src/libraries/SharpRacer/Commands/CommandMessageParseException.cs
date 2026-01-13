namespace SharpRacer.Commands;

/// <summary>
/// The exception thrown when an error occurs while parsing a <see cref="CommandMessage"/>.
/// </summary>
public sealed class CommandMessageParseException : Exception
{
    /// <summary>
    /// Initializes a new <see cref="CommandMessageParseException"/> instance with the specified message.
    /// </summary>
    /// <param name="message">The error message string.</param>
    public CommandMessageParseException(string? message)
        : base(message)
    {
    }

    /// <summary>
    /// Initializes a new <see cref="CommandMessageParseException"/> instance with the specified message and a reference to the inner
    /// exception that is the cause of this exception.
    /// </summary>
    /// <param name="message">The error message string.</param>
    /// <param name="innerException">The inner exception reference.</param>
    public CommandMessageParseException(string? message, Exception? innerException)
        : base(message, innerException)
    {

    }

    /// <summary>
    /// Returns a <see cref="CommandMessageParseException"/> that wraps a <see cref="CommandMessageReaderException"/> thrown when parsing
    /// <see cref="CommandMessage.Arg1"/>.
    /// </summary>
    /// <param name="readerException">
    /// The <see cref="CommandMessageReaderException"/> thrown when parsing <see cref="CommandMessage.Arg1"/>.
    /// </param>
    /// <returns>
    /// A <see cref="CommandMessageParseException"/> that wraps a <see cref="CommandMessageReaderException"/> thrown when parsing
    /// <see cref="CommandMessage.Arg1"/>.
    /// </returns>
    public static CommandMessageParseException Arg1ReaderException(CommandMessageReaderException readerException)
    {
        return new CommandMessageParseException(
            $"Failed to parse {nameof(CommandMessage)}.{nameof(CommandMessage.Arg1)}: {readerException.Message}",
            readerException);
    }

    /// <summary>
    /// Returns a <see cref="CommandMessageParseException"/> that describes an invalid value in <see cref="CommandMessage.Arg1"/>.
    /// </summary>
    /// <typeparam name="T">The value type.</typeparam>
    /// <param name="expected">The expected value.</param>
    /// <param name="actual">The value returned from <see cref="CommandMessage.Arg1"/>.</param>
    /// <returns>A <see cref="CommandMessageParseException"/> that describes an invalid value in <see cref="CommandMessage.Arg1"/>.</returns>
    public static CommandMessageParseException InvalidArg1Value<T>(T expected, T actual)
    {
        return new CommandMessageParseException(
            $"{nameof(CommandMessage)}.{nameof(CommandMessage.Arg1)} value is invalid for this command. Expected: {expected}. Actual: {actual}.");
    }

    /// <summary>
    /// Throws a <see cref="CommandMessageParseException"/> if <see cref="CommandMessage.CommandId"/> is not equal to
    /// <paramref name="commandId"/>.
    /// </summary>
    /// <param name="commandMessage"></param>
    /// <param name="commandId"></param>
    public static void ThrowIfCommandIdNotEqual(CommandMessage commandMessage, ushort commandId)
    {
        if (commandMessage.CommandId != commandId)
        {
            throw new CommandMessageParseException(
                $"Expected command ID '{commandId}' but {nameof(CommandMessage)} has command ID '{commandMessage.CommandId}'.");
        }
    }
}
