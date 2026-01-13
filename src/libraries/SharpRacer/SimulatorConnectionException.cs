namespace SharpRacer;

/// <summary>
/// The exception thrown when an error occurs establishing a connection to the simulator.
/// </summary>
public class SimulatorConnectionException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SimulatorConnectionException"/> class with the specified message.
    /// </summary>
    /// <param name="message">The error message string.</param>
    public SimulatorConnectionException(string? message)
        : base(message)
    {

    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SimulatorConnectionException"/> class with the specified message and a reference to
    /// the inner exception that is the cause of this exception.
    /// </summary>
    /// <param name="message">The error message string.</param>
    /// <param name="innerException">The inner exception reference.</param>
    public SimulatorConnectionException(string? message, Exception? innerException)
        : base(message, innerException)
    {

    }
}
