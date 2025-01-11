namespace SharpRacer.Internal.Connections.Requests;
internal sealed class PendingRequestCounter : IPendingRequestCounter
{
    private int _pendingConnectionCount;
    private int _queuedAsyncRequestCount;
    private int _executingSynchronousRequestCount;

    public PendingRequestCounter()
    {

    }

    public void DecrementExecutingSynchronousRequestCount()
    {
        Interlocked.Decrement(ref _pendingConnectionCount);
        Interlocked.Decrement(ref _executingSynchronousRequestCount);
    }

    public void DecrementQueuedAsyncRequestCount()
    {
        Interlocked.Decrement(ref _pendingConnectionCount);
        Interlocked.Decrement(ref _queuedAsyncRequestCount);
    }

    public void IncrementExecutingSynchronousRequestCount()
    {
        Interlocked.Increment(ref _pendingConnectionCount);
        Interlocked.Increment(ref _executingSynchronousRequestCount);
    }

    public void IncrementQueuedAsyncRequestCount()
    {
        Interlocked.Increment(ref _pendingConnectionCount);
        Interlocked.Increment(ref _queuedAsyncRequestCount);
    }

    public bool HasPendingRequests()
    {
        return Interlocked.CompareExchange(ref _pendingConnectionCount, 0, 0) != 0;
    }

    public bool HasExecutingSynchronousRequests()
    {
        return Interlocked.CompareExchange(ref _executingSynchronousRequestCount, 0, 0) != 0;
    }

    public bool HasQueuedAsyncRequests()
    {
        return Interlocked.CompareExchange(ref _queuedAsyncRequestCount, 0, 0) != 0;
    }
}
