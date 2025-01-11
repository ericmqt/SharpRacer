namespace SharpRacer.Internal.Connections;
internal interface IConnectionSignals
{
    void ClearConnectionAvailableSignal();
    void ClearConnectionExceptionSignal();
    void SetConnectionAvailableSignal();
    void SetConnectionExceptionSignal();
    void SetCreateConnectionSignal();
}
