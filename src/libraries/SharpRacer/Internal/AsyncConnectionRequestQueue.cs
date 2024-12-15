using System.Collections.Concurrent;

namespace SharpRacer.Internal;
internal sealed class AsyncConnectionRequestQueue
{
    private readonly IAsyncConnectionRequestCompletionSource _completionSource;
    private readonly SemaphoreSlim _processQueueSemaphore;
    private readonly ConcurrentQueue<AsyncConnectionRequest> _queue;

    public AsyncConnectionRequestQueue(IAsyncConnectionRequestCompletionSource requestCompletionSource)
    {
        _completionSource = requestCompletionSource ?? throw new ArgumentNullException(nameof(requestCompletionSource));

        _queue = new ConcurrentQueue<AsyncConnectionRequest>();

        // Note: SemaphoreSlim has no thread affinity, so ProcessQueue can be called from any thread without causing issues
        _processQueueSemaphore = new SemaphoreSlim(1, 1);
    }

    public void Enqueue(AsyncConnectionRequest request)
    {
        _queue.Enqueue(request);
    }

    /// <summary>
    /// Examines each pending request and attempts to complete it. If the request was unable to be completed, it is returned to the queue.
    /// </summary>
    /// <param name="force">
    /// When <see langword="true"/>, this method will block while another call to <see cref="ProcessQueue(bool)"/> is in progress before
    /// executing, ensuring every request is processed at least once. Otherwise, the method returns immediately if the queue is already
    /// being processed.
    /// </param>
    public void ProcessQueue(bool force = false)
    {
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
                return;
            }
        }

        try
        {
            var uncompletedRequests = new List<AsyncConnectionRequest>(_queue.Count);

            while (_queue.TryDequeue(out var request))
            {
                if (!_completionSource.TryCompleteRequest(request))
                {
                    // Unable to complete request, so return it for further processing
                    uncompletedRequests.Add(request);
                }
            }

            for (int i = 0; i < uncompletedRequests.Count; i++)
            {
                _queue.Enqueue(uncompletedRequests[i]);
            }
        }
        finally
        {
            // If we entered the try block, we are guaranteed to have entered the semaphore so it must be released before exiting.
            _processQueueSemaphore.Release();
        }
    }
}
