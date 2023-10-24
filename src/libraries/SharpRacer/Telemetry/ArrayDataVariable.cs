namespace SharpRacer.Telemetry;

public class ArrayDataVariable<T> : DataVariable, IArrayDataVariable<T>
    where T : unmanaged
{
    public ArrayDataVariable(string name, int valueCount)
        : base(name, valueCount)
    {
    }

    public ArrayDataVariable(DataVariableInfo dataVariableInfo)
        : base(dataVariableInfo)
    {

    }

    public int ValueCount { get; }
}
