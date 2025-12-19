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

    public ConnectionDataMemoryOwner(IMappedMemory memoryAccessor, IConnectionDataFileLifetime dataFileLifetime)
    {
        ArgumentNullException.ThrowIfNull(memoryAccessor);
        ArgumentNullException.ThrowIfNull(dataFileLifetime);

        _memoryAccessor = memoryAccessor;
        _dataFileLifetimeHandle = dataFileLifetime.AcquireLifetimeHandle();

        Memory = _memoryAccessor.Memory;
    }

    public int HandleCount => _owners.Count;
    public bool IsClosed => _isClosed;
    public bool IsDisposed => _isDisposed;
    public ReadOnlyMemory<byte> Memory { get; }

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
