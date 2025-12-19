namespace SharpRacer.IO.Internal;

internal interface IConnectionDataFile : IDisposable
{
    IEnumerable<IConnectionDataFileLifetimeHandle> Handles { get; }
    bool IsDisposed { get; }
    bool IsOpen { get; }
    ReadOnlyMemory<byte> Memory { get; }

    void Close();
    IConnectionDataHandle GetMemoryHandle();
    ConnectionDataSpanHandle GetSpanHandle();
}
