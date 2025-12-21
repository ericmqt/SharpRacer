namespace SharpRacer.Telemetry;

/// <summary>
/// Defines a type-safe representation of a telemetry variable whose value is an array with elements of type <typeparamref name="T"/>.
/// </summary>
/// <typeparam name="T">The telemetry variable array element type.</typeparam>
public interface IArrayTelemetryVariable<T> : ITelemetryVariable<T>, ITelemetryVariableValueAccessor<T[]>
    where T : unmanaged
{
    /// <summary>
    /// Gets the number of values in the array.
    /// </summary>
    int ValueCount { get; }
}
