using Moq;
using SharpRacer.Internal.Connections.Requests;
using SharpRacer.Interop;

namespace SharpRacer.Internal.Connections;
public class ConnectionAcquisitionWorkerTests_Thread
{
    [Theory]
    [MemberData(nameof(GetDataReadyWaitCount))]
    public void AbortAfterPendingRequestsExhaustedTest(int totalDataReadyWaitCount)
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
        int waitCount = 0;

        dataReadyEventFactoryMock.Setup(x => x.CreateAutoResetEvent(It.IsAny<bool>()))
            .Callback(AssertWorkerThread)
            .Returns(dataReadyEvent)
            .Verifiable(Times.Once());

        handlerMock.Setup(x => x.TryAbort())
            .Callback(AssertWorkerThread)
            .Returns(true)
            .Verifiable(Times.Once());

        requestManagerMock.Setup(x => x.HasPendingRequests())
            .Callback(AssertWorkerThread)
            .Returns(() =>
            {
                if (waitCount < totalDataReadyWaitCount)
                {
                    waitCount++;
                    return true;
                }

                return false;
            });

        requestManagerMock.Setup(x => x.ProcessAsyncRequestQueue(It.IsAny<IConnectionProvider>(), It.IsAny<bool>()))
            .Callback(AssertWorkerThread);

        var worker = new ConnectionAcquisitionWorker(
           handlerMock.Object,
           connectionFactoryMock.Object,
           requestManagerMock.Object,
           connectionProviderMock.Object,
           connectionOwnerMock.Object,
           dataReadyEventFactoryMock.Object);

        worker.DataReadyWaitTimeout = dataReadyWaitTimeout;

        worker.Start();

        AssertWorkerThreadCompleted(worker);

        Assert.Equal(waitCount, totalDataReadyWaitCount);

        requestManagerMock.Verify(x => x.ProcessAsyncRequestQueue(connectionProviderMock.Object, false), Times.Exactly(waitCount));

