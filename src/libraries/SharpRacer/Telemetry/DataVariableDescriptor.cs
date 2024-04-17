namespace SharpRacer.Telemetry;

/// <summary>
/// Provides information about a telemetry variable which may be used to locate the variable at run-time or serve as a placeholder for a
/// variable that is not available in the current simulator session or telemetry file.
/// </summary>
public class DataVariableDescriptor
{
    /// <summary>
    /// Initializes an instance of <see cref="DataVariableDescriptor"/>.
    /// </summary>
    /// <param name="name">The telemetry variable name.</param>
    /// <param name="valueType">The type of the telemetry variable value.</param>
    /// <param name="valueCount">The number of values represented by the telemetry variable.</param>
    public DataVariableDescriptor(string name, DataVariableValueType valueType, int valueCount)
    {
        ArgumentException.ThrowIfNullOrEmpty(name);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(valueCount);

        Name = name;
        ValueType = valueType;
        ValueCount = valueCount;
    }

    /// <summary>
    /// Gets the name of the telemetry variable.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Gets the number of values represented by the telemetry variable.
    /// </summary>
    public int ValueCount { get; }

    /// <summary>
    /// Gets the type of value represented by the telemetry variable.
    /// </summary>
    public DataVariableValueType ValueType { get; }
}
