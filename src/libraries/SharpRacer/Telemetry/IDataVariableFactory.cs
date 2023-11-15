namespace SharpRacer.Telemetry;

/// <summary>
/// Creates strongly-typed telemetry variable objects implementing <see cref="IDataVariable"/> from a simulator session or telemetry file.
/// </summary>
/// <remarks>
/// If a given telemetry variable is not available in the associated simulator session or telemetry file, a placeholder is created which
/// cannot be read from, denoted by <see cref="IDataVariable.IsAvailable"/>.
/// </remarks>
public interface IDataVariableFactory
{
    IArrayDataVariable<T> CreateArray<T>(string name, int arrayLength)
        where T : unmanaged;

    IScalarDataVariable<T> CreateScalar<T>(string name)
        where T : unmanaged;

    TImplementation CreateType<TImplementation>(string name)
        where TImplementation : class, IDataVariable, ICreateDataVariable<TImplementation>, new();
}
