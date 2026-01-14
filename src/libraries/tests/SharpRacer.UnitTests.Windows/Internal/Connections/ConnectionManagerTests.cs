using Moq;
using SharpRacer.Internal.Connections.Requests;

namespace SharpRacer.Internal.Connections;

public partial class ConnectionManagerTests
{
    [Fact]
    public void Connect_Test()
    {
        var mocks = new MockRepository(MockBehavior.Strict);

        var connectionAcquisitionHandlerMock = mocks.Create<IConnectionAcquisitionHandler>();
        var objectManagerMock = mocks.Create<IConnectionObjectManager>();
        var innerConnectionMock = mocks.Create<IOpenInnerConnection>();
        var outerConnectionMock = mocks.Create<IOuterConnection>();
        var signalsMock = mocks.Create<IConnectionSignalWaiter>();

        objectManagerMock.Setup(x => x.GetConnection()).Returns(innerConnectionMock.Object);

        signalsMock.Setup(x => x.Wait(It.IsAny<bool>(), It.IsAny<TimeSpan>()))
            .Returns(ConnectionSignalWaitResult.ConnectionAvailable);

        innerConnectionMock.Setup(x => x.Attach(It.IsAny<IOuterConnection>()))
            .Returns(true);

        var connectionManager = new ConnectionManager(
            signalsMock.Object,
            new ConnectionRequestManager(),
            objectManagerMock.Object,
            connectionAcquisitionHandlerMock.Object);

        connectionManager.Connect(outerConnectionMock.Object);

        signalsMock.Verify(x => x.Wait(true, Timeout.InfiniteTimeSpan), Times.Once());
        innerConnectionMock.Verify(x => x.Attach(outerConnectionMock.Object), Times.Once());

        mocks.Verify();
    }

    [Fact]
    public void Connect_ConnectionExceptionTest()
    {
        var mocks = new MockRepository(MockBehavior.Strict);

        var connectionAcquisitionHandlerMock = mocks.Create<IConnectionAcquisitionHandler>();
        var objectManagerMock = mocks.Create<IConnectionObjectManager>();
        var innerConnectionMock = mocks.Create<IOpenInnerConnection>();
        var outerConnectionMock = mocks.Create<IOuterConnection>();
        var signalsMock = mocks.Create<IConnectionSignalWaiter>();

        var connectionExceptionMessage = "This is all your fault!";
        var connectionException = new SimulatorConnectionException(connectionExceptionMessage);

        objectManagerMock.Setup(x => x.GetConnectionException())
            .Returns(connectionException)
            .Verifiable(Times.Once());

        signalsMock.Setup(x => x.Wait(It.IsAny<bool>(), It.IsAny<TimeSpan>()))
            .Returns(ConnectionSignalWaitResult.ConnectionException);

        innerConnectionMock.Setup(x => x.Attach(It.IsAny<IOuterConnection>()))
            .Returns(true);

        var connectionManager = new ConnectionManager(
            signalsMock.Object,
            new ConnectionRequestManager(),
            objectManagerMock.Object,
            connectionAcquisitionHandlerMock.Object);

        var returnedException = Assert.Throws<SimulatorConnectionException>(() => connectionManager.Connect(outerConnectionMock.Object));

        Assert.IsType<SimulatorConnectionException>(returnedException);
        Assert.Equal(connectionExceptionMessage, returnedException.Message);

        signalsMock.Verify(x => x.Wait(true, Timeout.InfiniteTimeSpan), Times.Once());

        mocks.Verify();
    }

    [Fact]
    public void Connect_WithTimeoutTest()
    {
        var mocks = new MockRepository(MockBehavior.Strict);

        var connectionAcquisitionHandlerMock = mocks.Create<IConnectionAcquisitionHandler>();
        var objectManagerMock = mocks.Create<IConnectionObjectManager>();
        var innerConnectionMock = mocks.Create<IOpenInnerConnection>();
        var outerConnectionMock = mocks.Create<IOuterConnection>();
        var signalsMock = mocks.Create<IConnectionSignalWaiter>();

        objectManagerMock.Setup(x => x.GetConnection()).Returns(innerConnectionMock.Object);

        signalsMock.Setup(x => x.Wait(It.IsAny<bool>(), It.IsAny<TimeSpan>()))
            .Returns(ConnectionSignalWaitResult.ConnectionAvailable);

        innerConnectionMock.Setup(x => x.Attach(It.IsAny<IOuterConnection>()))
            .Returns(true);

        var connectionManager = new ConnectionManager(
            signalsMock.Object,
            new ConnectionRequestManager(),
            objectManagerMock.Object,
            connectionAcquisitionHandlerMock.Object);

        var timeout = TimeSpan.FromSeconds(5);
        connectionManager.Connect(outerConnectionMock.Object, timeout);

        signalsMock.Verify(x => x.Wait(true, timeout), Times.Once());
        innerConnectionMock.Verify(x => x.Attach(outerConnectionMock.Object), Times.Once());

        mocks.Verify();
    }

    [Fact]
    public void DefaultInstanceTest()
    {
        Assert.NotNull(ConnectionManager.Default);
    }

