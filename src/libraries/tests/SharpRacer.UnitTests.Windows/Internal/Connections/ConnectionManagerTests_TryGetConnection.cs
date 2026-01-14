using Moq;
using SharpRacer.Internal.Connections.Requests;
using SharpRacer.Interop;

namespace SharpRacer.Internal.Connections;
public class ConnectionManagerTests_TryGetConnection
{
    [Theory]
    [MemberData(nameof(AsyncRequest_CanBeginConnectionTestData))]
    public void AsyncRequest_CanBeginConnectionTest(TimeSpan timeout, bool canBeginConnectionAttempt)
    {
        var mocks = new MockRepository(MockBehavior.Strict);

        var asyncRequestMock = mocks.Create<IAsyncConnectionRequest>();
        var connectionAcquisitionHandlerMock = mocks.Create<IConnectionAcquisitionHandler>();
        var objectManagerMock = mocks.Create<IConnectionObjectManager>();
        var outerConnectionMock = mocks.Create<IOuterConnection>();
        var signalsMock = mocks.Create<IConnectionSignalWaiter>();

        asyncRequestMock.SetupGet(x => x.Timeout)
            .Returns(timeout);

        asyncRequestMock.SetupGet(x => x.CanBeginConnectionAttempt)
            .Returns(canBeginConnectionAttempt);

        signalsMock.Setup(x => x.Wait(It.IsAny<bool>(), It.IsAny<TimeSpan>()))
            .Returns(ConnectionSignalWaitResult.WaitTimeout)
            .Verifiable(Times.Once());

        var connectionManager = new ConnectionManager(
            signalsMock.Object,
            new ConnectionRequestManager(),
            objectManagerMock.Object,
            connectionAcquisitionHandlerMock.Object);

        IOpenInnerConnection? innerConnectionResult = null;

        Assert.False(connectionManager.TryGetConnection(asyncRequestMock.Object, out innerConnectionResult));

        // Ensure Wait receives TimeSpan.Zero regardless of request timeout
        signalsMock.Verify(x => x.Wait(canBeginConnectionAttempt, TimeSpan.Zero), Times.Once());

        mocks.Verify();
    }

    [Fact]
    public void ConnectionAvailableTest()
    {
        var mocks = new MockRepository(MockBehavior.Strict);

        var connectionAcquisitionHandlerMock = mocks.Create<IConnectionAcquisitionHandler>();
        var objectManagerMock = mocks.Create<IConnectionObjectManager>();
        var innerConnectionMock = mocks.Create<IOpenInnerConnection>();
        var signalsMock = mocks.Create<IConnectionSignalWaiter>();

        objectManagerMock.Setup(x => x.GetConnection())
            .Returns(innerConnectionMock.Object)
            .Verifiable(Times.Once());

        signalsMock.Setup(x => x.Wait(It.IsAny<bool>(), It.IsAny<TimeSpan>()))
            .Returns(ConnectionSignalWaitResult.ConnectionAvailable);

        var connectionManager = new ConnectionManager(
            signalsMock.Object,
            new ConnectionRequestManager(),
            objectManagerMock.Object,
            connectionAcquisitionHandlerMock.Object);

        var timeout = TimeSpan.FromSeconds(5);

        Assert.True(connectionManager.TryGetConnection(timeout, true, out var innerConnectionResult));

        Assert.Equal(innerConnectionMock.Object, innerConnectionResult);

        signalsMock.Verify(x => x.Wait(true, timeout), Times.Once());

        mocks.Verify();
    }

