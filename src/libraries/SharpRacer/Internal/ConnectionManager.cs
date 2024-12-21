using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Versioning;
using SharpRacer.IO;

namespace SharpRacer.Internal;

[SupportedOSPlatform("windows5.1.2600")]
internal sealed partial class ConnectionManager : IConnectionPool, IAsyncConnectionRequestCompletionSource
{
    // Outer connection tracking
    private bool _canAddOuterConnections;
    private readonly List<ISimulatorOuterConnection> _outerConnections;
    private readonly object _outerConnectionsLock = new object();

    // Synchronization primitives
    private readonly ManualResetEventSlim _allowConnectSignal;
    private readonly SemaphoreSlim _destroyConnectionSemaphore;
    private readonly ConnectionWaitHandles _waitHandles;

    private SimulatorConnectionException? _connectionException;
    private MemoryMappedDataFile? _dataFile;
    private DataReadyCallback? _dataReadyCallback;
    private IOpenInnerConnection? _innerConnection;
    private int _nextConnectionId;
    private int _pendingConnectionCount;
    private readonly AsyncConnectionRequestQueue _requestQueue;

    private ConnectionManager()
    {
        _requestQueue = new AsyncConnectionRequestQueue(this);
        _outerConnections = [];

        _allowConnectSignal = new ManualResetEventSlim(initialState: true);
        _destroyConnectionSemaphore = new SemaphoreSlim(1, 1);
        _waitHandles = new ConnectionWaitHandles();
    }

    internal static IConnectionPool Default { get; } = new ConnectionManager();

    public void Connect(ISimulatorOuterConnection outerConnection)
    {
        Connect(outerConnection, Timeout.InfiniteTimeSpan);
    }

