using Moq;

namespace SharpRacer.Internal.Connections.Requests;
public class ConnectionRequestFactoryTests
{
    [Fact]
    public void CreateAsyncRequest_DisposeAsyncScopeOnRequestCtorExceptionTest()
    {
        bool enteredRequestScope = false;
        bool exitedRequestScope = false;
        int queuedAsyncRequestCount = 0;

        var outerConnectionMock = new Mock<IOuterConnection>(MockBehavior.Strict);

        var requestCounterMock = new Mock<IPendingRequestCounter>(MockBehavior.Strict);

        requestCounterMock.Setup(x => x.IncrementQueuedAsyncRequestCount())
            .Callback(() =>
            {
                queuedAsyncRequestCount++;
                enteredRequestScope = true;
            });

        requestCounterMock.Setup(x => x.DecrementQueuedAsyncRequestCount())
            .Callback(() =>
            {
                queuedAsyncRequestCount--;
                exitedRequestScope = true;
            });

        var requestSignalsMock = new Mock<IConnectionRequestSignals>(MockBehavior.Strict);

        requestSignalsMock.Setup(x => x.WaitForAsyncRequestCreation());

        // Null time provider will throw from AsyncConnectionRequest constructor
        var requestFactory = new ConnectionRequestFactory(requestCounterMock.Object, requestSignalsMock.Object, timeProvider: null!);

        Assert.ThrowsAny<Exception>(() => requestFactory.CreateAsyncRequest(outerConnectionMock.Object, TimeSpan.FromSeconds(3)));

        requestCounterMock.Verify(x => x.IncrementQueuedAsyncRequestCount(), Times.Once);
        requestCounterMock.Verify(x => x.DecrementQueuedAsyncRequestCount(), Times.Once);

        Assert.True(enteredRequestScope);
        Assert.True(exitedRequestScope);
        Assert.Equal(0, queuedAsyncRequestCount);
    }

    [Fact]
    public void CreateAsyncRequest_Test()
    {
        var outerConnectionMock = new Mock<IOuterConnection>(MockBehavior.Strict);

        var requestCounterMock = new Mock<IPendingRequestCounter>(MockBehavior.Strict);
        requestCounterMock.Setup(x => x.IncrementQueuedAsyncRequestCount());
        requestCounterMock.Setup(x => x.DecrementQueuedAsyncRequestCount());

        var requestSignalsMock = new Mock<IConnectionRequestSignals>(MockBehavior.Strict);

        requestSignalsMock.Setup(x => x.WaitForAsyncRequestCreation());

        var requestFactory = new ConnectionRequestFactory(requestCounterMock.Object, requestSignalsMock.Object, TimeProvider.System);

        var request = requestFactory.CreateAsyncRequest(outerConnectionMock.Object);

        Assert.Equal(outerConnectionMock.Object, request.OuterConnection);
        Assert.Equal(Timeout.InfiniteTimeSpan, request.Timeout);
        Assert.True(request.CanBeginConnectionAttempt);

        requestSignalsMock.Verify(x => x.WaitForAsyncRequestCreation(), Times.Once);
    }

    [Fact]
    public void CreateAsyncRequest_TimeoutTest()
    {
        var outerConnectionMock = new Mock<IOuterConnection>(MockBehavior.Strict);

        var requestCounterMock = new Mock<IPendingRequestCounter>(MockBehavior.Strict);
        requestCounterMock.Setup(x => x.IncrementQueuedAsyncRequestCount());
        requestCounterMock.Setup(x => x.DecrementQueuedAsyncRequestCount());

        var requestSignalsMock = new Mock<IConnectionRequestSignals>(MockBehavior.Strict);

        requestSignalsMock.Setup(x => x.WaitForAsyncRequestCreation());

        var requestFactory = new ConnectionRequestFactory(requestCounterMock.Object, requestSignalsMock.Object, TimeProvider.System);

        var timeout = TimeSpan.FromSeconds(4);
        var request = requestFactory.CreateAsyncRequest(outerConnectionMock.Object, timeout);

        Assert.Equal(outerConnectionMock.Object, request.OuterConnection);
        Assert.Equal(timeout, request.Timeout);
        Assert.True(request.CanBeginConnectionAttempt);

        requestSignalsMock.Verify(x => x.WaitForAsyncRequestCreation(), Times.Once);
    }

    [Fact]
    public void CreateAsyncRequest_ThrowsOnNegativeTimeoutTest()
    {
        var outerConnectionMock = new Mock<IOuterConnection>(MockBehavior.Strict);
        var requestCounterMock = new Mock<IPendingRequestCounter>(MockBehavior.Strict);
        var requestSignalsMock = new Mock<IConnectionRequestSignals>(MockBehavior.Strict);

        var requestFactory = new ConnectionRequestFactory(requestCounterMock.Object, requestSignalsMock.Object, TimeProvider.System);

        Assert.Throws<ArgumentOutOfRangeException>(
            () => requestFactory.CreateAsyncRequest(outerConnectionMock.Object, TimeSpan.FromSeconds(-1)));
    }

