using Moq;
using SharpRacer.Interop;

namespace SharpRacer.Internal.Connections;
public class ConnectionWorkerThreadTests
{
    [Fact]
    public void Ctor_Test()
    {
        var mocks = new MockRepository(MockBehavior.Strict);

        var ownerMock = mocks.Create<IConnectionWorkerThreadOwner>();
        var dataReadyEventFactoryMock = mocks.Create<IDataReadyEventFactory>();

        var worker = new ConnectionWorkerThread(ownerMock.Object, dataReadyEventFactoryMock.Object, TimeProvider.System);

        Assert.NotNull(worker.Thread);
        Assert.Equal(ConnectionWorkerThread.ThreadName, worker.Thread.Name);
        Assert.NotEqual(ThreadState.Running, worker.Thread.ThreadState);
    }

    [Fact]
    public void Dispose_Test()
    {
        var mocks = new MockRepository(MockBehavior.Strict);

        var ownerMock = mocks.Create<IConnectionWorkerThreadOwner>();
        var dataReadyEventFactoryMock = mocks.Create<IDataReadyEventFactory>();

        var worker = new ConnectionWorkerThread(ownerMock.Object, dataReadyEventFactoryMock.Object, TimeProvider.System);

        worker.Dispose();

        Assert.Throws<ObjectDisposedException>(worker.Start);
        Assert.Throws<ObjectDisposedException>(worker.Stop);
    }

    [Fact]
    public void Stop_Test()
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

        worker.Stop();
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
