using Moq;
using SharpRacer.Internal.Connections;
using SharpRacer.IO;
using SharpRacer.IO.Internal;
using SharpRacer.Testing.IO;
using SharpRacer.Testing.Telemetry;

namespace SharpRacer.Interop;

public class SessionInfoStringTests
{
    [Fact]
    public void GetSpan_Test()
    {
        var fakeVariableHeaders = new FakeTelemetryVariableHeaders();
        var fakeVariables = new FakeTelemetryVariables(fakeVariableHeaders);
        var fakeData = DataFileMemory.Create(b => b.ConfigureVariables(fakeVariableHeaders.Headers));

        var fakeMmf = new FakeMemoryMappedDataFile(fakeData.Memory);
        var connectionDataFile = new ConnectionDataFile(fakeMmf);
        var outerConnectionTracker = new OuterConnectionTracker(closeOnEmpty: true);

        var mocks = new MockRepository(MockBehavior.Strict);

        // Configure the open inner connection mocks so we can expose our data file to the outer connection
        var openInnerConnectionMocks = new OpenInnerConnectionMocks(mocks);

        openInnerConnectionMocks.ConnectionOwner.Setup(x => x.NewConnectionId()).Returns(1);

        var openInnerConnection = new OpenInnerConnection(
            openInnerConnectionMocks.ConnectionOwner.Object,
            connectionDataFile,
            outerConnectionTracker,
            openInnerConnectionMocks.ClosedConnectionFactory.Object,
            openInnerConnectionMocks.WorkerThreadFactory.Object,
            TimeProvider.System);

        // Configure the outer connection
        var connectionMocks = new SimulatorConnectionMock(mocks);

        connectionMocks.ConnectionManager.Setup(x => x.Connect(It.IsAny<IOuterConnection>(), It.IsAny<TimeSpan>()));
        connectionMocks.TelemetryVariableInfoProvider.Setup(x => x.OnTelemetryVariablesActivated(It.IsAny<ISimulatorConnection>()));

        // Create the outer connection object
        var connection = connectionMocks.CreateInstance();
        var outerConnection = (IOuterConnection)connection;

        // Transition the outer connection to the opened connection state so we can read from our data file
        connection.Open();
        outerConnection.SetOpenInnerConnection(openInnerConnection);

        Assert.Equal(SimulatorConnectionState.Open, connection.State);

        // Update the session info string
        var sessionInfoStr = "Testing123";
        var sessionInfoStrBytes = SessionInfoString.Encoding.GetBytes(sessionInfoStr);
        fakeData.UpdateSessionInfo(sessionInfoStr);

        var sessionInfoSpan = SessionInfoString.GetSpan(connection);

        Assert.Equal(sessionInfoStrBytes.Length, sessionInfoSpan.Length);
        Assert.True(sessionInfoStrBytes.SequenceEqual(sessionInfoSpan));

        var sessionInfoStringResult = SessionInfoString.Encoding.GetString(sessionInfoSpan);

        Assert.Equal(sessionInfoStr, sessionInfoStringResult);
    }

    [Fact]
    public void GetSpan_EmptySpanTest()
    {
        var fakeVariableHeaders = new FakeTelemetryVariableHeaders();
        var fakeVariables = new FakeTelemetryVariables(fakeVariableHeaders);
        var fakeData = DataFileMemory.Create(b => b.ConfigureVariables(fakeVariableHeaders.Headers));

        var fakeMmf = new FakeMemoryMappedDataFile(fakeData.Memory);
        var connectionDataFile = new ConnectionDataFile(fakeMmf);
        var outerConnectionTracker = new OuterConnectionTracker(closeOnEmpty: true);

        var mocks = new MockRepository(MockBehavior.Strict);

        // Configure the open inner connection mocks so we can expose our data file to the outer connection
        var openInnerConnectionMocks = new OpenInnerConnectionMocks(mocks);

        openInnerConnectionMocks.ConnectionOwner.Setup(x => x.NewConnectionId()).Returns(1);

        var openInnerConnection = new OpenInnerConnection(
            openInnerConnectionMocks.ConnectionOwner.Object,
            connectionDataFile,
            outerConnectionTracker,
            openInnerConnectionMocks.ClosedConnectionFactory.Object,
            openInnerConnectionMocks.WorkerThreadFactory.Object,
            TimeProvider.System);

        // Configure the outer connection
        var connectionMocks = new SimulatorConnectionMock(mocks);

        connectionMocks.ConnectionManager.Setup(x => x.Connect(It.IsAny<IOuterConnection>(), It.IsAny<TimeSpan>()));
        connectionMocks.TelemetryVariableInfoProvider.Setup(x => x.OnTelemetryVariablesActivated(It.IsAny<ISimulatorConnection>()));

        // Create the outer connection object
        var connection = connectionMocks.CreateInstance();
        var outerConnection = (IOuterConnection)connection;

        // Transition the outer connection to the opened connection state so we can read from our data file
        connection.Open();
        outerConnection.SetOpenInnerConnection(openInnerConnection);

        Assert.Equal(SimulatorConnectionState.Open, connection.State);

        // Session info string should be initialized to zero length if we don't update it first
        var sessionInfoSpan = SessionInfoString.GetSpan(connection);

        Assert.Equal(0, sessionInfoSpan.Length);
    }

