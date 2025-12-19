namespace SharpRacer.IO.Internal;
internal sealed class ConnectionDataSpanOwner : IConnectionDataSpanOwner
{
    private readonly IConnectionDataFileLifetimeHandle _dataFileLifetimeHandle;
    private bool _isClosed;
    private bool _isDisposed;
    private ulong _nextTokenId;
    private readonly IConnectionDataSpanFactory _spanFactory;
    private readonly HashSet<ConnectionDataSpanHandleToken> _tokens = [];
    private readonly ReaderWriterLockSlim _tokensLock = new();

    public ConnectionDataSpanOwner(IConnectionDataSpanFactory spanFactory, IConnectionDataFileLifetimeHandle dataFileLifetimeHandle)
    {
        _spanFactory = spanFactory ?? throw new ArgumentNullException(nameof(spanFactory));
        _dataFileLifetimeHandle = dataFileLifetimeHandle ?? throw new ArgumentNullException(nameof(dataFileLifetimeHandle));
    }

    public void Close()
    {
        ObjectDisposedException.ThrowIf(_isDisposed, this);

        _tokensLock.EnterWriteLock();

        try
        {
            _isClosed = true;

            if (_tokens.Count == 0)
            {
                Dispose();
            }
        }
        finally
        {
            _tokensLock.ExitWriteLock();
        }
    }

    public void Dispose()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    public ConnectionDataSpanHandle Rent()
    {
        ObjectDisposedException.ThrowIf(_isDisposed, this);

        _tokensLock.EnterReadLock();

        try
        {
            if (_isClosed)
            {
                throw new InvalidOperationException("The span owner is closed.");
            }

            var token = CreateToken();

            return new ConnectionDataSpanHandle(this, token, _spanFactory.Create());
        }
        finally
        {
            _tokensLock.ExitReadLock();
        }
    }

    public void Return(ref readonly ConnectionDataSpanHandle owner)
    {
        _tokens.Remove(owner.Token);

        if (_isClosed && _tokens.Count == 0)
        {
            Dispose();
        }
    }

    private ConnectionDataSpanHandleToken CreateToken()
    {
        ConnectionDataSpanHandleToken token;

        do
        {
            token = new ConnectionDataSpanHandleToken(Interlocked.Increment(ref _nextTokenId));
        }
        while (token != ConnectionDataSpanHandleToken.Zero && !_tokens.Add(token));

        return token;
    }

    private void Dispose(bool disposing)
    {
        if (!_isDisposed)
        {
            if (disposing)
            {
                // TODO: Dispose remaining owners?

                _spanFactory.Dispose();
                _dataFileLifetimeHandle.Dispose();
            }

            _isDisposed = true;
        }
    }
}
