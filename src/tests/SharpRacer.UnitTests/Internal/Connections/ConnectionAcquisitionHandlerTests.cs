using Moq;
using SharpRacer.Internal.Connections.Requests;

namespace SharpRacer.Internal.Connections;
public class ConnectionAcquisitionHandlerTests
{
    [Fact]
    public void SetConnection_Test()
    {
        var mocks = new MockRepository(MockBehavior.Strict);

        var connectionProviderMock = mocks.Create<IConnectionProvider>();
        var innerConnectionMock = mocks.Create<IOpenInnerConnection>();
        var objectManagerMock = mocks.Create<IConnectionObjectManager>();
        var requestManagerMock = mocks.Create<IConnectionRequestManager>();
        var signalsMock = mocks.Create<IConnectionSignals>();

        var isConnectionSet = false;
        var isRequestQueueProcessed = false;
        var isConnectionWorkerThreadStarted = false;

        objectManagerMock.Setup(x => x.SetConnection(It.IsAny<IOpenInnerConnection>()))
            .Callback(() =>
            {
                isConnectionSet = true;

                Assert.False(isRequestQueueProcessed);
                Assert.False(isConnectionWorkerThreadStarted);
            });

        requestManagerMock.Setup(x => x.ProcessAsyncRequestQueue(It.IsAny<IConnectionProvider>(), It.IsAny<bool>()))
            .Callback(() =>
            {
                isRequestQueueProcessed = true;

                Assert.True(isConnectionSet);
                Assert.False(isConnectionWorkerThreadStarted);
            });

        innerConnectionMock.Setup(x => x.StartWorkerThread())
            .Callback(() =>
            {
                isConnectionWorkerThreadStarted = true;

                Assert.True(isConnectionSet);
                Assert.True(isRequestQueueProcessed);
            })
            .Verifiable(Times.Once());

        var handler = new ConnectionAcquisitionHandler(
            objectManagerMock.Object,
            requestManagerMock.Object,
            signalsMock.Object);

        handler.SetConnection(connectionProviderMock.Object, innerConnectionMock.Object);

        objectManagerMock.Verify(x => x.SetConnection(innerConnectionMock.Object), Times.Once());
        requestManagerMock.Verify(x => x.ProcessAsyncRequestQueue(connectionProviderMock.Object, true), Times.Once());

        mocks.Verify();
    }

    [Fact]
    public void SetConnectionExceptionTest()
    {
        var mocks = new MockRepository(MockBehavior.Strict);

        var asyncRequestQueueMock = mocks.Create<IAsyncConnectionRequestQueue>();
        var connectionProviderMock = mocks.Create<IConnectionProvider>();
        var objectManagerMock = mocks.Create<IConnectionObjectManager>();
        var signalsMock = mocks.Create<IConnectionSignals>();

        var isConnectionExceptionSignalSet = false;
        var isCreateConnectionSignalSet = false;

        var exceptionObj = new SimulatorConnectionException("Test");
        SimulatorConnectionException? connectionException = null;

        asyncRequestQueueMock.Setup(x => x.ProcessQueue(It.IsAny<IConnectionProvider>(), It.IsAny<bool>()))
            .Callback(() =>
            {
                Assert.True(isConnectionExceptionSignalSet);
                Assert.False(isCreateConnectionSignalSet);
            })
            .Returns(true);

        objectManagerMock.Setup(x => x.ClearConnectionException())
            .Callback(() =>
            {
                Assert.True(isConnectionExceptionSignalSet);
                Assert.False(isCreateConnectionSignalSet);

                isConnectionExceptionSignalSet = false;
            });

        objectManagerMock.Setup(x => x.SetConnectionException(It.IsAny<SimulatorConnectionException>()))
            .Callback((SimulatorConnectionException pException) =>
            {
                connectionException = pException;

                Assert.False(isConnectionExceptionSignalSet);
                Assert.False(isCreateConnectionSignalSet);

                isConnectionExceptionSignalSet = true;
            });

        signalsMock.Setup(x => x.SetCreateConnectionSignal())
            .Callback(() =>
            {
                Assert.False(isConnectionExceptionSignalSet);
                Assert.False(isCreateConnectionSignalSet);

                isCreateConnectionSignalSet = true;
            });

        var handler = new ConnectionAcquisitionHandler(
            objectManagerMock.Object,
            new ConnectionRequestManager(asyncRequestQueue: asyncRequestQueueMock.Object),
            signalsMock.Object);

        handler.SetConnectionException(connectionProviderMock.Object, exceptionObj);

        Assert.True(isCreateConnectionSignalSet);
        Assert.False(isConnectionExceptionSignalSet);

        Assert.NotNull(connectionException);
        Assert.Equal(exceptionObj, connectionException);

        asyncRequestQueueMock.Verify(x => x.ProcessQueue(connectionProviderMock.Object, true), Times.Once());
        objectManagerMock.Verify(x => x.ClearConnectionException(), Times.Once());
        objectManagerMock.Verify(x => x.SetConnectionException(exceptionObj), Times.Once());

        mocks.Verify();
    }

