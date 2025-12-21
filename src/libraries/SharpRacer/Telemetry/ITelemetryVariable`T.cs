namespace SharpRacer.Telemetry;

/// <summary>
/// Defines a telemetry variable with a value element of type <typeparamref name="T"/>.
/// </summary>
/// <typeparam name="T">The type of the telemetry variable value. If the variable is an array, this is the type of the array element.</typeparam>
public interface ITelemetryVariable<T> : ITelemetryVariable
    where T : unmanaged
{
    /// <summary>
    /// Gets the length, in bytes, of an individual value of the telemetry variable. If the telemetry variable is an array variable, this
    /// is the size of the element of the array.
    /// </summary>
    int ValueSize { get; }
}
