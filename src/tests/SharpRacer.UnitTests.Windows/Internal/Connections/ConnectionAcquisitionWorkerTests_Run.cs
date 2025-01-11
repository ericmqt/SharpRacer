using Moq;
using SharpRacer.Internal.Connections.Requests;
using SharpRacer.Interop;

namespace SharpRacer.Internal.Connections;
public class ConnectionAcquisitionWorkerTests_Run
{
    [Fact]
    public void ConnectionExceptionTest()
    {
        var mocks = new MockRepository(MockBehavior.Strict);

        var handlerMock = mocks.Create<IConnectionAcquisitionHandler>();
        var connectionFactoryMock = mocks.Create<IOpenInnerConnectionFactory>();
        var connectionOwnerMock = mocks.Create<IOpenInnerConnectionOwner>();
        var requestManagerMock = mocks.Create<IConnectionRequestManager>();
        var connectionProviderMock = mocks.Create<IConnectionProvider>();
        var dataReadyEventFactoryMock = mocks.Create<IDataReadyEventFactory>();

        var dataReadyWaitTimeout = TimeSpan.Zero;
        var dataReadyEvent = new AutoResetEvent(initialState: true);
        var exceptionObj = new InvalidOperationException("Test");
        SimulatorConnectionException? connectionException = null;

        dataReadyEventFactoryMock.Setup(x => x.CreateAutoResetEvent(It.IsAny<bool>()))
            .Returns(dataReadyEvent)
            .Verifiable(Times.Once());

        connectionFactoryMock.Setup(x => x.Create(It.IsAny<IOpenInnerConnectionOwner>()))
            .Throws(exceptionObj)
            .Verifiable(Times.Once());

        handlerMock.Setup(x => x.SetConnectionException(It.IsAny<IConnectionProvider>(), It.IsAny<SimulatorConnectionException>()))
            .Callback((IConnectionProvider _, SimulatorConnectionException exception) => connectionException = exception)
            .Verifiable(Times.Once());

        ConnectionAcquisitionWorker.Run(
            handlerMock.Object,
            connectionFactoryMock.Object,
            connectionOwnerMock.Object,
            requestManagerMock.Object,
            connectionProviderMock.Object,
            dataReadyEventFactoryMock.Object,
            dataReadyWaitTimeout);

        Assert.NotNull(connectionException);
        Assert.NotNull(connectionException.InnerException);
        Assert.Equal(exceptionObj, connectionException.InnerException);

        connectionFactoryMock.Verify(x => x.Create(connectionOwnerMock.Object), Times.Once());
        handlerMock.Verify(x => x.SetConnectionException(connectionProviderMock.Object, connectionException), Times.Once());

        mocks.Verify();
    }

    [Fact]
    public void CreateConnectionTest()
    {
        var mocks = new MockRepository(MockBehavior.Strict);

        var handlerMock = mocks.Create<IConnectionAcquisitionHandler>();
        var connectionFactoryMock = mocks.Create<IOpenInnerConnectionFactory>();
        var connectionOwnerMock = mocks.Create<IOpenInnerConnectionOwner>();
        var requestManagerMock = mocks.Create<IConnectionRequestManager>();
        var connectionProviderMock = mocks.Create<IConnectionProvider>();
        var dataReadyEventFactoryMock = mocks.Create<IDataReadyEventFactory>();

        var connectionMock = mocks.Create<IOpenInnerConnection>();

        var dataReadyWaitTimeout = TimeSpan.Zero;
        var dataReadyEvent = new AutoResetEvent(initialState: true);

        dataReadyEventFactoryMock.Setup(x => x.CreateAutoResetEvent(It.IsAny<bool>()))
            .Returns(dataReadyEvent)
            .Verifiable(Times.Once());

        connectionFactoryMock.Setup(x => x.Create(It.IsAny<IOpenInnerConnectionOwner>()))
            .Returns(connectionMock.Object)
            .Verifiable(Times.Once());

        handlerMock.Setup(x => x.SetConnection(It.IsAny<IConnectionProvider>(), It.IsAny<IOpenInnerConnection>()))
            .Verifiable(Times.Once());

        ConnectionAcquisitionWorker.Run(
            handlerMock.Object,
            connectionFactoryMock.Object,
            connectionOwnerMock.Object,
            requestManagerMock.Object,
            connectionProviderMock.Object,
            dataReadyEventFactoryMock.Object,
            dataReadyWaitTimeout);

        connectionFactoryMock.Verify(x => x.Create(connectionOwnerMock.Object), Times.Once());
        handlerMock.Verify(x => x.SetConnection(connectionProviderMock.Object, connectionMock.Object), Times.Once());

        mocks.Verify();
    }

