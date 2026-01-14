using Moq;
using SharpRacer.Internal.Connections;

namespace SharpRacer;
partial class SimulatorConnectionTests
{
    [Fact]
    public void Open_Test()
    {
        var timeout = Timeout.InfiniteTimeSpan;

        var mocks = new SimulatorConnectionMock();
        var connection = mocks.CreateInstance();

        mocks.ConnectionManager.Setup(x => x.Connect(It.IsAny<IOuterConnection>(), It.IsAny<TimeSpan>()))
            .Callback<IOuterConnection, TimeSpan>(connectionManagerConnect);

        Assert.Equal(SimulatorConnectionState.None, connection.State);

        // Open
        connection.Open();

        Assert.Equal(SimulatorConnectionState.Connecting, connection.State);

        mocks.ConnectionManager.Verify(x => x.Connect(connection, timeout), Times.Once());

        void connectionManagerConnect(IOuterConnection outerConnection, TimeSpan connectTimeout)
        {
            Assert.Equal(timeout, connectTimeout);
            Assert.Equal(connection, outerConnection);
        }
    }

    [Fact]
    public void Open_ReturnIfAlreadyOpenTest()
    {
        bool isOpen = false;

        var mocks = new SimulatorConnectionMock();
        var openInnerConnectionMock = mocks.MockRepository.Create<IOpenInnerConnection>();

        // Create SimulatorConnection object and get the IOuterConnection implementation
        var connection = mocks.CreateInstance();
        var outerConnection = (IOuterConnection)connection;

        mocks.ConnectionManager.Setup(x => x.Connect(It.IsAny<IOuterConnection>(), It.IsAny<TimeSpan>()))
            .Callback<IOuterConnection, TimeSpan>(connectionManagerConnect);

        mocks.TelemetryVariableInfoProvider.Setup(x => x.OnTelemetryVariablesActivated(It.IsAny<ISimulatorConnection>()));

        // Call Open() to put connection into Connecting state
        connection.Open();

        // Invoke SetOpenInnerConnection
        outerConnection.SetOpenInnerConnection(openInnerConnectionMock.Object);

        Assert.Equal(SimulatorConnectionState.Open, connection.State);
        isOpen = true;

        // Call Open() again
        connection.Open();

        void connectionManagerConnect(IOuterConnection outerConnection, TimeSpan connectTimeout)
        {
            Assert.Equal(connection, outerConnection);

            if (isOpen)
            {
                Assert.Fail($"{nameof(IConnectionManager)}.{nameof(IConnectionManager.Connect)} called while connection was open!");
            }
        }
    }

    [Fact]
    public void Open_SetConnectionStateNoneOnExceptionThrownTest()
    {
        bool connectingStateSet = false;

        var mocks = new SimulatorConnectionMock();
        var closedInnerConnectionMock = mocks.MockRepository.Create<IClosedInnerConnection>();

        mocks.ConnectionManager.Setup(x => x.Connect(It.IsAny<IOuterConnection>(), It.IsAny<TimeSpan>()))
            .Callback(() => throw new InvalidOperationException());

        // Create SimulatorConnection object and get the IOuterConnection implementation
        var connection = mocks.CreateInstance();
        var outerConnection = (IOuterConnection)connection;

        Assert.Equal(SimulatorConnectionState.None, connection.State);

        // Invoke Open and expect our exception
        Assert.Throws<InvalidOperationException>(connection.Open);

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
    public void Open_ThrowIfClosedTest()
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

        Assert.Throws<InvalidOperationException>(connection.Open);

        mocks.CancellationTokenSource.Verify(x => x.Cancel(), Times.Once);
    }

    [Fact]
    public void Open_ThrowIfConnectionInProgressTest()
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
        Assert.Throws<InvalidOperationException>(connection.Open);
        connectReturnSignal.Set();
        openThread.Join(TimeSpan.FromSeconds(5));

        void onOpenThreadStart()
        {
            connection.Open();
        }
    }
    
    [Fact]
    public void Open_ThrowIfDisposedTest()
    {
        var connection = new SimulatorConnection();
        connection.Dispose();

        Assert.Throws<ObjectDisposedException>(() => connection.Open());
        Assert.Throws<ObjectDisposedException>(() => connection.Open(TimeSpan.FromSeconds(5)));
    }

    [Fact]
    public void Open_TimeoutTest()
    {
        var timeout = TimeSpan.FromSeconds(3);

        var mocks = new SimulatorConnectionMock();
        var connection = mocks.CreateInstance();

        mocks.ConnectionManager.Setup(x => x.Connect(It.IsAny<IOuterConnection>(), It.IsAny<TimeSpan>()))
            .Callback<IOuterConnection, TimeSpan>(connectionManagerConnect);

        Assert.Equal(SimulatorConnectionState.None, connection.State);

        // Open
        connection.Open(timeout);

        Assert.Equal(SimulatorConnectionState.Connecting, connection.State);

        mocks.ConnectionManager.Verify(x => x.Connect(connection, timeout), Times.Once());

        void connectionManagerConnect(IOuterConnection outerConnection, TimeSpan connectTimeout)
        {
            Assert.Equal(timeout, connectTimeout);
            Assert.Equal(connection, outerConnection);
        }
    }
}
