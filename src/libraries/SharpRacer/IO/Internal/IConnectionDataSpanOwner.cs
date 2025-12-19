namespace SharpRacer.IO.Internal;

internal interface IConnectionDataSpanOwner : IDisposable
{
    bool IsClosed { get; }
    bool IsDisposed { get; }

    void Close();
    ConnectionDataSpanHandle AcquireSpanHandle();
    void ReleaseSpanHandle(ref readonly ConnectionDataSpanHandle owner);
}