    [Fact]
    public void CreateAsyncScope_Test()
    {
        var requestCounterMock = new Mock<IPendingRequestCounter>(MockBehavior.Strict);

        requestCounterMock.Setup(x => x.IncrementQueuedAsyncRequestCount());

        var requestSignalsMock = new Mock<IConnectionRequestSignals>(MockBehavior.Strict);

        requestSignalsMock.Setup(x => x.WaitForAsyncRequestCreation());

        var requestFactory = new ConnectionRequestFactory(requestCounterMock.Object, requestSignalsMock.Object, TimeProvider.System);

        var scope = requestFactory.CreateAsyncScope();

        requestCounterMock.Verify(x => x.IncrementQueuedAsyncRequestCount(), Times.Once);
        requestSignalsMock.Verify(x => x.WaitForAsyncRequestCreation(), Times.Once);
    }

    [Fact]
    public void CreateRequest_Test()
    {
        var outerConnectionMock = new Mock<IOuterConnection>(MockBehavior.Strict);

        var requestCounterMock = new Mock<IPendingRequestCounter>(MockBehavior.Strict);
        requestCounterMock.Setup(x => x.IncrementQueuedAsyncRequestCount());
        requestCounterMock.Setup(x => x.DecrementQueuedAsyncRequestCount());

        var requestSignalsMock = new Mock<IConnectionRequestSignals>(MockBehavior.Strict);

        requestSignalsMock.Setup(x => x.WaitForAsyncRequestCreation());

        var requestFactory = new ConnectionRequestFactory(requestCounterMock.Object, requestSignalsMock.Object, TimeProvider.System);

        var timeout = TimeSpan.FromSeconds(4);
        var request = requestFactory.CreateRequest(outerConnectionMock.Object);

        Assert.Equal(outerConnectionMock.Object, request.OuterConnection);
        Assert.Equal(Timeout.InfiniteTimeSpan, request.Timeout);
    }

    [Fact]
    public void CreateRequest_TimeoutTest()
    {
        var outerConnectionMock = new Mock<IOuterConnection>(MockBehavior.Strict);

        var requestCounterMock = new Mock<IPendingRequestCounter>(MockBehavior.Strict);
        requestCounterMock.Setup(x => x.IncrementQueuedAsyncRequestCount());
        requestCounterMock.Setup(x => x.DecrementQueuedAsyncRequestCount());

        var requestSignalsMock = new Mock<IConnectionRequestSignals>(MockBehavior.Strict);

        requestSignalsMock.Setup(x => x.WaitForAsyncRequestCreation());

        var requestFactory = new ConnectionRequestFactory(requestCounterMock.Object, requestSignalsMock.Object, TimeProvider.System);

        var timeout = TimeSpan.FromSeconds(4);
        var request = requestFactory.CreateRequest(outerConnectionMock.Object, timeout);

        Assert.Equal(outerConnectionMock.Object, request.OuterConnection);
        Assert.Equal(timeout, request.Timeout);
    }

    [Fact]
    public void CreateRequest_ThrowsOnNegativeTimeoutTest()
    {
        var outerConnectionMock = new Mock<IOuterConnection>(MockBehavior.Strict);
        var requestCounterMock = new Mock<IPendingRequestCounter>(MockBehavior.Strict);
        var requestSignalsMock = new Mock<IConnectionRequestSignals>(MockBehavior.Strict);

        var requestFactory = new ConnectionRequestFactory(requestCounterMock.Object, requestSignalsMock.Object, TimeProvider.System);

        Assert.Throws<ArgumentOutOfRangeException>(
            () => requestFactory.CreateRequest(outerConnectionMock.Object, TimeSpan.FromSeconds(-1)));
    }

    [Fact]
    public void CreateScope_Test()
    {
        var requestCounterMock = new Mock<IPendingRequestCounter>(MockBehavior.Strict);

        requestCounterMock.Setup(x => x.IncrementExecutingSynchronousRequestCount());

        var requestSignalsMock = new Mock<IConnectionRequestSignals>(MockBehavior.Strict);

        requestSignalsMock.Setup(x => x.WaitForRequestExecution());

        var requestFactory = new ConnectionRequestFactory(requestCounterMock.Object, requestSignalsMock.Object, TimeProvider.System);

        var scope = requestFactory.CreateScope();

        requestCounterMock.Verify(x => x.IncrementExecutingSynchronousRequestCount(), Times.Once);
        requestSignalsMock.Verify(x => x.WaitForRequestExecution(), Times.Once);
    }
}
