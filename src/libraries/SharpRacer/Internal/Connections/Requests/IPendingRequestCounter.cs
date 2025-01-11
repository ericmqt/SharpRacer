namespace SharpRacer.Internal.Connections.Requests;
internal interface IPendingRequestCounter
{
    void DecrementExecutingSynchronousRequestCount();
    void DecrementQueuedAsyncRequestCount();
    void IncrementExecutingSynchronousRequestCount();
    void IncrementQueuedAsyncRequestCount();
    bool HasPendingRequests();
    bool HasExecutingSynchronousRequests();
    bool HasQueuedAsyncRequests();
}
