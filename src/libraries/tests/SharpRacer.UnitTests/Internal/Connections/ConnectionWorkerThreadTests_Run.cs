using Microsoft.Extensions.Time.Testing;
using Moq;
using SharpRacer.Interop;

namespace SharpRacer.Internal.Connections;
public class ConnectionWorkerThreadTests_Run
{
    [Fact]
    public void CancellationTakesPrecedenceOverDataReadySignalTest()
    {
        var mocks = new MockRepository(MockBehavior.Strict);

        var ownerMock = mocks.Create<IConnectionWorkerThreadOwner>();
        var dataReadyEventFactoryMock = mocks.Create<IDataReadyEventFactory>();

        var cancellationSource = new CancellationTokenSource();
        var dataReadyEvent = new AutoResetEvent(true);
        var fakeOwner = new FakeConnectionWorkerThreadOwner(ownerMock);

        dataReadyEventFactoryMock.Setup(x => x.CreateAutoResetEvent(It.IsAny<bool>()))
            .Returns(dataReadyEvent)
            .Verifiable(Times.Once());

        ownerMock.Setup(x => x.OnDataReady());

        ownerMock.Setup(x => x.OnWorkerThreadExit(It.IsAny<bool>()));

        cancellationSource.Cancel();

        ConnectionWorkerThread.Run(fakeOwner, dataReadyEventFactoryMock.Object, TimeProvider.System, cancellationSource.Token);

        ownerMock.Verify(x => x.OnDataReady(), Times.Never());
        ownerMock.Verify(x => x.OnWorkerThreadExit(true), Times.Once());

        mocks.Verify();
    }

    [Fact]
    public void CreatesDataReadyEventTest()
    {
        var mocks = new MockRepository(MockBehavior.Strict);

        var ownerMock = mocks.Create<IConnectionWorkerThreadOwner>();
        var dataReadyEventFactoryMock = mocks.Create<IDataReadyEventFactory>();

        var dataReadyEvent = new AutoResetEvent(false);
        var fakeOwner = new FakeConnectionWorkerThreadOwner(ownerMock);

        dataReadyEventFactoryMock.Setup(x => x.CreateAutoResetEvent(It.IsAny<bool>()))
            .Returns(dataReadyEvent)
            .Verifiable(Times.Once());

        ownerMock.Setup(x => x.OnWorkerThreadExit(It.IsAny<bool>()));

        var cancellationSource = new CancellationTokenSource();
        cancellationSource.Cancel();

        ConnectionWorkerThread.Run(fakeOwner, dataReadyEventFactoryMock.Object, TimeProvider.System, cancellationSource.Token);

        ownerMock.Verify(x => x.OnWorkerThreadExit(true), Times.Once());

        mocks.Verify();
    }

    [Fact]
    public void OnDataReadyTest()
    {
        var mocks = new MockRepository(MockBehavior.Strict);

        var ownerMock = mocks.Create<IConnectionWorkerThreadOwner>();
        var dataReadyEventFactoryMock = mocks.Create<IDataReadyEventFactory>();

        var cancellationSource = new CancellationTokenSource();
        var dataReadyEvent = new AutoResetEvent(true);
        var fakeOwner = new FakeConnectionWorkerThreadOwner(ownerMock);

        dataReadyEventFactoryMock.Setup(x => x.CreateAutoResetEvent(It.IsAny<bool>()))
            .Returns(dataReadyEvent)
            .Verifiable(Times.Once());

        ownerMock.Setup(x => x.OnDataReady())
            .Callback(cancellationSource.Cancel);

        ownerMock.Setup(x => x.OnWorkerThreadExit(It.IsAny<bool>()));

        ConnectionWorkerThread.Run(fakeOwner, dataReadyEventFactoryMock.Object, TimeProvider.System, cancellationSource.Token);

        ownerMock.Verify(x => x.OnDataReady(), Times.Once());
        ownerMock.Verify(x => x.OnWorkerThreadExit(true), Times.Once());

        mocks.Verify();
    }

    [Fact]
    public void WaitTimeout_IdleTimeoutElapsedTest()
    {
        var mocks = new MockRepository(MockBehavior.Strict);

        var ownerMock = mocks.Create<IConnectionWorkerThreadOwner>();
        var dataReadyEventFactoryMock = mocks.Create<IDataReadyEventFactory>();

        var dataReadyEvent = new AutoResetEvent(false);
        var idleTimeout = TimeSpan.FromSeconds(5);

        var fakeOwner = new FakeConnectionWorkerThreadOwner(ownerMock);
        fakeOwner.SetSimulatorStatus(1);

        var timeProvider = new FakeTimeProvider(DateTimeOffset.UtcNow);

        dataReadyEventFactoryMock.Setup(x => x.CreateAutoResetEvent(It.IsAny<bool>()))
            .Returns(dataReadyEvent)
            .Verifiable(Times.Once());

        // Avance time after accessing property value to simulate timing out for the second iteration of the loop
        ownerMock.SetupGet(x => x.IdleTimeout)
            .Returns(idleTimeout)
            .Callback(() => timeProvider.Advance(idleTimeout + TimeSpan.FromSeconds(1)));

        ownerMock.Setup(x => x.OnWorkerThreadExit(It.IsAny<bool>()));

        ConnectionWorkerThread.Run(fakeOwner, dataReadyEventFactoryMock.Object, timeProvider, default);

        ownerMock.Verify(x => x.OnWorkerThreadExit(false), Times.Once());

        mocks.Verify();
    }

    [Fact]
    public void WaitTimeout_SimulatorStatusZeroTest()
    {
        var mocks = new MockRepository(MockBehavior.Strict);

        var ownerMock = mocks.Create<IConnectionWorkerThreadOwner>();
        var dataReadyEventFactoryMock = mocks.Create<IDataReadyEventFactory>();

        var dataReadyEvent = new AutoResetEvent(false);
        var fakeOwner = new FakeConnectionWorkerThreadOwner(ownerMock);
        fakeOwner.SetSimulatorStatus(0);

        dataReadyEventFactoryMock.Setup(x => x.CreateAutoResetEvent(It.IsAny<bool>()))
            .Returns(dataReadyEvent)
            .Verifiable(Times.Once());

        ownerMock.Setup(x => x.OnWorkerThreadExit(It.IsAny<bool>()));

        ConnectionWorkerThread.Run(fakeOwner, dataReadyEventFactoryMock.Object, TimeProvider.System, default);

        ownerMock.Verify(x => x.OnWorkerThreadExit(false), Times.Once());

        mocks.Verify();
    }
}
