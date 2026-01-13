namespace SharpRacer.Commands;

/// <summary>
/// The exception thrown when an error occurs while reading data from a <see cref="CommandMessage"/>.
/// </summary>
public sealed class CommandMessageReaderException : Exception
{
    /// <summary>
    /// Initializes a new <see cref="CommandMessageReaderException"/> instance with the specified message.
    /// </summary>
    /// <param name="message">The error message string.</param>
    public CommandMessageReaderException(string? message)
        : base(message)
    {
    }

    /// <summary>
    /// Initializes a new <see cref="CommandMessageReaderException"/> instance with the specified message and a reference to the inner
    /// exception that is the cause of this exception.
    /// </summary>
    /// <param name="message">The error message string.</param>
    /// <param name="innerException">The inner exception reference.</param>
    public CommandMessageReaderException(string? message, Exception? innerException)
        : base(message, innerException)
    {
    }

    /// <summary>
    /// Returns a <see cref="CommandMessageReaderException"/> that describes an error where attempting to read an enumeration value failed
    /// because the value is not defined by the enumeration.
    /// </summary>
    /// <typeparam name="TEnum">The enumeration type.</typeparam>
    /// <param name="arg">The undefined value that was read.</param>
    /// <returns>A <see cref="CommandMessageReaderException"/> describing the error.</returns>
    public static CommandMessageReaderException ReadUndefinedEnumerationValue<TEnum>(ushort arg)
        where TEnum : unmanaged, Enum
    {
        return new CommandMessageReaderException(
            $"Failed to read value as '{typeof(TEnum)}': Value '{arg}' is not defined.");
    }
}
