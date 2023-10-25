using System.Runtime.InteropServices;

namespace SharpRacer.Telemetry;

/// <summary>
/// Represents a telemetry variable with a value of type <typeparamref name="T"/>.
/// </summary>
/// <typeparam name="T">The telemetry variable value type.</typeparam>
public class DataVariable<T> : DataVariableBase<T>, IDataVariable<T>
    where T : unmanaged
{
    /// <summary>
    /// Initializes an instance of <see cref="DataVariable{T}"/> from the specified <seealso cref="DataVariableInfo"/>.
    /// </summary>
    /// <param name="dataVariableInfo">The <see cref="DataVariableInfo"/> object from which to initialize this instance.</param>
    /// <exception cref="ArgumentNullException"><paramref name="dataVariableInfo"/> is <see langword="null" />.</exception>
    public DataVariable(DataVariableInfo dataVariableInfo)
        : base(dataVariableInfo)
    {
    }

    /// <summary>
    /// Initializes an instance of <see cref="DataVariable{T}"/> with the specified variable name that represents a telemetry variable that
    /// is not available in the current context.
    /// </summary>
    /// <param name="name">The telemetry variable name.</param>
    protected internal DataVariable(string name)
        : base(name, valueCount: 1)
    {
    }

    /// <inheritdoc />
    public T Read(ReadOnlySpan<byte> source)
    {
        ThrowIfUnavailable();

        var valueBytes = GetDataSpan(source);

        return MemoryMarshal.Read<T>(valueBytes);
    }
}
