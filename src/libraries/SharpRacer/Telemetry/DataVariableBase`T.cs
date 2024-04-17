using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace SharpRacer.Telemetry;

/// <summary>
/// Provides a base class for type-safe <see cref="IDataVariable{T}"/> implementations.
/// </summary>
/// <typeparam name="T">
/// The value type represented by the variable or, if the variable is an array, the element type of the array.
/// </typeparam>
public abstract class DataVariableBase<T> : IDataVariable<T>
    where T : unmanaged
{
    /// <summary>
    /// Initializes an instance of <see cref="DataVariableBase{T}"/> from the specified <see cref="DataVariableInfo"/>.
    /// </summary>
    /// <param name="variableInfo">The <see cref="DataVariableInfo"/> from which to initialize the instance.</param>
    /// <exception cref="ArgumentNullException"><paramref name="variableInfo"/> is <see langword="null" />.</exception>
    /// <exception cref="DataVariableInitializationException">
    /// Type parameter <typeparamref name="T"/> is not compatible with the value type specified by <paramref name="variableInfo"/>.
    /// </exception>
    protected DataVariableBase(DataVariableInfo variableInfo)
    {
        ArgumentNullException.ThrowIfNull(variableInfo);
        DataVariableInitializationException.ThrowIfValueTypeArgumentIsInvalid<T>(GetType(), variableInfo.ValueType);

        Name = variableInfo.Name;

        ValueCount = variableInfo.ValueCount;
        ValueSize = Unsafe.SizeOf<T>();

        DataOffset = variableInfo.Offset;
        DataLength = Unsafe.SizeOf<T>() * variableInfo.ValueCount;

        VariableInfo = variableInfo;
        IsAvailable = true;
    }

    /// <summary>
    /// Initializes an instance of <see cref="DataVariableBase{T}"/> from the specified variable name, value count, and <see cref="DataVariableInfo"/>.
    /// </summary>
    /// <param name="name">The telemetry variable name.</param>
    /// <param name="valueCount">The number of <typeparamref name="T"/> elements represented by the telemetry variable.</param>
    /// <param name="variableInfo">
    /// The <see cref="DataVariableInfo"/> with which to initialize the instance. If <see langword="null" />, the resulting object
    /// represents a variable that is unavailable in the current context.
    /// </param>
    /// <exception cref="ArgumentException"><paramref name="name"/> is an empty string.</exception>
    /// <exception cref="ArgumentNullException"><paramref name="name"/> is <see langword="null" />.</exception>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="valueCount"/> is less than one.</exception>
    /// <exception cref="DataVariableInitializationException">
    /// <paramref name="name"/> is not equal to the variable name specified by <paramref name="variableInfo"/>.
    /// 
    /// -OR-
    /// 
    /// <paramref name="valueCount"/> is not equal to the value count specified by <paramref name="variableInfo"/>.
    /// 
    /// -OR-
    /// 
    /// Type parameter <typeparamref name="T"/> is not compatible with the value type specified by <paramref name="variableInfo"/>.
    /// </exception>
    internal DataVariableBase(string name, int valueCount, DataVariableInfo? variableInfo)
    {
        ArgumentException.ThrowIfNullOrEmpty(name);
        ArgumentOutOfRangeException.ThrowIfLessThan(valueCount, 1);

        if (variableInfo != null)
        {
            DataVariableInitializationException.ThrowIfDataVariableInfoNameIsNotEqual(GetType(), variableInfo, name);
            DataVariableInitializationException.ThrowIfDataVariableInfoValueCountIsNotEqual(GetType(), variableInfo, valueCount);
            DataVariableInitializationException.ThrowIfValueTypeArgumentIsInvalid<T>(GetType(), variableInfo.ValueType);
        }

        Name = name;

        ValueCount = valueCount;
        ValueSize = Unsafe.SizeOf<T>();

        DataLength = Unsafe.SizeOf<T>() * valueCount;
        DataOffset = variableInfo?.Offset ?? -1;

        IsAvailable = variableInfo != null;
        VariableInfo = variableInfo;
    }

    /// <summary>
    /// Initializes an instance of <see cref="DataVariableBase{T}"/> from the specified variable descriptor and optional <see cref="DataVariableInfo"/>.
    /// </summary>
    /// <param name="variableDescriptor">
    /// The variable descriptor which provides required values in the event that <paramref name="variableInfo"/> is <see langword="null"/>
    /// (i.e. the variable is not available in the current context).
    /// </param>
    /// <param name="variableInfo">
    /// The <see cref="DataVariableInfo"/> with which to initialize the instance. If <see langword="null" />, the resulting object
    /// represents a variable that is unavailable in the current context.
    /// </param>
    /// <exception cref="ArgumentNullException"><paramref name="variableDescriptor"/> is <see langword="null" />.</exception>
    /// <exception cref="DataVariableInitializationException">
    /// The variable names specified by <paramref name="variableDescriptor"/> and <paramref name="variableInfo"/> do not match.
    /// 
    /// -OR-
    /// 
    /// The variable value counts specified by <paramref name="variableDescriptor"/> and <paramref name="variableInfo"/> do not match.
    /// 
    /// -OR-
    /// 
    /// Type parameter <typeparamref name="T"/> is not compatible with the value type specified by either
    /// <paramref name="variableDescriptor"/> or <paramref name="variableInfo"/>.
    /// </exception>
    internal DataVariableBase(DataVariableDescriptor variableDescriptor, DataVariableInfo? variableInfo)
    {
        ArgumentNullException.ThrowIfNull(variableDescriptor);
        DataVariableInitializationException.ThrowIfValueTypeArgumentIsInvalid<T>(GetType(), variableDescriptor.ValueType);

        if (variableInfo != null)
        {
            DataVariableInitializationException.ThrowIfDataVariableInfoNameIsNotEqual(GetType(), variableInfo, variableDescriptor.Name);
            DataVariableInitializationException.ThrowIfDataVariableInfoValueCountIsNotEqual(GetType(), variableInfo, variableDescriptor.ValueCount);
            DataVariableInitializationException.ThrowIfValueTypeArgumentIsInvalid<T>(GetType(), variableInfo.ValueType);
        }

        Name = variableDescriptor.Name;

        ValueCount = variableDescriptor.ValueCount;
        ValueSize = Unsafe.SizeOf<T>();

        DataLength = Unsafe.SizeOf<T>() * variableDescriptor.ValueCount;
        DataOffset = variableInfo?.Offset ?? -1;

        IsAvailable = variableInfo != null;
        VariableInfo = variableInfo;
    }

    /// <inheritdoc />
    public int DataLength { get; }

    /// <inheritdoc />
    public int DataOffset { get; }

    /// <inheritdoc />
    [MemberNotNullWhen(true, nameof(VariableInfo))]
    public bool IsAvailable { get; }

    /// <inheritdoc />
    public string Name { get; }

    /// <summary>
    /// Gets the number of <typeparamref name="T"/> elements represented by the telemetry variable. If greater than one, the telemetry
    /// variable is an array variable.
    /// </summary>
    public int ValueCount { get; }

    /// <inheritdoc />
    public int ValueSize { get; }

    /// <inheritdoc />
    public DataVariableInfo? VariableInfo { get; }

    /// <inheritdoc />
    public ReadOnlySpan<byte> GetDataSpan(ReadOnlySpan<byte> source)
    {
        if (!IsAvailable)
        {
            throw new DataVariableUnavailableException(Name);
        }

        return source.Slice(DataOffset, DataLength);
    }

    /// <summary>
    /// Throws a <see cref="DataVariableUnavailableException"/> when <see cref="IsAvailable"/> is <see langword="false" />.
    /// </summary>
    /// <exception cref="DataVariableUnavailableException">The telemetry variable is not available in the current context.</exception>
    [MemberNotNull(nameof(VariableInfo))]
    protected void ThrowIfUnavailable()
    {
        if (!IsAvailable)
        {
            throw new DataVariableUnavailableException(Name);
        }
    }
}
