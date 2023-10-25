namespace SharpRacer.Telemetry;

/// <summary>
/// Represents a telemetry variable that describes a single value.
/// </summary>
/// <typeparam name="T">The type of the variable value.</typeparam>
public interface IDataVariable<T> : IDataVariable
    where T : unmanaged
{
    /// <summary>
    /// Reads the value from the specified span.
    /// </summary>
    /// <param name="source">A span of bytes representing a telemetry data frame.</param>
    /// <returns>The value of type <typeparamref name="T"/> read from the specified span.</returns>
    /// <exception cref="DataVariableUnavailableException">The telemetry variable is not available in the current context.</exception>
    T Read(ReadOnlySpan<byte> source);
}
