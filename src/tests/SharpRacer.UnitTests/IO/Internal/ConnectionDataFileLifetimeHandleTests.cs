using Moq;

namespace SharpRacer.IO.Internal;
public class ConnectionDataFileLifetimeHandleTests
{
    [Fact]
    public void Ctor_Test()
    {
        var mocks = new MockRepository(MockBehavior.Strict);

        var lifetimeMock = mocks.Create<IConnectionDataFileLifetime>();

        var handle = new ConnectionDataFileLifetimeHandle(lifetimeMock.Object);

        Assert.False(handle.IsDisposed);
    }

    [Fact]
    public void Dispose_Test()
    {
        var mocks = new MockRepository(MockBehavior.Strict);

        var lifetimeMock = mocks.Create<IConnectionDataFileLifetime>();

        var handle = new ConnectionDataFileLifetimeHandle(lifetimeMock.Object);

        lifetimeMock.Setup(x => x.ReleaseLifetimeHandle(It.IsAny<IConnectionDataFileLifetimeHandle>()));

        // Dispose the handle
        handle.Dispose();
        Assert.True(handle.IsDisposed);

        lifetimeMock.Verify(x => x.ReleaseLifetimeHandle(handle), Times.Once());
    }

    [Fact]
    public void Release_Test()
    {
        var mocks = new MockRepository(MockBehavior.Strict);

        var lifetimeMock = mocks.Create<IConnectionDataFileLifetime>();

        var handle = new ConnectionDataFileLifetimeHandle(lifetimeMock.Object);

        lifetimeMock.Setup(x => x.ReleaseLifetimeHandle(It.IsAny<IConnectionDataFileLifetimeHandle>()));

        // Release the handle and trigger disposal
        handle.Release();
        Assert.True(handle.IsDisposed);

        lifetimeMock.Verify(x => x.ReleaseLifetimeHandle(handle), Times.Once());
    }
}
