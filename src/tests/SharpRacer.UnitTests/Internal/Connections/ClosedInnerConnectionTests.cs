using Moq;
using SharpRacer.IO.Internal;

namespace SharpRacer.Internal.Connections;
public class ClosedInnerConnectionTests
{
    [Fact]
    public void Ctor_Test()
    {
        var memoryObj = new Memory<byte>([0xDE, 0xAD, 0xBE, 0xEF]);

        var mocks = new MockRepository(MockBehavior.Strict);

        var dataFileMock = mocks.Create<IConnectionDataFile>();
        dataFileMock.SetupGet(x => x.Memory).Returns(memoryObj);

        var connectionTrackerMock = mocks.Create<IOuterConnectionTracker>();
        connectionTrackerMock.SetupGet(x => x.CloseOnEmpty).Returns(true);

        var connection = new ClosedInnerConnection(dataFileMock.Object, connectionTrackerMock.Object);

        Assert.Equal(0, connection.ConnectionId);
        Assert.Equal(SimulatorConnectionState.Closed, connection.State);
        Assert.Equal(Timeout.InfiniteTimeSpan, connection.IdleTimeout);

        Assert.True(connection.Data.SequenceEqual(dataFileMock.Object.Memory.Span));
        Assert.Equal(dataFileMock.Object, connection.DataFile);
    }

    [Fact]
    public void Ctor_ThrowsArgumentNullExceptionTest()
    {
        var memoryObj = new Memory<byte>([0xDE, 0xAD, 0xBE, 0xEF]);

        var mocks = new MockRepository(MockBehavior.Strict);

        var dataFileMock = mocks.Create<IConnectionDataFile>();
        dataFileMock.SetupGet(x => x.Memory).Returns(memoryObj);

        var connectionTrackerMock = mocks.Create<IOuterConnectionTracker>();
        connectionTrackerMock.SetupGet(x => x.CloseOnEmpty).Returns(true);

        Assert.Throws<ArgumentNullException>(() => new ClosedInnerConnection(null!, connectionTrackerMock.Object));
        Assert.Throws<ArgumentNullException>(() => new ClosedInnerConnection(dataFileMock.Object, null!));
    }

    [Fact]
    public void Ctor_ThrowsIfConnectionTrackerCloseOnEmptyEqualsFalseTest()
    {
        var mocks = new MockRepository(MockBehavior.Strict);

        var dataFileMock = mocks.Create<IConnectionDataFile>();
        var connectionTrackerMock = mocks.Create<IOuterConnectionTracker>();

        connectionTrackerMock.SetupGet(x => x.CloseOnEmpty).Returns(false);

        Assert.Throws<ArgumentException>(() => new ClosedInnerConnection(dataFileMock.Object, connectionTrackerMock.Object));
    }

    [Fact]
    public void Close_Test()
    {
        var mocks = new MockRepository(MockBehavior.Strict);

        var dataFileMock = mocks.Create<IConnectionDataFile>();
        dataFileMock.Setup(x => x.Dispose());

        var connectionTrackerMock = mocks.Create<IOuterConnectionTracker>();

        connectionTrackerMock.Setup(x => x.Close());

        connectionTrackerMock.SetupGet(x => x.CloseOnEmpty).Returns(true);

        // Prevent auto-disposal by having tracker return IsEmpty = false
        connectionTrackerMock.SetupGet(x => x.IsEmpty).Returns(false);

        var connection = new ClosedInnerConnection(dataFileMock.Object, connectionTrackerMock.Object);

        connection.Close();

        connectionTrackerMock.Verify(x => x.Close(), Times.Once());
        dataFileMock.Verify(x => x.Dispose(), Times.Never());
    }

