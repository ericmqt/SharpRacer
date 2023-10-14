namespace SharpRacer.Telemetry;

/// <summary>
/// Represents a telemetry variable that describes a single value.
/// </summary>
/// <typeparam name="T">The type of the variable value.</typeparam>
public interface IDataVariable<T> : IDataVariable
    where T : unmanaged
{
}
