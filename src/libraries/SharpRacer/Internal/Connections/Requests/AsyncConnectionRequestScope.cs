namespace SharpRacer.Internal.Connections.Requests;
internal sealed class AsyncConnectionRequestScope : IDisposable
{
    private readonly IPendingRequestCounter _requestCounter;
    private bool _isDisposed;

    internal AsyncConnectionRequestScope(IPendingRequestCounter requestCounter)
    {
        _requestCounter = requestCounter;

        _requestCounter.IncrementQueuedAsyncRequestCount();
    }

    public void Dispose()
    {
        if (!_isDisposed)
        {
            _requestCounter.DecrementQueuedAsyncRequestCount();

            _isDisposed = true;
        }
    }
}
