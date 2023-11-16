namespace SharpRacer.Telemetry.Variables;

/// <summary>
/// Defines a type-safe representation of a telemetry variable whose value is a single value of type <typeparamref name="T"/>.
/// </summary>
/// <typeparam name="T">The telemetry variable value type.</typeparam>
public interface IScalarDataVariable<T> : IDataVariable<T>, IDataVariableValueAccessor<T>
    where T : unmanaged
{
}
