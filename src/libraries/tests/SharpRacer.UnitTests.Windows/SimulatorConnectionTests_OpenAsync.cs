using Moq;
using SharpRacer.Internal.Connections;

namespace SharpRacer;
partial class SimulatorConnectionTests
{
    [Fact]
    public async Task OpenAsync_Test()
    {
        var timeout = Timeout.InfiniteTimeSpan;

        var mocks = new SimulatorConnectionMock();
        var connection = mocks.CreateInstance();

        mocks.ConnectionManager.Setup(x => x.ConnectAsync(It.IsAny<IOuterConnection>(), It.IsAny<TimeSpan>(), It.IsAny<CancellationToken>()))
            .Callback<IOuterConnection, TimeSpan, CancellationToken>(connectionManagerConnect)
            .Returns(Task.CompletedTask);

        using var testCancellationSource = new CancellationTokenSource();
        CancellationToken linkedToken = default;

        mocks.CancellationTokenSource.SetupGet(x => x.Token).Returns(testCancellationSource.Token);

        mocks.CancellationTokenSource.Setup(x => x.CreateLinkedTokenSource(It.IsAny<CancellationToken>()))
            .Returns<CancellationToken>(ct =>
            {
                var lts = CancellationTokenSource.CreateLinkedTokenSource(testCancellationSource.Token, ct);
                linkedToken = lts.Token;
                return lts;
            });

        Assert.Equal(SimulatorConnectionState.None, connection.State);

        // Open
        await connection.OpenAsync();

        Assert.Equal(SimulatorConnectionState.Connecting, connection.State);

        // Verify parameterless OpenAsync() used infinite timeout and the default CancellationToken
        mocks.CancellationTokenSource.Verify(x => x.CreateLinkedTokenSource(default));
        mocks.ConnectionManager.Verify(x => x.ConnectAsync(connection, timeout, linkedToken), Times.Once());

        void connectionManagerConnect(IOuterConnection outerConnection, TimeSpan connectTimeout, CancellationToken cancellationToken)
        {
            Assert.Equal(timeout, connectTimeout);
            Assert.Equal(connection, outerConnection);
        }
    }

    [Fact]
    public async Task OpenAsync_CancellationTokenTest()
    {
        using var externalCancellationTokenSource = new CancellationTokenSource();

        var timeout = Timeout.InfiniteTimeSpan;

        var mocks = new SimulatorConnectionMock();
        var connection = mocks.CreateInstance();

        mocks.ConnectionManager.Setup(x => x.ConnectAsync(It.IsAny<IOuterConnection>(), It.IsAny<TimeSpan>(), It.IsAny<CancellationToken>()))
            .Callback<IOuterConnection, TimeSpan, CancellationToken>(connectionManagerConnect)
            .Returns(Task.CompletedTask);

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

        Assert.Equal(SimulatorConnectionState.None, connection.State);

        // Open
        await connection.OpenAsync(externalCancellationTokenSource.Token);

        Assert.Equal(SimulatorConnectionState.Connecting, connection.State);

        // Verify parameterless OpenAsync() used infinite timeout and the specified CancellationToken
        mocks.CancellationTokenSource.Verify(x => x.CreateLinkedTokenSource(externalCancellationTokenSource.Token));
        mocks.ConnectionManager.Verify(x => x.ConnectAsync(connection, timeout, linkedToken), Times.Once());

        void connectionManagerConnect(IOuterConnection outerConnection, TimeSpan connectTimeout, CancellationToken cancellationToken)
        {
            Assert.Equal(timeout, connectTimeout);
            Assert.Equal(connection, outerConnection);
        }
    }

    [Fact]
    public async Task OpenAsync_ReturnIfAlreadyOpenTest()
    {
        bool isOpen = false;

        var mocks = new SimulatorConnectionMock();
        var openInnerConnectionMock = mocks.MockRepository.Create<IOpenInnerConnection>();

        // Create SimulatorConnection object and get the IOuterConnection implementation
        var connection = mocks.CreateInstance();
        var outerConnection = (IOuterConnection)connection;

        mocks.ConnectionManager.Setup(x => x.ConnectAsync(It.IsAny<IOuterConnection>(), It.IsAny<TimeSpan>(), It.IsAny<CancellationToken>()))
            .Callback<IOuterConnection, TimeSpan, CancellationToken>(connectionManagerConnect)
            .Returns(Task.CompletedTask);

        mocks.TelemetryVariableInfoProvider.Setup(x => x.OnTelemetryVariablesActivated(It.IsAny<ISimulatorConnection>()));

        // Configure ConnectionCancellationTokenSource
        using var mockedCancellationSource = new CancellationTokenSource();

        mocks.CancellationTokenSource.SetupGet(x => x.Token).Returns(mockedCancellationSource.Token);

        mocks.CancellationTokenSource.Setup(x => x.CreateLinkedTokenSource(It.IsAny<CancellationToken>()))
            .Returns<CancellationToken>(ct =>
            {
                return CancellationTokenSource.CreateLinkedTokenSource(mockedCancellationSource.Token, ct);
            });

        // Call OpenAsync() to put connection into Connecting state
        await connection.OpenAsync();

        // Invoke SetOpenInnerConnection
        outerConnection.SetOpenInnerConnection(openInnerConnectionMock.Object);

        Assert.Equal(SimulatorConnectionState.Open, connection.State);
        isOpen = true;

        // Call OpenAsync() again
        await connection.OpenAsync();

        void connectionManagerConnect(IOuterConnection outerConnection, TimeSpan connectTimeout, CancellationToken cancellationToken)
        {
            Assert.Equal(connection, outerConnection);

            if (isOpen)
            {
                Assert.Fail($"{nameof(IConnectionManager)}.{nameof(IConnectionManager.Connect)} called while connection was open!");
            }
        }
    }

