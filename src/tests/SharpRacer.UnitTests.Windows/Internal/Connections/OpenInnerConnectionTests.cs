using Moq;
using SharpRacer.IO.Internal;

namespace SharpRacer.Internal.Connections;
public class OpenInnerConnectionTests
{
    public static int ConnectionId { get; } = 123;
    public static Memory<byte> DataFileMemory { get; } = new Memory<byte>([0xDE, 0xAD, 0xBE, 0xEF]);

    [Fact]
    public void Ctor_Test()
    {
        var mocks = new OpenInnerConnectionMocks(MockBehavior.Strict);

        mocks.ConnectionOwner.Setup(x => x.NewConnectionId()).Returns(ConnectionId);
        mocks.DataFile.SetupGet(x => x.Memory).Returns(DataFileMemory);

        var openInnerConnection = new OpenInnerConnection(
            mocks.ConnectionOwner.Object,
            mocks.DataFile.Object,
            new OuterConnectionTracker(closeOnEmpty: true),
            mocks.ClosedConnectionFactory.Object,
            mocks.WorkerThreadFactory.Object,
            mocks.TimeProvider);

        Assert.NotNull(openInnerConnection);
        Assert.Equal(ConnectionId, openInnerConnection.ConnectionId);
        Assert.Equal(mocks.DataFile.Object, openInnerConnection.DataFile);
        Assert.True(openInnerConnection.DataFile.Memory.Span.SequenceEqual(DataFileMemory.Span));
        Assert.Equal(SimulatorConnectionState.Open, openInnerConnection.State);

        mocks.ConnectionOwner.Verify(x => x.NewConnectionId(), Times.Once);
        mocks.ClosedConnectionFactory.Verify(x => x.CreateClosedInnerConnection(openInnerConnection), Times.Once);
    }

    [Fact]
    public void AcquireDataHandle_Test()
    {
        var mocks = new OpenInnerConnectionMocks(MockBehavior.Strict);

        mocks.ConnectionOwner.Setup(x => x.NewConnectionId()).Returns(ConnectionId);
        mocks.ConnectionOwner.Setup(x => x.OnConnectionClosing(It.IsAny<IOpenInnerConnection>()));
        mocks.DataFile.SetupGet(x => x.Memory).Returns(DataFileMemory);

        var dataFileMemoryPoolMock = mocks.Create<IConnectionDataMemoryOwner>();

        mocks.DataFile.Setup(x => x.GetMemoryHandle())
            .Returns(new ConnectionDataHandle(DataFileMemory, dataFileMemoryPoolMock.Object))
            .Verifiable(Times.Once);

        mocks.OuterConnectionTracker.SetupGet(x => x.CloseOnEmpty).Returns(true);

        var openInnerConnection = new OpenInnerConnection(
            mocks.ConnectionOwner.Object,
            mocks.DataFile.Object,
            mocks.OuterConnectionTracker.Object,
            mocks.ClosedConnectionFactory.Object,
            mocks.WorkerThreadFactory.Object,
            mocks.TimeProvider);

        var memoryOwner = openInnerConnection.AcquireDataHandle();

        Assert.NotNull(memoryOwner);
        Assert.True(memoryOwner.Memory.Span.SequenceEqual(DataFileMemory.Span));

        mocks.Verify();
    }

    [Fact]
    public void Attach_Test()
    {
        var mocks = new OpenInnerConnectionMocks(MockBehavior.Strict);

        mocks.ConnectionOwner.Setup(x => x.NewConnectionId()).Returns(ConnectionId);
        mocks.DataFile.SetupGet(x => x.Memory).Returns(DataFileMemory);

        var openInnerConnection = new OpenInnerConnection(
            mocks.ConnectionOwner.Object,
            mocks.DataFile.Object,
            new OuterConnectionTracker(closeOnEmpty: true),
            mocks.ClosedConnectionFactory.Object,
            mocks.WorkerThreadFactory.Object,
            mocks.TimeProvider);

        var outerConnectionMock = mocks.Create<IOuterConnection>();
        outerConnectionMock.Setup(x => x.SetOpenInnerConnection(It.IsAny<IOpenInnerConnection>()));

        Assert.True(openInnerConnection.Attach(outerConnectionMock.Object));

        outerConnectionMock.Verify(x => x.SetOpenInnerConnection(openInnerConnection), Times.Once);
    }

