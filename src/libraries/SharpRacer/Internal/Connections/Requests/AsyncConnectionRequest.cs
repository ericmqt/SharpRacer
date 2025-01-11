namespace SharpRacer.Internal.Connections.Requests;
internal sealed class AsyncConnectionRequest : IAsyncConnectionRequest
{
    private readonly CancellationTokenRegistration _cancellationTokenRegistration;
    private readonly long _requestCreationTimestamp;
    private readonly IDisposable _requestLifetimeScope;
    private readonly TimeProvider _timeProvider;
    private bool _isDisposed;

    public AsyncConnectionRequest(
        IOuterConnection outerConnection,
        TimeSpan timeout,
        IDisposable requestLifetimeScope,
        TimeProvider timeProvider,
        CancellationToken cancellationToken)
    {
        OuterConnection = outerConnection;
        Timeout = timeout;

        _requestLifetimeScope = requestLifetimeScope;
        _timeProvider = timeProvider;

        Completion = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        CanBeginConnectionAttempt = Timeout > TimeSpan.Zero || Timeout == System.Threading.Timeout.InfiniteTimeSpan;

        _cancellationTokenRegistration = cancellationToken.Register(() => Completion.TrySetCanceled());
        _requestCreationTimestamp = _timeProvider.GetTimestamp();
    }

    public bool CanBeginConnectionAttempt { get; }
    public TaskCompletionSource Completion { get; }
    public IOuterConnection OuterConnection { get; }
    public TimeSpan Timeout { get; }

    public void Dispose()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    public bool IsTimedOut()
    {
        // Zero timeout does not count as having timed out because the request will either complete or throw TimeoutException on first
        // call to TryComplete if no other conditions apply.

        if (Timeout == TimeSpan.Zero || Timeout == System.Threading.Timeout.InfiniteTimeSpan)
        {
            return false;
        }

        var timeoutRemaining = Timeout - _timeProvider.GetElapsedTime(_requestCreationTimestamp);

        return timeoutRemaining < TimeSpan.Zero;
    }

    public bool TryComplete(IConnectionProvider connectionProvider)
    {
        if (_isDisposed)
        {
            // We're disposed, so eject the request from the queue
            return true;
        }

        if (Completion.Task.IsCompleted || Completion.Task.IsCanceled)
        {
            _requestLifetimeScope.Dispose();
            return true;
        }

        if (IsTimedOut())
        {
            // Throw and then immediately capture to preserve the stack trace for the awaiter
            try { throw new TimeoutException("The timeout period elapsed before a connection could be established."); }
            catch (TimeoutException timeoutEx)
            {
                Completion.TrySetException(timeoutEx);
            }

            _requestLifetimeScope.Dispose();
            return true;
        }

        try
        {
            if (connectionProvider.TryGetConnection(this, out var innerConnection))
            {
                if (!innerConnection.Attach(OuterConnection))
                {
                    return false;
                }

                // We have retreived the inner connection object and attached it to the outer connection. Success!
                Completion.TrySetResult();

                _requestLifetimeScope.Dispose();
                return true;
            }

            // If we failed to get a connection when the requested timeout was zero, stash an exception in the resulting task to be
            // thrown when it is awaited.
            if (Timeout == TimeSpan.Zero)
            {
                Completion.TrySetException(ConnectionExceptionHelper.FailedToConnect_SimulatorNotAvailable());

                _requestLifetimeScope.Dispose();
                return true;
            }

            // Request cannot be completed, allow it to be queued again for another attempt later
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

            Completion.TrySetException(ConnectionExceptionHelper.FailedToConnect(ex));

            _requestLifetimeScope.Dispose();
            return true;
        }
    }

    private void Dispose(bool disposing)
    {
        if (!_isDisposed)
        {
            if (disposing)
            {
                if (!Completion.Task.IsCompleted)
                {
                    try
                    {
                        throw new ObjectDisposedException(
                            typeof(AsyncConnectionRequest).FullName, "Request was disposed before it could be completed.");
                    }
                    catch (ObjectDisposedException disposedEx)
                    {
                        Completion.TrySetException(disposedEx);
                    }
                }

                _requestLifetimeScope.Dispose();
                _cancellationTokenRegistration.Dispose();
            }

            _isDisposed = true;
        }
    }
}
