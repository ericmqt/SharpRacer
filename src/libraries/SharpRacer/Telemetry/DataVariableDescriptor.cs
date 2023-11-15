namespace SharpRacer.Telemetry;

/// <summary>
/// 
/// </summary>
public class DataVariableDescriptor
{
    public DataVariableDescriptor(string name, DataVariableValueType valueType, int valueCount)
    {
        ArgumentException.ThrowIfNullOrEmpty(name);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(valueCount);

        Name = name;
        ValueType = valueType;
        ValueCount = valueCount;
    }

    public string Name { get; }

    public int ValueCount { get; }

    public DataVariableValueType ValueType { get; }
}