    [Fact]
    public void Attach_ReturnsFalseIfDisposedTest()
    {
        var mocks = new OpenInnerConnectionMocks(MockBehavior.Strict);

        mocks.ConnectionOwner.Setup(x => x.NewConnectionId()).Returns(ConnectionId);
        mocks.ConnectionOwner.Setup(x => x.OnConnectionClosing(It.IsAny<IOpenInnerConnection>()));
        mocks.DataFile.SetupGet(x => x.Memory).Returns(DataFileMemory);

        mocks.ClosedInnerConnection.Setup(x => x.Close());
        mocks.DataFile.Setup(x => x.Close());
        mocks.WorkerThread.Setup(x => x.Dispose());

        var openInnerConnection = new OpenInnerConnection(
            mocks.ConnectionOwner.Object,
            mocks.DataFile.Object,
            new OuterConnectionTracker(closeOnEmpty: true),
            mocks.ClosedConnectionFactory.Object,
            mocks.WorkerThreadFactory.Object,
            mocks.TimeProvider);

        openInnerConnection.Dispose();

        var outerConnectionMock = mocks.Create<IOuterConnection>();
        Assert.False(openInnerConnection.Attach(outerConnectionMock.Object));

        outerConnectionMock.Verify(x => x.SetOpenInnerConnection(It.IsAny<IOpenInnerConnection>()), Times.Never);
    }

    [Fact]
    public void Attach_ReturnsFalseIfConnectionTrackerIsClosedTest()
    {
        var mocks = new OpenInnerConnectionMocks(MockBehavior.Strict);

        mocks.ConnectionOwner.Setup(x => x.NewConnectionId()).Returns(ConnectionId);
        mocks.DataFile.SetupGet(x => x.Memory).Returns(DataFileMemory);
        mocks.OuterConnectionTracker.Setup(x => x.Attach(It.IsAny<IOuterConnection>())).Returns(false);

        var outerConnectionMock = mocks.Create<IOuterConnection>();

        var openInnerConnection = new OpenInnerConnection(
            mocks.ConnectionOwner.Object,
            mocks.DataFile.Object,
            mocks.OuterConnectionTracker.Object,
            mocks.ClosedConnectionFactory.Object,
            mocks.WorkerThreadFactory.Object,
            mocks.TimeProvider);

        Assert.False(openInnerConnection.Attach(outerConnectionMock.Object));

        mocks.OuterConnectionTracker.Verify(x => x.Attach(outerConnectionMock.Object), Times.Once());
        outerConnectionMock.Verify(x => x.SetOpenInnerConnection(It.IsAny<IOpenInnerConnection>()), Times.Never);
    }

    [Fact]
    public void Close_Test()
    {
        var mocks = new OpenInnerConnectionMocks(MockBehavior.Strict);

        mocks.ConnectionOwner.Setup(x => x.NewConnectionId()).Returns(ConnectionId);
        mocks.ConnectionOwner.Setup(x => x.OnConnectionClosing(It.IsAny<IOpenInnerConnection>()));
        mocks.DataFile.SetupGet(x => x.Memory).Returns(DataFileMemory);

        var openInnerConnection = new OpenInnerConnection(
            mocks.ConnectionOwner.Object,
            mocks.DataFile.Object,
            mocks.OuterConnectionTracker.Object,
            mocks.ClosedConnectionFactory.Object,
            mocks.WorkerThreadFactory.Object,
            mocks.TimeProvider);

        // Close multiple times to ensure we're only telling the owner the connection is closing exactly one time
        openInnerConnection.Close();
        openInnerConnection.Close();
        openInnerConnection.Close();

        mocks.ConnectionOwner.Verify(x => x.OnConnectionClosing(openInnerConnection), Times.Once);
    }

