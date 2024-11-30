namespace SharpRacer.Telemetry;

/// <summary>
/// Defines a contract for reading a value of type <typeparamref name="TValue"/> from a telemetry variable.
/// </summary>
/// <typeparam name="TValue"></typeparam>
public interface IDataVariableValueAccessor<TValue>
{
    /// <summary>
    /// Reads the value from the specified span.
    /// </summary>
    /// <param name="source">A span of bytes representing a telemetry data frame.</param>
    /// <returns>A value of type <typeparamref name="TValue"/> read from the specified span.</returns>
    /// <exception cref="DataVariableUnavailableException">The telemetry variable is not available in the current context.</exception>
    TValue Read(ReadOnlySpan<byte> source);
}
