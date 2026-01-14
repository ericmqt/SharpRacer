using Moq;
using SharpRacer.Internal.Connections.Requests;
using SharpRacer.Interop;

namespace SharpRacer.Internal.Connections;
public class ConnectionAcquisitionWorkerTests
{
    [Fact]
    public void Ctor_Test()
    {
        var mocks = new MockRepository(MockBehavior.Strict);

        var worker = new ConnectionAcquisitionWorker(
            mocks.Create<IConnectionAcquisitionHandler>().Object,
            mocks.Create<IOpenInnerConnectionFactory>().Object,
            mocks.Create<IConnectionRequestManager>().Object,
            mocks.Create<IConnectionProvider>().Object,
            mocks.Create<IOpenInnerConnectionOwner>().Object,
            mocks.Create<IDataReadyEventFactory>().Object);

        Assert.NotNull(worker.Thread);
        Assert.True(worker.Thread.IsBackground);
        Assert.Equal(ConnectionAcquisitionWorker.ThreadName, worker.Thread.Name);
        Assert.True(worker.DataReadyWaitTimeout > TimeSpan.Zero);
        Assert.True(worker.DataReadyWaitTimeout != Timeout.InfiniteTimeSpan);

        mocks.Verify();
    }

    [Fact]
    public void Start_Test()
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

        var worker = new ConnectionAcquisitionWorker(
            handlerMock.Object,
            connectionFactoryMock.Object,
            requestManagerMock.Object,
            connectionProviderMock.Object,
            connectionOwnerMock.Object,
            dataReadyEventFactoryMock.Object);

        worker.DataReadyWaitTimeout = dataReadyWaitTimeout;

        worker.Start();

        Assert.True(worker.Thread.Join(TimeSpan.FromSeconds(5)), "Worker thread did not complete within timeout period");

        requestManagerMock.Verify(x => x.HasPendingRequests(), Times.Once());
        handlerMock.Verify(x => x.TryAbort(), Times.Once());

        mocks.Verify();
    }
}