    [Fact]
    public void CloseOuterConnection_Test()
    {
        var mocks = new OpenInnerConnectionMocks(MockBehavior.Strict);

        mocks.ConnectionOwner.Setup(x => x.NewConnectionId()).Returns(ConnectionId);
        mocks.ConnectionOwner.Setup(x => x.OnConnectionClosing(It.IsAny<IOpenInnerConnection>()));
        mocks.DataFile.SetupGet(x => x.Memory).Returns(DataFileMemory);

        // Detach() => true
        mocks.OuterConnectionTracker.Setup(x => x.Detach(It.IsAny<IOuterConnection>())).Returns(true);
        mocks.OuterConnectionTracker.SetupGet(x => x.IsClosed).Returns(false);

        var outerConnectionMock = mocks.Create<IOuterConnection>();
        outerConnectionMock.Setup(x => x.SetClosedInnerConnection(It.IsAny<IInnerConnection>()));

        var openInnerConnection = new OpenInnerConnection(
            mocks.ConnectionOwner.Object,
            mocks.DataFile.Object,
            mocks.OuterConnectionTracker.Object,
            mocks.ClosedConnectionFactory.Object,
            mocks.WorkerThreadFactory.Object,
            mocks.TimeProvider);

        openInnerConnection.CloseOuterConnection(outerConnectionMock.Object);

        outerConnectionMock.Verify(x => x.SetClosedInnerConnection(mocks.ClosedInnerConnection.Object), Times.Once);
        mocks.OuterConnectionTracker.VerifyGet(x => x.IsClosed, Times.Once);
        mocks.ConnectionOwner.Verify(x => x.OnConnectionClosing(openInnerConnection), Times.Never);
    }

    [Fact]
    public void CloseOuterConnection_ClosesConnectionIfLastTrackedOuterConnectionTest()
    {
        var mocks = new OpenInnerConnectionMocks(MockBehavior.Strict);

        mocks.ConnectionOwner.Setup(x => x.NewConnectionId()).Returns(ConnectionId);
        mocks.ConnectionOwner.Setup(x => x.OnConnectionClosing(It.IsAny<IOpenInnerConnection>()));
        mocks.DataFile.SetupGet(x => x.Memory).Returns(DataFileMemory);

        // Detach() => true
        mocks.OuterConnectionTracker.Setup(x => x.Detach(It.IsAny<IOuterConnection>())).Returns(true);
        mocks.OuterConnectionTracker.SetupGet(x => x.IsClosed).Returns(true);

        var outerConnectionMock = mocks.Create<IOuterConnection>();
        outerConnectionMock.Setup(x => x.SetClosedInnerConnection(It.IsAny<IInnerConnection>()));

        var openInnerConnection = new OpenInnerConnection(
            mocks.ConnectionOwner.Object,
            mocks.DataFile.Object,
            mocks.OuterConnectionTracker.Object,
            mocks.ClosedConnectionFactory.Object,
            mocks.WorkerThreadFactory.Object,
            mocks.TimeProvider);

        openInnerConnection.CloseOuterConnection(outerConnectionMock.Object);

        outerConnectionMock.Verify(x => x.SetClosedInnerConnection(mocks.ClosedInnerConnection.Object), Times.Once);
        mocks.OuterConnectionTracker.VerifyGet(x => x.IsClosed, Times.Once);
        mocks.ConnectionOwner.Verify(x => x.OnConnectionClosing(openInnerConnection), Times.Once);
    }

