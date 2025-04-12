using DotNext.IO.MemoryMappedFiles;

namespace SharpRacer.IO.Internal;

internal sealed class ConnectionDataFile : IConnectionDataFile, IConnectionDataFileLifetime
{
    private readonly List<IConnectionDataFileLifetimeHandle> _handles;
    private readonly object _handleLock = new();
    private bool _isClosed;
    private bool _isDisposed;
    private readonly IMappedMemory _mappedMemory;
    private readonly IMemoryMappedDataFile _memoryMappedDataFile;
    private readonly IDataFileMemoryPool _memoryPool;
    private readonly IDataFileSpanPool _spanPool;

    public ConnectionDataFile(IMemoryMappedDataFile memoryMappedDataFile)
    {
        _memoryMappedDataFile = memoryMappedDataFile ?? throw new ArgumentNullException(nameof(memoryMappedDataFile));
        _mappedMemory = _memoryMappedDataFile.CreateMemoryAccessor();
        _handles = [];

        _memoryPool = new DataFileMemoryPool(_memoryMappedDataFile, this);
        _spanPool = new DataFileMemorySpanPool(_memoryMappedDataFile, this);
    }

    public bool IsDisposed => _isDisposed;
    public bool IsOpen => !_isClosed;
    public ReadOnlyMemory<byte> Memory => _mappedMemory.Memory;

    public IConnectionDataFileLifetimeHandle AcquireLifetimeHandle()
    {
        lock (_handleLock)
        {
            if (_isClosed)
            {
                throw new InvalidOperationException("The data file is closed.");
            }

            var handle = new ConnectionDataFileLifetimeHandle(this);

            _handles.Add(handle);

            return handle;
        }
    }

    public void Close()
    {
        if (!_isClosed)
        {
            _isClosed = true;

            _memoryPool.Close();
            _spanPool.Close();
        }
    }

    public void Dispose()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    public void ReleaseLifetimeHandle(IConnectionDataFileLifetimeHandle handle)
    {
        lock (_handleLock)
        {
            if (!_handles.Remove(handle))
            {
                return;
            }

            if (_isClosed && _handles.Count == 0)
            {
                Dispose();
            }
        }
    }

    public IDataFileMemoryOwner RentMemory()
    {
        return _memoryPool.Rent();
    }

    public DataFileSpanOwner RentSpan()
    {
        return _spanPool.Rent();
    }

    private void Dispose(bool disposing)
    {
        if (!_isDisposed)
        {
            if (disposing)
            {
                _isClosed = true;

                _memoryPool.Dispose();
                _spanPool.Dispose();

                _mappedMemory.Dispose();
                _memoryMappedDataFile.Dispose();
            }

            _isDisposed = true;
        }
    }
}
