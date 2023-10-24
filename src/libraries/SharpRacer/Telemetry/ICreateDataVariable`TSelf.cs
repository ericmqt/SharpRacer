namespace SharpRacer.Telemetry;

public interface ICreateDataVariable<TSelf, T>
    where TSelf : IDataVariable<T>
    where T : unmanaged
{
    static abstract TSelf Create(string variableName);
    static abstract TSelf Create(DataVariableInfo dataVariableInfo);
}
