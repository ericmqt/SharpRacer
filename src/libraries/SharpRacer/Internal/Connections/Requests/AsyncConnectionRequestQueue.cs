using System.Collections.Concurrent;

namespace SharpRacer.Internal.Connections.Requests;
internal class AsyncConnectionRequestQueue : IAsyncConnectionRequestQueue
{
    private readonly SemaphoreSlim _processQueueSemaphore;
    private readonly ConcurrentQueue<IAsyncConnectionRequest> _queue;

    public AsyncConnectionRequestQueue()
    {
        _queue = new ConcurrentQueue<IAsyncConnectionRequest>();

        // Note: SemaphoreSlim has no thread affinity, so ProcessQueue can be called from any thread without causing issues
        _processQueueSemaphore = new SemaphoreSlim(1, 1);
    }

    public void Enqueue(IAsyncConnectionRequest request)
    {
        _queue.Enqueue(request);
    }

    public bool ProcessQueue(IConnectionProvider connectionProvider, bool force = false)
    {
        return ProcessQueue(connectionProvider, force, out _);
    }

    public bool ProcessQueue(IConnectionProvider connectionProvider, bool force, out bool queueEmptied)
    {
        queueEmptied = false;

        if (force)
        {
            // Block until we are able to enter the semaphore
            _processQueueSemaphore.Wait();
        }
        else
        {
            // Attempt to enter the semaphore without waiting
            if (!_processQueueSemaphore.Wait(0))
            {
                // Failed to enter the semaphore, so another call to this method is already in progress.
                return false;
            }
        }

        try
        {
            var uncompletedRequests = new List<IAsyncConnectionRequest>(_queue.Count);

            while (_queue.TryDequeue(out var request))
            {
                if (!request.TryComplete(connectionProvider))
                {
                    // Unable to complete request, so return it for further processing
                    uncompletedRequests.Add(request);
                }
            }

            for (int i = 0; i < uncompletedRequests.Count; i++)
            {
                _queue.Enqueue(uncompletedRequests[i]);
            }

            queueEmptied = _queue.IsEmpty;

            return true;
        }
        finally
        {
            // If we entered the try block, we are guaranteed to have entered the semaphore so it must be released before exiting.
            _processQueueSemaphore.Release();
        }
    }
}
