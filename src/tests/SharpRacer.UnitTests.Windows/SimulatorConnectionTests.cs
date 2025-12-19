using Moq;
using SharpRacer.Internal.Connections;
using SharpRacer.IO;
using SharpRacer.Telemetry;

namespace SharpRacer;
public partial class SimulatorConnectionTests
{
    [Fact]
    public void Ctor_Test()
    {
        var connection = new SimulatorConnection();

        Assert.Equal(SimulatorConnectionState.None, connection.State);
        Assert.False(connection.CanRead);
        Assert.Empty(connection.DataVariables);
    }

    [Fact]
    public void Ctor_ConnectionManagerTest()
    {
        var mocks = new SimulatorConnectionMock();
        var connection = mocks.CreateInstance();

        mocks.DataVariableInfoProvider.SetupGet(x => x.DataVariables).Returns(Enumerable.Empty<DataVariableInfo>());

        Assert.Equal(SimulatorConnectionState.None, connection.State);
        Assert.False(connection.CanRead);
        Assert.Empty(connection.DataVariables);
    }

    [Fact]
    public void Ctor_ThrowsArgumentNullExceptionTest()
    {
        var mocks = new SimulatorConnectionMock();

        Assert.Throws<ArgumentNullException>(
            () => new SimulatorConnection(null!, mocks.DataVariableInfoProvider.Object, mocks.CancellationTokenSource.Object));

        Assert.Throws<ArgumentNullException>(
            () => new SimulatorConnection(mocks.ConnectionManager.Object, null!, mocks.CancellationTokenSource.Object));

        Assert.Throws<ArgumentNullException>(
            () => new SimulatorConnection(mocks.ConnectionManager.Object, mocks.DataVariableInfoProvider.Object, null!));
    }

    [Fact]
    public void AcquireDataHandle_Test()
    {
        var mocks = new SimulatorConnectionMock();

        var dataHandleMock = mocks.MockRepository.Create<IConnectionDataHandle>();
        var openInnerConnectionMock = mocks.MockRepository.Create<IOpenInnerConnection>();

        openInnerConnectionMock.Setup(x => x.AcquireDataHandle()).Returns(dataHandleMock.Object);

        // Create SimulatorConnection object and get the IOuterConnection implementation
        var connection = mocks.CreateInstance();
        var outerConnection = (IOuterConnection)connection;

        mocks.ConnectionManager.Setup(x => x.Connect(It.IsAny<IOuterConnection>(), It.IsAny<TimeSpan>()));

        mocks.DataVariableInfoProvider.Setup(x => x.OnDataVariablesActivated(It.IsAny<ISimulatorConnection>()));

        // Call Open() to put connection into Connecting state
        connection.Open();

        // Invoke SetOpenInnerConnection
        outerConnection.SetOpenInnerConnection(openInnerConnectionMock.Object);

        // Get the IConnectionDataHandle
        var memoryHandle = connection.AcquireDataHandle();

        Assert.Equal(dataHandleMock.Object, memoryHandle);

        openInnerConnectionMock.Verify(x => x.AcquireDataHandle(), Times.Once);
    }

    [Fact]
    public void Close_Test()
    {
        var mocks = new SimulatorConnectionMock();
        var openInnerConnectionMock = mocks.MockRepository.Create<IOpenInnerConnection>();

        openInnerConnectionMock.Setup(x => x.CloseOuterConnection(It.IsAny<IOuterConnection>()));

        // Create SimulatorConnection object and get the IOuterConnection implementation
        var connection = mocks.CreateInstance();
        var outerConnection = (IOuterConnection)connection;

        mocks.ConnectionManager.Setup(x => x.Connect(It.IsAny<IOuterConnection>(), It.IsAny<TimeSpan>()));

        mocks.DataVariableInfoProvider.Setup(x => x.OnDataVariablesActivated(It.IsAny<ISimulatorConnection>()));

        // Call Open() to put connection into Connecting state
        connection.Open();

        // Invoke SetOpenInnerConnection
        outerConnection.SetOpenInnerConnection(openInnerConnectionMock.Object);

        // Close the connection
        connection.Close();

        openInnerConnectionMock.Verify(x => x.CloseOuterConnection(connection), Times.Once);
    }

