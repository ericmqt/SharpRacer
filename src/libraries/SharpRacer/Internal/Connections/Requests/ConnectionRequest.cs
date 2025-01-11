namespace SharpRacer.Internal.Connections.Requests;
internal readonly ref struct ConnectionRequest
{
    private readonly IConnectionRequestScopeFactory _executionScopeFactory;
    private readonly TimeProvider _timeProvider;

    public ConnectionRequest(
        IOuterConnection outerConnection,
        TimeSpan timeout,
        IConnectionRequestScopeFactory executionScopeFactory,
        TimeProvider timeProvider)
    {
        OuterConnection = outerConnection;
        Timeout = timeout;
        _executionScopeFactory = executionScopeFactory;
        _timeProvider = timeProvider;
    }

    public readonly IOuterConnection OuterConnection { get; }
    public readonly TimeSpan Timeout { get; }

    public readonly State CreateExecutionState() => new(Timeout, _timeProvider);

    public readonly Scope EnterExecutionScope() => _executionScopeFactory.CreateScope();

    public readonly void Execute(IConnectionProvider connectionProvider)
    {
        // Create our state object which helps keep track of timeout remaining, etc.
        var state = CreateExecutionState();

        do
        {
            // Guards against entering TryGetConnection when we need to briefly block calls to that method in certain cases, also
            // increases the pending connection count for the duration of execution.
            using (var scope = EnterExecutionScope())
            {
                if (!connectionProvider.TryGetConnection(state.TimeoutRemaining, state.CanBeginConnectionAttempt, out var innerConnection))
                {
                    // Timeout elapsed
                    break;
                }

                if (innerConnection.Attach(OuterConnection))
                {
                    // Outer connection attached, operation complete.
                    return;
                }

                // Received inner connection while Attach returned false, indicating that it was about to close, so update state and
                // try again as long as there is enough time remaining.
            }

            // Inner connection began closing after being returned from TryGetConnection, so update the request state and try again if
            // there is time remaining.
            state.Update();
        }
        while (!state.IsTimedOut);

        // Timeout period elapsed without a connection being established or a connection exception being thrown
        throw RequestExceptions.RequestTimeout();
    }

    internal ref struct Scope
    {
        private readonly IPendingRequestCounter _requestCounter;
        private bool _isDisposed;

        public Scope(IPendingRequestCounter requestCounter)
        {
            _requestCounter = requestCounter;

            _requestCounter.IncrementExecutingSynchronousRequestCount();
        }

        public void Dispose()
        {
            if (!_isDisposed)
            {
                _requestCounter.DecrementExecutingSynchronousRequestCount();

                _isDisposed = true;
            }
        }
    }

    internal ref struct State
    {
        private readonly long _requestStart;
        private readonly TimeProvider _timeProvider;
        private TimeSpan _timeoutRemaining;

        public State(TimeSpan timeout, TimeProvider timeProvider)
        {
            Timeout = timeout;
            _timeProvider = timeProvider;

            IsTimeoutInfinite = timeout == System.Threading.Timeout.InfiniteTimeSpan;

            _timeoutRemaining = IsTimeoutInfinite ? System.Threading.Timeout.InfiniteTimeSpan : timeout;

            _requestStart = _timeProvider.GetTimestamp();
        }

        public readonly bool CanBeginConnectionAttempt
            => IsTimeoutInfinite || TimeoutRemaining > TimeSpan.Zero;

        public readonly bool IsTimedOut
            => !IsTimeoutInfinite && TimeoutRemaining < TimeSpan.Zero;

        public readonly bool IsTimeoutInfinite { get; }
        public readonly TimeSpan Timeout { get; }
        public readonly TimeSpan TimeoutRemaining => _timeoutRemaining;

        public void Update()
        {
            if (!IsTimeoutInfinite)
            {
                _timeoutRemaining = Timeout - _timeProvider.GetElapsedTime(_requestStart);
            }
        }
    }
}
