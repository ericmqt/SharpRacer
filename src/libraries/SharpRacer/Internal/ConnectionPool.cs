using System.Collections.Concurrent;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Versioning;

namespace SharpRacer.Internal;

[SupportedOSPlatform("windows5.1.2600")]
internal sealed partial class ConnectionPool : IConnectionPool
{
    // Outer connection tracking
    private bool _canAddOuterConnections;
    private readonly List<SimulatorConnection> _outerConnections;
    private readonly object _outerConnectionsLock = new object();

    // Synchronization primitives
    private readonly ManualResetEventSlim _allowConnectSignal;
    private readonly SemaphoreSlim _destroyConnectionSemaphore;
    private readonly SemaphoreSlim _requestQueueProcessingSemaphore;
    private readonly ConnectionWaitHandles _waitHandles;

    private SimulatorConnectionException? _connectionException;
    private DataReadyCallback? _dataReadyCallback;
    private OpenInternalConnection? _internalConnection;
    private int _nextConnectionId;
    private int _pendingConnectionCount;
    private readonly ConcurrentQueue<AsyncConnectionRequest> _requestQueue;

    private ConnectionPool()
    {
        _requestQueue = new ConcurrentQueue<AsyncConnectionRequest>();
        _outerConnections = new List<SimulatorConnection>();

        _allowConnectSignal = new ManualResetEventSlim(initialState: true);
        _destroyConnectionSemaphore = new SemaphoreSlim(1, 1);
        _requestQueueProcessingSemaphore = new SemaphoreSlim(1, 1);
        _waitHandles = new ConnectionWaitHandles();
    }

    internal static IConnectionPool Default { get; } = new ConnectionPool();

    public void Connect(SimulatorConnection outerConnection)
    {
        Connect(outerConnection, Timeout.InfiniteTimeSpan);
    }

    public void Connect(SimulatorConnection outerConnection, TimeSpan timeout)
    {
        ArgumentNullException.ThrowIfNull(outerConnection);

        var isTimeoutInfinite = timeout == Timeout.InfiniteTimeSpan;
        var throwTimeoutException = timeout > TimeSpan.Zero;
        var connectStart = TimeProvider.System.GetTimestamp();

        // Loop until a connection is successful or timeout expires. The do/while exists solely for handling the case where an
        // internal connection object was received, but AttachOuterConnection returned false due to the internal connection starting to
        // close after acquisition.
        do
        {
            _allowConnectSignal.Wait();

            Interlocked.Increment(ref _pendingConnectionCount);

            try
            {
                if (!TryGetConnection(timeout, timeout > TimeSpan.Zero, out var internalConnection))
                {
                    // Timeout elapsed
                    break;
                }

                // Check the outer connection can be attached
                if (AttachOuterConnection(internalConnection, outerConnection))
                {
                    return;
                }

                // Internal connection began closing after being returned from TryGetConnection, so update the timeout period and try again
                if (!isTimeoutInfinite)
                {
                    timeout -= TimeProvider.System.GetElapsedTime(connectStart);
                }
            }
            finally
            {
                Interlocked.Decrement(ref _pendingConnectionCount);
            }
        }
        while (isTimeoutInfinite || timeout >= TimeSpan.Zero);

        // Exited the loop without successfully connecting
        if (throwTimeoutException)
        {
            throw new TimeoutException("The timeout period elapsed before a connection could be established.");
        }

        throw new SimulatorConnectionException("Failed to establish a connection to the simulator.");
    }

    public Task ConnectAsync(SimulatorConnection outerConnection, CancellationToken cancellationToken = default)
    {
        return ConnectAsync(outerConnection, Timeout.InfiniteTimeSpan, cancellationToken);
    }

    public async Task ConnectAsync(SimulatorConnection outerConnection, TimeSpan timeout, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(outerConnection);

        var completion = new TaskCompletionSource();

        CancellationTokenRegistration cancellationRegistration = default;

        if (cancellationToken != default)
        {
            cancellationRegistration = cancellationToken.Register(() => completion.TrySetCanceled());
        }

        try
        {
            _allowConnectSignal.Wait(CancellationToken.None);
            Interlocked.Increment(ref _pendingConnectionCount);

            var request = new AsyncConnectionRequest(outerConnection, completion, timeout);

            // Test if the request can be immediately completed, otherwise add it to the queue
            if (!TryCompleteRequest(request))
            {
                _requestQueue.Enqueue(request);

                ThreadPool.QueueUserWorkItem<object?>(_ => ProcessRequestQueue(), null, false);
            }

            await completion.Task;
        }
        finally
        {
            if (cancellationRegistration != default)
            {
                cancellationRegistration.Unregister();
            }
        }
    }

