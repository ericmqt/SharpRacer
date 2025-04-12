namespace SharpRacer.IO.Internal;

internal interface IConnectionDataFile : IDisposable
{
    bool IsDisposed { get; }
    bool IsOpen { get; }
    ReadOnlyMemory<byte> Memory { get; }

    void Close();
    IDataFileMemoryOwner RentMemory();
    DataFileSpanOwner RentSpan();
}
