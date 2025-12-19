namespace SharpRacer.IO.Internal;

internal interface IConnectionDataSpanOwner : IDisposable
{
    bool IsClosed { get; }
    bool IsDisposed { get; }

    void Close();
    ConnectionDataSpanHandle Rent();
    void Return(ref readonly ConnectionDataSpanHandle owner);
}
