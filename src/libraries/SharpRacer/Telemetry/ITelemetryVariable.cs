namespace SharpRacer.Telemetry;

/// <summary>
/// Defines a base interface for type-safe telemetry variable implementations.
/// </summary>
public interface ITelemetryVariable
{
    /// <summary>
    /// Gets the length, in bytes, of the data represented by the variable.
    /// </summary>
    int DataLength { get; }

    /// <summary>
    /// Gets the offset into the data frame where the variable data is located.
    /// context.
    /// </summary>
    /// <remarks>
    /// If the variable is not available in the current context, returns -1.
    /// </remarks>
    int DataOffset { get; }

    /// <summary>
    /// Gets a value indicating if the telemetry variable is available in the current context.
    /// </summary>
    bool IsAvailable { get; }

    /// <summary>
    /// Gets the variable name.
    /// </summary>
    string Name { get; }

    /// <summary>
    /// Gets the <see cref="TelemetryVariableInfo"/> object from which this instance was created. Returns <see langword="null" /> if the
    /// telemetry variable is not available in the current context.
    /// </summary>
    TelemetryVariableInfo? VariableInfo { get; }

    /// <summary>
    /// Gets a read-only span of bytes over the telemetry variable data in the specified span.
    /// </summary>
    /// <param name="source">A span of bytes representing a frame of telemetry data.</param>
    /// <returns>A read-only span of bytes over the variable value.</returns>
    /// <exception cref="TelemetryVariableUnavailableException">The telemetry variable is not available in the current context.</exception>
    /// <exception cref="ArgumentOutOfRangeException">The telemetry variable is not within the range of <paramref name="source"/>.</exception>
    ReadOnlySpan<byte> GetDataSpan(ReadOnlySpan<byte> source);
}
