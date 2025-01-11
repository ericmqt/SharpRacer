namespace SharpRacer.Internal.Connections.Requests;
internal class ConnectionRequestManager : IConnectionRequestManager
{
    private readonly IAsyncConnectionRequestQueue _asyncRequestQueue;
    private readonly IPendingRequestCounter _requestCounter;
    private readonly IConnectionRequestFactory _requestFactory;
    private readonly IConnectionRequestSignals _requestSignals;

    public ConnectionRequestManager()
        : this(TimeProvider.System)
    {

    }

    public ConnectionRequestManager(TimeProvider timeProvider)
    {
        _asyncRequestQueue = new AsyncConnectionRequestQueue();
        _requestCounter = new PendingRequestCounter();
        _requestSignals = new ConnectionRequestSignals();
        _requestFactory = new ConnectionRequestFactory(_requestCounter, _requestSignals, timeProvider);
    }

    public ConnectionRequestManager(
        IConnectionRequestFactory? requestFactory = null,
        IAsyncConnectionRequestQueue? asyncRequestQueue = null,
        IConnectionRequestSignals? requestSignals = null,
        IPendingRequestCounter? requestCounter = null,
        TimeProvider? timeProvider = null)
    {
        _asyncRequestQueue = asyncRequestQueue ?? new AsyncConnectionRequestQueue();
        _requestSignals = requestSignals ?? new ConnectionRequestSignals();
        _requestCounter = requestCounter ?? new PendingRequestCounter();

        _requestFactory = requestFactory ??
            new ConnectionRequestFactory(_requestCounter, _requestSignals, timeProvider ?? TimeProvider.System);
    }

    public IAsyncConnectionRequest CreateAsyncRequest(IOuterConnection outerConnection, CancellationToken cancellationToken = default)
    {
        return _requestFactory.CreateAsyncRequest(outerConnection, cancellationToken);
    }

    public IAsyncConnectionRequest CreateAsyncRequest(
        IOuterConnection outerConnection,
        TimeSpan timeout,
        CancellationToken cancellationToken = default)
    {
        return _requestFactory.CreateAsyncRequest(outerConnection, timeout, cancellationToken);
    }

    public ConnectionRequest CreateRequest(IOuterConnection outerConnection)
    {
        return _requestFactory.CreateRequest(outerConnection);
    }

    public ConnectionRequest CreateRequest(IOuterConnection outerConnection, TimeSpan timeout)
    {
        return _requestFactory.CreateRequest(outerConnection, timeout);
    }

    public BlockConnectionRequestsScope CreateRequestBlockingScope()
    {
        return new BlockConnectionRequestsScope(_requestSignals);
    }

    public bool HasPendingRequests()
    {
        return _requestCounter.HasPendingRequests();
    }

    public void ProcessAsyncRequestQueue(IConnectionProvider connectionProvider, bool force = false)
    {
        _asyncRequestQueue.ProcessQueue(connectionProvider, force);
    }

    public void QueueAsyncRequest(IAsyncConnectionRequest request)
    {
        _asyncRequestQueue.Enqueue(request);
    }
}
