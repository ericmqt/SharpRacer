using System.IO.MemoryMappedFiles;
using DotNext.IO.MemoryMappedFiles;

namespace SharpRacer.IO.Internal;
internal sealed class MappedDataFile : IMemoryMappedDataFile
{
    private readonly IMappedMemory _mappedMemory;
    private readonly ReadOnlyMemory<byte> _memory;
    private readonly MemoryMappedFile _memoryMappedFile;
    private bool _isDisposed;

    public MappedDataFile(MemoryMappedFile memoryMappedFile)
    {
        _memoryMappedFile = memoryMappedFile;

        _mappedMemory = _memoryMappedFile.CreateMemoryAccessor(access: MemoryMappedFileAccess.Read);
        _memory = _mappedMemory.Memory;
    }

    public ReadOnlyMemory<byte> Data
    {
        get
        {
            ObjectDisposedException.ThrowIf(_isDisposed, this);

            return _memory;
        }
    }

    public IMappedMemory CreateMemoryAccessor()
    {
        return _memoryMappedFile.CreateMemoryAccessor(access: MemoryMappedFileAccess.Read);
    }

    public IMemoryMappedFileSpanFactory CreateSpanAccessor()
    {
        return new MemoryMappedFileSpanFactory(_memoryMappedFile.CreateViewAccessor(0, 0, MemoryMappedFileAccess.Read));
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
                _mappedMemory.Dispose();
                _memoryMappedFile.Dispose();
            }

            _isDisposed = true;
        }
    }

}
