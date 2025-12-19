using System.IO.MemoryMappedFiles;

namespace SharpRacer.IO.Internal;
internal sealed unsafe class ConnectionDataSpanFactory : IConnectionDataSpanFactory
{
    private readonly MemoryMappedViewAccessor _accessor;
    private bool _isDisposed;
    private readonly int _length;
    private readonly byte* _ptr;

    public ConnectionDataSpanFactory(MemoryMappedViewAccessor accessor)
    {
        _accessor = accessor ?? throw new ArgumentNullException(nameof(accessor));

        _ptr = default;
        _length = int.CreateSaturating(_accessor.Capacity);

        _accessor.SafeMemoryMappedViewHandle.AcquirePointer(ref _ptr);
    }

    public ReadOnlySpan<byte> Create()
    {
        ObjectDisposedException.ThrowIf(_isDisposed, this);

        return new ReadOnlySpan<byte>(_ptr, _length);
    }

    public void Dispose()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    private void Dispose(bool disposing)
    {
        if (!_isDisposed)
        {
            if (disposing)
            {
                _accessor.SafeMemoryMappedViewHandle.ReleasePointer();
                _accessor.Dispose();
            }

            _isDisposed = true;
        }
    }
}