    [Fact]
    public void CloseOuterConnection_DoNothingIfOuterConnectionNotOwnedTest()
    {
        var mocks = new OpenInnerConnectionMocks(MockBehavior.Strict);

        mocks.ConnectionOwner.Setup(x => x.NewConnectionId()).Returns(ConnectionId);
        mocks.ConnectionOwner.Setup(x => x.OnConnectionClosing(It.IsAny<IOpenInnerConnection>()));
        mocks.DataFile.SetupGet(x => x.Memory).Returns(DataFileMemory);

        // Detach() => false
        mocks.OuterConnectionTracker.Setup(x => x.Detach(It.IsAny<IOuterConnection>())).Returns(false);
        mocks.OuterConnectionTracker.SetupGet(x => x.IsClosed).Returns(true);

        var outerConnectionMock = mocks.Create<IOuterConnection>();
        outerConnectionMock.Setup(x => x.SetClosedInnerConnection(It.IsAny<IInnerConnection>()));

        var openInnerConnection = new OpenInnerConnection(
            mocks.ConnectionOwner.Object,
            mocks.DataFile.Object,
            mocks.OuterConnectionTracker.Object,
            mocks.ClosedConnectionFactory.Object,
            mocks.WorkerThreadFactory.Object,
            mocks.TimeProvider);

        openInnerConnection.CloseOuterConnection(outerConnectionMock.Object);

        outerConnectionMock.Verify(x => x.SetClosedInnerConnection(It.IsAny<IInnerConnection>()), Times.Never);
        mocks.ConnectionOwner.Verify(x => x.OnConnectionClosing(openInnerConnection), Times.Never);
        mocks.OuterConnectionTracker.VerifyGet(x => x.IsClosed, Times.Never);
    }

    [Fact]
    public void Detach_ClosesConnectionOnLastTrackedConnectionDetachedTest()
    {
        var mocks = new OpenInnerConnectionMocks(MockBehavior.Strict);

        mocks.ConnectionOwner.Setup(x => x.NewConnectionId()).Returns(ConnectionId);
        mocks.ConnectionOwner.Setup(x => x.OnConnectionClosing(It.IsAny<IOpenInnerConnection>()));
        mocks.DataFile.SetupGet(x => x.Memory).Returns(DataFileMemory);

        // Detach() => true
        mocks.OuterConnectionTracker.Setup(x => x.Detach(It.IsAny<IOuterConnection>())).Returns(true);
        mocks.OuterConnectionTracker.SetupGet(x => x.IsClosed)
            .Returns(true)
            .Verifiable(Times.Once);

        var outerConnectionMock = mocks.Create<IOuterConnection>();

        var openInnerConnection = new OpenInnerConnection(
            mocks.ConnectionOwner.Object,
            mocks.DataFile.Object,
            mocks.OuterConnectionTracker.Object,
            mocks.ClosedConnectionFactory.Object,
            mocks.WorkerThreadFactory.Object,
            mocks.TimeProvider);

        openInnerConnection.Detach(outerConnectionMock.Object);

        mocks.ConnectionOwner.Verify(x => x.OnConnectionClosing(openInnerConnection), Times.Once);
    }

    [Fact]
    public void Detach_DoesNotInvokeCloseIfOuterConnectionNotOwnedTest()
    {
        var mocks = new OpenInnerConnectionMocks(MockBehavior.Strict);

        mocks.ConnectionOwner.Setup(x => x.NewConnectionId()).Returns(ConnectionId);

        mocks.ConnectionOwner.Setup(x => x.OnConnectionClosing(It.IsAny<IOpenInnerConnection>()))
            .Verifiable(Times.Never);

        mocks.DataFile.SetupGet(x => x.Memory).Returns(DataFileMemory);

        // Detach() => false
        mocks.OuterConnectionTracker.Setup(x => x.Detach(It.IsAny<IOuterConnection>())).Returns(false);

        mocks.OuterConnectionTracker.SetupGet(x => x.IsClosed)
            .Returns(true)
            .Verifiable(Times.Never);

        var outerConnectionMock = mocks.Create<IOuterConnection>();

        var openInnerConnection = new OpenInnerConnection(
            mocks.ConnectionOwner.Object,
            mocks.DataFile.Object,
            mocks.OuterConnectionTracker.Object,
            mocks.ClosedConnectionFactory.Object,
            mocks.WorkerThreadFactory.Object,
            mocks.TimeProvider);

        openInnerConnection.Detach(outerConnectionMock.Object);

        mocks.Verify();
    }

