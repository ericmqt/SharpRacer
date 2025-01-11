namespace SharpRacer.Internal.Connections;
internal interface IOpenInnerConnection : IInnerConnection
{
    bool Attach(IOuterConnection outerConnection);
    IDisposable AcquireDataFileLifetimeHandle();
    void StartWorkerThread();
}
