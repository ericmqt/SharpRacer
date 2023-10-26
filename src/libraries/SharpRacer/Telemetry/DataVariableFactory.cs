using System.Diagnostics.CodeAnalysis;

namespace SharpRacer.Telemetry;

/// <inheritdoc cref="IDataVariableFactory" />
internal class DataVariableFactory : IDataVariableFactory
{
    private readonly IEnumerable<DataVariableInfo> _dataVariables;

    public DataVariableFactory(IEnumerable<DataVariableInfo> dataVariables)
    {
        _dataVariables = dataVariables ?? throw new ArgumentNullException(nameof(dataVariables));
    }

    /// <summary>
    /// Creates an instance of <see cref="IDataVariable{T}"/> that represents the telemetry variable with the specified name.
    /// </summary>
    /// <typeparam name="T">The telemetry variable value type.</typeparam>
    /// <param name="name">The name of the telemetry variable.</param>
    /// <returns>An instance of <see cref="IDataVariable{T}"/> that represents the telemetry variable.</returns>
    /// <exception cref="ArgumentException">
    /// <paramref name="name"/> is an empty string.
    /// 
    /// -OR-
    /// 
    /// The telemetry variable matched by <paramref name="name"/> is an array variable.
    /// 
    /// -OR-
    /// 
    /// Type parameter <typeparamref name="T"/> is not compatible with the telemetry variable matched by <paramref name="name"/>.
    /// </exception>
    /// <exception cref="ArgumentNullException"><paramref name="name"/> is <see langword="null" />.</exception>
    public IDataVariable<T> Create<T>(string name)
        where T : unmanaged
    {
        ArgumentException.ThrowIfNullOrEmpty(name);

        if (!TryGetDataVariableInfo(name, out DataVariableInfo? dataVariableInfo))
        {
            return new DataVariable<T>(name);
        }

        if (!IsValueTypeMatch<T>(dataVariableInfo.ValueType))
        {
            throw new ArgumentException(
                $"Type parameter '{typeof(T)}' is invalid for telemetry variable '{name}' with value type '{dataVariableInfo.ValueType}'.",
                nameof(name));
        }

        if (dataVariableInfo.ValueCount > 1)
        {
            throw new ArgumentException(
                $"Telemetry variable matching the value of parameter '{nameof(name)}' ({name}) is an array variable.", nameof(name));
        }

        return new DataVariable<T>(dataVariableInfo);

    }

    /// <summary>
    /// Creates an instance of <see cref="IArrayDataVariable{T}"/> that represents the array telemetry variable with the specified name
    /// and array length.
    /// </summary>
    /// <typeparam name="T">The telemetry variable array element type.</typeparam>
    /// <param name="name">The name of the telemetry variable.</param>
    /// <param name="arrayLength">The length of the array of values represented by the telemetry variable.</param>
    /// <param name="isTimeSliceArray">If <see langword="true" />, the array represents a single value over time.</param>
    /// <returns>An instance of <see cref="IArrayDataVariable{T}"/> that represents the specified array telemetry variable.</returns>
    /// <exception cref="ArgumentException">
    /// <paramref name="name"/> is an empty string.
    /// 
    /// -OR-
    /// 
    /// <paramref name="arrayLength"/> does not match the telemetry variable obtained from the current simulator session or telemetry file.
    /// 
    /// -OR-
    /// 
    /// <paramref name="isTimeSliceArray"/> does not match telemetry variable obtained from the current simulator session or telemetry file.
    /// 
    /// -OR-
    /// 
    /// Type parameter <typeparamref name="T"/> is not compatible with the telemetry variable matched by <paramref name="name"/>.
    /// </exception>
    /// <exception cref="ArgumentNullException"><paramref name="name"/> is <see langword="null" />.</exception>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="arrayLength"/> is not greater than 1.</exception>
    public IArrayDataVariable<T> CreateArray<T>(string name, int arrayLength, bool isTimeSliceArray)
        where T : unmanaged
    {
        ArgumentException.ThrowIfNullOrEmpty(name);
        ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(arrayLength, 1);

        if (!TryGetDataVariableInfo(name, out var dataVariableInfo))
        {
            return new ArrayDataVariable<T>(name, arrayLength, isTimeSliceArray);
        }

        if (!IsValueTypeMatch<T>(dataVariableInfo.ValueType))
        {
            throw new ArgumentException(
                $"Type parameter '{typeof(T)}' is invalid for telemetry variable '{name}' with value type '{dataVariableInfo.ValueType}'.",
                nameof(name));
        }

        if (dataVariableInfo.ValueCount != arrayLength)
        {
            throw new ArgumentException(
                $"'{nameof(arrayLength)}' ({arrayLength}) is not equal to the value count of telemetry variable '{name}'.",
                nameof(arrayLength));
        }

        if (dataVariableInfo.IsTimeSliceArray != isTimeSliceArray)
        {
            throw new ArgumentException(
                $"'{nameof(isTimeSliceArray)}' is {isTimeSliceArray} but does not match the value for telemetry variable '{name}'.",
                nameof(isTimeSliceArray));
        }

        return new ArrayDataVariable<T>(dataVariableInfo);
    }