    [Fact]
    public void Close_DisposeIfNoRemainingOuterConnectionsTest()
    {
        var mocks = new MockRepository(MockBehavior.Strict);

        var dataFileMock = mocks.Create<IConnectionDataFile>();
        dataFileMock.Setup(x => x.Dispose());

        var connectionTrackerMock = mocks.Create<IOuterConnectionTracker>();

        connectionTrackerMock.Setup(x => x.Close());

        connectionTrackerMock.SetupGet(x => x.CloseOnEmpty).Returns(true);

        // Allow auto-disposal by having tracker return IsEmpty = true
        connectionTrackerMock.SetupGet(x => x.IsEmpty).Returns(true);

        var connection = new ClosedInnerConnection(dataFileMock.Object, connectionTrackerMock.Object);

        connection.Close();

        connectionTrackerMock.Verify(x => x.Close(), Times.Once());
        dataFileMock.Verify(x => x.Dispose(), Times.Once());
    }

    [Fact]
    public void Close_DoNothingIfClosedTest()
    {
        var mocks = new MockRepository(MockBehavior.Strict);

        var dataFileMock = mocks.Create<IConnectionDataFile>();
        dataFileMock.Setup(x => x.Dispose());

        var connectionTrackerMock = mocks.Create<IOuterConnectionTracker>();

        connectionTrackerMock.Setup(x => x.Close());

        connectionTrackerMock.SetupGet(x => x.CloseOnEmpty).Returns(true);
        connectionTrackerMock.SetupGet(x => x.IsEmpty).Returns(false);

        var connection = new ClosedInnerConnection(dataFileMock.Object, connectionTrackerMock.Object);

        connection.Close();

        // Ensure only one invocation of OuterConnectionTracker.Close() to mark our inner connection closed
        connectionTrackerMock.Verify(x => x.Close(), Times.Once());

        // Close again to short-circuit because we are closed
        connection.Close();

        dataFileMock.Verify(x => x.Dispose(), Times.Never());
    }

    [Fact]
    public void Close_DoNothingIfDisposedTest()
    {
        var mocks = new MockRepository(MockBehavior.Strict);

        var dataFileMock = mocks.Create<IConnectionDataFile>();
        dataFileMock.Setup(x => x.Dispose());

        var connectionTrackerMock = mocks.Create<IOuterConnectionTracker>();

        connectionTrackerMock.Setup(x => x.Close());

        connectionTrackerMock.SetupGet(x => x.CloseOnEmpty).Returns(true);
        connectionTrackerMock.SetupGet(x => x.IsEmpty).Returns(false);

        var connection = new ClosedInnerConnection(dataFileMock.Object, connectionTrackerMock.Object);

        // Dispose so we short-circuit because we're disposed this time
        connection.Dispose();

        connection.Close();

        dataFileMock.Verify(x => x.Dispose(), Times.Once());
    }

    [Fact]
    public void CloseOuterConnection_Test()
    {
        var mocks = new MockRepository(MockBehavior.Strict);

        var dataFileMock = mocks.Create<IConnectionDataFile>();
        var connectionTrackerMock = mocks.Create<IOuterConnectionTracker>();
        var outerConnectionMock = mocks.Create<IOuterConnection>();

        connectionTrackerMock.SetupGet(x => x.CloseOnEmpty).Returns(true);

        // Prevent auto-disposal through tracker
        connectionTrackerMock.SetupGet(x => x.IsClosed).Returns(false);
        connectionTrackerMock.SetupGet(x => x.IsEmpty).Returns(false);

        connectionTrackerMock.Setup(x => x.Detach(It.IsAny<IOuterConnection>()))
            .Returns<IOuterConnection>(conn => conn == outerConnectionMock.Object);

        var connection = new ClosedInnerConnection(dataFileMock.Object, connectionTrackerMock.Object);

        connection.CloseOuterConnection(outerConnectionMock.Object);

        connectionTrackerMock.Verify(x => x.Detach(outerConnectionMock.Object));
        dataFileMock.Verify(x => x.Dispose(), Times.Never());
    }

