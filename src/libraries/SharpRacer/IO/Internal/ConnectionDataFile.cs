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
    private readonly IConnectionDataMemoryOwner _memoryOwner;
    private readonly IConnectionDataSpanOwner _spanOwner;

    internal ConnectionDataFile(IMemoryMappedDataFile memoryMappedDataFile, IMappedMemory mappedMemory)
    {
        _memoryMappedDataFile = memoryMappedDataFile ?? throw new ArgumentNullException(nameof(memoryMappedDataFile));
        _mappedMemory = mappedMemory ?? throw new ArgumentNullException(nameof(mappedMemory));

        _handles = [];

        _memoryOwner = new ConnectionDataMemoryOwner(_memoryMappedDataFile.CreateMemoryAccessor(), this);
        _spanOwner = new ConnectionDataSpanOwner(_memoryMappedDataFile.CreateSpanFactory(), this);
    }

    internal ConnectionDataFile(
        IMemoryMappedDataFile memoryMappedDataFile,
        IMappedMemory mappedMemory,
        IConnectionDataMemoryOwner memoryPool,
        IConnectionDataSpanOwner spanPool)
    {
        _memoryMappedDataFile = memoryMappedDataFile ?? throw new ArgumentNullException(nameof(memoryMappedDataFile));
        _mappedMemory = mappedMemory ?? throw new ArgumentNullException(nameof(mappedMemory));
        _memoryOwner = memoryPool ?? throw new ArgumentNullException(nameof(memoryPool));
        _spanOwner = spanPool ?? throw new ArgumentNullException(nameof(spanPool));

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

            _memoryOwner.Close();
            _spanOwner.Close();
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

        return _memoryOwner.AcquireMemoryHandle();
    }

    public ConnectionDataSpanHandle RentSpan()
    {
        ObjectDisposedException.ThrowIf(_isDisposed, this);

        return _spanOwner.AcquireSpanHandle();
    }

    private void Dispose(bool disposing)
    {
        if (!_isDisposed)
        {
            if (disposing)
            {
                _isClosed = true;

                _memoryOwner.Dispose();
                _spanOwner.Dispose();

                _mappedMemory.Dispose();
                _memoryMappedDataFile.Dispose();
            }

            _isDisposed = true;
        }
    }
}
