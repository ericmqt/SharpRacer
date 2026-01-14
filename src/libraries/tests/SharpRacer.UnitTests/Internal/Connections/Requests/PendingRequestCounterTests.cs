namespace SharpRacer.Internal.Connections.Requests;
public class PendingRequestCounterTests
{
    [Fact]
    public void Ctor_Test()
    {
        var counter = new PendingRequestCounter();

        Assert.False(counter.HasPendingRequests());
        Assert.False(counter.HasExecutingSynchronousRequests());
        Assert.False(counter.HasQueuedAsyncRequests());
    }

    [Fact]
    public void HasPendingRequests_Test()
    {
        var counter = new PendingRequestCounter();

        Assert.False(counter.HasPendingRequests());

        counter.IncrementExecutingSynchronousRequestCount();
        Assert.True(counter.HasPendingRequests());
        Assert.True(counter.HasExecutingSynchronousRequests());
        Assert.False(counter.HasQueuedAsyncRequests());

        counter.IncrementQueuedAsyncRequestCount();
        Assert.True(counter.HasPendingRequests());
        Assert.True(counter.HasExecutingSynchronousRequests());
        Assert.True(counter.HasQueuedAsyncRequests());

        counter.DecrementExecutingSynchronousRequestCount();
        Assert.True(counter.HasPendingRequests());
        Assert.False(counter.HasExecutingSynchronousRequests());
        Assert.True(counter.HasQueuedAsyncRequests());

        counter.DecrementQueuedAsyncRequestCount();
        Assert.False(counter.HasPendingRequests());
        Assert.False(counter.HasExecutingSynchronousRequests());
        Assert.False(counter.HasQueuedAsyncRequests());
    }

    [Fact]
    public void HasExecutingSynchronousRequests_Test()
    {
        var counter = new PendingRequestCounter();

        Assert.False(counter.HasPendingRequests());
        Assert.False(counter.HasExecutingSynchronousRequests());
        Assert.False(counter.HasQueuedAsyncRequests());

        counter.IncrementExecutingSynchronousRequestCount();
        Assert.True(counter.HasPendingRequests());
        Assert.True(counter.HasExecutingSynchronousRequests());
        Assert.False(counter.HasQueuedAsyncRequests());

        counter.DecrementExecutingSynchronousRequestCount();
        Assert.False(counter.HasPendingRequests());
        Assert.False(counter.HasExecutingSynchronousRequests());
        Assert.False(counter.HasQueuedAsyncRequests());
    }

    [Fact]
    public void HasQueuedAsyncRequests_Test()
    {
        var counter = new PendingRequestCounter();

        Assert.False(counter.HasPendingRequests());
        Assert.False(counter.HasExecutingSynchronousRequests());
        Assert.False(counter.HasQueuedAsyncRequests());

        counter.IncrementQueuedAsyncRequestCount();
        Assert.True(counter.HasPendingRequests());
        Assert.False(counter.HasExecutingSynchronousRequests());
        Assert.True(counter.HasQueuedAsyncRequests());

        counter.DecrementQueuedAsyncRequestCount();
        Assert.False(counter.HasPendingRequests());
        Assert.False(counter.HasExecutingSynchronousRequests());
        Assert.False(counter.HasQueuedAsyncRequests());
    }
}
