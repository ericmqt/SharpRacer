using SharpRacer.IO;

namespace SharpRacer.Internal.Connections;
internal interface IConnectionWorkerThreadOwner
{
    TimeSpan IdleTimeout { get; }

    ConnectionDataSpanHandle AcquireDataSpanHandle();
    void OnDataReady();
    void OnWorkerThreadExit(bool canceled);
}