    [Fact]
    public void GetSpan_ThrowOnCanReadEqualsFalseTest()
    {
        var mocks = new MockRepository(MockBehavior.Strict);

        var connectionMock = mocks.Create<ISimulatorConnection>();
        connectionMock.SetupGet(x => x.CanRead).Returns(false);

        Assert.Throws<InvalidOperationException>(() => SessionInfoString.GetSpan(connectionMock.Object));
    }

    [Fact]
    public void GetSpan_ThrowOnNullConnectionTest()
    {
        Assert.Throws<ArgumentNullException>(() => SessionInfoString.GetSpan(null!));
    }

    [Fact]
    public void Read_Test()
    {
        var fakeVariableHeaders = new FakeTelemetryVariableHeaders();
        var fakeVariables = new FakeTelemetryVariables(fakeVariableHeaders);
        var fakeData = DataFileMemory.Create(b => b.ConfigureVariables(fakeVariableHeaders.Headers));

        var fakeMmf = new FakeMemoryMappedDataFile(fakeData.Memory);
        var connectionDataFile = new ConnectionDataFile(fakeMmf);
        var outerConnectionTracker = new OuterConnectionTracker(closeOnEmpty: true);

        var mocks = new MockRepository(MockBehavior.Strict);

        // Configure the open inner connection mocks so we can expose our data file to the outer connection
        var openInnerConnectionMocks = new OpenInnerConnectionMocks(mocks);

        openInnerConnectionMocks.ConnectionOwner.Setup(x => x.NewConnectionId()).Returns(1);

        var openInnerConnection = new OpenInnerConnection(
            openInnerConnectionMocks.ConnectionOwner.Object,
            connectionDataFile,
            outerConnectionTracker,
            openInnerConnectionMocks.ClosedConnectionFactory.Object,
            openInnerConnectionMocks.WorkerThreadFactory.Object,
            TimeProvider.System);

        // Configure the outer connection
        var connectionMocks = new SimulatorConnectionMock(mocks);

        connectionMocks.ConnectionManager.Setup(x => x.Connect(It.IsAny<IOuterConnection>(), It.IsAny<TimeSpan>()));
        connectionMocks.TelemetryVariableInfoProvider.Setup(x => x.OnTelemetryVariablesActivated(It.IsAny<ISimulatorConnection>()));

        // Create the outer connection object
        var connection = connectionMocks.CreateInstance();
        var outerConnection = (IOuterConnection)connection;

        // Transition the outer connection to the opened connection state so we can read from our data file
        connection.Open();
        outerConnection.SetOpenInnerConnection(openInnerConnection);

        Assert.Equal(SimulatorConnectionState.Open, connection.State);

        // Update the session info string
        var sessionInfoStr = "Testing123";
        fakeData.UpdateSessionInfo(sessionInfoStr);

        var sessionInfoStringResult = SessionInfoString.Read(connection);
        Assert.Equal(sessionInfoStr, sessionInfoStringResult);
    }

    [Fact]
    public void Read_VersionOutParameterTest()
    {
        var fakeVariableHeaders = new FakeTelemetryVariableHeaders();
        var fakeVariables = new FakeTelemetryVariables(fakeVariableHeaders);
        var fakeData = DataFileMemory.Create(b => b.ConfigureVariables(fakeVariableHeaders.Headers));

        var fakeMmf = new FakeMemoryMappedDataFile(fakeData.Memory);
        var connectionDataFile = new ConnectionDataFile(fakeMmf);
        var outerConnectionTracker = new OuterConnectionTracker(closeOnEmpty: true);

        var mocks = new MockRepository(MockBehavior.Strict);

        // Configure the open inner connection mocks so we can expose our data file to the outer connection
        var openInnerConnectionMocks = new OpenInnerConnectionMocks(mocks);

        openInnerConnectionMocks.ConnectionOwner.Setup(x => x.NewConnectionId()).Returns(1);

        var openInnerConnection = new OpenInnerConnection(
            openInnerConnectionMocks.ConnectionOwner.Object,
            connectionDataFile,
            outerConnectionTracker,
            openInnerConnectionMocks.ClosedConnectionFactory.Object,
            openInnerConnectionMocks.WorkerThreadFactory.Object,
            TimeProvider.System);

        // Configure the outer connection
        var connectionMocks = new SimulatorConnectionMock(mocks);

        connectionMocks.ConnectionManager.Setup(x => x.Connect(It.IsAny<IOuterConnection>(), It.IsAny<TimeSpan>()));
        connectionMocks.TelemetryVariableInfoProvider.Setup(x => x.OnTelemetryVariablesActivated(It.IsAny<ISimulatorConnection>()));

        // Create the outer connection object
        var connection = connectionMocks.CreateInstance();
        var outerConnection = (IOuterConnection)connection;

        // Transition the outer connection to the opened connection state so we can read from our data file
        connection.Open();
        outerConnection.SetOpenInnerConnection(openInnerConnection);

        Assert.Equal(SimulatorConnectionState.Open, connection.State);

        // Update the session info string
        var sessionInfoStr = "Testing123";
        var sessionInfoVersion = 3;
        fakeData.UpdateSessionInfo(sessionInfoStr, sessionInfoVersion);

        var sessionInfoStringResult = SessionInfoString.Read(connection, out var sessionInfoVersionResult);

        Assert.Equal(sessionInfoStr, sessionInfoStringResult);
        Assert.Equal(sessionInfoVersion, sessionInfoVersionResult);
    }

