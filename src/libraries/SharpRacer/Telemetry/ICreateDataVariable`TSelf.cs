namespace SharpRacer.Telemetry;

/// <summary>
/// Defines static factory methods used for creating an instance of type <typeparamref name="TSelf"/>.
/// </summary>
/// <typeparam name="TSelf">A class type implementing <see cref="IDataVariable"/> with a parameterless constructor.</typeparam>
public interface ICreateDataVariable<TSelf>
    where TSelf : class, IDataVariable, new()
{
    /// <summary>
    /// Creates an instance of <typeparamref name="TSelf"/> from the specified <see cref="DataVariableInfo"/>.
    /// </summary>
    /// <param name="dataVariableInfo">The <see cref="DataVariableInfo"/> from which to create an instance of <typeparamref name="TSelf"/>.</param>
    /// <returns>An instance of <typeparamref name="TSelf"/> created from the specified <see cref="DataVariableInfo"/>.</returns>
    /// <exception cref="DataVariableInitializationException">
    /// The specified <see cref="DataVariableInfo"/> could not be used to initialize an instance of <typeparamref name="TSelf"/>.
    /// </exception>
    static abstract TSelf Create(DataVariableInfo dataVariableInfo);
}