    [Fact]
    public void Close_ReturnsIfAlreadyClosedTest()
    {
        var mocks = new SimulatorConnectionMock();

        var closedInnerConnectionMock = mocks.MockRepository.Create<IClosedInnerConnection>();

        closedInnerConnectionMock.Setup(x => x.CloseOuterConnection(It.IsAny<IOuterConnection>()))
            .Throws(new InvalidOperationException("CloseOuterConnection not allowed in this test"));

        mocks.CancellationTokenSource.Setup(x => x.Cancel());

        // Create SimulatorConnection object and get the IOuterConnection implementation
        var connection = mocks.CreateInstance();
        var outerConnection = (IOuterConnection)connection;

        // Invoke SetClosedInnerConnection
        outerConnection.SetClosedInnerConnection(closedInnerConnectionMock.Object);

        // Close the connection when it's already closed
        connection.Close();

        closedInnerConnectionMock.Verify(x => x.CloseOuterConnection(It.IsAny<IOuterConnection>()), Times.Never);
    }

    [Fact]
    public void Close_ThrowIfDisposedTest()
    {
        var connection = new SimulatorConnection();

        connection.Dispose();

        Assert.Throws<ObjectDisposedException>(() => connection.Close());
    }

    [Fact]
    public void Close_ThrowIfStateEqualsNoneTest()
    {
        var connection = new SimulatorConnection();

        Assert.Throws<InvalidOperationException>(() => connection.Close());
    }

    [Fact]
    public void CreateDataReader_ThrowIfNotOpenTest()
    {
        var connection = new SimulatorConnection();

        Assert.Throws<InvalidOperationException>(() => connection.CreateDataReader());
    }

    [Fact]
    public void CreateDataReader_ThrowIfDisposedTest()
    {
        var connection = new SimulatorConnection();
        connection.Dispose();

        Assert.Throws<ObjectDisposedException>(() => connection.CreateDataReader());
    }

    [Fact]
    public void NotifyDataVariableActivatedTest()
    {
        string variableName = "Foo";
        Action<DataVariableInfo> callback = (dataVariableInfo) => { };

        var mocks = new SimulatorConnectionMock();
        var connection = mocks.CreateInstance();

        mocks.DataVariableInfoProvider.Setup(x => x.NotifyDataVariableActivated(It.IsAny<string>(), It.IsAny<Action<DataVariableInfo>>()));

        connection.NotifyDataVariableActivated(variableName, callback);

        mocks.DataVariableInfoProvider.Verify(x => x.NotifyDataVariableActivated(variableName, callback));
    }

    [Fact]
    public void NotifyDataVariableActivated_ThrowOnInvalidArgsTest()
    {
        string variableName = "Foo";
        Action<DataVariableInfo> callback = (dataVariableInfo) => { };

        var mocks = new SimulatorConnectionMock();
        var connection = mocks.CreateInstance();

        mocks.DataVariableInfoProvider.Setup(x => x.NotifyDataVariableActivated(It.IsAny<string>(), It.IsAny<Action<DataVariableInfo>>()));

        Assert.Throws<ArgumentException>(() => connection.NotifyDataVariableActivated(string.Empty, callback));
        Assert.Throws<ArgumentNullException>(() => connection.NotifyDataVariableActivated(null!, callback));
        Assert.Throws<ArgumentNullException>(() => connection.NotifyDataVariableActivated(variableName, null!));
    }

    [Fact]
    public void NotifyDataVariableActivated_ThrowIfDisposedTest()
    {
        string variableName = "Foo";
        Action<DataVariableInfo> callback = (dataVariableInfo) => { };

        var connection = new SimulatorConnection();
        connection.Dispose();

        Assert.Throws<ObjectDisposedException>(() => connection.NotifyDataVariableActivated(variableName, callback));
    }

    [Fact]
    public void SetClosedInnerConnectionTest()
    {
        var mocks = new SimulatorConnectionMock();
        var connection = mocks.CreateInstance();

        var closedInnerConnectionMock = mocks.MockRepository.Create<IClosedInnerConnection>();

        mocks.CancellationTokenSource.Setup(x => x.Cancel());

        // Create SimulatorConnection object and get the IOuterConnection implementation
        var outerConnection = (IOuterConnection)connection;

        // Invoke SetClosedInnerConnection
        outerConnection.SetClosedInnerConnection(closedInnerConnectionMock.Object);

        Assert.Equal(SimulatorConnectionState.Closed, connection.State);
        mocks.CancellationTokenSource.Verify(x => x.Cancel(), Times.Once);
    }

