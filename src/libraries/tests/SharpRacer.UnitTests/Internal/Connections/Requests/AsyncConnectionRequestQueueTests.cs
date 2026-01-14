using Moq;

namespace SharpRacer.Internal.Connections.Requests;
public class AsyncConnectionRequestQueueTests
{
    [Fact]
    public void ProcessQueue_Test()
    {
        var requestMock = new Mock<IAsyncConnectionRequest>(MockBehavior.Strict);
        var connectionProviderMock = new Mock<IConnectionProvider>(MockBehavior.Strict);

        requestMock.Setup(x => x.TryComplete(connectionProviderMock.Object)).Returns(true);

        var queue = new AsyncConnectionRequestQueue();
        queue.Enqueue(requestMock.Object);

        Assert.True(queue.ProcessQueue(connectionProviderMock.Object, force: false));

        // Process again to ensure TryComplete only gets called once, indicating the request is no longer in the queue
        Assert.True(queue.ProcessQueue(connectionProviderMock.Object, force: false));

        requestMock.Verify(x => x.TryComplete(connectionProviderMock.Object), Times.Once);
    }

    [Fact]
    public void ProcessQueue_EmptiedQueueOutParameterTest()
    {
        var completeRequest = false;

        var requestMock = new Mock<IAsyncConnectionRequest>(MockBehavior.Strict);
        var connectionProviderMock = new Mock<IConnectionProvider>(MockBehavior.Strict);

        requestMock.Setup(x => x.TryComplete(connectionProviderMock.Object)).Returns(() => completeRequest);

        var queue = new AsyncConnectionRequestQueue();
        queue.Enqueue(requestMock.Object);

        queue.ProcessQueue(connectionProviderMock.Object, false, out var queueEmptied);
        Assert.False(queueEmptied);

        // Allow request to complete and check queue was emptied
        completeRequest = true;

        queue.ProcessQueue(connectionProviderMock.Object, false, out queueEmptied);

        Assert.True(queueEmptied);
    }

    [Fact]
    public void ProcessQueue_ForceTest()
    {
        var requestMock = new Mock<IAsyncConnectionRequest>(MockBehavior.Strict);
        var connectionProviderMock = new Mock<IConnectionProvider>(MockBehavior.Strict);

        // Signal to indicate ProcessQueue is running
        var isThreadProcessingQueue = new ManualResetEventSlim(initialState: false);

        // Block thread's ProcessQueue indefinitely so we can try ProcessQueue on this thread
        var requestCompleteBlocker = new ManualResetEventSlim(initialState: false);

        // Signal to notify when threadpool ProcessQueue has finished and we can assert the result
        var forceProcessQueueCompletedSignal = new ManualResetEventSlim(initialState: false);

        requestMock.Setup(x => x.TryComplete(connectionProviderMock.Object))
            .Callback(() =>
            {
                isThreadProcessingQueue.Set();
                requestCompleteBlocker.Wait();
            })
            .Returns(false); // force request to be re-queued

        var queue = new AsyncConnectionRequestQueue();
        queue.Enqueue(requestMock.Object);

        var processingThreadAction = new Action(() => queue.ProcessQueue(connectionProviderMock.Object, force: false));

        var processingThread = new Thread(processingThreadAction.Invoke) { IsBackground = true };
        processingThread.Start();

        Assert.True(isThreadProcessingQueue.Wait(TimeSpan.FromSeconds(5)));

        bool forceProcessQueueResult = false;

        // Invoke on thread pool so we can unblock the thread currently processing the queue
        ThreadPool.QueueUserWorkItem(_ =>
        {
            forceProcessQueueResult = queue.ProcessQueue(connectionProviderMock.Object, force: true);
            forceProcessQueueCompletedSignal.Set();
        }, null);

        requestCompleteBlocker.Set();
        forceProcessQueueCompletedSignal.Wait(TimeSpan.FromSeconds(5));

        Assert.True(forceProcessQueueResult);
    }

    [Fact]
    public void ProcessQueue_ReturnsFalseWhenRunningOnAnotherThreadTest()
    {
        var requestMock = new Mock<IAsyncConnectionRequest>(MockBehavior.Strict);
        var connectionProviderMock = new Mock<IConnectionProvider>(MockBehavior.Strict);

        // Signal to indicate ProcessQueue is running
        var isThreadProcessingQueue = new ManualResetEventSlim(initialState: false);

        // Block thread's ProcessQueue indefinitely so we can try ProcessQueue on this thread
        var requestCompleteBlocker = new ManualResetEventSlim(initialState: false);

        requestMock.Setup(x => x.TryComplete(connectionProviderMock.Object))
            .Callback(() =>
            {
                isThreadProcessingQueue.Set();
                requestCompleteBlocker.Wait();
            })
            .Returns(true);

        var queue = new AsyncConnectionRequestQueue();
        queue.Enqueue(requestMock.Object);

        var processingThreadAction = new Action(() => queue.ProcessQueue(connectionProviderMock.Object, force: false));

        var processingThread = new Thread(processingThreadAction.Invoke) { IsBackground = true };
        processingThread.Start();

        Assert.True(isThreadProcessingQueue.Wait(TimeSpan.FromSeconds(5)));
        Assert.False(queue.ProcessQueue(connectionProviderMock.Object, force: false));

        requestCompleteBlocker.Set();
    }
}
