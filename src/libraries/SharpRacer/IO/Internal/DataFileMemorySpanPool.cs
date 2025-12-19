using DotNext.IO.MemoryMappedFiles;

namespace SharpRacer.IO.Internal;
internal sealed class DataFileMemorySpanPool : IConnectionDataSpanOwner
{
    private readonly IConnectionDataFileLifetimeHandle _dataFileHandle;
    private bool _isClosed;
    private bool _isDisposed;
    private readonly IMappedMemory _mappedMemory;
    private readonly ReadOnlyMemory<byte> _memory;
    private ulong _nextTokenId;
    private readonly HashSet<ConnectionDataSpanHandleToken> _tokens;
    private readonly ReaderWriterLockSlim _tokensLock;

    public DataFileMemorySpanPool(IMemoryMappedDataFile memoryMappedFile, IConnectionDataFileLifetime dataFileLifetime)
    {
        ArgumentNullException.ThrowIfNull(memoryMappedFile);
        ArgumentNullException.ThrowIfNull(dataFileLifetime);

        _dataFileHandle = dataFileLifetime.AcquireLifetimeHandle();
        _mappedMemory = memoryMappedFile.CreateMemoryAccessor();
        _memory = _mappedMemory.Memory;

        _tokens = [];
        _tokensLock = new ReaderWriterLockSlim();
        _nextTokenId = 1;
    }

    public bool IsClosed => _isClosed;
    internal int OwnerCount => _tokens.Count;

    public bool IsDisposed => _isDisposed;

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
                throw new InvalidOperationException("The pool is closed.");
            }

            var token = CreateToken();

            return new ConnectionDataSpanHandle(this, token, _memory.Span);
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

                _mappedMemory.Dispose();
                _dataFileHandle.Dispose();
            }

            _isDisposed = true;
        }
    }
}