    [Fact]
    public void SetClosedInnerConnection_EventsTest()
    {
        bool closedEventRaised = false;
        bool stateChangedEventRaised = false;
        SimulatorConnectionState oldState;

        var mocks = new SimulatorConnectionMock();
        var connection = mocks.CreateInstance();

        var closedInnerConnectionMock = mocks.MockRepository.Create<IClosedInnerConnection>();

        mocks.CancellationTokenSource.Setup(x => x.Cancel());

        // Create SimulatorConnection object and get the IOuterConnection implementation
        var outerConnection = (IOuterConnection)connection;

        oldState = connection.State;

        connection.Closed += onConnectionClosedEventHandler;
        connection.StateChanged += onConnectionStateChangedEventHandler;

        // Invoke SetClosedInnerConnection
        outerConnection.SetClosedInnerConnection(closedInnerConnectionMock.Object);

        Assert.Equal(SimulatorConnectionState.Closed, connection.State);

        Assert.True(closedEventRaised, $"{nameof(ISimulatorConnection)}.{nameof(ISimulatorConnection.Closed)} event not raised!");
        Assert.True(stateChangedEventRaised, $"{nameof(ISimulatorConnection)}.{nameof(ISimulatorConnection.StateChanged)} event not raised!");

        mocks.CancellationTokenSource.Verify(x => x.Cancel(), Times.Once);

        void onConnectionClosedEventHandler(object? sender, EventArgs e)
        {
            Assert.Equal(connection, sender as SimulatorConnection);

            closedEventRaised = true;
            connection.Closed -= onConnectionClosedEventHandler;
        }

        void onConnectionStateChangedEventHandler(object? sender, SimulatorConnectionStateChangedEventArgs e)
        {
            Assert.Equal(connection, sender as SimulatorConnection);
            Assert.Equal(SimulatorConnectionState.Closed, e.NewState);
            Assert.Equal(oldState, e.OldState);

            stateChangedEventRaised = true;
            connection.StateChanged -= onConnectionStateChangedEventHandler;
        }
    }

    [Fact]
    public void SetClosedInnerConnection_ReturnIfDisposedTest()
    {
        var mocks = new SimulatorConnectionMock();

        var closedInnerConnectionMock = mocks.MockRepository.Create<IClosedInnerConnection>();

        mocks.CancellationTokenSource.Setup(x => x.Cancel());
        mocks.CancellationTokenSource.Setup(x => x.Dispose());

        // Create SimulatorConnection object and get the IOuterConnection implementation
        var connection = mocks.CreateInstance();
        var outerConnection = (IOuterConnection)connection;

        var preDisposalState = connection.State;
        bool stateChangedEventRaised = false;

        connection.StateChanged += onConnectionStateChangedEventHandler;
        connection.Dispose();

        // Invoke SetClosedInnerConnection
        outerConnection.SetClosedInnerConnection(closedInnerConnectionMock.Object);

        Assert.NotEqual(SimulatorConnectionState.Closed, connection.State);
        Assert.Equal(preDisposalState, connection.State);

        // CancellationTokenSource.Cancel() is called once from the Dispose() method instead of being called from our method
        mocks.CancellationTokenSource.Verify(x => x.Cancel(), Times.Once);

        Assert.False(stateChangedEventRaised);

        void onConnectionStateChangedEventHandler(object? sender, SimulatorConnectionStateChangedEventArgs e)
        {
            stateChangedEventRaised = true;
        }
    }

    [Fact]
    public void SetOpenInnerConnection_Test()
    {
        var mocks = new SimulatorConnectionMock();

        var openInnerConnectionMock = mocks.MockRepository.Create<IOpenInnerConnection>();

        // Create SimulatorConnection object and get the IOuterConnection implementation
        var connection = mocks.CreateInstance();
        var outerConnection = (IOuterConnection)connection;

        mocks.ConnectionManager.Setup(x => x.Connect(It.IsAny<IOuterConnection>(), It.IsAny<TimeSpan>()));

        mocks.DataVariableInfoProvider.Setup(x => x.OnDataVariablesActivated(It.IsAny<ISimulatorConnection>()))
            .Callback(onDataVariablesActivated);

        Assert.Equal(SimulatorConnectionState.None, connection.State);

        // Call Open() to put connection into Connecting state
        connection.Open();

        Assert.Equal(SimulatorConnectionState.Connecting, connection.State);

        // Invoke SetOpenInnerConnection
        outerConnection.SetOpenInnerConnection(openInnerConnectionMock.Object);

        Assert.Equal(SimulatorConnectionState.Open, connection.State);

        mocks.DataVariableInfoProvider.Verify(x => x.OnDataVariablesActivated(connection), Times.Once);

        void onDataVariablesActivated(ISimulatorConnection conn)
        {
            Assert.Equal(conn, connection);
        }
    }

