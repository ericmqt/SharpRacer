using Moq;

namespace SharpRacer.Internal.Connections.Requests;
public class ConnectionRequestManagerTests
{
    [Fact]
    public void Ctor_Test()
    {
        var requestManager = new ConnectionRequestManager();
        var requestManager2 = new ConnectionRequestManager(TimeProvider.System);
    }

    [Fact]
    public void CreateAsyncRequest_Test()
    {
        var outerConnectionMock = new Mock<IOuterConnection>(MockBehavior.Strict);
        var cancellationSource = new CancellationTokenSource();

        var requestQueueMock = new Mock<IAsyncConnectionRequestQueue>(MockBehavior.Strict);
        var requestCounterMock = new Mock<IPendingRequestCounter>(MockBehavior.Strict);
        var requestFactoryMock = new Mock<IConnectionRequestFactory>(MockBehavior.Strict);
        var requestSignalsMock = new Mock<IConnectionRequestSignals>(MockBehavior.Strict);
        var requestResultMock = new Mock<IAsyncConnectionRequest>(MockBehavior.Strict);

        requestFactoryMock.Setup(x => x.CreateAsyncRequest(
            It.Is<IOuterConnection>(i => i == outerConnectionMock.Object),
            It.Is<CancellationToken>(i => i == cancellationSource.Token)))
            .Returns(requestResultMock.Object);

        var requestManager = new ConnectionRequestManager(
            requestFactoryMock.Object, requestQueueMock.Object, requestSignalsMock.Object, requestCounterMock.Object);

        var request = requestManager.CreateAsyncRequest(outerConnectionMock.Object, cancellationSource.Token);

        Assert.NotNull(request);
        Assert.Equal(requestResultMock.Object, request);

        requestFactoryMock.VerifyAll();
    }

    [Fact]
    public void CreateAsyncRequest_TimeoutTest()
    {
        var outerConnectionMock = new Mock<IOuterConnection>(MockBehavior.Strict);
        var cancellationSource = new CancellationTokenSource();
        var timeout = TimeSpan.FromMinutes(3);

        var requestCounterMock = new Mock<IPendingRequestCounter>(MockBehavior.Strict);
        var requestFactoryMock = new Mock<IConnectionRequestFactory>(MockBehavior.Strict);
        var requestQueueMock = new Mock<IAsyncConnectionRequestQueue>(MockBehavior.Strict);
        var requestSignalsMock = new Mock<IConnectionRequestSignals>(MockBehavior.Strict);

        var requestResultMock = new Mock<IAsyncConnectionRequest>(MockBehavior.Strict);

        requestFactoryMock.Setup(x => x.CreateAsyncRequest(
            It.Is<IOuterConnection>(i => i == outerConnectionMock.Object),
            It.Is<TimeSpan>(i => i == timeout),
            It.Is<CancellationToken>(i => i == cancellationSource.Token)))
            .Returns(requestResultMock.Object);

        var requestManager = new ConnectionRequestManager(
            requestFactoryMock.Object, requestQueueMock.Object, requestSignalsMock.Object, requestCounterMock.Object);

        var request = requestManager.CreateAsyncRequest(outerConnectionMock.Object, timeout, cancellationSource.Token);

        Assert.NotNull(request);
        Assert.Equal(requestResultMock.Object, request);

        requestFactoryMock.VerifyAll();
    }

    [Fact]
    public void CreateRequest_Test()
    {
        var outerConnectionMock = new Mock<IOuterConnection>(MockBehavior.Strict);

        var requestCounterMock = new Mock<IPendingRequestCounter>(MockBehavior.Strict);
        var requestQueueMock = new Mock<IAsyncConnectionRequestQueue>(MockBehavior.Strict);
        var requestSignalsMock = new Mock<IConnectionRequestSignals>(MockBehavior.Strict);

        // Can't mock something that returns a ref struct so we have to use the normal factory here
        var requestFactory = new ConnectionRequestFactory(requestCounterMock.Object, requestSignalsMock.Object, TimeProvider.System);

        var requestManager = new ConnectionRequestManager(
            requestFactory, requestQueueMock.Object, requestSignalsMock.Object, requestCounterMock.Object);

        var request = requestManager.CreateRequest(outerConnectionMock.Object);

        Assert.Equal(outerConnectionMock.Object, request.OuterConnection);
        Assert.Equal(Timeout.InfiniteTimeSpan, request.Timeout);
    }

    [Fact]
    public void CreateRequest_TimeoutTest()
    {
        var outerConnectionMock = new Mock<IOuterConnection>(MockBehavior.Strict);
        var timeout = TimeSpan.FromMinutes(3);

        var requestCounterMock = new Mock<IPendingRequestCounter>(MockBehavior.Strict);
        var requestQueueMock = new Mock<IAsyncConnectionRequestQueue>(MockBehavior.Strict);
        var requestSignalsMock = new Mock<IConnectionRequestSignals>(MockBehavior.Strict);

        // Can't mock something that returns a ref struct so we have to use the normal factory here
        var requestFactory = new ConnectionRequestFactory(requestCounterMock.Object, requestSignalsMock.Object, TimeProvider.System);

        var requestManager = new ConnectionRequestManager(
            requestFactory, requestQueueMock.Object, requestSignalsMock.Object, requestCounterMock.Object);

        var request = requestManager.CreateRequest(outerConnectionMock.Object, timeout);

        Assert.Equal(outerConnectionMock.Object, request.OuterConnection);
        Assert.Equal(timeout, request.Timeout);
    }

