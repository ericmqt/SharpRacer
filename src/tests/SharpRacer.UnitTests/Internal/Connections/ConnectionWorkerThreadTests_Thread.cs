using Microsoft.Extensions.Time.Testing;
using Moq;
using SharpRacer.Interop;

namespace SharpRacer.Internal.Connections;
public class ConnectionWorkerThreadTests_Thread
{
    [Fact]
    public void DataReadySignalReceivedAfterWaitTimeoutTest()
    {
        var mocks = new MockRepository(MockBehavior.Strict);

        var ownerMock = mocks.Create<IConnectionWorkerThreadOwner>();
        var dataReadyEventFactoryMock = mocks.Create<IDataReadyEventFactory>();

        var dataReadyEvent = new AutoResetEvent(false);
        bool wasDataReadyEventSignaled = false;
        var idleTimeout = TimeSpan.FromSeconds(5);

        var fakeOwner = new FakeConnectionWorkerThreadOwner(ownerMock);
        fakeOwner.SetSimulatorStatus(1);

        var timeProvider = new FakeTimeProvider(DateTimeOffset.UtcNow);

        dataReadyEventFactoryMock.Setup(x => x.CreateAutoResetEvent(It.IsAny<bool>()))
            .Returns(dataReadyEvent)
            .Verifiable(Times.Once());

        // Signal the data-ready event only after the loop has received a wait timeout once, simultaneously advancing time so the loop
        // exits with an idle timeout after receiving the data-ready signal once
        ownerMock.SetupGet(x => x.IdleTimeout)
            .Returns(idleTimeout)
            .Callback(() =>
            {
                if (!wasDataReadyEventSignaled)
                {
                    dataReadyEvent.Set();

                    wasDataReadyEventSignaled = true;
                }

                timeProvider.Advance(idleTimeout + TimeSpan.FromSeconds(1));
            });

        ownerMock.Setup(x => x.OnDataReady());

        ownerMock.Setup(x => x.OnWorkerThreadExit(It.IsAny<bool>()))
            .Callback(() => ownerMock.Verify(x => x.OnDataReady(), Times.Once()));

        var worker = new ConnectionWorkerThread(fakeOwner, dataReadyEventFactoryMock.Object, timeProvider);
        worker.Start();

        AssertWorkerThreadCompleted(worker);

        ownerMock.Verify(x => x.OnDataReady(), Times.Once());
        ownerMock.Verify(x => x.OnWorkerThreadExit(false), Times.Once());

        mocks.Verify();
    }

    [Fact]
    public void DisposeStopsThreadTest()
    {
        var mocks = new MockRepository(MockBehavior.Strict);

        var ownerMock = mocks.Create<IConnectionWorkerThreadOwner>();
        var dataReadyEventFactoryMock = mocks.Create<IDataReadyEventFactory>();

        var dataReadyEvent = new AutoResetEvent(true);
        var idleTimeout = TimeSpan.FromSeconds(5);

        var fakeOwner = new FakeConnectionWorkerThreadOwner(ownerMock);
        fakeOwner.SetSimulatorStatus(1);

        dataReadyEventFactoryMock.Setup(x => x.CreateAutoResetEvent(It.IsAny<bool>()))
           .Returns(dataReadyEvent)
           .Verifiable(Times.Once());

        int onDataReadyInvocationCount = 0;

        ownerMock.Setup(x => x.OnDataReady())
            .Callback(() =>
            {
                Interlocked.Increment(ref onDataReadyInvocationCount);
                dataReadyEvent.Set();
            });

        ownerMock.Setup(x => x.OnWorkerThreadExit(It.IsAny<bool>()));

        var worker = new ConnectionWorkerThread(fakeOwner, dataReadyEventFactoryMock.Object, TimeProvider.System);
        worker.Start();

        SpinWait.SpinUntil(() => onDataReadyInvocationCount > 5);

        worker.Dispose();
        AssertWorkerThreadCompleted(worker);

        ownerMock.Verify(x => x.OnDataReady(), Times.Exactly(onDataReadyInvocationCount));
        ownerMock.Verify(x => x.OnWorkerThreadExit(true), Times.Once());

        mocks.Verify();
    }

    private static void AssertWorkerThreadCompleted(ConnectionWorkerThread worker)
    {
        if ((worker.Thread.ThreadState & ThreadState.Unstarted) != ThreadState.Unstarted)
        {
            Assert.True(worker.Thread.Join(TimeSpan.FromSeconds(5)), "Worker thread did not complete within timeout period");
        }
    }
}