    [Fact]
    public void SetOpenInnerConnection_EventsTest()
    {
        bool openedEventRaised = false;
        bool stateChangedEventRaised = false;
        bool isOpenInvoked = false;

        var mocks = new SimulatorConnectionMock();
        var connection = mocks.CreateInstance();

        var openInnerConnectionMock = mocks.MockRepository.Create<IOpenInnerConnection>();

        // Create SimulatorConnection object and get the IOuterConnection implementation
        var outerConnection = (IOuterConnection)connection;

        connection.Opened += onConnectionOpenedEventHandler;
        connection.StateChanged += onConnectionStateChangedEventHandler;

        mocks.ConnectionManager.Setup(x => x.Connect(It.IsAny<IOuterConnection>(), It.IsAny<TimeSpan>()));

        mocks.DataVariableInfoProvider.Setup(x => x.OnDataVariablesActivated(It.IsAny<ISimulatorConnection>()));

        // Call Open() to put connection into Connecting state
        connection.Open();
        isOpenInvoked = true;

        // Invoke SetOpenInnerConnection
        outerConnection.SetOpenInnerConnection(openInnerConnectionMock.Object);

        Assert.True(openedEventRaised, $"{nameof(ISimulatorConnection)}.{nameof(ISimulatorConnection.Opened)} event not raised!");
        Assert.True(stateChangedEventRaised, $"{nameof(ISimulatorConnection)}.{nameof(ISimulatorConnection.StateChanged)} event not raised!");

        void onConnectionOpenedEventHandler(object? sender, EventArgs e)
        {
            Assert.Equal(connection, sender as SimulatorConnection);

            openedEventRaised = true;
            connection.Opened -= onConnectionOpenedEventHandler;
        }

        void onConnectionStateChangedEventHandler(object? sender, SimulatorConnectionStateChangedEventArgs e)
        {
            Assert.Equal(connection, sender as SimulatorConnection);

            if (isOpenInvoked)
            {
                Assert.Equal(SimulatorConnectionState.Open, e.NewState);
                Assert.Equal(SimulatorConnectionState.Connecting, e.OldState);
            }

            stateChangedEventRaised = true;
            connection.StateChanged -= onConnectionStateChangedEventHandler;
        }
    }

    [Fact]
    public void SetOpenInnerConnection_ReturnIfDisposedTest()
    {
        var mocks = new SimulatorConnectionMock();

        mocks.CancellationTokenSource.Setup(x => x.Cancel());
        mocks.CancellationTokenSource.Setup(x => x.Dispose());

        var openInnerConnectionMock = mocks.MockRepository.Create<IOpenInnerConnection>();

        // Create SimulatorConnection object and get the IOuterConnection implementation
        var connection = mocks.CreateInstance();
        var outerConnection = (IOuterConnection)connection;

        var preDisposalState = connection.State;
        bool stateChangedEventRaised = false;

        connection.StateChanged += onConnectionStateChangedEventHandler;

        connection.Dispose();

        // Invoke SetOpenInnerConnection
        outerConnection.SetOpenInnerConnection(openInnerConnectionMock.Object);

        // CancellationTokenSource.Cancel() is called once from the Dispose() method instead of being called from our method
        mocks.CancellationTokenSource.Verify(x => x.Cancel(), Times.Once);

        Assert.False(stateChangedEventRaised);
        Assert.NotEqual(SimulatorConnectionState.Open, connection.State);
        Assert.Equal(preDisposalState, connection.State);

        void onConnectionStateChangedEventHandler(object? sender, SimulatorConnectionStateChangedEventArgs e)
        {
            stateChangedEventRaised = true;
        }
    }

    [Fact]
    public void SetOpenInnerConnection_ReturnIfNotInConnectingStateTest()
    {
        var mocks = new SimulatorConnectionMock();
        var connection = mocks.CreateInstance();

        var openInnerConnectionMock = mocks.MockRepository.Create<IOpenInnerConnection>();

        var outerConnection = (IOuterConnection)connection;

        Assert.Equal(SimulatorConnectionState.None, connection.State);

        outerConnection.SetOpenInnerConnection(openInnerConnectionMock.Object);

        Assert.Equal(SimulatorConnectionState.None, connection.State);
    }

