using Moq;

namespace SharpRacer.Internal.Connections;
public class ConnectionObjectManagerTests
{
    [Fact]
    public void ClearConnection_Test()
    {
        bool objectManagerHasConnection = false;

        var signalsMock = new Mock<IConnectionSignals>(MockBehavior.Strict);
        var objectManager = new ConnectionObjectManager(signalsMock.Object);

        var connectionMock = new Mock<IOpenInnerConnection>(MockBehavior.Strict);
        connectionMock.Setup(x => x.Dispose());

        signalsMock.Setup(x => x.SetConnectionAvailableSignal()).Callback(() => objectManagerHasConnection = true);
        signalsMock.Setup(x => x.ClearConnectionAvailableSignal()).Callback(() => objectManagerHasConnection = false);

        objectManager.SetConnection(connectionMock.Object);

        Assert.True(objectManager.ClearConnection(connectionMock.Object));
        Assert.NotNull(objectManager.GetConnection());
        Assert.False(objectManagerHasConnection);

        connectionMock.Verify(x => x.Dispose(), Times.Once);
        connectionMock.VerifyNoOtherCalls();

        signalsMock.Verify(x => x.ClearConnectionAvailableSignal(), Times.Once);
        signalsMock.Verify(x => x.SetConnectionAvailableSignal(), Times.Once);
        signalsMock.VerifyNoOtherCalls();
    }

    [Fact]
    public void ClearConnection_ReturnsFalseForUnmatchedConnectionTest()
    {
        bool objectManagerHasConnection = false;

        var signalsMock = new Mock<IConnectionSignals>(MockBehavior.Strict);
        var objectManager = new ConnectionObjectManager(signalsMock.Object);

        var connectionMock1 = new Mock<IOpenInnerConnection>(MockBehavior.Strict);
        var connectionMock2 = new Mock<IOpenInnerConnection>(MockBehavior.Strict);

        signalsMock.Setup(x => x.SetConnectionAvailableSignal()).Callback(() => objectManagerHasConnection = true);
        signalsMock.Setup(x => x.ClearConnectionAvailableSignal()).Callback(() => objectManagerHasConnection = false);

        objectManager.SetConnection(connectionMock1.Object);
        var clearedConnection = objectManager.ClearConnection(connectionMock2.Object);

        Assert.False(clearedConnection);
        Assert.True(objectManagerHasConnection);
        Assert.Equal(connectionMock1.Object, objectManager.GetConnection());

        signalsMock.Verify(x => x.ClearConnectionAvailableSignal(), Times.Never);
        signalsMock.Verify(x => x.SetConnectionAvailableSignal(), Times.Once);
        signalsMock.VerifyNoOtherCalls();
    }

    [Fact]
    public void ClearConnectionException_Test()
    {
        var exceptionObj = new SimulatorConnectionException("Test");
        bool objectManagerHasException = false;

        var signalsMock = new Mock<IConnectionSignals>(MockBehavior.Strict);
        var objectManager = new ConnectionObjectManager(signalsMock.Object);

        signalsMock.Setup(x => x.ClearConnectionExceptionSignal()).Callback(() => objectManagerHasException = false);
        signalsMock.Setup(x => x.SetConnectionExceptionSignal()).Callback(() => objectManagerHasException = true);

        objectManager.SetConnectionException(exceptionObj);
        Assert.True(objectManagerHasException);

        objectManager.ClearConnectionException();

        Assert.False(objectManagerHasException);
        Assert.Null(objectManager.GetConnectionException());

        signalsMock.Verify(x => x.ClearConnectionExceptionSignal(), Times.Once);
        signalsMock.Verify(x => x.SetConnectionExceptionSignal(), Times.Once);
        signalsMock.VerifyNoOtherCalls();
    }

    [Fact]
    public void GetConnection_Test()
    {
        var connectionMock = new Mock<IOpenInnerConnection>(MockBehavior.Strict);

        var signalsMock = new Mock<IConnectionSignals>(MockBehavior.Strict);
        var objectManager = new ConnectionObjectManager(signalsMock.Object);

        signalsMock.Setup(x => x.SetConnectionAvailableSignal());

        objectManager.SetConnection(connectionMock.Object);

        var returnedConnection = objectManager.GetConnection();

        Assert.NotNull(returnedConnection);
        Assert.Equal(connectionMock.Object, returnedConnection);
    }

    [Fact]
    public void GetConnection_NoConnectionTest()
    {
        var signalsMock = new Mock<IConnectionSignals>(MockBehavior.Strict);
        var objectManager = new ConnectionObjectManager(signalsMock.Object);

        var returnedConnection = objectManager.GetConnection();

        Assert.Null(returnedConnection);
    }

    [Fact]
    public void GetConnectionException_Test()
    {
        var exceptionObj = new SimulatorConnectionException("Test");

        var signalsMock = new Mock<IConnectionSignals>(MockBehavior.Strict);
        var objectManager = new ConnectionObjectManager(signalsMock.Object);

        signalsMock.Setup(x => x.SetConnectionExceptionSignal());

        objectManager.SetConnectionException(exceptionObj);

        var returnedException = objectManager.GetConnectionException();

        Assert.NotNull(returnedException);
        Assert.Equal(exceptionObj, returnedException);
    }

    [Fact]
    public void GetConnectionException_NoExceptionTest()
    {
        var signalsMock = new Mock<IConnectionSignals>(MockBehavior.Strict);
        var objectManager = new ConnectionObjectManager(signalsMock.Object);

        signalsMock.Setup(x => x.SetConnectionExceptionSignal());

        var returnedException = objectManager.GetConnectionException();

        Assert.Null(returnedException);
    }

    [Fact]
    public void SetConnection_Test()
    {
        var connectionMock = new Mock<IOpenInnerConnection>(MockBehavior.Strict);

        var signalsMock = new Mock<IConnectionSignals>(MockBehavior.Strict);
        var objectManager = new ConnectionObjectManager(signalsMock.Object);

        signalsMock.Setup(x => x.SetConnectionAvailableSignal());

        objectManager.SetConnection(connectionMock.Object);
        var returnedConnection = objectManager.GetConnection();

        // Assert
        signalsMock.Verify(x => x.SetConnectionAvailableSignal(), Times.Once);
        signalsMock.VerifyNoOtherCalls();

        Assert.NotNull(returnedConnection);
        Assert.Equal(connectionMock.Object, returnedConnection);
    }

    [Fact]
    public void SetConnectionException_Test()
    {
        var exceptionObj = new SimulatorConnectionException("Test");

        var signalsMock = new Mock<IConnectionSignals>(MockBehavior.Strict);
        var objectManager = new ConnectionObjectManager(signalsMock.Object);

        signalsMock.Setup(x => x.SetConnectionExceptionSignal());

        Assert.Null(objectManager.GetConnectionException());

        objectManager.SetConnectionException(exceptionObj);

        Assert.NotNull(objectManager.GetConnectionException());
        Assert.Equal(exceptionObj, objectManager.GetConnectionException());

        signalsMock.Verify(x => x.SetConnectionExceptionSignal(), Times.Once);
        signalsMock.VerifyNoOtherCalls();
    }
}
