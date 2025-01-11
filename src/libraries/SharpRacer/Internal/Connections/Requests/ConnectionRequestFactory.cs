namespace SharpRacer.Internal.Connections.Requests;
internal sealed class ConnectionRequestFactory : IConnectionRequestFactory, IConnectionRequestScopeFactory
{
    private readonly IPendingRequestCounter _requestCounter;
    private readonly IConnectionRequestSignals _requestSignals;
    private readonly TimeProvider _timeProvider;

    public ConnectionRequestFactory(
        IPendingRequestCounter requestCounter,
        IConnectionRequestSignals requestSignals,
        TimeProvider timeProvider)
    {
        _requestCounter = requestCounter;
        _requestSignals = requestSignals;
        _timeProvider = timeProvider;
    }

    public IAsyncConnectionRequest CreateAsyncRequest(
        IOuterConnection outerConnection,
        CancellationToken cancellationToken = default)
    {
        return CreateAsyncRequest(outerConnection, Timeout.InfiniteTimeSpan, cancellationToken);
    }

    public IAsyncConnectionRequest CreateAsyncRequest(
        IOuterConnection outerConnection,
        TimeSpan timeout,
        CancellationToken cancellationToken = default)
    {
        RequestExceptions.ThrowIfTimeoutArgumentIsNegative(timeout);

        var requestScope = CreateAsyncScope();

        try
        {
            return new AsyncConnectionRequest(outerConnection, timeout, requestScope, _timeProvider, cancellationToken);
        }
        catch (Exception)
        {
            requestScope.Dispose();

            throw;
        }
    }

    public AsyncConnectionRequestScope CreateAsyncScope()
    {
        _requestSignals.WaitForAsyncRequestCreation();

        return new AsyncConnectionRequestScope(_requestCounter);
    }

    public ConnectionRequest CreateRequest(IOuterConnection outerConnection)
    {
        return CreateRequest(outerConnection, Timeout.InfiniteTimeSpan);
    }

    public ConnectionRequest CreateRequest(IOuterConnection outerConnection, TimeSpan timeout)
    {
        RequestExceptions.ThrowIfTimeoutArgumentIsNegative(timeout);

        return new ConnectionRequest(outerConnection, timeout, this, _timeProvider);
    }

    public ConnectionRequest.Scope CreateScope()
    {
        _requestSignals.WaitForRequestExecution();

        return new ConnectionRequest.Scope(_requestCounter);
    }
}