    [Fact]
    public void WaitForDataReadyTest()
    {
        var mocks = new SimulatorConnectionMock();
        var openInnerConnectionMock = mocks.MockRepository.Create<IOpenInnerConnection>();

        mocks.ConnectionManager.Setup(x => x.Connect(It.IsAny<IOuterConnection>(), It.IsAny<TimeSpan>()));

        mocks.DataVariableInfoProvider.Setup(x => x.OnDataVariablesActivated(It.IsAny<ISimulatorConnection>()));

        openInnerConnectionMock.Setup(x => x.WaitForDataReady(It.IsAny<CancellationToken>())).Returns(true);

        // Configure ConnectionCancellationTokenSource
        using var mockedCancellationSource = new CancellationTokenSource();

        mocks.CancellationTokenSource.SetupGet(x => x.Token).Returns(mockedCancellationSource.Token);

        var connection = mocks.CreateInstance();
        var outerConnection = (IOuterConnection)connection;

        // Transition to Open connection state
        connection.Open();
        outerConnection.SetOpenInnerConnection(openInnerConnectionMock.Object);

        Assert.True(connection.WaitForDataReady());
        openInnerConnectionMock.Verify(x => x.WaitForDataReady(mockedCancellationSource.Token), Times.Once);
    }

    [Fact]
    public void WaitForDataReady_ReturnsFalseIfConnectionCancellationTokenSignaledTest()
    {
        var mocks = new SimulatorConnectionMock();
        var openInnerConnectionMock = mocks.MockRepository.Create<IOpenInnerConnection>();

        mocks.ConnectionManager.Setup(x => x.Connect(It.IsAny<IOuterConnection>(), It.IsAny<TimeSpan>()));

        mocks.DataVariableInfoProvider.Setup(x => x.OnDataVariablesActivated(It.IsAny<ISimulatorConnection>()));

        // Configure ConnectionCancellationTokenSource
        using var mockedCancellationSource = new CancellationTokenSource();

        mocks.CancellationTokenSource.SetupGet(x => x.Token).Returns(mockedCancellationSource.Token);

        var connection = mocks.CreateInstance();
        var outerConnection = (IOuterConnection)connection;

        // Transition to Open connection state
        connection.Open();
        outerConnection.SetOpenInnerConnection(openInnerConnectionMock.Object);

        // Cancel before calling
        mockedCancellationSource.Cancel();

        Assert.False(connection.WaitForDataReady());
    }

    [Fact]
    public void WaitForDataReady_ReturnsFalseIfNotOpenTest()
    {
        var connection = new SimulatorConnection();

        Assert.False(connection.WaitForDataReady());
    }

    [Fact]
    public void WaitForDataReady_ThrowIfDisposedTest()
    {
        var connection = new SimulatorConnection();
        connection.Dispose();

        Assert.Throws<ObjectDisposedException>(() => connection.WaitForDataReady());
    }

