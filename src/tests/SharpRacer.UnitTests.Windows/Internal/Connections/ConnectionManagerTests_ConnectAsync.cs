using Moq;
using SharpRacer.Internal.Connections.Requests;

namespace SharpRacer.Internal.Connections;
public class ConnectionManagerTests_ConnectAsync
{
    [Fact]
    public async Task CreateAsyncRequest_NoTimeoutTest()
    {
        var mocks = new MockRepository(MockBehavior.Strict);

        var asyncRequestMock = mocks.Create<IAsyncConnectionRequest>();
        var connectionAcquisitionHandlerMock = mocks.Create<IConnectionAcquisitionHandler>();
        var objectManagerMock = mocks.Create<IConnectionObjectManager>();
        var outerConnectionMock = mocks.Create<IOuterConnection>();
        var requestManagerMock = mocks.Create<IConnectionRequestManager>();
        var signalsMock = mocks.Create<IConnectionSignalWaiter>();

        var asyncRequestCompletion = new TaskCompletionSource();
        asyncRequestCompletion.TrySetResult();

        asyncRequestMock.Setup(x => x.TryComplete(It.IsAny<IConnectionProvider>()))
            .Returns(true);

        asyncRequestMock.Setup(x => x.Dispose());
        asyncRequestMock.SetupGet(x => x.Completion).Returns(asyncRequestCompletion);

        requestManagerMock.Setup(
                x => x.CreateAsyncRequest(
                    It.IsAny<IOuterConnection>(),
                    It.IsAny<TimeSpan>(),
                    It.IsAny<CancellationToken>()))
            .Returns(asyncRequestMock.Object);

        requestManagerMock.Setup(x => x.QueueAsyncRequest(It.IsAny<IAsyncConnectionRequest>()));
        requestManagerMock.Setup(x => x.ProcessAsyncRequestQueue(It.IsAny<IConnectionProvider>(), It.IsAny<bool>()));

        var connectionManager = new ConnectionManager(
            signalsMock.Object,
            requestManagerMock.Object,
            objectManagerMock.Object,
            connectionAcquisitionHandlerMock.Object);

        // No timeout specified
        var cancellationSource = new CancellationTokenSource();
        await connectionManager.ConnectAsync(outerConnectionMock.Object, cancellationSource.Token);

        requestManagerMock.Verify(
            x => x.CreateAsyncRequest(outerConnectionMock.Object, Timeout.InfiniteTimeSpan, cancellationSource.Token), Times.Once());

        requestManagerMock.Verify(x => x.QueueAsyncRequest(asyncRequestMock.Object), Times.Never());
        requestManagerMock.Verify(x => x.ProcessAsyncRequestQueue(connectionManager, false), Times.Never());

        asyncRequestMock.Verify(x => x.Dispose(), Times.Once());
        asyncRequestMock.Verify(x => x.TryComplete(connectionManager), Times.Once());

        mocks.Verify();
    }

