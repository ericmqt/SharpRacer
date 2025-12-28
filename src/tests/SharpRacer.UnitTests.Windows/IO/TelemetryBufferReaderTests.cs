using Moq;
using SharpRacer.Internal.Connections;
using SharpRacer.IO.Internal;
using SharpRacer.Testing.IO;
using SharpRacer.Testing.Telemetry;

namespace SharpRacer.IO;

public class TelemetryBufferReaderTests
{
    [Fact]
    public void Ctor_SimulatorConnectionTest()
    {
        // We can't mock ISimulatorConnection.AcquireDataSpanHandle because it returns a ref struct, so we have to use a real
        // SimulatorConnection object and configure it with an inner connection that contains our faked data file.

        var mocks = new MockRepository(MockBehavior.Strict);
        var connectionMocks = new SimulatorConnectionMock(mocks);

        var fakeVariableHeaders = new FakeTelemetryVariableHeaders();
        var fakeData = DataFileMemory.Create(b => b.ConfigureVariables(fakeVariableHeaders.Headers));

        var fakeSpanFactory = new FakeConnectionDataSpanFactory(fakeData.Memory);

        var fakeMmf = new FakeMemoryMappedDataFile(fakeData.Memory);
        var connectionDataFile = new ConnectionDataFile(fakeMmf);

        var openInnerConnectionMocks = new OpenInnerConnectionMocks(mocks);

        openInnerConnectionMocks.ConnectionOwner.Setup(x => x.NewConnectionId()).Returns(1);

        var openInnerConnection = new OpenInnerConnection(
            openInnerConnectionMocks.ConnectionOwner.Object,
            connectionDataFile,
            openInnerConnectionMocks.OuterConnectionTracker.Object,
            openInnerConnectionMocks.ClosedConnectionFactory.Object,
            openInnerConnectionMocks.WorkerThreadFactory.Object,
            TimeProvider.System);

        connectionMocks.ConnectionManager.Setup(x => x.Connect(It.IsAny<IOuterConnection>(), It.IsAny<TimeSpan>()));
        connectionMocks.TelemetryVariableInfoProvider.Setup(x => x.OnTelemetryVariablesActivated(It.IsAny<ISimulatorConnection>()));

        // Open the connection
        var connection = connectionMocks.CreateInstance();
        var outerConnection = (IOuterConnection)connection;

        connection.Open();
        outerConnection.SetOpenInnerConnection(openInnerConnection);

        Assert.Equal(SimulatorConnectionState.Open, connection.State);

        // Create the reader
        var reader = new TelemetryBufferReader(connection);

        Assert.True(reader.IsHandleOwnedByReader);
        Assert.Equal(fakeVariableHeaders.BufferSize, reader.BufferLength);
    }

    [Fact]
    public void Ctor_ThrowIfConnectionIsNotOpenTest()
    {
        var mocks = new MockRepository(MockBehavior.Strict);

        var connection = mocks.Create<ISimulatorConnection>();

        // SimulatorConnectionState.None
        connection.SetupGet(x => x.State).Returns(SimulatorConnectionState.None);
        Assert.Throws<ArgumentException>(() => new TelemetryBufferReader(connection.Object));

        // SimulatorConnectionState.Connecting
        connection.SetupGet(x => x.State).Returns(SimulatorConnectionState.Connecting);
        Assert.Throws<ArgumentException>(() => new TelemetryBufferReader(connection.Object));

        // SimulatorConnectionState.Closed
        connection.SetupGet(x => x.State).Returns(SimulatorConnectionState.Closed);
        Assert.Throws<ArgumentException>(() => new TelemetryBufferReader(connection.Object));
    }
}