    [Fact]
    public void SetConnectionException_OpensRequestBlockingScopeTest()
    {
        var mocks = new MockRepository(MockBehavior.Strict);

        var connectionProviderMock = mocks.Create<IConnectionProvider>();
        var objectManagerMock = mocks.Create<IConnectionObjectManager>();
        var requestSignalsMock = mocks.Create<IConnectionRequestSignals>();
        var signalsMock = mocks.Create<IConnectionSignals>();

        var allowAsyncRequestCreation = true;
        var allowRequestExecution = true;
        var exceptionObj = new SimulatorConnectionException("Test");
        SimulatorConnectionException? connectionException = null;

        objectManagerMock.Setup(x => x.ClearConnectionException());

        objectManagerMock.Setup(x => x.SetConnectionException(It.IsAny<SimulatorConnectionException>()))
            .Callback((SimulatorConnectionException pException) => connectionException = pException);

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
            .Callback(() =>
            {
                Assert.False(allowAsyncRequestCreation);
                Assert.False(allowRequestExecution);
            })
            .Verifiable(Times.Once());

        var requestManager = new ConnectionRequestManager(requestSignals: requestSignalsMock.Object);
        var handler = new ConnectionAcquisitionHandler(objectManagerMock.Object, requestManager, signalsMock.Object);

        handler.SetConnectionException(connectionProviderMock.Object, exceptionObj);

        Assert.True(allowAsyncRequestCreation);
        Assert.True(allowRequestExecution);

        Assert.NotNull(connectionException);
        Assert.Equal(exceptionObj, connectionException);

        objectManagerMock.Verify(x => x.SetConnectionException(exceptionObj), Times.Once());

        mocks.Verify();
    }

    [Fact]
    public void SetConnectionException_WaitUntilPendingRequestsClearedTest()
    {
        var mocks = new MockRepository(MockBehavior.Strict);

        var connectionProviderMock = mocks.Create<IConnectionProvider>();
        var objectManagerMock = mocks.Create<IConnectionObjectManager>();
        var requestCounterMock = mocks.Create<IPendingRequestCounter>();
        var requestSignalsMock = mocks.Create<IConnectionRequestSignals>();
        var signalsMock = mocks.Create<IConnectionSignals>();

        var allowAsyncRequestCreation = true;
        var allowRequestExecution = true;
        var hasPendingRequests = true;
        int pendingRequestMaxChecks = 2;
        int pendingRequestCheckCount = 0;
        var exceptionObj = new SimulatorConnectionException("Test");

        objectManagerMock.Setup(x => x.ClearConnectionException());

        objectManagerMock.Setup(x => x.SetConnectionException(It.IsAny<SimulatorConnectionException>()));

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

        requestCounterMock.Setup(x => x.HasPendingRequests())
            .Returns(getHasPendingRequests);

        signalsMock.Setup(x => x.SetCreateConnectionSignal())
            .Callback(() =>
            {
                Assert.False(allowAsyncRequestCreation);
                Assert.False(allowRequestExecution);
            })
            .Verifiable(Times.Once());

        var requestManager = new ConnectionRequestManager(
            requestSignals: requestSignalsMock.Object,
            requestCounter: requestCounterMock.Object);

        var handler = new ConnectionAcquisitionHandler(objectManagerMock.Object, requestManager, signalsMock.Object);

        handler.SetConnectionException(connectionProviderMock.Object, exceptionObj);

        requestCounterMock.Verify(x => x.HasPendingRequests(), Times.Exactly(pendingRequestMaxChecks));

        bool getHasPendingRequests()
        {
            if (hasPendingRequests)
            {
                pendingRequestCheckCount++;
            }

            if (pendingRequestCheckCount >= pendingRequestMaxChecks)
            {
                hasPendingRequests = false;
            }

            return hasPendingRequests;
        }
    }

