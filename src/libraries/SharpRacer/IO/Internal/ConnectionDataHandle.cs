namespace SharpRacer.IO.Internal;

internal sealed class ConnectionDataHandle : IConnectionDataHandle
{
    private bool _isDisposed;
    private readonly ReadOnlyMemory<byte> _memory;
    private readonly IConnectionDataMemoryOwner _pool;

    public ConnectionDataHandle(ReadOnlyMemory<byte> memory, IConnectionDataMemoryOwner pool)
    {
        _memory = memory;
        _pool = pool;
    }

    public ReadOnlyMemory<byte> Memory
    {
        get
        {
            ObjectDisposedException.ThrowIf(_isDisposed, this);

            return _memory;
        }
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
                _pool.Return(this);
            }

            _isDisposed = true;
        }
    }
}