    [Fact]
    public async Task OpenAsync_SetConnectionStateNoneOnExceptionThrownTest()
    {
        bool connectingStateSet = false;

        var mocks = new SimulatorConnectionMock();
        var closedInnerConnectionMock = mocks.MockRepository.Create<IClosedInnerConnection>();

        mocks.ConnectionManager.Setup(x => x.ConnectAsync(It.IsAny<IOuterConnection>(), It.IsAny<TimeSpan>(), It.IsAny<CancellationToken>()))
            .Callback(() => throw new InvalidOperationException());

        // Configure ConnectionCancellationTokenSource
        using var mockedCancellationSource = new CancellationTokenSource();

        mocks.CancellationTokenSource.SetupGet(x => x.Token).Returns(mockedCancellationSource.Token);

        mocks.CancellationTokenSource.Setup(x => x.CreateLinkedTokenSource(It.IsAny<CancellationToken>()))
            .Returns<CancellationToken>(ct =>
            {
                return CancellationTokenSource.CreateLinkedTokenSource(mockedCancellationSource.Token, ct);
            });

        // Create SimulatorConnection object and get the IOuterConnection implementation
        var connection = mocks.CreateInstance();
        var outerConnection = (IOuterConnection)connection;

        Assert.Equal(SimulatorConnectionState.None, connection.State);

        // Invoke Open and expect our exception
        await Assert.ThrowsAsync<InvalidOperationException>(() => connection.OpenAsync());

        Assert.Equal(SimulatorConnectionState.None, connection.State);

        void onConnectionStateChangedEventHandler(object? sender, SimulatorConnectionStateChangedEventArgs e)
        {
            Assert.Equal(connection, sender as SimulatorConnection);

            if (e.NewState == SimulatorConnectionState.Connecting)
            {
                connectingStateSet = true;
            }

            if (e.NewState == SimulatorConnectionState.None)
            {
                Assert.True(connectingStateSet, "SimulatorConnectionState was never set to Connecting before being set to None!");
            }

            connection.StateChanged -= onConnectionStateChangedEventHandler;
        }
    }

    [Fact]
    public async Task OpenAsync_ThrowIfClosedTest()
    {
        var mocks = new SimulatorConnectionMock();
        var closedInnerConnectionMock = mocks.MockRepository.Create<IClosedInnerConnection>();

        mocks.CancellationTokenSource.Setup(x => x.Cancel());

        // Create SimulatorConnection object and get the IOuterConnection implementation
        var connection = mocks.CreateInstance();
        var outerConnection = (IOuterConnection)connection;

        // Invoke SetClosedInnerConnection
        outerConnection.SetClosedInnerConnection(closedInnerConnectionMock.Object);

        Assert.Equal(SimulatorConnectionState.Closed, connection.State);
        await Assert.ThrowsAsync<InvalidOperationException>(() => connection.OpenAsync());
    }

    [Fact]
    public async Task OpenAsync_ThrowIfConnectionInProgressTest()
    {
        var mocks = new SimulatorConnectionMock();
        var connection = mocks.CreateInstance();

        var connectReturnSignal = new AutoResetEvent(false);
        var threadInvokedOpenMethodSignal = new AutoResetEvent(false);

        mocks.ConnectionManager.Setup(x => x.Connect(It.IsAny<IOuterConnection>(), It.IsAny<TimeSpan>()))
            .Callback(() =>
            {
                threadInvokedOpenMethodSignal.Set();
                Assert.True(connectReturnSignal.WaitOne(TimeSpan.FromSeconds(5)), "ummmm");
            });

        Assert.Equal(SimulatorConnectionState.None, connection.State);

        // Invoke Open on a different thread first
        var openThread = new Thread(onOpenThreadStart) { IsBackground = true };
        openThread.Start();

        Assert.True(threadInvokedOpenMethodSignal.WaitOne(TimeSpan.FromSeconds(5)),
            "Thread for Open() did not complete within timeout period");

        // Call Open() again, expecting we fail due to the Open() call already made
        await Assert.ThrowsAsync<InvalidOperationException>(() => connection.OpenAsync());
        connectReturnSignal.Set();
        openThread.Join(TimeSpan.FromSeconds(5));

        void onOpenThreadStart()
        {
            connection.Open();
        }
    }

    [Fact]
    public async Task OpenAsync_ThrowIfDisposedTest()
    {
        var connection = new SimulatorConnection();
        connection.Dispose();

        await Assert.ThrowsAsync<ObjectDisposedException>(async () => await connection.OpenAsync(cancellationToken: default));
        await Assert.ThrowsAsync<ObjectDisposedException>(async () => await connection.OpenAsync(TimeSpan.FromSeconds(5), default));
    }
}
