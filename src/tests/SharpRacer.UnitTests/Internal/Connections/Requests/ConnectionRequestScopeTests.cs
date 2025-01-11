using Moq;

namespace SharpRacer.Internal.Connections.Requests;
public class ConnectionRequestScopeTests
{
    [Fact]
    public void Dispose_InvokeCallbackTest()
    {
        int requestCount = 0;

        var requestCounterMock = new Mock<IPendingRequestCounter>(MockBehavior.Strict);

        requestCounterMock.Setup(x => x.DecrementExecutingSynchronousRequestCount())
            .Callback(() => requestCount--);

        requestCounterMock.Setup(x => x.IncrementExecutingSynchronousRequestCount())
            .Callback(() => requestCount++);

        var scope = new ConnectionRequest.Scope(requestCounterMock.Object);
        Assert.Equal(1, requestCount);

        scope.Dispose();
        Assert.Equal(0, requestCount);

        requestCounterMock.Verify(x => x.DecrementExecutingSynchronousRequestCount(), Times.Once);
        requestCounterMock.Verify(x => x.IncrementExecutingSynchronousRequestCount(), Times.Once);
    }

    [Fact]
    public void Dispose_MultipleDisposalsInvokeCallbackOnlyOnceTest()
    {
        int requestCount = 0;

        var requestCounterMock = new Mock<IPendingRequestCounter>(MockBehavior.Strict);

        requestCounterMock.Setup(x => x.DecrementExecutingSynchronousRequestCount())
            .Callback(() => requestCount--);

        requestCounterMock.Setup(x => x.IncrementExecutingSynchronousRequestCount())
            .Callback(() => requestCount++);

        var scope = new ConnectionRequest.Scope(requestCounterMock.Object);
        Assert.Equal(1, requestCount);

        scope.Dispose();
        scope.Dispose();
        scope.Dispose();
        scope.Dispose();

        Assert.Equal(0, requestCount);

        requestCounterMock.Verify(x => x.DecrementExecutingSynchronousRequestCount(), Times.Once);
        requestCounterMock.Verify(x => x.IncrementExecutingSynchronousRequestCount(), Times.Once);
    }
}