    [Fact]
    public void TryAbort_UsesRequestBlockingScopeTest()
    {
        var allowAsyncRequestCreation = true;
        var allowRequestExecution = true;

        var mocks = new MockRepository(MockBehavior.Strict);

        var objectManagerMock = mocks.Create<IConnectionObjectManager>();
        var signalsMock = mocks.Create<IConnectionSignals>();

        var requestSignalsMock = mocks.Create<IConnectionRequestSignals>();
        var requestFactoryMock = mocks.Create<IConnectionRequestFactory>();
        var requestCounterMock = mocks.Create<IPendingRequestCounter>();
        var requestQueueMock = mocks.Create<IAsyncConnectionRequestQueue>();

        requestCounterMock.Setup(x => x.HasPendingRequests()).Returns(true);

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

        var requestManager = new ConnectionRequestManager(
            requestFactoryMock.Object, requestQueueMock.Object, requestSignalsMock.Object, requestCounterMock.Object);

        var handler = new ConnectionAcquisitionHandler(
            objectManagerMock.Object,
            requestManager,
            signalsMock.Object);

        Assert.False(handler.TryAbort());
        Assert.True(allowRequestExecution);
        Assert.True(allowAsyncRequestCreation);

        mocks.Verify();
    }

    [Fact]
    public void TryAbort_HasPendingRequestsTest()
    {
        var mocks = new MockRepository(MockBehavior.Strict);

        var objectManagerMock = mocks.Create<IConnectionObjectManager>();
        var signalsMock = mocks.Create<IConnectionSignals>();

        var requestSignalsMock = mocks.Create<IConnectionRequestSignals>();
        var requestFactoryMock = mocks.Create<IConnectionRequestFactory>();
        var requestCounterMock = mocks.Create<IPendingRequestCounter>(MockBehavior.Loose);
        var requestQueueMock = mocks.Create<IAsyncConnectionRequestQueue>();

        requestCounterMock.Setup(x => x.HasPendingRequests()).Returns(true);

        requestSignalsMock.Setup(x => x.AllowAsyncRequestCreation())
            .Verifiable(Times.Once());

        requestSignalsMock.Setup(x => x.AllowRequestExecution())
            .Verifiable(Times.Once());

        requestSignalsMock.Setup(x => x.BlockAsyncRequestCreation())
            .Verifiable(Times.Once());

        requestSignalsMock.Setup(x => x.BlockRequestExecution())
            .Verifiable(Times.Once());

        var requestManager = new ConnectionRequestManager(
            requestFactoryMock.Object, requestQueueMock.Object, requestSignalsMock.Object, requestCounterMock.Object);

        var handler = new ConnectionAcquisitionHandler(
            objectManagerMock.Object,
            requestManager,
            signalsMock.Object);

        Assert.False(handler.TryAbort());

        mocks.Verify();
    }

    [Fact]
    public void TryAbort_SetCreateConnectionSignalOnNoPendingRequestsTest()
    {
        var mocks = new MockRepository(MockBehavior.Strict);

        var objectManagerMock = mocks.Create<IConnectionObjectManager>();
        var signalsMock = mocks.Create<IConnectionSignals>();

        var requestSignalsMock = mocks.Create<IConnectionRequestSignals>();
        var requestFactoryMock = mocks.Create<IConnectionRequestFactory>();
        var requestCounterMock = mocks.Create<IPendingRequestCounter>(MockBehavior.Loose);
        var requestQueueMock = mocks.Create<IAsyncConnectionRequestQueue>();

        signalsMock.Setup(x => x.SetCreateConnectionSignal()).Verifiable(Times.Once());

        requestCounterMock.Setup(x => x.HasPendingRequests()).Returns(false);

        requestSignalsMock.Setup(x => x.AllowAsyncRequestCreation())
            .Verifiable(Times.Once());

        requestSignalsMock.Setup(x => x.AllowRequestExecution())
            .Verifiable(Times.Once());

        requestSignalsMock.Setup(x => x.BlockAsyncRequestCreation())
            .Verifiable(Times.Once());

        requestSignalsMock.Setup(x => x.BlockRequestExecution())
            .Verifiable(Times.Once());

        var requestManager = new ConnectionRequestManager(
            requestFactoryMock.Object, requestQueueMock.Object, requestSignalsMock.Object, requestCounterMock.Object);

        var handler = new ConnectionAcquisitionHandler(
            objectManagerMock.Object,
            requestManager,
            signalsMock.Object);

        Assert.True(handler.TryAbort());

        mocks.Verify();
    }
}
