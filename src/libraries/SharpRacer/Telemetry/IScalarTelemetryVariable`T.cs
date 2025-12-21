namespace SharpRacer.Telemetry;

/// <summary>
/// Defines a type-safe representation of a telemetry variable whose value is a single value of type <typeparamref name="T"/>.
/// </summary>
/// <typeparam name="T">The telemetry variable value type.</typeparam>
public interface IScalarTelemetryVariable<T> : ITelemetryVariable<T>, ITelemetryVariableValueAccessor<T>
    where T : unmanaged
{
}
