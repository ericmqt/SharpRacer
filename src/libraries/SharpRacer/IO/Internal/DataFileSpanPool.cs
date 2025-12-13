namespace SharpRacer.IO.Internal;

internal sealed class DataFileSpanPool : IDataFileSpanPool
{
    private readonly IConnectionDataFileLifetimeHandle _dataFileHandle;
    private bool _isClosed;
    private bool _isDisposed;
    private ulong _nextTokenId;
    private readonly IMemoryMappedFileSpanFactory _spanFactory;
    private readonly HashSet<DataFileSpanOwnerToken> _tokens;
    private readonly ReaderWriterLockSlim _tokensLock;

    public DataFileSpanPool(IMemoryMappedFileSpanFactory spanFactory, IConnectionDataFileLifetime dataFileLifetime)
    {
        ArgumentNullException.ThrowIfNull(spanFactory);
        ArgumentNullException.ThrowIfNull(dataFileLifetime);

        _dataFileHandle = dataFileLifetime.AcquireLifetimeHandle();
        _spanFactory = spanFactory;

        _tokens = [];
        _tokensLock = new ReaderWriterLockSlim();
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

    public DataFileSpanOwner Rent()
    {
        ObjectDisposedException.ThrowIf(_isDisposed, this);

        _tokensLock.EnterReadLock();

        try
        {
            if (_isClosed)
            {
                throw new InvalidOperationException("The pool is closed.");
            }

            var token = CreateToken();

            return new DataFileSpanOwner(this, token, _spanFactory.CreateReadOnlySpan());
        }
        finally
        {
            _tokensLock.ExitReadLock();
        }
    }

    public void Return(ref readonly DataFileSpanOwner owner)
    {
        var ownerRemoved = _tokens.Remove(owner.Token);

        if (_isClosed && _tokens.Count == 0)
        {
            Dispose();
        }
    }

    private DataFileSpanOwnerToken CreateToken()
    {
        DataFileSpanOwnerToken token;

        do
        {
            token = new DataFileSpanOwnerToken(Interlocked.Increment(ref _nextTokenId));
        }
        while (!_tokens.Add(token));

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
                _dataFileHandle.Dispose();
            }

            _isDisposed = true;
        }
    }
}
