namespace SharpRacer.Telemetry;
public class DataVariable<T> : DataVariable, IDataVariable<T>
    where T : unmanaged
{
    public DataVariable(string name)
        : base(name)
    {
    }

    public DataVariable(DataVariableInfo dataVariableInfo)
        : base(dataVariableInfo)
    {
    }
}