    [Fact]
    public async Task CreateAsyncRequest_WithTimeoutTest()
    {
        var mocks = new MockRepository(MockBehavior.Strict);

        var asyncRequestMock = mocks.Create<IAsyncConnectionRequest>();
        var connectionAcquisitionHandlerMock = mocks.Create<IConnectionAcquisitionHandler>();
        var objectManagerMock = mocks.Create<IConnectionObjectManager>();
        var outerConnectionMock = mocks.Create<IOuterConnection>();
        var requestManagerMock = mocks.Create<IConnectionRequestManager>();
        var signalsMock = mocks.Create<IConnectionSignalWaiter>();

        var asyncRequestCompletion = new TaskCompletionSource();
        asyncRequestCompletion.TrySetResult();

        asyncRequestMock.Setup(x => x.TryComplete(It.IsAny<IConnectionProvider>()))
            .Returns(true);

        asyncRequestMock.Setup(x => x.Dispose());
        asyncRequestMock.SetupGet(x => x.Completion).Returns(asyncRequestCompletion);

        requestManagerMock.Setup(
                x => x.CreateAsyncRequest(
                    It.IsAny<IOuterConnection>(),
                    It.IsAny<TimeSpan>(),
                    It.IsAny<CancellationToken>()))
            .Returns(asyncRequestMock.Object);

        requestManagerMock.Setup(x => x.QueueAsyncRequest(It.IsAny<IAsyncConnectionRequest>()));
        requestManagerMock.Setup(x => x.ProcessAsyncRequestQueue(It.IsAny<IConnectionProvider>(), It.IsAny<bool>()));

        var connectionManager = new ConnectionManager(
            signalsMock.Object,
            requestManagerMock.Object,
            objectManagerMock.Object,
            connectionAcquisitionHandlerMock.Object);

        // With timeout
        var cancellationSource = new CancellationTokenSource();
        var timeout = TimeSpan.FromSeconds(5);
        await connectionManager.ConnectAsync(outerConnectionMock.Object, timeout, cancellationSource.Token);

        requestManagerMock.Verify(
            x => x.CreateAsyncRequest(outerConnectionMock.Object, timeout, cancellationSource.Token), Times.Once());

        requestManagerMock.Verify(x => x.QueueAsyncRequest(asyncRequestMock.Object), Times.Never());
        requestManagerMock.Verify(x => x.ProcessAsyncRequestQueue(connectionManager, false), Times.Never());

        asyncRequestMock.Verify(x => x.Dispose(), Times.Once());
        asyncRequestMock.Verify(x => x.TryComplete(connectionManager), Times.Once());

        mocks.Verify();
    }

    [Fact]
    public async Task QueueIfNotImmediatelyCompleted_NoTimeoutTest()
    {
        var mocks = new MockRepository(MockBehavior.Strict);

        var asyncRequestMock = mocks.Create<IAsyncConnectionRequest>();
        var connectionAcquisitionHandlerMock = mocks.Create<IConnectionAcquisitionHandler>();
        var objectManagerMock = mocks.Create<IConnectionObjectManager>();
        var outerConnectionMock = mocks.Create<IOuterConnection>();
        var requestManagerMock = mocks.Create<IConnectionRequestManager>();
        var signalsMock = mocks.Create<IConnectionSignalWaiter>();

        var asyncRequestCompletion = new TaskCompletionSource();
        asyncRequestCompletion.TrySetResult();

        // Set up TryComplete to return false so request is queued
        asyncRequestMock.Setup(x => x.TryComplete(It.IsAny<IConnectionProvider>()))
            .Returns(false)
            .Verifiable(Times.Once());

        asyncRequestMock.Setup(x => x.Dispose()).Verifiable(Times.Once());

        asyncRequestMock.SetupGet(x => x.Completion)
            .Returns(asyncRequestCompletion)
            .Verifiable(Times.Once());

        requestManagerMock.Setup(
                x => x.CreateAsyncRequest(
                    It.IsAny<IOuterConnection>(),
                    It.IsAny<TimeSpan>(),
                    It.IsAny<CancellationToken>()))
            .Returns(asyncRequestMock.Object)
            .Verifiable(Times.Once());

        requestManagerMock.Setup(x => x.QueueAsyncRequest(It.IsAny<IAsyncConnectionRequest>()));
        requestManagerMock.Setup(x => x.ProcessAsyncRequestQueue(It.IsAny<IConnectionProvider>(), It.IsAny<bool>()));

        var connectionManager = new ConnectionManager(
            signalsMock.Object,
            requestManagerMock.Object,
            objectManagerMock.Object,
            connectionAcquisitionHandlerMock.Object);

        var cancellationSource = new CancellationTokenSource();

        await connectionManager.ConnectAsync(outerConnectionMock.Object, cancellationSource.Token);

        requestManagerMock.Verify(x => x.QueueAsyncRequest(asyncRequestMock.Object), Times.Once());
        requestManagerMock.Verify(x => x.ProcessAsyncRequestQueue(connectionManager, false), Times.Once());

        mocks.Verify();
    }

