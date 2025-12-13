namespace SharpRacer.IO.Internal;

internal interface IConnectionDataFileLifetimeHandle : IDisposable
{
    bool IsDisposed { get; }

    void Release();
}