    [Fact]
    public async Task WaitForDataReadyAsyncTest()
    {
        var mocks = new SimulatorConnectionMock();
        var openInnerConnectionMock = mocks.MockRepository.Create<IOpenInnerConnection>();

        mocks.ConnectionManager.Setup(x => x.ConnectAsync(It.IsAny<IOuterConnection>(), It.IsAny<TimeSpan>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        mocks.DataVariableInfoProvider.Setup(x => x.OnDataVariablesActivated(It.IsAny<ISimulatorConnection>()));

        openInnerConnectionMock.Setup(x => x.WaitForDataReadyAsync(It.IsAny<CancellationToken>()))
            .Returns(async () => await ValueTask.FromResult(true));

        // Configure ConnectionCancellationTokenSource
        using var mockedCancellationSource = new CancellationTokenSource();
        CancellationToken linkedToken = default;

        mocks.CancellationTokenSource.SetupGet(x => x.Token).Returns(mockedCancellationSource.Token);

        mocks.CancellationTokenSource.Setup(x => x.CreateLinkedTokenSource(It.IsAny<CancellationToken>()))
            .Returns<CancellationToken>(ct =>
            {
                var lts = CancellationTokenSource.CreateLinkedTokenSource(mockedCancellationSource.Token, ct);
                linkedToken = lts.Token;
                return lts;
            });

        var connection = mocks.CreateInstance();
        var outerConnection = (IOuterConnection)connection;

        // Transition to Open connection state
        await connection.OpenAsync();
        outerConnection.SetOpenInnerConnection(openInnerConnectionMock.Object);

        var waitResult = await connection.WaitForDataReadyAsync();
        Assert.True(waitResult);
        openInnerConnectionMock.Verify(x => x.WaitForDataReadyAsync(mockedCancellationSource.Token), Times.Once);
    }

    [Fact]
    public async Task WaitForDataReadyAsync_LinkedCancellationTokenTest()
    {
        using var externalCancellationTokenSource = new CancellationTokenSource();

        var mocks = new SimulatorConnectionMock();
        var openInnerConnectionMock = mocks.MockRepository.Create<IOpenInnerConnection>();

        mocks.ConnectionManager.Setup(x => x.ConnectAsync(It.IsAny<IOuterConnection>(), It.IsAny<TimeSpan>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        mocks.DataVariableInfoProvider.Setup(x => x.OnDataVariablesActivated(It.IsAny<ISimulatorConnection>()));

        openInnerConnectionMock.Setup(x => x.WaitForDataReadyAsync(It.IsAny<CancellationToken>()))
            .Returns(async () => await ValueTask.FromResult(true));

        // Configure ConnectionCancellationTokenSource
        using var mockedCancellationSource = new CancellationTokenSource();
        CancellationToken linkedToken = default;

        mocks.CancellationTokenSource.SetupGet(x => x.Token).Returns(mockedCancellationSource.Token);

        mocks.CancellationTokenSource.Setup(x => x.CreateLinkedTokenSource(It.IsAny<CancellationToken>()))
            .Returns<CancellationToken>(ct =>
            {
                var lts = CancellationTokenSource.CreateLinkedTokenSource(mockedCancellationSource.Token, ct);
                linkedToken = lts.Token;
                return lts;
            });

        var connection = mocks.CreateInstance();
        var outerConnection = (IOuterConnection)connection;

        // Transition to Open connection state
        await connection.OpenAsync();
        outerConnection.SetOpenInnerConnection(openInnerConnectionMock.Object);

        var waitResult = await connection.WaitForDataReadyAsync(externalCancellationTokenSource.Token);
        Assert.True(waitResult);
        openInnerConnectionMock.Verify(x => x.WaitForDataReadyAsync(linkedToken), Times.Once);
    }

    [Fact]
    public async Task WaitForDataReadyAsync_ReturnsFalseIfConnectionCancellationTokenSignaledTest()
    {
        var mocks = new SimulatorConnectionMock();
        var openInnerConnectionMock = mocks.MockRepository.Create<IOpenInnerConnection>();

        mocks.ConnectionManager.Setup(x => x.ConnectAsync(It.IsAny<IOuterConnection>(), It.IsAny<TimeSpan>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        mocks.DataVariableInfoProvider.Setup(x => x.OnDataVariablesActivated(It.IsAny<ISimulatorConnection>()));

        openInnerConnectionMock.Setup(x => x.WaitForDataReadyAsync(It.IsAny<CancellationToken>()))
            .Returns(async () => await ValueTask.FromResult(true));

        // Configure ConnectionCancellationTokenSource
        using var mockedCancellationSource = new CancellationTokenSource();
        CancellationToken linkedToken = default;

        mocks.CancellationTokenSource.SetupGet(x => x.Token).Returns(mockedCancellationSource.Token);

        mocks.CancellationTokenSource.Setup(x => x.CreateLinkedTokenSource(It.IsAny<CancellationToken>()))
            .Returns<CancellationToken>(ct =>
            {
                var lts = CancellationTokenSource.CreateLinkedTokenSource(mockedCancellationSource.Token, ct);
                linkedToken = lts.Token;
                return lts;
            });

        var connection = mocks.CreateInstance();
        var outerConnection = (IOuterConnection)connection;

        // Transition to Open connection state
        await connection.OpenAsync();
        outerConnection.SetOpenInnerConnection(openInnerConnectionMock.Object);

        // Cancel
        mockedCancellationSource.Cancel();

        var waitResult = await connection.WaitForDataReadyAsync();
        Assert.False(waitResult);
    }

    [Fact]
    public async Task WaitForDataReadyAsync_ReturnsFalseIfNotOpenTest()
    {
        var connection = new SimulatorConnection();

        var waitResult = await connection.WaitForDataReadyAsync();
        Assert.False(waitResult);
    }

    [Fact]
    public async Task WaitForDataReadyAsync_ThrowIfDisposedTest()
    {
        var connection = new SimulatorConnection();
        connection.Dispose();

        await Assert.ThrowsAsync<ObjectDisposedException>(async () => await connection.WaitForDataReadyAsync());
    }
}
