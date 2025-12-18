using DotNext.IO.MemoryMappedFiles;

namespace SharpRacer.IO.Internal;

internal sealed class ConnectionDataMemoryOwner : IConnectionDataMemoryOwner
{
    private readonly IConnectionDataFileLifetimeHandle _dataFileLifetimeHandle;
    private bool _isClosed;
    private bool _isDisposed;
    private readonly IMappedMemory _memoryAccessor;
    private readonly List<IConnectionDataHandle> _owners = [];
    private readonly object _rentalLock = new();

    public ConnectionDataMemoryOwner(IMemoryMappedDataFile memoryMappedFile, IConnectionDataFileLifetime dataFileLifetime)
    {
        ArgumentNullException.ThrowIfNull(memoryMappedFile);
        ArgumentNullException.ThrowIfNull(dataFileLifetime);

        _dataFileLifetimeHandle = dataFileLifetime.AcquireLifetimeHandle();
        _memoryAccessor = memoryMappedFile.CreateMemoryAccessor();

        Memory = _memoryAccessor.Memory;
    }

    public ConnectionDataMemoryOwner(IMappedMemory memoryAccessor, IConnectionDataFileLifetime dataFileLifetime)
    {
        ArgumentNullException.ThrowIfNull(memoryAccessor);
        ArgumentNullException.ThrowIfNull(dataFileLifetime);

        _memoryAccessor = memoryAccessor;
        _dataFileLifetimeHandle = dataFileLifetime.AcquireLifetimeHandle();

        Memory = _memoryAccessor.Memory;
    }

    public ConnectionDataMemoryOwner(IMappedMemory memoryAccessor, IConnectionDataFileLifetimeHandle dataFileLifetimeHandle)
    {
        _memoryAccessor = memoryAccessor ?? throw new ArgumentNullException(nameof(memoryAccessor));
        _dataFileLifetimeHandle = dataFileLifetimeHandle ?? throw new ArgumentNullException(nameof(dataFileLifetimeHandle));

        Memory = _memoryAccessor.Memory;
    }

    internal bool IsClosed => _isClosed;
    public ReadOnlyMemory<byte> Memory { get; }
    internal int OwnerCount => _owners.Count;

    public void Close()
    {
        lock (_rentalLock)
        {
            _isClosed = true;

            // Dispose immediately if there are no outstanding rentals that can be returned to trigger self-disposal
            if (_owners.Count == 0)
            {
                Dispose();
            }
        }
    }

    public void Dispose()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    public IConnectionDataHandle Rent()
    {
        ObjectDisposedException.ThrowIf(_isDisposed, this);

        lock (_rentalLock)
        {
            if (_isClosed)
            {
                throw new InvalidOperationException("The pool is closed.");
            }

            var owner = new ConnectionDataHandle(Memory, this);

            _owners.Add(owner);

            return owner;
        }
    }

    public void Return(IConnectionDataHandle memoryOwner)
    {
        _owners.Remove(memoryOwner);

        if (_isClosed && _owners.Count == 0)
        {
            Dispose();
        }
    }

    private void Dispose(bool disposing)
    {
        if (!_isDisposed)
        {
            if (disposing)
            {
                // TODO: Dispose remaining owners?
                _memoryAccessor.Dispose();
                _dataFileLifetimeHandle.Dispose();
            }

            _isDisposed = true;
        }
    }
}
