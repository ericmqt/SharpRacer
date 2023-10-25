using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace SharpRacer.Telemetry;

/// <summary>
/// Defines a base class for type-safe <see cref="IDataVariable"/> implementations.
/// </summary>
/// <typeparam name="T">
/// The value type represented by the variable or, if the variable is an array, the element type of the array.
/// </typeparam>
public abstract class DataVariableBase<T> : IDataVariable
    where T : unmanaged
{
    /// <summary>
    /// Initializes an instance of <see cref="DataVariableBase{T}"/> that represents a telemetry variable that is not available in the
    /// current context. Sets <see cref="IsAvailable"/> to <see langword="false" />.
    /// </summary>
    /// <param name="name">The name of the telemetry variable.</param>
    /// <param name="valueCount">The number of values in the telemetry variable.</param>
    /// <exception cref="ArgumentException">
    /// <paramref name="name"/> is <see langword="null" /> or an empty string.
    /// </exception>
    protected DataVariableBase(string name, int valueCount)
    {
        Name = !string.IsNullOrEmpty(name)
           ? name
           : throw new ArgumentException($"'{nameof(name)}' cannot be null or empty.", nameof(name));

        DataOffset = -1;
        DataLength = Unsafe.SizeOf<T>() * valueCount;

        IsAvailable = false;
    }

    /// <summary>
    /// Initializes an instance of  <see cref="DataVariableBase{T}"/> from the specified <see cref="DataVariableInfo"/>.
    /// </summary>
    /// <param name="dataVariableInfo">The <see cref="DataVariableInfo"/> object from which to initialize this instance.</param>
    /// <exception cref="ArgumentNullException"><paramref name="dataVariableInfo"/> is <see langword="null" />.</exception>
    protected DataVariableBase(DataVariableInfo dataVariableInfo)
    {
        ArgumentNullException.ThrowIfNull(dataVariableInfo);

        Name = dataVariableInfo.Name;
        IsAvailable = true;

        DataOffset = dataVariableInfo.Offset;
        DataLength = Unsafe.SizeOf<T>() * dataVariableInfo.ValueCount;
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
    protected void ThrowIfUnavailable()
    {
        if (!IsAvailable)
        {
            throw new DataVariableUnavailableException(Name);
        }
    }
}