    [Fact]
    public void Read_ThrowOnCanReadEqualsFalseTest()
    {
        var mocks = new MockRepository(MockBehavior.Strict);

        var connectionMock = mocks.Create<ISimulatorConnection>();
        connectionMock.SetupGet(x => x.CanRead).Returns(false);

        Assert.Throws<InvalidOperationException>(() => SessionInfoString.Read(connectionMock.Object));
        Assert.Throws<InvalidOperationException>(() => SessionInfoString.Read(connectionMock.Object, out _));
    }

    [Fact]
    public void Read_ThrowOnNullConnectionTest()
    {
        Assert.Throws<ArgumentNullException>(() => SessionInfoString.Read(null!));
        Assert.Throws<ArgumentNullException>(() => SessionInfoString.Read(null!, out _));
    }

    [Fact]
    public void ReadVersion_Test()
    {
        var fakeVariableHeaders = new FakeTelemetryVariableHeaders();
        var fakeVariables = new FakeTelemetryVariables(fakeVariableHeaders);
        var fakeData = DataFileMemory.Create(b => b.ConfigureVariables(fakeVariableHeaders.Headers));

        var fakeMmf = new FakeMemoryMappedDataFile(fakeData.Memory);
        var connectionDataFile = new ConnectionDataFile(fakeMmf);
        var outerConnectionTracker = new OuterConnectionTracker(closeOnEmpty: true);

        var mocks = new MockRepository(MockBehavior.Strict);

        // Configure the open inner connection mocks so we can expose our data file to the outer connection
        var openInnerConnectionMocks = new OpenInnerConnectionMocks(mocks);

        openInnerConnectionMocks.ConnectionOwner.Setup(x => x.NewConnectionId()).Returns(1);

        var openInnerConnection = new OpenInnerConnection(
            openInnerConnectionMocks.ConnectionOwner.Object,
            connectionDataFile,
            outerConnectionTracker,
            openInnerConnectionMocks.ClosedConnectionFactory.Object,
            openInnerConnectionMocks.WorkerThreadFactory.Object,
            TimeProvider.System);

        // Configure the outer connection
        var connectionMocks = new SimulatorConnectionMock(mocks);

        connectionMocks.ConnectionManager.Setup(x => x.Connect(It.IsAny<IOuterConnection>(), It.IsAny<TimeSpan>()));
        connectionMocks.TelemetryVariableInfoProvider.Setup(x => x.OnTelemetryVariablesActivated(It.IsAny<ISimulatorConnection>()));

        // Create the outer connection object
        var connection = connectionMocks.CreateInstance();
        var outerConnection = (IOuterConnection)connection;

        // Transition the outer connection to the opened connection state so we can read from our data file
        connection.Open();
        outerConnection.SetOpenInnerConnection(openInnerConnection);

        Assert.Equal(SimulatorConnectionState.Open, connection.State);

        // Update the session info string
        var sessionInfoStr = "Testing123";
        var sessionInfoVersion = 3;
        fakeData.UpdateSessionInfo(sessionInfoStr, sessionInfoVersion);

        var sessionInfoVersionResult = SessionInfoString.ReadVersion(connection);

        Assert.Equal(sessionInfoVersion, sessionInfoVersionResult);
    }

    [Fact]
    public void ReadVersion_ThrowOnCanReadEqualsFalseTest()
    {
        var mocks = new MockRepository(MockBehavior.Strict);

        var connectionMock = mocks.Create<ISimulatorConnection>();
        connectionMock.SetupGet(x => x.CanRead).Returns(false);

        Assert.Throws<InvalidOperationException>(() => SessionInfoString.ReadVersion(connectionMock.Object));
    }

    [Fact]
    public void ReadVersion_ThrowOnNullConnectionTest()
    {
        Assert.Throws<ArgumentNullException>(() => SessionInfoString.ReadVersion(null!));
    }
}
