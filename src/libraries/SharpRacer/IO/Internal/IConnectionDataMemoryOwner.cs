namespace SharpRacer.IO.Internal;

internal interface IConnectionDataMemoryOwner : IDisposable
{
    bool IsClosed { get; }
    bool IsDisposed { get; }
    ReadOnlyMemory<byte> Memory { get; }

    void Close();
    IConnectionDataHandle Rent();
    void Return(IConnectionDataHandle memoryOwner);
}
