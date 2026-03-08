using Moq;
using SharpRacer.Internal.Connections;

namespace SharpRacer;

partial class SimulatorConnectionTests
{
    [Fact]
    public void Dispose_Test()
    {
        var mocks = new SimulatorConnectionMock();

        mocks.CancellationTokenSource.Setup(x => x.Cancel());
        mocks.CancellationTokenSource.Setup(x => x.Dispose());
        mocks.ConnectionManager.Setup(x => x.Connect(It.IsAny<IOuterConnection>(), It.IsAny<TimeSpan>()));
        mocks.TelemetryVariableInfoProvider.Setup(x => x.OnTelemetryVariablesActivated(It.IsAny<ISimulatorConnection>()));

        var openInnerConnectionMock = mocks.MockRepository.Create<IOpenInnerConnection>();

        openInnerConnectionMock.Setup(x => x.Detach(It.IsAny<IOuterConnection>()));

        // Create SimulatorConnection object
        var connection = mocks.CreateInstance();
        var outerConnection = (IOuterConnection)connection;

        connection.Open();
        outerConnection.SetOpenInnerConnection(openInnerConnectionMock.Object);

        // Dispose
        connection.Dispose();

        // Verify
        mocks.CancellationTokenSource.Verify(x => x.Cancel(), Times.Once);
        mocks.CancellationTokenSource.Verify(x => x.Dispose(), Times.Once);
        openInnerConnectionMock.Verify(x => x.Detach(connection), Times.Once);
    }

    [Fact]
    public void Dispose_DoesNotRaiseEventsIfNeverOpenedTest()
    {
        bool closedEventRaised = false;
        bool stateChangedEventRaised = false;

        var mocks = new SimulatorConnectionMock();
        var openInnerConnectionMock = mocks.MockRepository.Create<IOpenInnerConnection>();

        // Create SimulatorConnection object
        var connection = mocks.CreateInstance();

        connection.Closed += onConnectionClosedEventHandler;
        connection.StateChanged += onConnectionStateChangedEventHandler;

        mocks.CancellationTokenSource.Setup(x => x.Cancel());
        mocks.CancellationTokenSource.Setup(x => x.Dispose());
        mocks.ConnectionManager.Setup(x => x.Connect(It.IsAny<IOuterConnection>(), It.IsAny<TimeSpan>()));

        mocks.TelemetryVariableInfoProvider.Setup(x => x.OnTelemetryVariablesActivated(It.IsAny<ISimulatorConnection>()));

        // Dispose connection
        connection.Dispose();

        Assert.Equal(SimulatorConnectionState.None, connection.State);

        Assert.False(closedEventRaised, $"{nameof(ISimulatorConnection)}.{nameof(ISimulatorConnection.Closed)} event raised!");
        Assert.False(stateChangedEventRaised, $"{nameof(ISimulatorConnection)}.{nameof(ISimulatorConnection.StateChanged)} event raised!");

        void onConnectionClosedEventHandler(object? sender, EventArgs e)
        {
            Assert.Equal(connection, sender as SimulatorConnection);

            closedEventRaised = true;

            connection.Closed -= onConnectionClosedEventHandler;
        }

        void onConnectionStateChangedEventHandler(object? sender, SimulatorConnectionStateChangedEventArgs e)
        {
            Assert.Equal(connection, sender as SimulatorConnection);

            stateChangedEventRaised = true;

            connection.StateChanged -= onConnectionStateChangedEventHandler;
        }
    }

    [Fact]
    public void Dispose_RaisesEventsIfStateNotEqualToNoneTest()
    {
        bool closedEventRaised = false;
        bool stateChangedEventRaised = false;

        var mocks = new SimulatorConnectionMock();
        var openInnerConnectionMock = mocks.MockRepository.Create<IOpenInnerConnection>();

        // Create SimulatorConnection object
        var connection = mocks.CreateInstance();

        mocks.CancellationTokenSource.Setup(x => x.Cancel());
        mocks.CancellationTokenSource.Setup(x => x.Dispose());
        mocks.ConnectionManager.Setup(x => x.Connect(It.IsAny<IOuterConnection>(), It.IsAny<TimeSpan>()));

        mocks.TelemetryVariableInfoProvider.Setup(x => x.OnTelemetryVariablesActivated(It.IsAny<ISimulatorConnection>()));

        // Call Open() to put connection into Connecting state
        connection.Open();

        connection.Closed += onConnectionClosedEventHandler;
        connection.StateChanged += onConnectionStateChangedEventHandler;

        // Dispose connection
        connection.Dispose();

        Assert.Equal(SimulatorConnectionState.Closed, connection.State);

        Assert.True(closedEventRaised, $"{nameof(ISimulatorConnection)}.{nameof(ISimulatorConnection.Closed)} event not raised!");
        Assert.True(stateChangedEventRaised, $"{nameof(ISimulatorConnection)}.{nameof(ISimulatorConnection.StateChanged)} event not raised!");

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

            stateChangedEventRaised = true;
            connection.StateChanged -= onConnectionStateChangedEventHandler;
        }
    }
}