    public void ReleaseOuterConnection(SimulatorConnection outerConnection)
    {
        var destroyInnerConnection = false;

        lock (_outerConnectionsLock)
        {
            // If the outer connection is not part of the current set, the internal connection would already have been disposed, so don't
            // do anything here if Remove returns false.

            if (_outerConnections.Remove(outerConnection)
                && _outerConnections.Count == 0
                && _canAddOuterConnections)
            {
                _canAddOuterConnections = false;

                destroyInnerConnection = true;
            }
        }

        if (destroyInnerConnection)
        {
            DestroyConnection();
        }
    }

    /// <summary>
    /// Allows <see cref="OpenInternalConnection"/> to tell the pool it has disconnected and that the pool should clean up associated
    /// outer connections before resetting the pool connection state.
    /// </summary>
    /// <param name="connection"></param>
    public void Return(OpenInternalConnection connection)
    {
        // If the connection isn't the current one, just clean it up and move on. I have no idea how this could occur, but better to be
        // safe than wreck the current connection for no reason.
        if (connection != _internalConnection)
        {
            connection.Dispose();

            return;
        }

        DestroyConnection();
    }

    private bool TryGetConnection(TimeSpan waitTimeout, bool allowCreateConnection, [NotNullWhen(true)] out OpenInternalConnection? internalConnection)
    {
        var waitResult = _waitHandles.WaitAny(allowCreateConnection, waitTimeout);

        switch (waitResult)
        {
            case ConnectionWaitHandles.ConnectionAvailableWaitIndex:
                Debug.Assert(_internalConnection != null, "Connected signal received but connection object was null");
                internalConnection = _internalConnection;
                return true;

            case ConnectionWaitHandles.ConnectionExceptionWaitIndex:
                Debug.Assert(_connectionException != null, "Connection exception signal received but exception was null");
                var cachedException = _connectionException;
                throw cachedException;

            case ConnectionWaitHandles.CreateConnectionWaitIndex:
                BeginConnection();
                return TryGetConnection(waitTimeout, false, out internalConnection);

            case WaitHandle.WaitTimeout:
                internalConnection = null;
                return false;

            default:
                throw new InvalidOperationException($"Unexpected wait result: {waitResult}");
        }
    }

    private bool AttachOuterConnection(OpenInternalConnection internalConnection, SimulatorConnection outerConnection)
    {
        lock (_outerConnectionsLock)
        {
            if (!_canAddOuterConnections)
            {
                return false;
            }

            if (!_outerConnections.Contains(outerConnection))
            {
                _outerConnections.Add(outerConnection);

                outerConnection.SetOpenInternalConnection(internalConnection);
            }

            // Return true even if owner already exists
            return true;
        }
    }

    private void BeginConnection()
    {
        var oldCallback = Interlocked.Exchange(
            ref _dataReadyCallback,
            DataReadyCallback.StartNew(OnDataReadyCallbackInvoked, TimeSpan.FromMilliseconds(32)));

        oldCallback?.Dispose();
    }

    private void CreateConnection()
    {
        try
        {
            var dataFile = MemoryMappedDataFile.Open();

            var connection = new OpenInternalConnection(
                dataFile,
                Interlocked.Increment(ref _nextConnectionId),
                this);

            SetConnection(connection);
        }
        catch (Exception ex)
        {
            var connEx = GetConnectionException("Failed to connect to the simulator. See inner exception for details.", ex);

            SetConnectionException(connEx);
        }
    }

    private void DestroyConnection()
    {
        _destroyConnectionSemaphore.Wait();

        try
        {
            if (_internalConnection is null)
            {
                return;
            }

            _canAddOuterConnections = false;

            // Disable the connection available signal, which was presumably set because we had an active internal connection, and put the
            // pool into a state where none of the connection wait handles are set. Any non-zero-timeout synchronous waiters will block
            // for the brief moment we need to reset.
            _waitHandles.ConnectionAvailableSignal.Reset();

            // Pause new connection requests while we destroy the current connection
            _allowConnectSignal.Reset();

            // Close the outer connections
            lock (_outerConnectionsLock)
            {
                if (_outerConnections.Count > 0)
                {
                    var closedConnection = new InactiveInternalConnection(
                        _internalConnection.DataFile.Freeze(), SimulatorConnectionState.Closed);

                    while (_outerConnections.Count > 0)
                    {
                        var outer = _outerConnections.First();

                        try { outer.SetClosedInternalConnection(closedConnection); }
                        catch
                        {
                            // Swallow exceptions, not that there should ever be any.
                        }

                        _outerConnections.Remove(outer);
                    }
                }
            }

            // Set the internal connection to null and retain the old value to dispose
            Interlocked.Exchange(ref _internalConnection, null)?.Dispose();

            // Allow a new connection to be established.
            _waitHandles.CreateConnectionSignal.Set();

            // Unblock pending attempts
            _allowConnectSignal.Set();
        }
        finally
        {
            _destroyConnectionSemaphore.Release();
        }
    }

