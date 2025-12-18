
namespace SharpRacer.IO.Internal;
internal sealed class EmptyConnectionDataFile : IConnectionDataFile
{
    private bool _isDisposed;

    public EmptyConnectionDataFile()
    {
        IsOpen = false;
    }

    public IEnumerable<IConnectionDataFileLifetimeHandle> Handles => Enumerable.Empty<IConnectionDataFileLifetimeHandle>();
    public bool IsDisposed => _isDisposed;
    public bool IsOpen { get; }
    public ReadOnlyMemory<byte> Memory { get; } = ReadOnlyMemory<byte>.Empty;

    public void Close()
    {

    }

    public void Dispose()
    {
        _isDisposed = true;
    }

    public IConnectionDataHandle RentMemory()
    {
        throw new InvalidOperationException("The connection is not open.");
    }

    public DataFileSpanOwner RentSpan()
    {
        throw new InvalidOperationException("The connection is not open.");
    }
}
