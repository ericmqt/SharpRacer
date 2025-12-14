
namespace SharpRacer.Internal.Connections;
internal sealed class ConnectionCancellationTokenSource : IConnectionCancellationTokenSource
{
    private readonly CancellationTokenSource _cancellationTokenSource;
    private bool _isDisposed;

    public ConnectionCancellationTokenSource()
    {
        _cancellationTokenSource = new CancellationTokenSource();
    }

    public CancellationToken Token => _cancellationTokenSource.Token;

    public void Cancel()
    {
        _cancellationTokenSource.Cancel();
    }

    public void Dispose()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    public CancellationTokenSource CreateLinkedTokenSource(CancellationToken cancellationToken)
    {
        return CancellationTokenSource.CreateLinkedTokenSource(Token, cancellationToken);
    }

    private void Dispose(bool disposing)
    {
        if (!_isDisposed)
        {
            if (disposing)
            {
                _cancellationTokenSource.Dispose();
            }

            _isDisposed = true;
        }
    }
}
