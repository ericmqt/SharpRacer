using System.Runtime.InteropServices;

namespace SharpRacer.Telemetry;

/// <summary>
/// Represents an array telemetry variable with an array element of type <typeparamref name="T"/>.
/// </summary>
/// <typeparam name="T">The telemetry variable array element type.</typeparam>
public class ArrayDataVariable<T> : DataVariableBase<T>, IArrayDataVariable<T>
    where T : unmanaged
{
    /// <summary>
    /// Initializes an instance of <see cref="ArrayDataVariable{T}"/> from the specified <seealso cref="DataVariableInfo"/>.
    /// </summary>
    /// <param name="dataVariableInfo">The <see cref="DataVariableInfo"/> object from which to initialize this instance.</param>
    /// <exception cref="ArgumentException">
    /// <paramref name="dataVariableInfo"/> is not an array telemetry variable.
    /// </exception>
    public ArrayDataVariable(DataVariableInfo dataVariableInfo)
        : base(dataVariableInfo)
    {
        if (dataVariableInfo.ValueCount <= 1)
        {
            throw new ArgumentException($"'{nameof(dataVariableInfo)}' is not an array telemetry variable.", nameof(dataVariableInfo));
        }

        IsTimeSliceArray = dataVariableInfo.IsTimeSliceArray;
        ArrayLength = dataVariableInfo.ValueCount;
    }

    /// <summary>
    /// Initializes an instance of <see cref="ArrayDataVariable{T}"/> with the specified name and array length that represents a telemetry
    /// variable that is not available in the current context.
    /// </summary>
    /// <param name="name">The telemetry variable name.</param>
    /// <param name="arrayLength">The number of elements in the array.</param>
    /// <param name="isTimeSliceArray">If <see langword="true" />, the array represents a single value over time.</param>
    /// <exception cref="ArgumentException"><paramref name="name"/> is <see langword="null" /> or an empty string.</exception>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="arrayLength"/> is not greater than 1.</exception>
    protected internal ArrayDataVariable(string name, int arrayLength, bool isTimeSliceArray)
        : base(name, arrayLength)
    {
        ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(arrayLength, 1);

        IsTimeSliceArray = isTimeSliceArray;
        ArrayLength = arrayLength;
    }

    /// <inheritdoc />
    public int ArrayLength { get; }

    /// <inheritdoc />
    public bool IsTimeSliceArray { get; }

    /// <inheritdoc />
    public T[] Read(ReadOnlySpan<byte> source)
    {
        ThrowIfUnavailable();

        // Slice the source span to our array variable data
        var arrayBytes = GetDataSpan(source);

        // Allocate an array of values of length ValueCount
        var valueArray = new T[ArrayLength];

        // Re-interpret the value array as a span of bytes and copy the source span into it
        arrayBytes.CopyTo(MemoryMarshal.AsBytes<T>(valueArray));

        return valueArray;
    }
}
