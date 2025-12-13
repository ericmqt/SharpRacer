using DotNext.IO.MemoryMappedFiles;

namespace SharpRacer.IO.Internal;

internal sealed class DataFileMemoryPool : IDataFileMemoryPool
{
    private readonly IConnectionDataFileLifetimeHandle _dataFileHandle;
    private bool _isClosed;
    private bool _isDisposed;
    private readonly IMappedMemory _mappedMemory;
    private readonly ReadOnlyMemory<byte> _memory;
    private readonly List<IDataFileMemoryOwner> _owners;
    private readonly object _rentalLock = new();

    public DataFileMemoryPool(IMemoryMappedDataFile memoryMappedFile, IConnectionDataFileLifetime dataFileLifetime)
    {
        ArgumentNullException.ThrowIfNull(memoryMappedFile);
        ArgumentNullException.ThrowIfNull(dataFileLifetime);

        _dataFileHandle = dataFileLifetime.AcquireLifetimeHandle();

        _mappedMemory = memoryMappedFile.CreateMemoryAccessor();
        _memory = _mappedMemory.Memory;
        _owners = [];
    }

    internal bool IsClosed => _isClosed;
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

    public IDataFileMemoryOwner Rent()
    {
        ObjectDisposedException.ThrowIf(_isDisposed, this);

        lock (_rentalLock)
        {
            if (_isClosed)
            {
                throw new InvalidOperationException("The pool is closed.");
            }

            var owner = new DataFileMemoryOwner(_memory, this);

            _owners.Add(owner);

            return owner;
        }
    }

    public void Return(IDataFileMemoryOwner memoryOwner)
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
                _mappedMemory.Dispose();
                _dataFileHandle.Dispose();
            }

            _isDisposed = true;
        }
    }
}
