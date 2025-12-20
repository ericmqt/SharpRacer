namespace SharpRacer.Internal.Connections.Requests;

internal interface IConnectionRequestManager
{
    ConnectionRequest CreateRequest(IOuterConnection outerConnection);
    ConnectionRequest CreateRequest(IOuterConnection outerConnection, TimeSpan timeout);

    IAsyncConnectionRequest CreateAsyncRequest(
        IOuterConnection outerConnection,
        CancellationToken cancellationToken = default);

    IAsyncConnectionRequest CreateAsyncRequest(
        IOuterConnection outerConnection,
        TimeSpan timeout,
        CancellationToken cancellationToken = default);

    BlockConnectionRequestsScope CreateRequestBlockingScope();
    bool HasPendingRequests();
    void ProcessAsyncRequestQueue(IConnectionProvider connectionProvider, bool force = false);
    void ProcessAsyncRequestQueueOnThreadPool(IConnectionProvider connectionProvider, bool force = false);
    void QueueAsyncRequest(IAsyncConnectionRequest request);
}