    [Fact]
    public async Task QueueIfNotImmediatelyCompleted_WithTimeoutTest()
    {
        var mocks = new MockRepository(MockBehavior.Strict);

        var asyncRequestMock = mocks.Create<IAsyncConnectionRequest>();
        var connectionAcquisitionHandlerMock = mocks.Create<IConnectionAcquisitionHandler>();
        var objectManagerMock = mocks.Create<IConnectionObjectManager>();
        var outerConnectionMock = mocks.Create<IOuterConnection>();
        var requestManagerMock = mocks.Create<IConnectionRequestManager>();
        var signalsMock = mocks.Create<IConnectionSignalWaiter>();

        var asyncRequestCompletion = new TaskCompletionSource();
        asyncRequestCompletion.TrySetResult();

        // Set up TryComplete to return false so request is queued
        asyncRequestMock.Setup(x => x.TryComplete(It.IsAny<IConnectionProvider>()))
            .Returns(false)
            .Verifiable(Times.Once());

        asyncRequestMock.Setup(x => x.Dispose()).Verifiable(Times.Once());

        asyncRequestMock.SetupGet(x => x.Completion)
            .Returns(asyncRequestCompletion)
            .Verifiable(Times.Once());

        requestManagerMock.Setup(
                x => x.CreateAsyncRequest(
                    It.IsAny<IOuterConnection>(),
                    It.IsAny<TimeSpan>(),
                    It.IsAny<CancellationToken>()))
            .Returns(asyncRequestMock.Object)
            .Verifiable(Times.Once());

        requestManagerMock.Setup(x => x.QueueAsyncRequest(It.IsAny<IAsyncConnectionRequest>()));
        requestManagerMock.Setup(x => x.ProcessAsyncRequestQueue(It.IsAny<IConnectionProvider>(), It.IsAny<bool>()));

        var connectionManager = new ConnectionManager(
            signalsMock.Object,
            requestManagerMock.Object,
            objectManagerMock.Object,
            connectionAcquisitionHandlerMock.Object);

        var cancellationSource = new CancellationTokenSource();
        var timeout = TimeSpan.FromSeconds(5);

        await connectionManager.ConnectAsync(outerConnectionMock.Object, timeout, cancellationSource.Token);

        requestManagerMock.Verify(x => x.QueueAsyncRequest(asyncRequestMock.Object), Times.Once());
        requestManagerMock.Verify(x => x.ProcessAsyncRequestQueue(connectionManager, false), Times.Once());

        mocks.Verify();
    }

    [Fact]
    public async Task ThrowIfOuterConnectionIsNullTest()
    {
        var mocks = new MockRepository(MockBehavior.Strict);

        var connectionAcquisitionHandlerMock = mocks.Create<IConnectionAcquisitionHandler>();
        var objectManagerMock = mocks.Create<IConnectionObjectManager>();
        var outerConnectionMock = mocks.Create<IOuterConnection>();
        var requestManagerMock = mocks.Create<IConnectionRequestManager>();
        var signalsMock = mocks.Create<IConnectionSignalWaiter>();

        var connectionManager = new ConnectionManager(
            signalsMock.Object,
            requestManagerMock.Object,
            objectManagerMock.Object,
            connectionAcquisitionHandlerMock.Object);

        await Assert.ThrowsAsync<ArgumentNullException>(
            () => connectionManager.ConnectAsync(null!, TimeSpan.FromSeconds(5)));
    }

    [Fact]
    public async Task ThrowIfTimeoutIsNegativeTest()
    {
        var mocks = new MockRepository(MockBehavior.Strict);

        var connectionAcquisitionHandlerMock = mocks.Create<IConnectionAcquisitionHandler>();
        var objectManagerMock = mocks.Create<IConnectionObjectManager>();
        var outerConnectionMock = mocks.Create<IOuterConnection>();
        var requestManagerMock = mocks.Create<IConnectionRequestManager>();
        var signalsMock = mocks.Create<IConnectionSignalWaiter>();

        var connectionManager = new ConnectionManager(
            signalsMock.Object,
            requestManagerMock.Object,
            objectManagerMock.Object,
            connectionAcquisitionHandlerMock.Object);

        await Assert.ThrowsAsync<ArgumentOutOfRangeException>(
            () => connectionManager.ConnectAsync(outerConnectionMock.Object, TimeSpan.FromSeconds(-1)));
    }
}
