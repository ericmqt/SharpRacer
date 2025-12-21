namespace SharpRacer.Telemetry;

/// <summary>
/// The exception that is thrown when attempting to perform a data operation with a <see cref="ITelemetryVariable"/> instance that is not
/// available in the current context.
/// </summary>
[Serializable]
public sealed class TelemetryVariableUnavailableException : Exception
{
    /// <summary>
    /// Initializes an instance of <see cref="TelemetryVariableUnavailableException"/> with the specified variable name.
    /// </summary>
    /// <param name="variableName">The name of the unavailable telemetry variable.</param>
    public TelemetryVariableUnavailableException(string variableName)
        : this(variableName, FormatDefaultMessage(variableName), innerException: null)
    {

    }

    /// <summary>
    /// Initializes an instance of <see cref="TelemetryVariableUnavailableException"/> with the specified variable name and message.
    /// </summary>
    /// <param name="variableName">The name of the unavailable telemetry variable.</param>
    /// <param name="message">The exception message.</param>
    public TelemetryVariableUnavailableException(string variableName, string? message)
        : this(variableName, message, innerException: null)
    {

    }

    /// <summary>
    /// Initializes an instance of <see cref="TelemetryVariableUnavailableException"/> with the specified variable name, message, and inner exception.
    /// </summary>
    /// <param name="variableName">The name of the unavailable telemetry variable.</param>
    /// <param name="message">The exception message.</param>
    /// <param name="innerException">The exception that is the cause of the current exception.</param>
    public TelemetryVariableUnavailableException(string variableName, string? message, Exception? innerException)
        : base(message, innerException)
    {
        VariableName = !string.IsNullOrEmpty(variableName)
            ? variableName
            : string.Empty;
    }

    /// <summary>
    /// Gets the name of the unavailable telemetry variable.
    /// </summary>
    public string VariableName { get; }

    private static string FormatDefaultMessage(string variableName)
    {
        return $"Telemetry variable '{variableName ?? string.Empty}' is unavailable in the current context.";
    }
}
