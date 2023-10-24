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
    public int ValueCount { get; }
}
