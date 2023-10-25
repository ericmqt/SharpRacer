namespace SharpRacer.Telemetry;

/// <summary>
/// Provides static factory methods for creating instances of concrete implementations of <see cref="IArrayDataVariable{T}"/>.
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
    static abstract TSelf Create(DataVariableInfo dataVariableInfo);

    /// <summary>
    /// Creates a placeholder instance of <typeparamref name="TSelf"/> that represents a telemetry variable which is unavailable in the
    /// current simulator session or telemetry file.
    /// </summary>
    /// <param name="variableName">The name of the telemetry variable.</param>
    /// <param name="valueCount">The number of elements in the value array represented by the telemetry variable.</param>
    /// <param name="isTimeSliceArray">If <see langword="true" />, the array represents a single value over time.</param>
    /// <returns>An instance of <typeparamref name="TSelf"/> created from the specified variable name and value count.</returns>
    static abstract TSelf Create(string variableName, int valueCount, bool isTimeSliceArray);
}