    private void ProcessRequestQueue()
    {
        _requestQueueProcessingSemaphore.Wait();

        try
        {
            var uncompletedRequests = new List<AsyncConnectionRequest>(_requestQueue.Count);

            while (_requestQueue.TryDequeue(out var request))
            {
                if (!TryCompleteRequest(request))
                {
                    // Unable to complete request, so return it for further processing
                    uncompletedRequests.Add(request);
                }

                uncompletedRequests.Add(request);
            }

            for (int i = 0; i < uncompletedRequests.Count; i++)
            {
                _requestQueue.Enqueue(uncompletedRequests[i]);
            }
        }
        finally
        {
            _requestQueueProcessingSemaphore.Release();
        }
    }

    private void SetConnection(OpenInternalConnection connection)
    {
        Debug.Assert(_internalConnection == null, "Internal connection is not null");

        _internalConnection = connection;
        _canAddOuterConnections = true;

        _waitHandles.ConnectionAvailableSignal.Set();

        ProcessRequestQueue();
    }

    private void SetConnectionException(SimulatorConnectionException exception)
    {
        // Pause new connections while exception is propagated to current set of connection waiters
        _allowConnectSignal.Reset();

        _connectionException = exception;

        _waitHandles.ConnectionExceptionSignal.Set();

        ProcessRequestQueue();

        // Wait for pending connections to drop to zero
        var spinner = new SpinWait();

        while (_pendingConnectionCount > 0)
        {
            spinner.SpinOnce();
        }

        // Everybody is out, stop signaling an exception
        _waitHandles.ConnectionExceptionSignal.Reset();
        _connectionException = null;

        // Allow another connection attempt
        _waitHandles.CreateConnectionSignal.Set();

        // Resume new connection requests
        _allowConnectSignal.Set();
    }

    private bool TryCompleteRequest(AsyncConnectionRequest request)
    {
        if (request.Completion.Task.IsCompleted)
        {
            Interlocked.Decrement(ref _pendingConnectionCount);

            return true;
        }

        if (request.Timeout > TimeSpan.Zero && request.IsTimedOut())
        {
            // Throw and then immediately capture to preserve the stack trace for the awaiter
            try { throw new TimeoutException("The timeout period elapsed before a connection could be established."); }
            catch (TimeoutException timeoutEx)
            {
                Interlocked.Decrement(ref _pendingConnectionCount);
                request.Completion.TrySetException(timeoutEx);
            }

            return true;
        }

        try
        {
            var canCreateConnection = request.Timeout > TimeSpan.Zero || request.Timeout == Timeout.InfiniteTimeSpan;

            if (TryGetConnection(TimeSpan.Zero, canCreateConnection, out var internalConnection))
            {
                if (!AttachOuterConnection(internalConnection, request.OuterConnection))
                {
                    return false;
                }

                Interlocked.Decrement(ref _pendingConnectionCount);
                request.Completion.TrySetResult();

                return true;
            }
            else
            {
                if (request.Timeout == TimeSpan.Zero)
                {
                    var connEx = GetConnectionException("Failed to connect to the simulator because the simulator is not available.");

                    Interlocked.Decrement(ref _pendingConnectionCount);
                    request.Completion.TrySetException(connEx);

                    return true;
                }
            }

            // Request cannot be completed under current conditions
            return false;
        }
        catch (Exception ex)
        {
            var connEx = GetConnectionException("Failed to connect to the simulator. See inner exception for details.", ex);

            Interlocked.Decrement(ref _pendingConnectionCount);
            request.Completion.TrySetException(connEx);

            return true;
        }
    }

    private void OnDataReadyCallbackInvoked(DataReadyCallback dataReadyCallback, bool timedOut)
    {
        if (!timedOut)
        {
            // Stop the callback, because from this point we'll have either a connection or an exception so there will be nothing to do
            dataReadyCallback.Dispose();

            CreateConnection();

            return;
        }

        // Callback timed out, so process the request queue or shut down if there are no more pending requests
        if (_pendingConnectionCount == 0)
        {
            // Prevent new connections and double-check there's nothing pending before we shut down
            _allowConnectSignal.Reset();

            try
            {
                if (_pendingConnectionCount == 0)
                {
                    dataReadyCallback.Dispose();

                    // Allow a new connection attempt
                    _waitHandles.CreateConnectionSignal.Set();

                    return;
                }
            }
            finally
            {
                _allowConnectSignal.Set();
            }
        }

        // We still have pending connections so process the queue
        ProcessRequestQueue();
    }

    private static SimulatorConnectionException GetConnectionException(string message, Exception? innerException = null)
    {
        try
        {
            if (innerException is null)
            {
                throw new SimulatorConnectionException(message);
            }

            throw new SimulatorConnectionException(message, innerException);
        }
        catch (SimulatorConnectionException connEx)
        {
            return connEx;
        }
    }
}
