namespace SharpRacer.Commands;

/// <summary>
/// Defines a simulator command.
/// </summary>
/// <typeparam name="TSelf">The type that implements the interface.</typeparam>
public interface ISimulatorCommand<TSelf> : IEquatable<TSelf>
    where TSelf : ISimulatorCommand<TSelf>
{
    /// <summary>
    /// Gets the value used by the simulator to identify a command.
    /// </summary>
    static abstract ushort CommandId { get; }

    /// <summary>
    /// Parses the specified <see cref="CommandMessage"/> into an instance of <see cref="ISimulatorCommand{TSelf}"/>.
    /// </summary>
    /// <param name="message">The command message to parse.</param>
    /// <returns>An instance of <see cref="ISimulatorCommand{TSelf}"/> equivalent to the command message.</returns>
    /// <exception cref="CommandMessageParseException">
    /// <paramref name="message"/> is not in the correct format.
    /// </exception>
    static abstract TSelf Parse(CommandMessage message);

    /// <summary>
    /// Attempts to parse the specified <see cref="CommandMessage"/> into an instance of <see cref="ISimulatorCommand{TSelf}"/>, returning
    /// a value that indicates if the operation was successful.
    /// </summary>
    /// <param name="message">The command message to parse.</param>
    /// <param name="result">
    /// The <see cref="ISimulatorCommand{TSelf}"/> parsed from the <see cref="CommandMessage"/> if the operation was successful, otherwise
    /// <see langword="default"/>.
    /// </param>
    /// <returns>
    /// <see langword="true"/> if parsing succeeded and <paramref name="result"/> contains a valid value, otherwise <see langword="false"/>.
    /// </returns>
    static abstract bool TryParse(CommandMessage message, out TSelf result);

    /// <summary>
    /// Converts the command into a <see cref="CommandMessage"/> value.
    /// </summary>
    /// <returns>The <see cref="CommandMessage"/> value that represents this instance.</returns>
    CommandMessage ToCommandMessage();

    /// <summary>
    /// Implicitly converts <paramref name="command"/> into a <see cref="CommandMessage"/> value.
    /// </summary>
    /// <param name="command"></param>
    static abstract implicit operator CommandMessage(TSelf command);
}
