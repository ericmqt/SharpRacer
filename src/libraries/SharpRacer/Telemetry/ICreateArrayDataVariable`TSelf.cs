namespace SharpRacer.Telemetry;

/// <summary>
/// Provides static factory methods for creating instances of strongly-typed implementations of <see cref="IArrayDataVariable{T}"/>.
/// </summary>
/// <typeparam name="TSelf">The <see cref="IArrayDataVariable{T}"/> implementation to create.</typeparam>
/// <typeparam name="T">
/// Type parameter <typeparamref name="T"/> of the <see cref="IArrayDataVariable{T}"/> interface implemented by <typeparamref name="TSelf"/>.
/// </typeparam>
public interface ICreateArrayDataVariable<TSelf, T>
    where TSelf : IArrayDataVariable<T>
    where T : unmanaged
{
    /// <summary>
    /// Creates an instance of <typeparamref name="TSelf"/> that represents a telemetry variable in the current simulator session or
    /// telemetry file.
    /// </summary>
    /// <param name="dataVariableInfo">
    /// The <see cref="DataVariableInfo"/> from which to create an instance of <typeparamref name="TSelf"/>.
    /// </param>
    /// <returns>An instance of <typeparamref name="TSelf"/> created from the specified <see cref="DataVariableInfo"/>.</returns>
    /// <remarks>
    /// This method exists to assist <see cref="IDataVariableFactory"/> implementations and should not be called by user code.
    /// </remarks>
    static abstract TSelf Create(DataVariableInfo dataVariableInfo);

    /// <summary>
    /// Creates a placeholder instance of <typeparamref name="TSelf"/> that represents a telemetry variable which is unavailable in the
    /// current simulator session or telemetry file.
    /// </summary>
    /// <param name="variableName">The telemetry variable name.</param>
    /// <param name="valueCount">The number of elements in the value array represented by the telemetry variable.</param>
    /// <returns>An instance of <typeparamref name="TSelf"/> created from the specified variable name and value count.</returns>
    /// <remarks>
    /// This method exists to assist <see cref="IDataVariableFactory"/> implementations and should not be called by user code.
    /// </remarks>
    static abstract TSelf Create(string variableName, int valueCount);
}
