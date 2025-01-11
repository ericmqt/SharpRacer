using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Versioning;
using SharpRacer.Internal.Connections.Requests;
using SharpRacer.Interop;

namespace SharpRacer.Internal.Connections;

[SupportedOSPlatform("windows")]
internal sealed class ConnectionManager : IConnectionManager, IConnectionProvider, IOpenInnerConnectionOwner
{
    private readonly IConnectionAcquisitionHandler _connectionAcquisitionHandler;
    private readonly IConnectionSignalWaiter _connectionSignals;
    private int _nextConnectionId;
    private readonly IConnectionObjectManager _objectManager;
    private readonly IConnectionRequestManager _requestManager;
    private readonly TimeProvider _timeProvider;

    public ConnectionManager()
        : this(new ConnectionSignals(), new ConnectionRequestManager())
    {

    }

    public ConnectionManager(IConnectionSignalWaiter connectionSignals, IConnectionRequestManager requestManager)
        : this(connectionSignals, requestManager, new ConnectionObjectManager(connectionSignals))
    {

    }

    public ConnectionManager(
        IConnectionSignalWaiter connectionSignals,
        IConnectionRequestManager requestManager,
        IConnectionObjectManager objectManager)
        : this(connectionSignals,
              requestManager,
              objectManager,
              new ConnectionAcquisitionHandler(objectManager, requestManager, connectionSignals))
    {

    }

    public ConnectionManager(
        IConnectionSignalWaiter connectionSignals,
        IConnectionRequestManager requestManager,
        IConnectionObjectManager objectManager,
        IConnectionAcquisitionHandler connectionAcquisitionHandler)
        : this(connectionSignals, requestManager, objectManager, connectionAcquisitionHandler, TimeProvider.System)
    {

    }

    public ConnectionManager(
        IConnectionSignalWaiter connectionSignals,
        IConnectionRequestManager requestManager,
        IConnectionObjectManager objectManager,
        IConnectionAcquisitionHandler connectionAcquisitionHandler,
        TimeProvider timeProvider)
    {
        _objectManager = objectManager;
        _connectionSignals = connectionSignals;
        _requestManager = requestManager;
        _connectionAcquisitionHandler = connectionAcquisitionHandler;
        _timeProvider = timeProvider;
    }

    public static ConnectionManager Default { get; } = new ConnectionManager();

    public void Connect(IOuterConnection outerConnection)
    {
        Connect(outerConnection, Timeout.InfiniteTimeSpan);
    }

    public void Connect(IOuterConnection outerConnection, TimeSpan timeout)
    {
        ArgumentNullException.ThrowIfNull(outerConnection);
        RequestExceptions.ThrowIfTimeoutArgumentIsNegative(timeout);

        var request = _requestManager.CreateRequest(outerConnection, timeout);

        request.Execute(this);
    }

    public Task ConnectAsync(IOuterConnection outerConnection, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(outerConnection);

        return ConnectAsync(outerConnection, Timeout.InfiniteTimeSpan, cancellationToken);
    }

    public async Task ConnectAsync(
        IOuterConnection outerConnection,
        TimeSpan timeout,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(outerConnection);
        RequestExceptions.ThrowIfTimeoutArgumentIsNegative(timeout);

        using var request = _requestManager.CreateAsyncRequest(outerConnection, timeout, cancellationToken);

        if (!request.TryComplete(this))
        {
            _requestManager.QueueAsyncRequest(request);

            // Process the queue on the thread pool so we don't block our caller, which is presumably an async method
            ThreadPool.QueueUserWorkItem<object?>(_ => _requestManager.ProcessAsyncRequestQueue(this, force: false), null, false);
        }

        await request.Completion.Task.ConfigureAwait(false);
    }

    public bool TryGetConnection(
        IAsyncConnectionRequest asyncConnectionRequest,
        [NotNullWhen(true)] out IOpenInnerConnection? innerConnection)
    {
        // TryGetConnection is called with zero timeout regardless of the specified request timeout in order to perform a non-blocking
        // test of whether a connection or connection exception is available, allowing other pending requests to be checked in a
        // timely manner.

        return TryGetConnection(TimeSpan.Zero, asyncConnectionRequest.CanBeginConnectionAttempt, out innerConnection);
    }

    public bool TryGetConnection(
        TimeSpan waitTimeout,
        bool allowCreateConnection,
        [NotNullWhen(true)] out IOpenInnerConnection? innerConnection)
    {
        var waitResult = _connectionSignals.Wait(allowCreateConnection, waitTimeout);

        switch (waitResult)
        {
            case ConnectionSignalWaitResult.ConnectionAvailable:
                var connectionObj = _objectManager.GetConnection();
                Debug.Assert(connectionObj != null, "Connected signal received but connection object was null");
                innerConnection = connectionObj;
                return true;

            case ConnectionSignalWaitResult.ConnectionException:
                var cachedException = _objectManager.GetConnectionException();
                Debug.Assert(cachedException != null, "Connection exception signal received but exception was null");
                throw cachedException;

            // Connection creation signal is an auto-reset event, so only one thread will be allowed through when it is signaled in order
            // to start the connection attempt, then the thread tries again with allowCreateConnection set to false.
            case ConnectionSignalWaitResult.CreateConnection:
                // Start a connection attempt on a background thread, which will run until success, error, or no pending connection
                // requests remain
                StartConnectionAcquisitionWorker();

                // Try again
                return TryGetConnection(waitTimeout, false, out innerConnection);

            case ConnectionSignalWaitResult.WaitTimeout:
                innerConnection = null;
                return false;

            default:
                throw new InvalidOperationException($"Unexpected wait result: {waitResult}");
        }
    }

    private void StartConnectionAcquisitionWorker()
    {
        var dataReadyEventFactory = DataReadyEventFactory.Default;
        var connectionFactory = new OpenInnerConnectionFactory(dataReadyEventFactory, _timeProvider);

        var worker = _connectionAcquisitionHandler.CreateWorker(this, connectionFactory, this, dataReadyEventFactory);

        worker.Start();
    }

    int IOpenInnerConnectionOwner.NewConnectionId()
    {
        return Interlocked.Increment(ref _nextConnectionId);
    }

    void IOpenInnerConnectionOwner.OnConnectionClosing(IOpenInnerConnection ownedConnection)
    {
        // Pause new connection requests while we destroy the current connection
        using var pauseNewRequestsScope = _requestManager.CreateRequestBlockingScope();

        if (!_objectManager.ClearConnection(ownedConnection))
        {
            // Dispose if this wasn't the connection owned by the object manager
            ownedConnection.Dispose();

            return;
        }

        // Allow a new connection to be established.
        _connectionSignals.SetCreateConnectionSignal();
    }
}
