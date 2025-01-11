namespace SharpRacer.Internal.Connections;
internal interface IConnectionSignalWaiter : IConnectionSignals
{
    ConnectionSignalWaitResult Wait(bool allowCreateConnection, TimeSpan waitTimeout);
}
