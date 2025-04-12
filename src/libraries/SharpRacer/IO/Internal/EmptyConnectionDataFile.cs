namespace SharpRacer.IO.Internal;
internal sealed class EmptyConnectionDataFile : IConnectionDataFile
{
    private bool _isDisposed;

    public EmptyConnectionDataFile()
    {
        IsOpen = false;
    }

    public bool IsDisposed => _isDisposed;
    public bool IsOpen { get; }
    public ReadOnlyMemory<byte> Memory { get; } = ReadOnlyMemory<byte>.Empty;

    public void Close()
    {

    }

    public void Dispose()
    {

    }

    public IDataFileMemoryOwner RentMemory()
    {
        throw new NotImplementedException();
    }

    public DataFileSpanOwner RentSpan()
    {
        throw new NotImplementedException();
    }
}