    /// <summary>
    /// Creates an instance of <typeparamref name="TImplementation"/> that represents the telemetry variable with the specified name.
    /// </summary>
    /// <typeparam name="TImplementation">A type implementing <see cref="IDataVariable{T}"/> and <see cref="ICreateDataVariable{TSelf, T}"/>.</typeparam>
    /// <typeparam name="T"></typeparam>
    /// <param name="name"></param>
    /// <returns>An instance of <typeparamref name="TImplementation"/> that represents the specified telemetry variable.</returns>
    /// <exception cref="ArgumentException">
    /// <paramref name="name"/> is an empty string.
    /// 
    /// -OR-
    /// 
    /// The telemetry variable matched by <paramref name="name"/> is an array variable.
    /// 
    /// -OR-
    /// 
    /// Type parameter <typeparamref name="T"/> is not compatible with the telemetry variable matched by <paramref name="name"/>.
    /// </exception>
    /// <exception cref="ArgumentNullException"><paramref name="name"/> is <see langword="null" />.</exception>
    public TImplementation Create<TImplementation, T>(string name)
        where TImplementation : IDataVariable<T>, ICreateDataVariable<TImplementation, T>
        where T : unmanaged
    {
        ArgumentException.ThrowIfNullOrEmpty(name);

        if (!TryGetDataVariableInfo(name, out var dataVariableInfo))
        {
            return TImplementation.Create(name);
        }

        if (!IsValueTypeMatch<T>(dataVariableInfo.ValueType))
        {
            throw new ArgumentException(
                $"Type parameter '{typeof(T)}' is invalid for telemetry variable '{name}' with value type '{dataVariableInfo.ValueType}'.",
                nameof(name));
        }

        if (dataVariableInfo.ValueCount > 1)
        {
            throw new ArgumentException(
                $"Telemetry variable matching the value of parameter '{nameof(name)}' ({name}) is an array variable.", nameof(name));
        }

        return TImplementation.Create(dataVariableInfo);
    }

    /// <summary>
    /// Creates an instance of <typeparamref name="TImplementation"/> that represents the array telemetry variable with the specified name
    /// and array length.
    /// </summary>
    /// <typeparam name="TImplementation"></typeparam>
    /// <typeparam name="T"></typeparam>
    /// <param name="name">The name of the telemetry variable.</param>
    /// <param name="arrayLength">The length of the array of values represented by the telemetry variable.</param>
    /// <param name="isTimeSliceArray">If <see langword="true" />, the array represents a single value over time.</param>
    /// <returns>An instance of <typeparamref name="TImplementation"/> that represents the specified array telemetry variable.</returns>
    /// <exception cref="ArgumentException">
    /// <paramref name="name"/> is an empty string.
    /// 
    /// -OR-
    /// 
    /// <paramref name="arrayLength"/> does not match the telemetry variable obtained from the current simulator session or telemetry file.
    /// 
    /// -OR-
    /// 
    /// <paramref name="isTimeSliceArray"/> does not match telemetry variable obtained from the current simulator session or telemetry file.
    /// 
    /// -OR-
    /// 
    /// Type parameter <typeparamref name="T"/> is not compatible with the telemetry variable matched by <paramref name="name"/>.
    /// </exception>
    /// <exception cref="ArgumentNullException"><paramref name="name"/> is <see langword="null" />.</exception>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="arrayLength"/> is not greater than 1.</exception>
    public TImplementation CreateArray<TImplementation, T>(string name, int arrayLength, bool isTimeSliceArray)
        where TImplementation : IArrayDataVariable<T>, ICreateArrayDataVariable<TImplementation, T>
        where T : unmanaged
    {
        ArgumentException.ThrowIfNullOrEmpty(name);
        ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(arrayLength, 1);

        if (!TryGetDataVariableInfo(name, out var dataVariableInfo))
        {
            return TImplementation.Create(name, arrayLength, isTimeSliceArray);
        }

        if (!IsValueTypeMatch<T>(dataVariableInfo.ValueType))
        {
            throw new ArgumentException(
                $"Type parameter '{typeof(T)}' is invalid for telemetry variable '{name}' with value type '{dataVariableInfo.ValueType}'.",
                nameof(name));
        }

        if (dataVariableInfo.ValueCount != arrayLength)
        {
            throw new ArgumentException(
                $"'{nameof(arrayLength)}' ({arrayLength}) is not equal to the value count of telemetry variable '{name}'.",
                nameof(arrayLength));
        }

        if (dataVariableInfo.IsTimeSliceArray != isTimeSliceArray)
        {
            throw new ArgumentException(
                $"'{nameof(isTimeSliceArray)}' is {isTimeSliceArray} but does not match the value for telemetry variable '{name}'.",
                nameof(isTimeSliceArray));
        }

        return TImplementation.Create(dataVariableInfo);
    }

    internal static bool IsValueTypeMatch<T>(DataVariableValueType valueType)
        where T : unmanaged
    {
        if (valueType == DataVariableValueType.Bool)
        {
            return typeof(T) == typeof(bool);
        }

        if (valueType == DataVariableValueType.Byte)
        {
            return typeof(T) == typeof(byte);
        }

        if (valueType == DataVariableValueType.Int || valueType == DataVariableValueType.Bitfield)
        {
            return typeof(T) == typeof(int);
        }

        if (valueType == DataVariableValueType.Float)
        {
            return typeof(T) == typeof(float);
        }

        if (valueType == DataVariableValueType.Double)
        {
            return typeof(T) == typeof(double);
        }

        return false;
    }

    private bool TryGetDataVariableInfo(string name, [NotNullWhen(true)] out DataVariableInfo? dataVariable)
    {
        ArgumentException.ThrowIfNullOrEmpty(name);

        dataVariable = _dataVariables.FirstOrDefault(x => string.Equals(x.Name, name, StringComparison.Ordinal));

        return dataVariable != null;
    }
}