    [Fact]
    public void NewConnectionId_Test()
    {
        var connectionManager = (IOpenInnerConnectionOwner)ConnectionManager.Default;

        int lastId = 0;

        for (int i = 0; i < 1000; i++)
        {
            var newId = connectionManager.NewConnectionId();

            Assert.Equal(++lastId, newId);
        }
    }

    [Fact]
    public void OnConnectionClosing_Test()
    {
        var allowAsyncRequestCreation = true;
        var allowRequestExecution = true;

        var mocks = new MockRepository(MockBehavior.Strict);

        var connectionAcquisitionHandlerMock = mocks.Create<IConnectionAcquisitionHandler>();
        var objectManagerMock = mocks.Create<IConnectionObjectManager>();
        var innerConnectionMock = mocks.Create<IOpenInnerConnection>();
        var signalsMock = mocks.Create<IConnectionSignalWaiter>();
        var requestSignalsMock = mocks.Create<IConnectionRequestSignals>();

        objectManagerMock.Setup(x => x.ClearConnection(It.IsAny<IOpenInnerConnection>()))
            .Callback(() =>
            {
                Assert.False(allowRequestExecution);
                Assert.False(allowAsyncRequestCreation);
            })
            .Returns(true);

        requestSignalsMock.Setup(x => x.AllowAsyncRequestCreation())
            .Callback(() => allowAsyncRequestCreation = true)
            .Verifiable(Times.Once());

        requestSignalsMock.Setup(x => x.AllowRequestExecution())
            .Callback(() => allowRequestExecution = true)
            .Verifiable(Times.Once());

        requestSignalsMock.Setup(x => x.BlockAsyncRequestCreation())
            .Callback(() => allowAsyncRequestCreation = false)
            .Verifiable(Times.Once());

        requestSignalsMock.Setup(x => x.BlockRequestExecution())
            .Callback(() => allowRequestExecution = false)
            .Verifiable(Times.Once());

        signalsMock.Setup(x => x.SetCreateConnectionSignal())
            .Verifiable(Times.Once());

        var requestManager = new ConnectionRequestManager(requestSignals: requestSignalsMock.Object);

        IOpenInnerConnectionOwner connectionManager = new ConnectionManager(
            signalsMock.Object,
            requestManager,
            objectManagerMock.Object,
            connectionAcquisitionHandlerMock.Object);

        connectionManager.OnConnectionClosing(innerConnectionMock.Object);

        Assert.True(allowAsyncRequestCreation);
        Assert.True(allowRequestExecution);

        objectManagerMock.Verify(x => x.ClearConnection(innerConnectionMock.Object), Times.Once());
        signalsMock.Verify(x => x.SetCreateConnectionSignal(), Times.Once());

        mocks.Verify();
    }

    [Fact]
    public void OnConnectionClosing_DisposesOrphanedConnectionTest()
    {
        var allowAsyncRequestCreation = true;
        var allowRequestExecution = true;

        var mocks = new MockRepository(MockBehavior.Strict);

        var connectionAcquisitionHandlerMock = mocks.Create<IConnectionAcquisitionHandler>();
        var objectManagerMock = mocks.Create<IConnectionObjectManager>();
        var innerConnectionMock = mocks.Create<IOpenInnerConnection>();
        var signalsMock = mocks.Create<IConnectionSignalWaiter>();
        var requestSignalsMock = mocks.Create<IConnectionRequestSignals>();

        innerConnectionMock.Setup(x => x.Dispose());

        objectManagerMock.Setup(x => x.ClearConnection(It.IsAny<IOpenInnerConnection>()))
            .Callback(() =>
            {
                Assert.False(allowRequestExecution);
                Assert.False(allowAsyncRequestCreation);
            })
            .Returns(false);

        requestSignalsMock.Setup(x => x.AllowAsyncRequestCreation())
            .Callback(() => allowAsyncRequestCreation = true)
            .Verifiable(Times.Once());

        requestSignalsMock.Setup(x => x.AllowRequestExecution())
            .Callback(() => allowRequestExecution = true)
            .Verifiable(Times.Once());

        requestSignalsMock.Setup(x => x.BlockAsyncRequestCreation())
            .Callback(() => allowAsyncRequestCreation = false)
            .Verifiable(Times.Once());

        requestSignalsMock.Setup(x => x.BlockRequestExecution())
            .Callback(() => allowRequestExecution = false)
            .Verifiable(Times.Once());

        signalsMock.Setup(x => x.SetCreateConnectionSignal())
            .Verifiable(Times.Never());

        var requestManager = new ConnectionRequestManager(requestSignals: requestSignalsMock.Object);

        IOpenInnerConnectionOwner connectionManager = new ConnectionManager(
            signalsMock.Object,
            requestManager,
            objectManagerMock.Object,
            connectionAcquisitionHandlerMock.Object);

        connectionManager.OnConnectionClosing(innerConnectionMock.Object);

        Assert.True(allowAsyncRequestCreation);
        Assert.True(allowRequestExecution);

        objectManagerMock.Verify(x => x.ClearConnection(innerConnectionMock.Object), Times.Once());
        innerConnectionMock.Verify(x => x.Dispose(), Times.Once());

        mocks.Verify();
    }


}
