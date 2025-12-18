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
    private readonly IConnectionDataMemoryOwner _memoryPool;
    private readonly IDataFileSpanPool _spanPool;

    internal ConnectionDataFile(IMemoryMappedDataFile memoryMappedDataFile, IMappedMemory mappedMemory)
    {
        _memoryMappedDataFile = memoryMappedDataFile ?? throw new ArgumentNullException(nameof(memoryMappedDataFile));
        _mappedMemory = mappedMemory ?? throw new ArgumentNullException(nameof(mappedMemory));

        _handles = [];

        _memoryPool = new ConnectionDataMemoryOwner(_memoryMappedDataFile, this);
        _spanPool = new DataFileMemorySpanPool(_memoryMappedDataFile, this);
    }

    internal ConnectionDataFile(
        IMemoryMappedDataFile memoryMappedDataFile,
        IMappedMemory mappedMemory,
        IConnectionDataMemoryOwner memoryPool,
        IDataFileSpanPool spanPool)
    {
        _memoryMappedDataFile = memoryMappedDataFile ?? throw new ArgumentNullException(nameof(memoryMappedDataFile));
        _mappedMemory = mappedMemory ?? throw new ArgumentNullException(nameof(mappedMemory));
        _memoryPool = memoryPool ?? throw new ArgumentNullException(nameof(memoryPool));
        _spanPool = spanPool ?? throw new ArgumentNullException(nameof(spanPool));

        _handles = [];
    }

    public IEnumerable<IConnectionDataFileLifetimeHandle> Handles => _handles;
    public bool IsDisposed => _isDisposed;
    public bool IsOpen => !_isClosed;
    public ReadOnlyMemory<byte> Memory => _mappedMemory.Memory;

    public IConnectionDataFileLifetimeHandle AcquireLifetimeHandle()
    {
        ObjectDisposedException.ThrowIf(_isDisposed, this);

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

    public IConnectionDataHandle RentMemory()
    {
        ObjectDisposedException.ThrowIf(_isDisposed, this);

        return _memoryPool.Rent();
    }

    public DataFileSpanOwner RentSpan()
    {
        ObjectDisposedException.ThrowIf(_isDisposed, this);

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
