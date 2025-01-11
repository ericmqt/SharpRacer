namespace SharpRacer.Internal.Connections;
internal interface IConnectionWorkerThreadOwner
{
    ReadOnlySpan<byte> Data { get; }
    TimeSpan IdleTimeout { get; }

    void OnDataReady();
    void OnWorkerThreadExit(bool canceled);
}
