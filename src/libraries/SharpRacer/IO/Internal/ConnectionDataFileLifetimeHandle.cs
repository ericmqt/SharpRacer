namespace SharpRacer.IO.Internal;

internal sealed class ConnectionDataFileLifetimeHandle : IConnectionDataFileLifetimeHandle
{
    private bool _isDisposed;
    private readonly IConnectionDataFileLifetime _lifetime;

    public ConnectionDataFileLifetimeHandle(IConnectionDataFileLifetime lifetime)
    {
        _lifetime = lifetime;
    }

    public bool IsDisposed => _isDisposed;

    public void Dispose()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    public void Release()
    {
        Dispose();
    }

    private void Dispose(bool disposing)
    {
        if (!_isDisposed)
        {
            if (disposing)
            {
                _lifetime.ReleaseLifetimeHandle(this);
            }

            _isDisposed = true;
        }
    }
}