    [Fact]
    public void CloseOuterConnection_DisposeIfTrackerEmptyAndClosedTest()
    {
        var mocks = new MockRepository(MockBehavior.Strict);

        var dataFileMock = mocks.Create<IConnectionDataFile>();
        dataFileMock.Setup(x => x.Dispose());

        var connectionTrackerMock = mocks.Create<IOuterConnectionTracker>();
        var outerConnectionMock = mocks.Create<IOuterConnection>();

        connectionTrackerMock.SetupGet(x => x.CloseOnEmpty).Returns(true);

        // Prevent auto-disposal through tracker
        connectionTrackerMock.SetupGet(x => x.IsClosed).Returns(true);
        connectionTrackerMock.SetupGet(x => x.IsEmpty).Returns(true);

        connectionTrackerMock.Setup(x => x.Detach(It.IsAny<IOuterConnection>()))
            .Returns<IOuterConnection>(conn => conn == outerConnectionMock.Object);

        var connection = new ClosedInnerConnection(dataFileMock.Object, connectionTrackerMock.Object);

        connection.CloseOuterConnection(outerConnectionMock.Object);

        connectionTrackerMock.Verify(x => x.Detach(outerConnectionMock.Object));
        dataFileMock.Verify(x => x.Dispose(), Times.Once());
    }

    [Fact]
    public void CloseOuterConnection_DoesNotAutoDisposeTrackerClosedButNotEmptyTest()
    {
        // Don't automatically dispose if outer connection tracker is closed but not empty
        var mocks = new MockRepository(MockBehavior.Strict);

        var dataFileMock = mocks.Create<IConnectionDataFile>();
        dataFileMock.Setup(x => x.Dispose());

        var connectionTrackerMock = mocks.Create<IOuterConnectionTracker>();
        var outerConnectionMock = mocks.Create<IOuterConnection>();

        connectionTrackerMock.SetupGet(x => x.CloseOnEmpty).Returns(true);

        // Prevent auto-disposal through tracker
        connectionTrackerMock.SetupGet(x => x.IsClosed).Returns(true);
        connectionTrackerMock.SetupGet(x => x.IsEmpty).Returns(false);

        connectionTrackerMock.Setup(x => x.Detach(It.IsAny<IOuterConnection>()))
            .Returns<IOuterConnection>(conn => conn == outerConnectionMock.Object);

        var connection = new ClosedInnerConnection(dataFileMock.Object, connectionTrackerMock.Object);

        connection.CloseOuterConnection(outerConnectionMock.Object);

        connectionTrackerMock.Verify(x => x.Detach(outerConnectionMock.Object));
        dataFileMock.Verify(x => x.Dispose(), Times.Never());
    }

    [Fact]
    public void CloseOuterConnection_DoesNotAutoDisposeIfTrackerEmptyButNotClosedTest()
    {
        // Don't automatically dispose if outer connection tracker is closed but not empty
        var mocks = new MockRepository(MockBehavior.Strict);

        var dataFileMock = mocks.Create<IConnectionDataFile>();
        dataFileMock.Setup(x => x.Dispose());

        var connectionTrackerMock = mocks.Create<IOuterConnectionTracker>();
        var outerConnectionMock = mocks.Create<IOuterConnection>();

        connectionTrackerMock.SetupGet(x => x.CloseOnEmpty).Returns(true);

        // Prevent auto-disposal through tracker
        connectionTrackerMock.SetupGet(x => x.IsClosed).Returns(false);
        connectionTrackerMock.SetupGet(x => x.IsEmpty).Returns(true);

        connectionTrackerMock.Setup(x => x.Detach(It.IsAny<IOuterConnection>()))
            .Returns<IOuterConnection>(conn => conn == outerConnectionMock.Object);

        var connection = new ClosedInnerConnection(dataFileMock.Object, connectionTrackerMock.Object);

        connection.CloseOuterConnection(outerConnectionMock.Object);

        connectionTrackerMock.Verify(x => x.Detach(outerConnectionMock.Object));
        dataFileMock.Verify(x => x.Dispose(), Times.Never());
    }