    [Fact]
    public void Dispose_Test()
    {
        var mocks = new OpenInnerConnectionMocks(MockBehavior.Strict);

        mocks.ConnectionOwner.Setup(x => x.NewConnectionId()).Returns(ConnectionId);
        mocks.DataFile.SetupGet(x => x.Memory).Returns(DataFileMemory);

        mocks.ClosedInnerConnection.Setup(x => x.Close()).Verifiable(Times.Once());
        mocks.DataFile.Setup(x => x.Close()).Verifiable(Times.Once());
        mocks.WorkerThread.Setup(x => x.Dispose()).Verifiable(Times.Once());

        // Outer connection to ensure transitioned to close on dispose
        var outerConnection1Mock = mocks.Create<IOuterConnection>();
        var outerConnection2Mock = mocks.Create<IOuterConnection>();

        outerConnection1Mock.Setup(x => x.SetOpenInnerConnection(It.IsAny<IOpenInnerConnection>()));

        // Throw when closing the first outer connection to ensure exception is swallowed and remaining outers are set closed
        outerConnection1Mock.Setup(x => x.SetClosedInnerConnection(It.IsAny<IClosedInnerConnection>()))
            .Throws(new InvalidOperationException());

        outerConnection2Mock.Setup(x => x.SetOpenInnerConnection(It.IsAny<IOpenInnerConnection>()));
        outerConnection2Mock.Setup(x => x.SetClosedInnerConnection(It.IsAny<IClosedInnerConnection>()));

        var openInnerConnection = new OpenInnerConnection(
            mocks.ConnectionOwner.Object,
            mocks.DataFile.Object,
            new OuterConnectionTracker(closeOnEmpty: true),
            mocks.ClosedConnectionFactory.Object,
            mocks.WorkerThreadFactory.Object,
            mocks.TimeProvider);

        openInnerConnection.Attach(outerConnection1Mock.Object);
        openInnerConnection.Attach(outerConnection2Mock.Object);

        openInnerConnection.Dispose();

        outerConnection1Mock.Verify(x => x.SetClosedInnerConnection(mocks.ClosedInnerConnection.Object), Times.Once);
        outerConnection2Mock.Verify(x => x.SetClosedInnerConnection(mocks.ClosedInnerConnection.Object), Times.Once);

        mocks.Verify();
    }

    [Fact]
    public void IdleTimeout_Test()
    {
        var mocks = new OpenInnerConnectionMocks(MockBehavior.Strict);

        mocks.ConnectionOwner.Setup(x => x.NewConnectionId()).Returns(ConnectionId);
        mocks.DataFile.SetupGet(x => x.Memory).Returns(DataFileMemory);

        var openInnerConnection = new OpenInnerConnection(
            mocks.ConnectionOwner.Object,
            mocks.DataFile.Object,
            mocks.OuterConnectionTracker.Object,
            mocks.ClosedConnectionFactory.Object,
            mocks.WorkerThreadFactory.Object,
            mocks.TimeProvider);

        var idleTimeout = TimeSpan.FromSeconds(234);
        openInnerConnection.IdleTimeout = idleTimeout;

        Assert.Equal(idleTimeout, openInnerConnection.IdleTimeout);
    }

    [Fact]
    public void IdleTimeout_ThrowsOnInfiniteTimeSpanTest()
    {
        var mocks = new OpenInnerConnectionMocks(MockBehavior.Strict);

        mocks.ConnectionOwner.Setup(x => x.NewConnectionId()).Returns(ConnectionId);
        mocks.DataFile.SetupGet(x => x.Memory).Returns(DataFileMemory);

        var openInnerConnection = new OpenInnerConnection(
            mocks.ConnectionOwner.Object,
            mocks.DataFile.Object,
            mocks.OuterConnectionTracker.Object,
            mocks.ClosedConnectionFactory.Object,
            mocks.WorkerThreadFactory.Object,
            mocks.TimeProvider);

        Assert.Throws<InvalidOperationException>(() => openInnerConnection.IdleTimeout = Timeout.InfiniteTimeSpan);
    }