    [Fact]
    public void CreateRequestBlockingScope_Test()
    {
        var requestCounterMock = new Mock<IPendingRequestCounter>(MockBehavior.Strict);
        var requestFactoryMock = new Mock<IConnectionRequestFactory>(MockBehavior.Strict);
        var requestQueueMock = new Mock<IAsyncConnectionRequestQueue>(MockBehavior.Strict);
        var requestSignalsMock = new Mock<IConnectionRequestSignals>(MockBehavior.Strict);

        requestSignalsMock.Setup(x => x.BlockAsyncRequestCreation());
        requestSignalsMock.Setup(x => x.BlockRequestExecution());

        var requestManager = new ConnectionRequestManager(
            requestFactoryMock.Object, requestQueueMock.Object, requestSignalsMock.Object, requestCounterMock.Object);

        var scope = requestManager.CreateRequestBlockingScope();

        requestSignalsMock.Verify(x => x.BlockAsyncRequestCreation(), Times.Once);
        requestSignalsMock.Verify(x => x.BlockRequestExecution(), Times.Once);
    }

    [Fact]
    public void HasPendingRequests_Test()
    {
        var hasPendingRequestsRetVal = true;

        var requestCounterMock = new Mock<IPendingRequestCounter>(MockBehavior.Strict);
        var requestFactoryMock = new Mock<IConnectionRequestFactory>(MockBehavior.Strict);
        var requestQueueMock = new Mock<IAsyncConnectionRequestQueue>(MockBehavior.Strict);
        var requestSignalsMock = new Mock<IConnectionRequestSignals>(MockBehavior.Strict);

        requestCounterMock.Setup(x => x.HasPendingRequests())
            .Returns(() => hasPendingRequestsRetVal);

        var requestManager = new ConnectionRequestManager(
            requestFactoryMock.Object, requestQueueMock.Object, requestSignalsMock.Object, requestCounterMock.Object);

        Assert.True(requestManager.HasPendingRequests());

        hasPendingRequestsRetVal = false;
        Assert.False(requestManager.HasPendingRequests());
    }

    [Fact]
    public void ProcessAsyncRequestQueue_Test()
    {
        var connectionProviderMock = new Mock<IConnectionProvider>(MockBehavior.Strict);
        var force = false;

        var requestCounterMock = new Mock<IPendingRequestCounter>(MockBehavior.Strict);
        var requestFactoryMock = new Mock<IConnectionRequestFactory>(MockBehavior.Strict);
        var requestQueueMock = new Mock<IAsyncConnectionRequestQueue>(MockBehavior.Strict);
        var requestSignalsMock = new Mock<IConnectionRequestSignals>(MockBehavior.Strict);

        requestQueueMock.Setup(x => x.ProcessQueue(It.IsAny<IConnectionProvider>(), It.Is<bool>(i => i == force)))
            .Returns(true);

        var requestManager = new ConnectionRequestManager(
            requestFactoryMock.Object, requestQueueMock.Object, requestSignalsMock.Object, requestCounterMock.Object);

        force = true;
        requestManager.ProcessAsyncRequestQueue(connectionProviderMock.Object, force);

        force = false;
        requestManager.ProcessAsyncRequestQueue(connectionProviderMock.Object, force);

        requestQueueMock.Verify(x => x.ProcessQueue(connectionProviderMock.Object, false), Times.Once);
        requestQueueMock.Verify(x => x.ProcessQueue(connectionProviderMock.Object, true), Times.Once);
    }

    [Fact]
    public void QueueAsyncRequest_Test()
    {
        var requestMock = new Mock<IAsyncConnectionRequest>(MockBehavior.Strict);

        var requestCounterMock = new Mock<IPendingRequestCounter>(MockBehavior.Strict);
        var requestFactoryMock = new Mock<IConnectionRequestFactory>(MockBehavior.Strict);
        var requestQueueMock = new Mock<IAsyncConnectionRequestQueue>(MockBehavior.Strict);
        var requestSignalsMock = new Mock<IConnectionRequestSignals>(MockBehavior.Strict);

        var requestManager = new ConnectionRequestManager(
            requestFactoryMock.Object, requestQueueMock.Object, requestSignalsMock.Object, requestCounterMock.Object);

        requestQueueMock.Setup(x => x.Enqueue(It.IsAny<IAsyncConnectionRequest>()));

        requestManager.QueueAsyncRequest(requestMock.Object);

        requestQueueMock.Verify(x => x.Enqueue(requestMock.Object), Times.Once);
    }
}
