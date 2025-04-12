using System.IO.MemoryMappedFiles;

namespace SharpRacer.IO.Internal;
internal sealed unsafe class MemoryMappedFileSpanFactory : IMemoryMappedFileSpanFactory
{
    private readonly MemoryMappedViewAccessor _accessor;
    private bool _isDisposed;
    private readonly byte* _ptr;

    public MemoryMappedFileSpanFactory(MemoryMappedViewAccessor accessor)
    {
        _accessor = accessor;

        _ptr = default;
        _accessor.SafeMemoryMappedViewHandle.AcquirePointer(ref _ptr);
    }

    public void Dispose()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    public ReadOnlySpan<byte> CreateReadOnlySpan()
    {
        ObjectDisposedException.ThrowIf(_isDisposed, this);

        return new ReadOnlySpan<byte>(_ptr, int.CreateSaturating(_accessor.Capacity));
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