    public void Connect(ISimulatorOuterConnection outerConnection, TimeSpan timeout)
    {
        ArgumentNullException.ThrowIfNull(outerConnection);

        var isTimeoutInfinite = timeout == Timeout.InfiniteTimeSpan;
        var throwTimeoutException = timeout > TimeSpan.Zero;
        var connectStart = TimeProvider.System.GetTimestamp();

        // Loop until a connection is successful or timeout expires. The do/while exists to handle the case where an inner connection
        // object was received, but AttachOuterConnection returned false due to the inner connection starting to close after acquisition.
        do
        {
            _allowConnectSignal.Wait();

            Interlocked.Increment(ref _pendingConnectionCount);

            try
            {
                if (!TryGetConnection(timeout, timeout > TimeSpan.Zero, out var innerConnection))
                {
                    // Timeout elapsed
                    break;
                }

                if (AttachOuterConnection(innerConnection, outerConnection))
                {
                    // Outer connection attached, operation complete.
                    return;
                }

                // Inner connection began closing after being returned from TryGetConnection, so update the timeout period and try again
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

    public Task ConnectAsync(ISimulatorOuterConnection outerConnection, CancellationToken cancellationToken = default)
    {
        return ConnectAsync(outerConnection, Timeout.InfiniteTimeSpan, cancellationToken);
    }

    public async Task ConnectAsync(ISimulatorOuterConnection outerConnection, TimeSpan timeout, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(outerConnection);

        var completion = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);

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

                // Process the queue on the thread pool so we don't block and can proceed to awaiting the task
                ThreadPool.QueueUserWorkItem<object?>(_ => _requestQueue.ProcessQueue(force: false), null, false);
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

    public void ReleaseOuterConnection(ISimulatorOuterConnection outerConnection)
    {
        var destroyInnerConnection = false;

        lock (_outerConnectionsLock)
        {
            // Stop tracking the outer connection and destroy the inner connection if no outer connections remain

            // If the outer connection is not part of the current set, the inner connection would already have been disposed, so don't
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
    /// Allows <see cref="IOpenInnerConnection"/> to tell the pool it has disconnected and that the pool should clean up associated
    /// outer connections before resetting the pool connection state.
    /// </summary>
    /// <param name="innerConnection"></param>
    public void Return(IOpenInnerConnection innerConnection)
    {
        // If the connection isn't the current one, just clean it up and move on. I have no idea how this could occur, but better to be
        // safe than wreck the current connection for no reason.
        if (innerConnection != _innerConnection)
        {
            innerConnection.Dispose();

            return;
        }

        DestroyConnection();
    }

    private bool TryGetConnection(TimeSpan waitTimeout, bool allowCreateConnection, [NotNullWhen(true)] out IOpenInnerConnection? innerConnection)
    {
        var waitResult = _waitHandles.WaitAny(allowCreateConnection, waitTimeout);

        switch (waitResult)
        {
            case ConnectionWaitHandles.ConnectionAvailableWaitIndex:
                Debug.Assert(_innerConnection != null, "Connected signal received but connection object was null");
                innerConnection = _innerConnection;
                return true;

            case ConnectionWaitHandles.ConnectionExceptionWaitIndex:
                Debug.Assert(_connectionException != null, "Connection exception signal received but exception was null");
                var cachedException = _connectionException;
                throw cachedException;

            // Connection creation signal is an auto-reset event, so only one thread will be allowed through when it is signaled in order
            // to start the connection attempt, then the thread tries again with allowCreateConnection set to false.
            case ConnectionWaitHandles.CreateConnectionWaitIndex:
                // Start a connection attempt, will will run until success, error, or no pending connection requests remain
                BeginConnection();

                return TryGetConnection(waitTimeout, false, out innerConnection);

            case WaitHandle.WaitTimeout:
                innerConnection = null;
                return false;

            default:
                throw new InvalidOperationException($"Unexpected wait result: {waitResult}");
        }
    }

    private bool AttachOuterConnection(IOpenInnerConnection innerConnection, ISimulatorOuterConnection outerConnection)
    {
        lock (_outerConnectionsLock)
        {
            if (!_canAddOuterConnections)
            {
                return false;
            }

            if (!_outerConnections.Contains(outerConnection))
            {
                // Begin tracking the outer connection
                _outerConnections.Add(outerConnection);

                // Set the inner connection and pass a lifetime handle for the memory-mapped data file
                outerConnection.SetOpenInnerConnection(innerConnection, innerConnection.AcquireDataFileLifetimeHandle());
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
            // Open the data file. If one already existed it'll self-dispose eventually once all referencing connections are disposed
            _dataFile = MemoryMappedDataFile.Open();

            var connection = new OpenInnerConnection(
                _dataFile,
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
            if (_innerConnection is null)
            {
                return;
            }

            _canAddOuterConnections = false;

            // Disable the connection available signal, which was presumably set because we had an active inner connection, and put the
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
                    // Create a new inner connection object to pass to our tracked SimulatorConnection instances. We "freeze" the
                    // memory-mapped data file (copy it to memory) and provide this as the new data source, allowing read operations to 
                    // continue to see valid data until they're able to respond to or detect the change in connection state.

                    var closedConnection = new InactiveInnerConnection(
                        _innerConnection.DataFile.Freeze(), SimulatorConnectionState.Closed);

                    while (_outerConnections.Count > 0)
                    {
                        var outer = _outerConnections.First();

                        try { outer.SetClosedInnerConnection(closedConnection); }
                        catch
                        {
                            // Swallow exceptions, not that there should ever be any.
                        }

                        // Stop tracking the outer connection, it is no longer our concern.
                        _outerConnections.Remove(outer);
                    }
                }
            }

            // Set the inner connection to null and retain the old value to dispose
            Interlocked.Exchange(ref _innerConnection, null)?.Dispose();

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

    private void SetConnection(IOpenInnerConnection connection)
    {
        Debug.Assert(_innerConnection == null, "Inner connection is not null");

        // Set inner connection object before signaling the connection available signal.
        _innerConnection = connection;
        _canAddOuterConnections = true;

        // Notify waiting threads that a connection is available
        _waitHandles.ConnectionAvailableSignal.Set();

        // Ensure every pending request gets processed. Otherwise, if the queue was already in the middle of being processed, there is a
        // small chance that some requests would be left uncompleted because the queue is no longer being periodically processed while
        // DataReadyCallback waits for the event to fire.

        _requestQueue.ProcessQueue(force: true);
    }

    private void SetConnectionException(SimulatorConnectionException exception)
    {
        // Pause new connections while exception is propagated to current set of connection waiters
        _allowConnectSignal.Reset();

        // Set the exception object before signaling to ensure waiting threads see a non-null exception object!
        _connectionException = exception;

        // Signal to waiting threads an connection exception is available.
        _waitHandles.ConnectionExceptionSignal.Set();

        // Ensure every pending request gets processed. Otherwise, if the queue was already in the middle of being processed, there is a
        // small chance that some requests would be left uncompleted because the queue is no longer being periodically processed while
        // DataReadyCallback waits for the event to fire.
        _requestQueue.ProcessQueue(force: true);

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
        if (request.Completion.Task.IsCompleted || request.Completion.Task.IsCanceled)
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
            // TryGetConnection is called with zero timeout regardless of the specified timeout period so that it does not block. Since
            // this method is called from the request queue processing loop, blocking here could prevent other requests from being
            // completed on time. If the request can't be completed right now, it will be tried again the next time the request queue gets
            // processed.
            //
            // The request timeout is still significant in determining whether we allow the request to initiate a connection attempt if one
            // is not already in progress. A request with zero timeout would not exist long enough to see the result of a connection
            // attempt it initiates.
            //
            // There is no need to calculate whether the timeout period remaining is greater than zero, because the request was determined
            // to have not timed out earlier in this method.

            var canCreateConnection = request.Timeout > TimeSpan.Zero || request.Timeout == Timeout.InfiniteTimeSpan;

            if (TryGetConnection(TimeSpan.Zero, canCreateConnection, out var innerConnection))
            {
                if (!AttachOuterConnection(innerConnection, request.OuterConnection))
                {
                    return false;
                }

                // We have retreived the inner connection object and attached it to the outer connection. The request is completed,
                // so decrement our request counter, set the result, and get out of here.

                Interlocked.Decrement(ref _pendingConnectionCount);
                request.Completion.TrySetResult();

                return true;
            }
            else
            {
                // If we failed to get a connection when the requested timeout was zero, stash an exception in the resulting task to be
                // thrown when it is awaited.

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
            // An exception was thrown from TryGetConnection indicating some form of failure to connect, i.e. failed to open the data file
            // for some reason. This is wrapped in a SimulatorConnectionException and stashed in the task object to be thrown when it gets
            // awaited.
            //
            // The original exception could be thrown as-is but wrapping it in a SimulatorConnectionException means callers only need to
            // catch a single exception type to catch any exceptions that could arise from a failed connection attempt.

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
                    // Stop the callback because there are no more pending connection requests
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
        _requestQueue.ProcessQueue(force: false);
    }

    bool IAsyncConnectionRequestCompletionSource.TryCompleteRequest(AsyncConnectionRequest request) => TryCompleteRequest(request);

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
