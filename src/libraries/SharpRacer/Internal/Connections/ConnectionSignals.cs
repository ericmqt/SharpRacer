namespace SharpRacer.Internal.Connections;
internal sealed class ConnectionSignals : IConnectionSignalWaiter, IConnectionSignals, IDisposable
{
    private readonly WaitHandle[] _connectionTestWaitHandles;
    private bool _isDisposed;
    private readonly WaitHandle[] _waitHandles;

    public ConnectionSignals()
        : this(createConnectionSignalInitialState: true)
    {

    }

    public ConnectionSignals(bool createConnectionSignalInitialState)
    {
        ConnectionAvailableSignal = new ManualResetEvent(initialState: false);
        ConnectionExceptionSignal = new ManualResetEvent(initialState: false);
        CreateConnectionSignal = new AutoResetEvent(initialState: createConnectionSignalInitialState);

        // Order of wait handles is important. If two or more are signaled during a call to WaitHandle.WaitAny or WaitAll, the handle with
        // the lowest index is returned.

        _waitHandles = [ConnectionAvailableSignal, ConnectionExceptionSignal, CreateConnectionSignal];

        // Exclude CreateConnectionSignal from the connection test wait handle array
        _connectionTestWaitHandles = [ConnectionAvailableSignal, ConnectionExceptionSignal];
    }

    public ManualResetEvent ConnectionAvailableSignal { get; }
    public ManualResetEvent ConnectionExceptionSignal { get; }
    public AutoResetEvent CreateConnectionSignal { get; }

    public void Dispose()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    public WaitHandle[] GetWaitHandles(bool allowCreateConnection)
    {
        return allowCreateConnection ? _waitHandles : _connectionTestWaitHandles;
    }

    public ConnectionSignalWaitResult Wait(bool allowCreateConnection, TimeSpan waitTimeout)
    {
        return allowCreateConnection
            ? (ConnectionSignalWaitResult)WaitHandle.WaitAny(_waitHandles, waitTimeout)
            : (ConnectionSignalWaitResult)WaitHandle.WaitAny(_connectionTestWaitHandles, waitTimeout);
    }

    void IConnectionSignals.ClearConnectionAvailableSignal()
    {
        ConnectionAvailableSignal.Reset();
    }

    void IConnectionSignals.ClearConnectionExceptionSignal()
    {
        ConnectionExceptionSignal.Reset();
    }

    void IConnectionSignals.SetConnectionAvailableSignal()
    {
        ConnectionAvailableSignal.Set();
    }

    void IConnectionSignals.SetConnectionExceptionSignal()
    {
        ConnectionExceptionSignal.Set();
    }

    void IConnectionSignals.SetCreateConnectionSignal()
    {
        CreateConnectionSignal.Set();
    }

    private void Dispose(bool disposing)
    {
        if (!_isDisposed)
        {
            if (disposing)
            {
                ConnectionAvailableSignal.Dispose();
                ConnectionExceptionSignal.Dispose();
                CreateConnectionSignal.Dispose();
            }

            _isDisposed = true;
        }
    }
}
