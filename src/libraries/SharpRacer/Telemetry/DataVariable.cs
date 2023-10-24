
using System.Diagnostics.CodeAnalysis;

namespace SharpRacer.Telemetry;

/// <summary>
/// Provides a base class implementation for <see cref="IDataVariable"/>.
/// </summary>
public abstract class DataVariable : IDataVariable
{
    /// <summary>
    /// Initializes an instance of <see cref="DataVariable"/> with the specified name.
    /// </summary>
    /// <param name="name">The name of the telemetry variable.</param>
    protected DataVariable(string name)
        : this(name, valueCount: 1)
    {

    }

    /// <summary>
    /// Initializes an instance of <see cref="DataVariable"/> with the specified name and value count.
    /// </summary>
    /// <param name="name">The name of the telemetry variable.</param>
    /// <param name="valueCount">The number of values represented by the telemetry variable.</param>
    protected DataVariable(string name, int valueCount)
    {
        ArgumentException.ThrowIfNullOrEmpty(name);

        Name = name;
    }

    /// <summary>
    /// Initializes an instance of <see cref="DataVariable"/> from the specified <see cref="DataVariableInfo"/>.
    /// </summary>
    /// <param name="dataVariableInfo"></param>
    /// <exception cref="ArgumentNullException"><paramref name="dataVariableInfo"/> is null.</exception>
    protected DataVariable(DataVariableInfo dataVariableInfo)
    {
        VariableInfo = dataVariableInfo ?? throw new ArgumentNullException(nameof(dataVariableInfo));

        Name = dataVariableInfo.Name;
        IsAvailable = true;
    }

    /// <inheritdoc />
    [MemberNotNullWhen(true, nameof(VariableInfo))]
    public bool IsAvailable { get; }

    /// <inheritdoc />
    public string Name { get; }

    protected DataVariableInfo? VariableInfo { get; }

    public ReadOnlySpan<byte> GetDataSpan(IDataFrame dataFrame)
    {
        throw new NotImplementedException();
    }

    public ReadOnlySpan<byte> GetDataSpan(Span<byte> source)
    {
        throw new NotImplementedException();
    }
}