    [Fact]
    public void ConnectionExceptionTest()
    {
        var mocks = new MockRepository(MockBehavior.Strict);

        var connectionAcquisitionHandlerMock = mocks.Create<IConnectionAcquisitionHandler>();
        var objectManagerMock = mocks.Create<IConnectionObjectManager>();
        var outerConnectionMock = mocks.Create<IOuterConnection>();
        var signalsMock = mocks.Create<IConnectionSignalWaiter>();

        var connectionExceptionMessage = "This is all your fault!";
        var connectionException = new SimulatorConnectionException(connectionExceptionMessage);

        objectManagerMock.Setup(x => x.GetConnectionException())
            .Returns(connectionException)
            .Verifiable(Times.Once());

        signalsMock.Setup(x => x.Wait(It.IsAny<bool>(), It.IsAny<TimeSpan>()))
            .Returns(ConnectionSignalWaitResult.ConnectionException);

        var connectionManager = new ConnectionManager(
            signalsMock.Object,
            new ConnectionRequestManager(),
            objectManagerMock.Object,
            connectionAcquisitionHandlerMock.Object);

        var timeout = TimeSpan.FromSeconds(5);
        IOpenInnerConnection? innerConnectionResult = null;

        var returnedException = Assert.Throws<SimulatorConnectionException>(
            () => connectionManager.TryGetConnection(timeout, true, out innerConnectionResult));

        Assert.Null(innerConnectionResult);
        Assert.IsType<SimulatorConnectionException>(returnedException);
        Assert.Equal(connectionExceptionMessage, returnedException.Message);

        signalsMock.Verify(x => x.Wait(true, timeout), Times.Once());

        mocks.Verify();
    }

    [Fact]
    public void CreateConnectionTest()
    {
        var mocks = new MockRepository(MockBehavior.Strict);

        var acquireConnectionWorkerMock = mocks.Create<IConnectionAcquisitionWorker>();
        var connectionAcquisitionHandlerMock = mocks.Create<IConnectionAcquisitionHandler>();
        var objectManagerMock = mocks.Create<IConnectionObjectManager>();
        var innerConnectionMock = mocks.Create<IOpenInnerConnection>();
        var signalsMock = mocks.Create<IConnectionSignalWaiter>();

        var timeout = TimeSpan.FromSeconds(5);
        var waitResult = ConnectionSignalWaitResult.CreateConnection;

        acquireConnectionWorkerMock.Setup(x => x.Start())
            .Callback(() =>
            {
                // Check the first Wait() call here since TryGetConnection is going to call itself on our single invocation
                verifyWaitCall(signalsMock, true, timeout);

                // Set next Wait() call result
                waitResult = ConnectionSignalWaitResult.ConnectionAvailable;
            });

        connectionAcquisitionHandlerMock.Setup(
            x => x.CreateWorker(
                It.IsAny<IOpenInnerConnectionOwner>(),
                It.IsAny<IOpenInnerConnectionFactory>(),
                It.IsAny<IConnectionProvider>(),
                It.IsAny<IDataReadyEventFactory>()))
            .Returns(acquireConnectionWorkerMock.Object);

        signalsMock.Setup(x => x.Wait(It.IsAny<bool>(), It.IsAny<TimeSpan>()))
            .Returns(() => waitResult);

        objectManagerMock.Setup(x => x.GetConnection())
            .Returns(innerConnectionMock.Object)
            .Verifiable(Times.Once());

        var connectionManager = new ConnectionManager(
            signalsMock.Object,
            new ConnectionRequestManager(),
            objectManagerMock.Object,
            connectionAcquisitionHandlerMock.Object);

        Assert.True(connectionManager.TryGetConnection(timeout, true, out var innerConnectionResult));
        Assert.Equal(innerConnectionResult, innerConnectionMock.Object);

        signalsMock.Verify(x => x.Wait(false, timeout), Times.Once());

        mocks.Verify();

        static void verifyWaitCall(Mock<IConnectionSignalWaiter> mock, bool allowCreateConnection, TimeSpan timeout)
        {
            mock.Verify(x => x.Wait(allowCreateConnection, timeout), Times.Once());
        }
    }