    [Fact]
    public void IdleTimeout_ThrowsOnNegativeTimeSpanTest()
    {
        var mocks = new OpenInnerConnectionMocks(MockBehavior.Strict);

        mocks.ConnectionOwner.Setup(x => x.NewConnectionId()).Returns(ConnectionId);
        mocks.DataFile.SetupGet(x => x.Memory).Returns(DataFileMemory);

        var openInnerConnection = new OpenInnerConnection(
            mocks.ConnectionOwner.Object,
            mocks.DataFile.Object,
            mocks.OuterConnectionTracker.Object,
            mocks.ClosedConnectionFactory.Object,
            mocks.WorkerThreadFactory.Object,
            mocks.TimeProvider);

        var negativeTimeout = TimeSpan.FromSeconds(-5);
        Assert.Throws<InvalidOperationException>(() => openInnerConnection.IdleTimeout = negativeTimeout);
    }

    [Fact]
    public void StartWorkerThread_Test()
    {
        var mocks = new OpenInnerConnectionMocks(MockBehavior.Strict);

        mocks.ConnectionOwner.Setup(x => x.NewConnectionId()).Returns(ConnectionId);
        mocks.DataFile.SetupGet(x => x.Memory).Returns(DataFileMemory);

        var openInnerConnection = new OpenInnerConnection(
            mocks.ConnectionOwner.Object,
            mocks.DataFile.Object,
            new OuterConnectionTracker(closeOnEmpty: true),
            mocks.ClosedConnectionFactory.Object,
            mocks.WorkerThreadFactory.Object,
            mocks.TimeProvider);

        mocks.WorkerThread.Setup(x => x.Start());

        openInnerConnection.StartWorkerThread();

        mocks.WorkerThreadFactory.Verify(x => x.Create(openInnerConnection, mocks.TimeProvider), Times.Once);
        mocks.WorkerThread.Verify(x => x.Start(), Times.Once);
    }

    [Fact]
    public void WaitForDataReady_ThrowsIfDisposedTest()
    {
        var mocks = new OpenInnerConnectionMocks(MockBehavior.Strict);

        mocks.ConnectionOwner.Setup(x => x.NewConnectionId()).Returns(ConnectionId);
        mocks.ConnectionOwner.Setup(x => x.OnConnectionClosing(It.IsAny<IOpenInnerConnection>()));
        mocks.DataFile.SetupGet(x => x.Memory).Returns(DataFileMemory);

        mocks.ClosedInnerConnection.Setup(x => x.Close());
        mocks.DataFile.Setup(x => x.Close());
        mocks.WorkerThread.Setup(x => x.Dispose());

        var openInnerConnection = new OpenInnerConnection(
            mocks.ConnectionOwner.Object,
            mocks.DataFile.Object,
            new OuterConnectionTracker(closeOnEmpty: true),
            mocks.ClosedConnectionFactory.Object,
            mocks.WorkerThreadFactory.Object,
            mocks.TimeProvider);

        openInnerConnection.Dispose();

        Assert.Throws<ObjectDisposedException>(() => openInnerConnection.WaitForDataReady(default));
    }

    [Fact]
    public async Task WaitForDataReadyAsync_ThrowsIfDisposedTest()
    {
        var mocks = new OpenInnerConnectionMocks(MockBehavior.Strict);

        mocks.ConnectionOwner.Setup(x => x.NewConnectionId()).Returns(ConnectionId);
        mocks.ConnectionOwner.Setup(x => x.OnConnectionClosing(It.IsAny<IOpenInnerConnection>()));
        mocks.DataFile.SetupGet(x => x.Memory).Returns(DataFileMemory);

        mocks.ClosedInnerConnection.Setup(x => x.Close());
        mocks.DataFile.Setup(x => x.Close());
        mocks.WorkerThread.Setup(x => x.Dispose());

        var openInnerConnection = new OpenInnerConnection(
            mocks.ConnectionOwner.Object,
            mocks.DataFile.Object,
            new OuterConnectionTracker(closeOnEmpty: true),
            mocks.ClosedConnectionFactory.Object,
            mocks.WorkerThreadFactory.Object,
            mocks.TimeProvider);

        openInnerConnection.Dispose();

        await Assert.ThrowsAsync<ObjectDisposedException>(async () => await openInnerConnection.WaitForDataReadyAsync(default));
    }
}
