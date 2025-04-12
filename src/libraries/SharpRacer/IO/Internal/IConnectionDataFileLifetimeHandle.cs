namespace SharpRacer.IO.Internal;

internal interface IConnectionDataFileLifetimeHandle : IDisposable
{
    void Release();
}