    [Fact]
    public void CreateConnectionStartsAcquisitionWorkerTest()
    {
        var mocks = new MockRepository(MockBehavior.Strict);

        var acquireConnectionWorkerMock = mocks.Create<IConnectionAcquisitionWorker>();
        var connectionAcquisitionHandlerMock = mocks.Create<IConnectionAcquisitionHandler>();
        var objectManagerMock = mocks.Create<IConnectionObjectManager>();
        var signalsMock = mocks.Create<IConnectionSignalWaiter>();

        var timeout = TimeSpan.FromSeconds(5);
        var waitResult = ConnectionSignalWaitResult.CreateConnection;

        acquireConnectionWorkerMock.Setup(x => x.Start())
            .Callback(() => waitResult = ConnectionSignalWaitResult.WaitTimeout) // Set next Wait() result so TryGetConnection exits
            .Verifiable(Times.Once());

        connectionAcquisitionHandlerMock
            .Setup(x => x.CreateWorker(
                It.IsAny<IOpenInnerConnectionOwner>(),
                It.IsAny<IOpenInnerConnectionFactory>(),
                It.IsAny<IConnectionProvider>(),
                It.IsAny<IDataReadyEventFactory>()))
            .Returns(acquireConnectionWorkerMock.Object)
            .Verifiable(Times.Once());

        signalsMock.Setup(x => x.Wait(It.IsAny<bool>(), It.IsAny<TimeSpan>()))
            .Returns(() => waitResult);

        var connectionManager = new ConnectionManager(
            signalsMock.Object,
            new ConnectionRequestManager(),
            objectManagerMock.Object,
            connectionAcquisitionHandlerMock.Object);

        Assert.False(connectionManager.TryGetConnection(timeout, true, out var innerConnectionResult));
        Assert.Null(innerConnectionResult);

        connectionAcquisitionHandlerMock.Verify(
            x => x.CreateWorker(
                connectionManager,
                It.IsAny<IOpenInnerConnectionFactory>(),
                connectionManager,
                DataReadyEventFactory.Default),
            Times.Once());

        mocks.Verify();
    }

    [Fact]
    public void InvalidWaitResultTest()
    {
        var mocks = new MockRepository(MockBehavior.Strict);

        var connectionAcquisitionHandlerMock = mocks.Create<IConnectionAcquisitionHandler>();
        var objectManagerMock = mocks.Create<IConnectionObjectManager>();
        var signalsMock = mocks.Create<IConnectionSignalWaiter>();

        var timeout = TimeSpan.FromSeconds(5);

        signalsMock.Setup(x => x.Wait(It.IsAny<bool>(), It.IsAny<TimeSpan>()))
            .Returns(() => (ConnectionSignalWaitResult)9999)
            .Verifiable(Times.Once());

        var connectionManager = new ConnectionManager(
            signalsMock.Object,
            new ConnectionRequestManager(),
            objectManagerMock.Object,
            connectionAcquisitionHandlerMock.Object);

        IOpenInnerConnection? innerConnectionResult = null;

        Assert.Throws<InvalidOperationException>(
            () => connectionManager.TryGetConnection(Timeout.InfiniteTimeSpan, true, out innerConnectionResult));

        mocks.Verify();
    }

    [Fact]
    public void WaitTimeoutTest()
    {
        var mocks = new MockRepository(MockBehavior.Strict);

        var connectionAcquisitionHandlerMock = mocks.Create<IConnectionAcquisitionHandler>();
        var objectManagerMock = mocks.Create<IConnectionObjectManager>();
        var signalsMock = mocks.Create<IConnectionSignalWaiter>();

        signalsMock.Setup(x => x.Wait(It.IsAny<bool>(), It.IsAny<TimeSpan>()))
            .Returns(ConnectionSignalWaitResult.WaitTimeout);

        var connectionManager = new ConnectionManager(
            signalsMock.Object,
            new ConnectionRequestManager(),
            objectManagerMock.Object,
            connectionAcquisitionHandlerMock.Object);

        var timeout = TimeSpan.FromSeconds(5);

        Assert.False(connectionManager.TryGetConnection(timeout, true, out var innerConnectionResult));
        Assert.Null(innerConnectionResult);

        signalsMock.Verify(x => x.Wait(true, timeout), Times.Once());

        mocks.Verify();
    }

    public static TheoryData<TimeSpan, bool> AsyncRequest_CanBeginConnectionTestData()
    {
        return new TheoryData<TimeSpan, bool>()
        {
            { TimeSpan.Zero, false },
            { TimeSpan.Zero, true},
            { Timeout.InfiniteTimeSpan, false},
            { Timeout.InfiniteTimeSpan, true},
            { TimeSpan.FromSeconds(30), false},
            { TimeSpan.FromSeconds(30), true}
        };
    }
}