        mocks.Verify();
    }

    [Theory]
    [MemberData(nameof(GetDataReadyWaitCount))]
    public void CreateConnectionTest(int totalDataReadyWaitCount)
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
        var dataReadyEvent = new AutoResetEvent(initialState: false);
        int waitCount = 0;

        dataReadyEventFactoryMock.Setup(x => x.CreateAutoResetEvent(It.IsAny<bool>()))
            .Callback(AssertWorkerThread)
            .Returns(dataReadyEvent)
            .Verifiable(Times.Once());

        connectionFactoryMock.Setup(x => x.Create(It.IsAny<IOpenInnerConnectionOwner>()))
            .Callback(AssertWorkerThread)
            .Returns(connectionMock.Object)
            .Verifiable(Times.Once());

        handlerMock.Setup(x => x.SetConnection(It.IsAny<IConnectionProvider>(), It.IsAny<IOpenInnerConnection>()))
            .Callback(AssertWorkerThread)
            .Verifiable(Times.Once());

        requestManagerMock.Setup(x => x.HasPendingRequests())
            .Callback(AssertWorkerThread)
            .Returns(true)
            .Callback(() =>
            {
                waitCount++;

                if (waitCount == totalDataReadyWaitCount)
                {
                    dataReadyEvent.Set();
                }
            });

        requestManagerMock.Setup(x => x.ProcessAsyncRequestQueue(It.IsAny<IConnectionProvider>(), It.IsAny<bool>()))
            .Callback(AssertWorkerThread);

        var worker = new ConnectionAcquisitionWorker(
           handlerMock.Object,
           connectionFactoryMock.Object,
           requestManagerMock.Object,
           connectionProviderMock.Object,
           connectionOwnerMock.Object,
           dataReadyEventFactoryMock.Object);

        worker.DataReadyWaitTimeout = dataReadyWaitTimeout;

        worker.Start();

        AssertWorkerThreadCompleted(worker);

        Assert.Equal(totalDataReadyWaitCount, waitCount);

        connectionFactoryMock.Verify(x => x.Create(connectionOwnerMock.Object), Times.Once());
        handlerMock.Verify(x => x.SetConnection(connectionProviderMock.Object, connectionMock.Object), Times.Once());
        requestManagerMock.Verify(x => x.HasPendingRequests(), Times.Exactly(totalDataReadyWaitCount));
        requestManagerMock.Verify(x => x.ProcessAsyncRequestQueue(connectionProviderMock.Object, false), Times.Exactly(totalDataReadyWaitCount));

        mocks.Verify();
    }

    [Theory]
    [MemberData(nameof(GetDataReadyWaitCount))]
    public void ConnectionExceptionTest(int totalDataReadyWaitCount)
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
        var exceptionObj = new InvalidOperationException("Test");
        SimulatorConnectionException? connectionException = null;
        int waitCount = 0;

        dataReadyEventFactoryMock.Setup(x => x.CreateAutoResetEvent(It.IsAny<bool>()))
            .Callback(AssertWorkerThread)
            .Returns(dataReadyEvent)
            .Verifiable(Times.Once());

        connectionFactoryMock.Setup(x => x.Create(It.IsAny<IOpenInnerConnectionOwner>()))
            .Callback(AssertWorkerThread)
            .Throws(exceptionObj)
            .Verifiable(Times.Once());

        handlerMock.Setup(x => x.SetConnectionException(It.IsAny<IConnectionProvider>(), It.IsAny<SimulatorConnectionException>()))
            .Callback((IConnectionProvider _, SimulatorConnectionException exception) =>
            {
                AssertWorkerThread();

                connectionException = exception;
            })
            .Verifiable(Times.Once());

        requestManagerMock.Setup(x => x.HasPendingRequests())
            .Callback(AssertWorkerThread)
            .Returns(true)
            .Callback(() =>
            {
                waitCount++;

                if (waitCount == totalDataReadyWaitCount)
                {
                    dataReadyEvent.Set();
                }
            });

        requestManagerMock.Setup(x => x.ProcessAsyncRequestQueue(It.IsAny<IConnectionProvider>(), It.IsAny<bool>()))
            .Callback(AssertWorkerThread);

        var worker = new ConnectionAcquisitionWorker(
           handlerMock.Object,
           connectionFactoryMock.Object,
           requestManagerMock.Object,
           connectionProviderMock.Object,
           connectionOwnerMock.Object,
           dataReadyEventFactoryMock.Object);

        worker.DataReadyWaitTimeout = dataReadyWaitTimeout;

        worker.Start();

        AssertWorkerThreadCompleted(worker);

        Assert.Equal(totalDataReadyWaitCount, waitCount);
        Assert.NotNull(connectionException);
        Assert.NotNull(connectionException.InnerException);
        Assert.Equal(exceptionObj, connectionException.InnerException);

        connectionFactoryMock.Verify(x => x.Create(connectionOwnerMock.Object), Times.Once());
        handlerMock.Verify(x => x.SetConnectionException(connectionProviderMock.Object, connectionException), Times.Once());
        requestManagerMock.Verify(x => x.HasPendingRequests(), Times.Exactly(totalDataReadyWaitCount));
        requestManagerMock.Verify(x => x.ProcessAsyncRequestQueue(connectionProviderMock.Object, false), Times.Exactly(totalDataReadyWaitCount));

        mocks.Verify();
    }

    private static void AssertWorkerThread()
    {
        Assert.True(Thread.CurrentThread.Name == ConnectionAcquisitionWorker.ThreadName,
            "Executing thread is not the worker thread.");
    }

    private static void AssertWorkerThreadCompleted(ConnectionAcquisitionWorker worker)
    {
        Assert.True(worker.Thread.Join(TimeSpan.FromSeconds(5)), "Worker thread did not complete within timeout period");
    }

    public static TheoryData<int> GetDataReadyWaitCount()
    {
        return new TheoryData<int>(1, 3, 17);
    }
}
