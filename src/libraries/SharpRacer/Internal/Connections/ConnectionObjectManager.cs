namespace SharpRacer.Internal.Connections;
internal sealed class ConnectionObjectManager : IConnectionObjectManager
{
    private IOpenInnerConnection? _connection;
    private SimulatorConnectionException? _connectionException;
    private readonly ReaderWriterLockSlim _readWriteLock;
    private readonly IConnectionSignals _signals;

    public ConnectionObjectManager(IConnectionSignals signals)
    {
        _signals = signals;

        _readWriteLock = new ReaderWriterLockSlim();
    }

    public bool ClearConnection(IOpenInnerConnection innerConnection)
    {
        _readWriteLock.EnterUpgradeableReadLock();

        try
        {
            if (innerConnection != _connection)
            {
                return false;
            }

            _readWriteLock.EnterWriteLock();

            try
            {
                _signals.ClearConnectionAvailableSignal();

                // Leave the connection object in place but dispose it. There is a very small chance a thread received the
                // ConnectionAvailable signal just before we entered the write lock so, if we set the connection object to null, a call to
                // GetConnection() could return null once it is unblocked after we've exited the write lock.
                //
                // We need to guarantee that a thread receiving the ConnectionAvailable signal can call GetConnection() and get a non-null
                // connection object. By the time we are here, the connection object will be returning false for calls to Attach(),
                // prompting the request to retry anyways.
                //
                // We can dispose it here, though, which is good enough.

                _connection.Dispose();

                return true;
            }
            finally
            {
                _readWriteLock.ExitWriteLock();
            }
        }
        finally
        {
            _readWriteLock.ExitUpgradeableReadLock();
        }
    }

    public void ClearConnectionException()
    {
        _readWriteLock.EnterWriteLock();

        try
        {
            _signals.ClearConnectionExceptionSignal();

            // This is OK here, unlike in ClearConnection, because calling this method requires:
            //     1) Blocking new requests from being queued
            //     2) Blocking execution of synchronous requests
            //     3) All pending async requests are processed
            // By the time we are here, there should be no threads having just received the exception signal about to read the exception.

            _connectionException = null;
        }
        finally
        {
            _readWriteLock.ExitWriteLock();
        }
    }

    public IOpenInnerConnection? GetConnection()
    {
        _readWriteLock.EnterReadLock();

        try { return _connection; }
        finally
        {
            _readWriteLock.ExitReadLock();
        }
    }

    public SimulatorConnectionException? GetConnectionException()
    {
        _readWriteLock.EnterReadLock();

        try { return _connectionException; }
        finally
        {
            _readWriteLock.ExitReadLock();
        }
    }

    public void SetConnection(IOpenInnerConnection innerConnection)
    {
        _readWriteLock.EnterWriteLock();

        try
        {
            _connection = innerConnection;
            _signals.SetConnectionAvailableSignal();
        }
        finally
        {
            _readWriteLock.ExitWriteLock();
        }
    }

    public void SetConnectionException(SimulatorConnectionException connectionException)
    {
        _readWriteLock.EnterWriteLock();

        try
        {
            _connectionException = connectionException;
            _signals.SetConnectionExceptionSignal();
        }
        finally
        {
            _readWriteLock.ExitWriteLock();
        }
    }
}