    [Fact]
    public void Detach_Test()
    {
        var mocks = new MockRepository(MockBehavior.Strict);

        var dataFileMock = mocks.Create<IConnectionDataFile>();
        var connectionTrackerMock = mocks.Create<IOuterConnectionTracker>();
        var outerConnectionMock = mocks.Create<IOuterConnection>();

        connectionTrackerMock.SetupGet(x => x.CloseOnEmpty).Returns(true);

        // Prevent auto-disposal through tracker
        connectionTrackerMock.SetupGet(x => x.IsClosed).Returns(false);
        connectionTrackerMock.SetupGet(x => x.IsEmpty).Returns(false);

        connectionTrackerMock.Setup(x => x.Detach(It.IsAny<IOuterConnection>()))
            .Returns<IOuterConnection>(conn => conn == outerConnectionMock.Object);

        var connection = new ClosedInnerConnection(dataFileMock.Object, connectionTrackerMock.Object);

        connection.Detach(outerConnectionMock.Object);

        connectionTrackerMock.Verify(x => x.Detach(outerConnectionMock.Object));
        dataFileMock.Verify(x => x.Dispose(), Times.Never());
    }

    [Fact]
    public void Detach_DisposeIfTrackerEmptyAndClosedTest()
    {
        var mocks = new MockRepository(MockBehavior.Strict);

        var dataFileMock = mocks.Create<IConnectionDataFile>();
        dataFileMock.Setup(x => x.Dispose());

        var connectionTrackerMock = mocks.Create<IOuterConnectionTracker>();
        var outerConnectionMock = mocks.Create<IOuterConnection>();

        connectionTrackerMock.SetupGet(x => x.CloseOnEmpty).Returns(true);

        // Prevent auto-disposal through tracker
        connectionTrackerMock.SetupGet(x => x.IsClosed).Returns(true);
        connectionTrackerMock.SetupGet(x => x.IsEmpty).Returns(true);

        connectionTrackerMock.Setup(x => x.Detach(It.IsAny<IOuterConnection>()))
            .Returns<IOuterConnection>(conn => conn == outerConnectionMock.Object);

        var connection = new ClosedInnerConnection(dataFileMock.Object, connectionTrackerMock.Object);

        connection.Detach(outerConnectionMock.Object);

        connectionTrackerMock.Verify(x => x.Detach(outerConnectionMock.Object));
        dataFileMock.Verify(x => x.Dispose(), Times.Once());
    }

    [Fact]
    public void Detach_DoesNotAutoDisposeTrackerClosedButNotEmptyTest()
    {
        // Don't automatically dispose if outer connection tracker is closed but not empty
        var mocks = new MockRepository(MockBehavior.Strict);

        var dataFileMock = mocks.Create<IConnectionDataFile>();
        dataFileMock.Setup(x => x.Dispose());

        var connectionTrackerMock = mocks.Create<IOuterConnectionTracker>();
        var outerConnectionMock = mocks.Create<IOuterConnection>();

        connectionTrackerMock.SetupGet(x => x.CloseOnEmpty).Returns(true);

        // Prevent auto-disposal through tracker
        connectionTrackerMock.SetupGet(x => x.IsClosed).Returns(true);
        connectionTrackerMock.SetupGet(x => x.IsEmpty).Returns(false);

        connectionTrackerMock.Setup(x => x.Detach(It.IsAny<IOuterConnection>()))
            .Returns<IOuterConnection>(conn => conn == outerConnectionMock.Object);

        var connection = new ClosedInnerConnection(dataFileMock.Object, connectionTrackerMock.Object);

        connection.Detach(outerConnectionMock.Object);

        connectionTrackerMock.Verify(x => x.Detach(outerConnectionMock.Object));
        dataFileMock.Verify(x => x.Dispose(), Times.Never());
    }

    [Fact]
    public void Detach_DoesNotAutoDisposeIfTrackerEmptyButNotClosedTest()
    {
        // Don't automatically dispose if outer connection tracker is closed but not empty
        var mocks = new MockRepository(MockBehavior.Strict);

        var dataFileMock = mocks.Create<IConnectionDataFile>();
        dataFileMock.Setup(x => x.Dispose());

        var connectionTrackerMock = mocks.Create<IOuterConnectionTracker>();
        var outerConnectionMock = mocks.Create<IOuterConnection>();

        connectionTrackerMock.SetupGet(x => x.CloseOnEmpty).Returns(true);

        // Prevent auto-disposal through tracker
        connectionTrackerMock.SetupGet(x => x.IsClosed).Returns(false);
        connectionTrackerMock.SetupGet(x => x.IsEmpty).Returns(true);

        connectionTrackerMock.Setup(x => x.Detach(It.IsAny<IOuterConnection>()))
            .Returns<IOuterConnection>(conn => conn == outerConnectionMock.Object);

        var connection = new ClosedInnerConnection(dataFileMock.Object, connectionTrackerMock.Object);

        connection.Detach(outerConnectionMock.Object);

        connectionTrackerMock.Verify(x => x.Detach(outerConnectionMock.Object));
        dataFileMock.Verify(x => x.Dispose(), Times.Never());
    }

