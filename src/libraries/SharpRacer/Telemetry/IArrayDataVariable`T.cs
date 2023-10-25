namespace SharpRacer.Telemetry;

/// <summary>
/// Represents a telemetry variable that describes an array of values of type <typeparamref name="T"/>.
/// </summary>
/// <typeparam name="T">The type of the elements in the array.</typeparam>
public interface IArrayDataVariable<T> : IDataVariable
    where T : unmanaged
{
    /// <summary>
    /// Gets the number of elements in the array.
    /// </summary>
    public int ArrayLength { get; }

    /// <summary>
    /// Gets a value indicating if the values of the array represent a single value over time.
    /// </summary>
    bool IsTimeSliceArray { get; }

    /// <summary>
    /// Reads the array of values from the specified source span.
    /// </summary>
    /// <param name="source">A span of bytes representing a telemetry data frame.</param>
    /// <returns>An array of values from the specified source span.</returns>
    /// <exception cref="DataVariableUnavailableException">The telemetry variable is not available in the current context.</exception>
    T[] Read(ReadOnlySpan<byte> source);
}