    [Fact]
    public void TryAbortTest()
    {
        var mocks = new MockRepository(MockBehavior.Strict);

        var handlerMock = mocks.Create<IConnectionAcquisitionHandler>();
        var connectionFactoryMock = mocks.Create<IOpenInnerConnectionFactory>();
        var connectionOwnerMock = mocks.Create<IOpenInnerConnectionOwner>();
        var requestManagerMock = mocks.Create<IConnectionRequestManager>();
        var connectionProviderMock = mocks.Create<IConnectionProvider>();
        var dataReadyEventFactoryMock = mocks.Create<IDataReadyEventFactory>();

        var dataReadyWaitTimeout = TimeSpan.Zero;
        var dataReadyEvent = new AutoResetEvent(initialState: false);

        dataReadyEventFactoryMock.Setup(x => x.CreateAutoResetEvent(It.IsAny<bool>()))
            .Returns(dataReadyEvent)
            .Verifiable(Times.Once());

        requestManagerMock.Setup(x => x.HasPendingRequests()).Returns(false);
        handlerMock.Setup(x => x.TryAbort()).Returns(true);

        ConnectionAcquisitionWorker.Run(
            handlerMock.Object,
            connectionFactoryMock.Object,
            connectionOwnerMock.Object,
            requestManagerMock.Object,
            connectionProviderMock.Object,
            dataReadyEventFactoryMock.Object,
            dataReadyWaitTimeout);

        requestManagerMock.Verify(x => x.HasPendingRequests(), Times.Once());
        handlerMock.Verify(x => x.TryAbort(), Times.Once());

        mocks.Verify();
    }

    [Fact]
    public void TryAbortReturnsFalseTest()
    {
        var mocks = new MockRepository(MockBehavior.Strict);

        var handlerMock = mocks.Create<IConnectionAcquisitionHandler>();
        var connectionFactoryMock = mocks.Create<IOpenInnerConnectionFactory>();
        var connectionOwnerMock = mocks.Create<IOpenInnerConnectionOwner>();
        var requestManagerMock = mocks.Create<IConnectionRequestManager>();
        var connectionProviderMock = mocks.Create<IConnectionProvider>();
        var dataReadyEventFactoryMock = mocks.Create<IDataReadyEventFactory>();

        var dataReadyWaitTimeout = TimeSpan.Zero;
        var dataReadyEvent = new AutoResetEvent(initialState: false);
        var tryAbortResult = false;

        dataReadyEventFactoryMock.Setup(x => x.CreateAutoResetEvent(It.IsAny<bool>()))
            .Returns(dataReadyEvent)
            .Verifiable(Times.Once());

        requestManagerMock.Setup(x => x.HasPendingRequests())
            .Returns(false);

        requestManagerMock.Setup(x => x.ProcessAsyncRequestQueue(It.IsAny<IConnectionProvider>(), It.IsAny<bool>()));

        handlerMock.Setup(x => x.TryAbort())
            .Returns(() => tryAbortResult)
            .Callback(() => tryAbortResult = true);

        ConnectionAcquisitionWorker.Run(
            handlerMock.Object,
            connectionFactoryMock.Object,
            connectionOwnerMock.Object,
            requestManagerMock.Object,
            connectionProviderMock.Object,
            dataReadyEventFactoryMock.Object,
            dataReadyWaitTimeout);

        requestManagerMock.Verify(x => x.HasPendingRequests(), Times.Exactly(2));
        handlerMock.Verify(x => x.TryAbort(), Times.Exactly(2));

        mocks.Verify();
    }
}
