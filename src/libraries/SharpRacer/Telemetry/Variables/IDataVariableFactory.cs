namespace SharpRacer.Telemetry.Variables;

/// <summary>
/// Creates strongly-typed telemetry variable objects implementing <see cref="IDataVariable"/> from a simulator session or telemetry file.
/// </summary>
/// <remarks>
/// If a given telemetry variable is not available in the associated simulator session or telemetry file, a placeholder is created which
/// cannot be read from, denoted by <see cref="IDataVariable.IsAvailable"/>.
/// </remarks>
public interface IDataVariableFactory
{
    /// <summary>
    /// Creates an instance of <see cref="IArrayDataVariable{T}"/> from the specified variable name and array length.
    /// </summary>
    /// <typeparam name="T">The telemetry variable array element type.</typeparam>
    /// <param name="name">The telemetry variable name.</param>
    /// <param name="arrayLength">The length of the array represented by the telemetry variable. Value must be greater than or equal to one.</param>
    /// <returns>
    /// An instance of <see cref="IArrayDataVariable{T}"/> representing the telemetry variable with the specified name and array length.
    /// </returns>
    /// <exception cref="ArgumentException"><paramref name="name"/> is an empty string.</exception>
    /// <exception cref="ArgumentNullException"><paramref name="name"/> is <see langword="null" />.</exception>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="arrayLength"/> is less than one.</exception>
    /// <exception cref="DataVariableInitializationException">
    /// An instance of <see cref="IArrayDataVariable{T}"/> cannot be initialized with the telemetry variable with the specified name.
    /// </exception>
    IArrayDataVariable<T> CreateArray<T>(string name, int arrayLength)
        where T : unmanaged;

    /// <summary>
    /// Creates an instance of <see cref="IScalarDataVariable{T}"/> from the specified variable name.
    /// </summary>
    /// <typeparam name="T">The telemetry variable value type.</typeparam>
    /// <param name="name">The telemetry variable name.</param>
    /// <returns>An instance of <see cref="IScalarDataVariable{T}"/> representing the telemetry variable with the specified name.</returns>
    /// <exception cref="ArgumentException">
    /// <paramref name="name"/> is an empty string.
    /// 
    /// -OR-
    /// 
    /// The telemetry variable is not a scalar variable.
    /// </exception>
    /// <exception cref="ArgumentNullException"><paramref name="name"/> is <see langword="null" />.</exception>
    /// <exception cref="DataVariableInitializationException">
    /// An instance of <see cref="IScalarDataVariable{T}"/> cannot be initialized with the telemetry variable with the specified name.
    /// </exception>
    IScalarDataVariable<T> CreateScalar<T>(string name)
        where T : unmanaged;

    /// <summary>
    /// Creates an instance of a typed telemetry variable from the specified variable name.
    /// </summary>
    /// <typeparam name="TImplementation">
    /// The type of the telemetry variable class which has a default parameterless constructor and implements <see cref="IDataVariable"/>
    /// and <see cref="ICreateDataVariable{TSelf}"/>.
    /// </typeparam>
    /// <param name="name">The name of the telemetry variable.</param>
    /// <returns>An instance of <typeparamref name="TImplementation"/> representing the telemetry variable with the specified name.</returns>
    /// <exception cref="ArgumentException"><paramref name="name"/> is an empty string.</exception>
    /// <exception cref="ArgumentNullException"><paramref name="name"/> is <see langword="null" />.</exception>
    /// <exception cref="DataVariableInitializationException">
    /// An instance of <typeparamref name="TImplementation"/> cannot be initialized with the telemetry variable with the specified name.
    /// </exception>
    TImplementation CreateType<TImplementation>(string name)
        where TImplementation : class, IDataVariable, ICreateDataVariable<TImplementation>, new();
}
