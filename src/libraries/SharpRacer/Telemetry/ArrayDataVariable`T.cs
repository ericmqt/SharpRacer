using System.Runtime.InteropServices;

namespace SharpRacer.Telemetry;

/// <summary>
/// Provides a type-safe representation of a telemetry variable whose value is an array with elements of type <typeparamref name="T"/>.
/// </summary>
/// <typeparam name="T">The telemetry variable array element type.</typeparam>
public class ArrayDataVariable<T> : DataVariableBase<T>, IArrayDataVariable<T>
    where T : unmanaged
{
    /// <summary>
    /// Initializes an instance of <see cref="ArrayDataVariable{T}"/> from the specified <seealso cref="DataVariableInfo"/>.
    /// </summary>
    /// <param name="variableInfo">The <see cref="DataVariableInfo"/> object from which to initialize the <see cref="ArrayDataVariable{T}"/> instance.</param>
    /// <exception cref="ArgumentNullException"><paramref name="variableInfo"/> is <see langword="null" />.</exception>
    /// <exception cref="DataVariableInitializationException">
    /// Type parameter <typeparamref name="T"/> is not compatible with the value type specified by <paramref name="variableInfo"/>.
    /// </exception>
    public ArrayDataVariable(DataVariableInfo variableInfo)
        : base(variableInfo)
    {

    }

    /// <summary>
    /// Initializes an instance of <see cref="ArrayDataVariable{T}"/> with the specified name and array length that represents a telemetry
    /// variable that is unavailable in the current context.
    /// </summary>
    /// <param name="name">The telemetry variable name.</param>
    /// <param name="arrayLength">The length of the array represented by the telemetry variable. Value must be greater than or equal to one.</param>
    /// <exception cref="ArgumentException"><paramref name="name"/> is an empty string.</exception>
    /// <exception cref="ArgumentNullException"><paramref name="name"/> is <see langword="null" />.</exception>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="arrayLength"/> is less than one.</exception>
    public ArrayDataVariable(string name, int arrayLength)
        : this(name, arrayLength, variableInfo: null)
    {

    }

    /// <summary>
    /// Initializes an instance of <see cref="ArrayDataVariable{T}"/> from the specified variable name, array length, and optional <see cref="DataVariableInfo"/>.
    /// </summary>
    /// <param name="name">The telemetry variable name.</param>
    /// <param name="arrayLength">The length of the array represented by the telemetry variable. Value must be greater than or equal to one.</param>
    /// <param name="variableInfo">
    /// The <see cref="DataVariableInfo"/> with which to initialize the instance. If <see langword="null" />, the resulting object
    /// represents a variable that is unavailable in the current context.
    /// </param>
    /// <exception cref="ArgumentException"><paramref name="name"/> is an empty string.</exception>
    /// <exception cref="ArgumentNullException"><paramref name="name"/> is <see langword="null" />.</exception>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="arrayLength"/> is less than one.</exception>
    /// <exception cref="DataVariableInitializationException">
    /// <paramref name="name"/> is not equal to the variable name specified by <paramref name="variableInfo"/>.
    /// 
    /// -OR-
    /// 
    /// <paramref name="arrayLength"/> is not equal to the value count specified by <paramref name="variableInfo"/>.
    /// 
    /// -OR-
    /// 
    /// Type parameter <typeparamref name="T"/> is not compatible with the value type specified by <paramref name="variableInfo"/>.
    /// </exception>
    protected internal ArrayDataVariable(string name, int arrayLength, DataVariableInfo? variableInfo)
        : base(name, arrayLength, variableInfo)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(arrayLength, 1);
    }

    /// <summary>
    /// Initializes an instance of <see cref="ArrayDataVariable{T}"/> from the specified variable descriptor and optional <see cref="DataVariableInfo"/>.
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
    protected internal ArrayDataVariable(DataVariableDescriptor variableDescriptor, DataVariableInfo? variableInfo)
        : base(variableDescriptor, variableInfo)
    {

    }

    /// <inheritdoc />
    public T[] Read(ReadOnlySpan<byte> source)
    {
        ThrowIfUnavailable();

        // Slice the source span to our array variable data
        var arrayBytes = GetDataSpan(source);

        // Allocate an array of values of length ValueCount
        var valueArray = new T[ValueCount];

        // Re-interpret the value array as a span of bytes and copy the source span into it
        arrayBytes.CopyTo(MemoryMarshal.AsBytes<T>(valueArray));

        return valueArray;
    }
}