    [Fact]
    public void Detach_UntrackedOuterConnectionTest()
    {
        var mocks = new MockRepository(MockBehavior.Strict);

        var dataFileMock = mocks.Create<IConnectionDataFile>();
        var connectionTrackerMock = mocks.Create<IOuterConnectionTracker>();
        var outerConnectionMock = mocks.Create<IOuterConnection>();

        connectionTrackerMock.SetupGet(x => x.CloseOnEmpty).Returns(true);

        // Prevent auto-disposal through tracker
        connectionTrackerMock.SetupGet(x => x.IsClosed).Returns(false);
        connectionTrackerMock.SetupGet(x => x.IsEmpty).Returns(false);

        connectionTrackerMock.Setup(x => x.Detach(It.IsAny<IOuterConnection>()))
            .Returns<IOuterConnection>(conn => conn != outerConnectionMock.Object);

        var connection = new ClosedInnerConnection(dataFileMock.Object, connectionTrackerMock.Object);

        connection.Detach(outerConnectionMock.Object);

        connectionTrackerMock.Verify(x => x.Detach(outerConnectionMock.Object));
        dataFileMock.Verify(x => x.Dispose(), Times.Never());
    }

    [Fact]
    public void RentDataFileMemory_Test()
    {
        var mocks = new MockRepository(MockBehavior.Strict);

        var dataFileMock = mocks.Create<IConnectionDataFile>();
        var connectionTrackerMock = mocks.Create<IOuterConnectionTracker>();

        connectionTrackerMock.SetupGet(x => x.CloseOnEmpty).Returns(true);

        var connection = new ClosedInnerConnection(dataFileMock.Object, connectionTrackerMock.Object);

        Assert.Throws<InvalidOperationException>(() => connection.RentDataFileMemory());
    }

    [Fact]
    public void RentDataFileSpan_Test()
    {
        var mocks = new MockRepository(MockBehavior.Strict);

        var dataFileMock = mocks.Create<IConnectionDataFile>();
        var connectionTrackerMock = mocks.Create<IOuterConnectionTracker>();

        connectionTrackerMock.SetupGet(x => x.CloseOnEmpty).Returns(true);

        var connection = new ClosedInnerConnection(dataFileMock.Object, connectionTrackerMock.Object);

        Assert.Throws<InvalidOperationException>(() => connection.RentDataFileSpan());
    }

    [Fact]
    public void WaitForDataReady_Test()
    {
        var mocks = new MockRepository(MockBehavior.Strict);

        var dataFileMock = mocks.Create<IConnectionDataFile>();
        var connectionTrackerMock = mocks.Create<IOuterConnectionTracker>();

        connectionTrackerMock.SetupGet(x => x.CloseOnEmpty).Returns(true);

        var connection = new ClosedInnerConnection(dataFileMock.Object, connectionTrackerMock.Object);

        Assert.False(connection.WaitForDataReady(default));
    }

    [Fact]
    public async Task WaitForDataReadyAsync_Test()
    {
        var mocks = new MockRepository(MockBehavior.Strict);

        var dataFileMock = mocks.Create<IConnectionDataFile>();
        var connectionTrackerMock = mocks.Create<IOuterConnectionTracker>();

        connectionTrackerMock.SetupGet(x => x.CloseOnEmpty).Returns(true);

        var connection = new ClosedInnerConnection(dataFileMock.Object, connectionTrackerMock.Object);

        Assert.False(await connection.WaitForDataReadyAsync(default));
    }
}
