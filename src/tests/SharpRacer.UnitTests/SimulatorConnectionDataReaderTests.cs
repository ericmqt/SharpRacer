using Moq;
using SharpRacer.Interop;
using SharpRacer.IO;
using SharpRacer.Testing.IO;
using SharpRacer.Testing.Telemetry;

namespace SharpRacer;

public class SimulatorConnectionDataReaderTests
{
    public static TheoryData<int, int, byte> ActiveTelemetryBufferTestData_MaxBuffers { get; }
        = new TheoryData<int, int, byte>()
        {
                { 0, 68910, 0x3C },
                { 1, 97147, 0xAD },
                { 2, 88314, 0xF3 },
                { 3, 88314, 0x42 },
        };

    public static TheoryData<int, int, byte> ActiveTelemetryBufferTestData_ThreeBuffers { get; }
        = new TheoryData<int, int, byte>()
        {
                { 0, 68910, 0x3C },
                { 1, 97147, 0xAD },
                { 2, 88314, 0xF3 }
        };

    [Fact]
    public void Ctor_Test()
    {
        var mocks = new MockRepository(MockBehavior.Strict);

        var fakeData = DataFileMemory.Create(b => b.ConfigureVariables(new FakeTelemetryVariableHeaders().Headers));

        var connectionDataHandle = mocks.Create<IConnectionDataHandle>();
        connectionDataHandle.SetupGet(x => x.Memory).Returns(fakeData.Memory);

        var connection = mocks.Create<ISimulatorConnection>();
        connection.SetupGet(x => x.State).Returns(SimulatorConnectionState.Open);
        connection.Setup(x => x.AcquireDataHandle()).Returns(connectionDataHandle.Object);

        var reader = new SimulatorConnectionDataReader(connection.Object);

        connection.VerifyGet(x => x.State, Times.Once);
        connection.Verify(x => x.AcquireDataHandle(), Times.Once);
    }

    [Fact]
    public void Ctor_ThrowsIfConnectionIsNullTest()
    {
        Assert.Throws<ArgumentNullException>(() => new SimulatorConnectionDataReader(connection: null!));
    }

    [Fact]
    public void Ctor_ThrowIfConnectionIsNotOpenTest()
    {
        var mocks = new MockRepository(MockBehavior.Strict);

        var fakeData = DataFileMemory.Create(b => b.ConfigureVariables(new FakeTelemetryVariableHeaders().Headers));

        var connectionDataHandle = mocks.Create<IConnectionDataHandle>();
        connectionDataHandle.SetupGet(x => x.Memory).Returns(fakeData.Memory);

        var connection = mocks.Create<ISimulatorConnection>();

        connection.Setup(x => x.AcquireDataHandle()).Returns(connectionDataHandle.Object);

        // SimulatorConnectionState.None
        connection.SetupGet(x => x.State).Returns(SimulatorConnectionState.None);
        Assert.Throws<ArgumentException>(() => new SimulatorConnectionDataReader(connection.Object));

        // SimulatorConnectionState.Connecting
        connection.SetupGet(x => x.State).Returns(SimulatorConnectionState.Connecting);
        Assert.Throws<ArgumentException>(() => new SimulatorConnectionDataReader(connection.Object));

        // SimulatorConnectionState.Closed
        connection.SetupGet(x => x.State).Returns(SimulatorConnectionState.Closed);
        Assert.Throws<ArgumentException>(() => new SimulatorConnectionDataReader(connection.Object));
    }

    [Fact]
    public void Dispose_Test()
    {
        var mocks = new MockRepository(MockBehavior.Strict);

        var fakeData = DataFileMemory.Create(b => b.ConfigureVariables(new FakeTelemetryVariableHeaders().Headers));

        var connectionDataHandle = mocks.Create<IConnectionDataHandle>();
        connectionDataHandle.SetupGet(x => x.Memory).Returns(fakeData.Memory);
        connectionDataHandle.Setup(x => x.Dispose());

        var connection = mocks.Create<ISimulatorConnection>();
        connection.SetupGet(x => x.State).Returns(SimulatorConnectionState.Open);
        connection.Setup(x => x.AcquireDataHandle()).Returns(connectionDataHandle.Object);
        connection.Setup(x => x.Dispose());

        var reader = new SimulatorConnectionDataReader(connection.Object);

        reader.Dispose();

        // Reader should never dispose connection, only the data handle
        connection.Verify(x => x.Dispose(), Times.Never);
        connectionDataHandle.Verify(x => x.Dispose(), Times.Once);
    }

    [Fact]
    public void GetSessionInfoStringSpanTest()
    {
        var mocks = new MockRepository(MockBehavior.Strict);

        var fakeData = DataFileMemory.Create(b => b.ConfigureVariables(new FakeTelemetryVariableHeaders().Headers));

        var connectionDataHandle = mocks.Create<IConnectionDataHandle>();
        connectionDataHandle.SetupGet(x => x.Memory).Returns(fakeData.Memory);

        var connection = mocks.Create<ISimulatorConnection>();
        connection.SetupGet(x => x.State).Returns(SimulatorConnectionState.Open);
        connection.Setup(x => x.AcquireDataHandle()).Returns(connectionDataHandle.Object);

        var reader = new SimulatorConnectionDataReader(connection.Object);

        // Update the session info string
        var sessionInfoStr = "Testing123";
        var sessionInfoStrBytes = SessionInfoString.Encoding.GetBytes(sessionInfoStr);
        fakeData.UpdateSessionInfo(sessionInfoStr);

        var sessionInfoSpan = reader.GetSessionInfoStringSpan();

        Assert.Equal(sessionInfoStrBytes.Length, sessionInfoSpan.Length);
        Assert.True(sessionInfoStrBytes.SequenceEqual(sessionInfoSpan));

        var sessionInfoStringResult = SessionInfoString.Encoding.GetString(sessionInfoSpan);

        Assert.Equal(sessionInfoStr, sessionInfoStringResult);
    }

    [Fact]
    public void GetSessionInfoStringSpan_EmptySpanTest()
    {
        var mocks = new MockRepository(MockBehavior.Strict);

        var fakeData = DataFileMemory.Create(b => b.ConfigureVariables(new FakeTelemetryVariableHeaders().Headers));

        var connectionDataHandle = mocks.Create<IConnectionDataHandle>();
        connectionDataHandle.SetupGet(x => x.Memory).Returns(fakeData.Memory);

        var connection = mocks.Create<ISimulatorConnection>();
        connection.SetupGet(x => x.State).Returns(SimulatorConnectionState.Open);
        connection.Setup(x => x.AcquireDataHandle()).Returns(connectionDataHandle.Object);

        var reader = new SimulatorConnectionDataReader(connection.Object);

        // Session info string should be initialized to zero length if we don't update it first
        var sessionInfoSpan = reader.GetSessionInfoStringSpan();

        Assert.Equal(0, sessionInfoSpan.Length);
    }

    [Fact]
    public void GetSessionInfoStringSpan_ThrowIfDisposedTest()
    {
        var mocks = new MockRepository(MockBehavior.Strict);

        var fakeData = DataFileMemory.Create(b => b.ConfigureVariables(new FakeTelemetryVariableHeaders().Headers));

        var connectionDataHandle = mocks.Create<IConnectionDataHandle>();
        connectionDataHandle.SetupGet(x => x.Memory).Returns(fakeData.Memory);
        connectionDataHandle.Setup(x => x.Dispose());

        var connection = mocks.Create<ISimulatorConnection>();
        connection.SetupGet(x => x.State).Returns(SimulatorConnectionState.Open);
        connection.Setup(x => x.AcquireDataHandle()).Returns(connectionDataHandle.Object);

        var reader = new SimulatorConnectionDataReader(connection.Object);

        reader.Dispose();

        Assert.Throws<ObjectDisposedException>(() => reader.GetSessionInfoStringSpan());
    }

    [Fact]
    public void GetTelemetryBufferLengthTest()
    {
        var mocks = new MockRepository(MockBehavior.Strict);

        var fakeVariableHeaders = new FakeTelemetryVariableHeaders();
        var fakeData = DataFileMemory.Create(b => b.ConfigureVariables(fakeVariableHeaders.Headers));

        var connectionDataHandle = mocks.Create<IConnectionDataHandle>();
        connectionDataHandle.SetupGet(x => x.Memory).Returns(fakeData.Memory);

        var connection = mocks.Create<ISimulatorConnection>();
        connection.SetupGet(x => x.State).Returns(SimulatorConnectionState.Open);
        connection.Setup(x => x.AcquireDataHandle()).Returns(connectionDataHandle.Object);

        var reader = new SimulatorConnectionDataReader(connection.Object);

        Assert.Equal(fakeVariableHeaders.BufferSize, reader.GetTelemetryBufferLength());
    }

    [Fact]
    public void GetTelemetryBufferLength_ThrowIfDisposedTest()
    {
        var mocks = new MockRepository(MockBehavior.Strict);

        var fakeData = DataFileMemory.Create(b => b.ConfigureVariables(new FakeTelemetryVariableHeaders().Headers));

        var connectionDataHandle = mocks.Create<IConnectionDataHandle>();
        connectionDataHandle.SetupGet(x => x.Memory).Returns(fakeData.Memory);
        connectionDataHandle.Setup(x => x.Dispose());

        var connection = mocks.Create<ISimulatorConnection>();
        connection.SetupGet(x => x.State).Returns(SimulatorConnectionState.Open);
        connection.Setup(x => x.AcquireDataHandle()).Returns(connectionDataHandle.Object);

        var reader = new SimulatorConnectionDataReader(connection.Object);

        reader.Dispose();

        Assert.Throws<ObjectDisposedException>(() => reader.GetTelemetryBufferLength());
    }

    [Fact]
    public void GetHeaderRefTest()
    {
        var mocks = new MockRepository(MockBehavior.Strict);

        var fakeData = DataFileMemory.Create(b => b.ConfigureVariables(new FakeTelemetryVariableHeaders().Headers));

        var connectionDataHandle = mocks.Create<IConnectionDataHandle>();
        connectionDataHandle.SetupGet(x => x.Memory).Returns(fakeData.Memory);

        var connection = mocks.Create<ISimulatorConnection>();
        connection.SetupGet(x => x.State).Returns(SimulatorConnectionState.Open);
        connection.Setup(x => x.AcquireDataHandle()).Returns(connectionDataHandle.Object);

        var reader = new SimulatorConnectionDataReader(connection.Object);

        ref readonly var headerRef = ref reader.GetHeaderRef();

        Assert.Equal(0, headerRef.Status);

        // Update the underlying data and check that the value of the read-only field changed. This ensures we did not copy the header by
        // value somehow.
        fakeData.UpdateFileHeader((ref h) => h.SetStatus(1));

        Assert.Equal(1, headerRef.Status);
    }

    [Fact]
    public void GetHeaderRef_ThrowIfDisposedTest()
    {
        var mocks = new MockRepository(MockBehavior.Strict);

        var fakeData = DataFileMemory.Create(b => b.ConfigureVariables(new FakeTelemetryVariableHeaders().Headers));

        var connectionDataHandle = mocks.Create<IConnectionDataHandle>();
        connectionDataHandle.SetupGet(x => x.Memory).Returns(fakeData.Memory);
        connectionDataHandle.Setup(x => x.Dispose());

        var connection = mocks.Create<ISimulatorConnection>();
        connection.SetupGet(x => x.State).Returns(SimulatorConnectionState.Open);
        connection.Setup(x => x.AcquireDataHandle()).Returns(connectionDataHandle.Object);

        var reader = new SimulatorConnectionDataReader(connection.Object);

        reader.Dispose();

        Assert.Throws<ObjectDisposedException>(() => reader.GetHeaderRef());
    }

    [Theory]
    [MemberData(nameof(ActiveTelemetryBufferTestData_MaxBuffers))]
    public void ReadActiveTelemetryBufferTest(int index, int tickCount, byte bufferFillByte)
    {
        var fakeData = DataFileMemory.Create(b => b.ConfigureVariables(new FakeTelemetryVariableHeaders().Headers));

        var mocks = new MockRepository(MockBehavior.Strict);

        var connectionDataHandle = mocks.Create<IConnectionDataHandle>();
        connectionDataHandle.SetupGet(x => x.Memory).Returns(fakeData.Memory);

        var connection = mocks.Create<ISimulatorConnection>();
        connection.SetupGet(x => x.State).Returns(SimulatorConnectionState.Open);
        connection.Setup(x => x.AcquireDataHandle()).Returns(connectionDataHandle.Object);

        // Configure header to indicate that all four buffers are used
        fakeData.UpdateFileHeader((ref h) => h.SetTelemetryBufferCount(DataFileConstants.MaxTelemetryBuffers));

        // Write a telemetry buffer
        var randomTickSource = new Random();

        fakeData.UpdateTelemetryBufferTickCount(index: 0, tickCount: randomTickSource.Next(minValue: 1, maxValue: tickCount - 1));
        fakeData.UpdateTelemetryBufferTickCount(index: 1, tickCount: randomTickSource.Next(minValue: 1, maxValue: tickCount - 1));
        fakeData.UpdateTelemetryBufferTickCount(index: 2, tickCount: randomTickSource.Next(minValue: 1, maxValue: tickCount - 1));
        fakeData.UpdateTelemetryBufferTickCount(index: 3, tickCount: randomTickSource.Next(minValue: 1, maxValue: tickCount - 1));

        // Configure the active telemetry buffer
        fakeData.UpdateTelemetryBufferTickCount(index, tickCount);
        fakeData.WriteTelemetryBuffer(index, (ref tb) => tb.Span.Fill(bufferFillByte));

        var reader = new SimulatorConnectionDataReader(connection.Object);

        var bufferData = reader.ReadActiveTelemetryBuffer();

        Assert.True(bufferData.All(b => b == bufferFillByte));
    }

    [Theory]
    [MemberData(nameof(ActiveTelemetryBufferTestData_ThreeBuffers))]
    public void ReadActiveTelemetryBuffer_OnlyThreeUsedBuffersTest(int index, int tickCount, byte bufferFillByte)
    {
        var fakeData = DataFileMemory.Create(b => b.ConfigureVariables(new FakeTelemetryVariableHeaders().Headers));

        var mocks = new MockRepository(MockBehavior.Strict);

        var connectionDataHandle = mocks.Create<IConnectionDataHandle>();
        connectionDataHandle.SetupGet(x => x.Memory).Returns(fakeData.Memory);

        var connection = mocks.Create<ISimulatorConnection>();
        connection.SetupGet(x => x.State).Returns(SimulatorConnectionState.Open);
        connection.Setup(x => x.AcquireDataHandle()).Returns(connectionDataHandle.Object);

        // Configure header to indicate that all four buffers are used
        fakeData.UpdateFileHeader((ref h) => h.SetTelemetryBufferCount(3));

        // Write a telemetry buffer
        var randomTickSource = new Random();

        fakeData.UpdateTelemetryBufferTickCount(index: 0, tickCount: randomTickSource.Next(minValue: 1, maxValue: tickCount - 1));
        fakeData.UpdateTelemetryBufferTickCount(index: 1, tickCount: randomTickSource.Next(minValue: 1, maxValue: tickCount - 1));
        fakeData.UpdateTelemetryBufferTickCount(index: 2, tickCount: randomTickSource.Next(minValue: 1, maxValue: tickCount - 1));
        fakeData.UpdateTelemetryBufferTickCount(index: 3, tickCount: int.MaxValue - 1);

        // Configure the active telemetry buffer
        fakeData.UpdateTelemetryBufferTickCount(index, tickCount);
        fakeData.WriteTelemetryBuffer(index, (ref tb) => tb.Span.Fill(bufferFillByte));

        var reader = new SimulatorConnectionDataReader(connection.Object);

        var bufferData = reader.ReadActiveTelemetryBuffer(out var tickCountResult);

        Assert.Equal(tickCount, tickCountResult);
        Assert.True(bufferData.All(b => b == bufferFillByte));
    }

    [Theory]
    [MemberData(nameof(ActiveTelemetryBufferTestData_MaxBuffers))]
    public void ReadActiveTelemetryBuffer_SpanDestinationTest(int index, int tickCount, byte bufferFillByte)
    {
        var fakeData = DataFileMemory.Create(b => b.ConfigureVariables(new FakeTelemetryVariableHeaders().Headers));

        var mocks = new MockRepository(MockBehavior.Strict);

        var connectionDataHandle = mocks.Create<IConnectionDataHandle>();
        connectionDataHandle.SetupGet(x => x.Memory).Returns(fakeData.Memory);

        var connection = mocks.Create<ISimulatorConnection>();
        connection.SetupGet(x => x.State).Returns(SimulatorConnectionState.Open);
        connection.Setup(x => x.AcquireDataHandle()).Returns(connectionDataHandle.Object);

        // Configure header to indicate that all four buffers are used
        fakeData.UpdateFileHeader((ref h) => h.SetTelemetryBufferCount(DataFileConstants.MaxTelemetryBuffers));

        // Write a telemetry buffer
        var randomTickSource = new Random();

        fakeData.UpdateTelemetryBufferTickCount(index: 0, tickCount: randomTickSource.Next(minValue: 1, maxValue: tickCount - 1));
        fakeData.UpdateTelemetryBufferTickCount(index: 1, tickCount: randomTickSource.Next(minValue: 1, maxValue: tickCount - 1));
        fakeData.UpdateTelemetryBufferTickCount(index: 2, tickCount: randomTickSource.Next(minValue: 1, maxValue: tickCount - 1));
        fakeData.UpdateTelemetryBufferTickCount(index: 3, tickCount: randomTickSource.Next(minValue: 1, maxValue: tickCount - 1));

        // Configure the active telemetry buffer
        fakeData.UpdateTelemetryBufferTickCount(index, tickCount);
        fakeData.WriteTelemetryBuffer(index, (ref tb) => tb.Span.Fill(bufferFillByte));

        var reader = new SimulatorConnectionDataReader(connection.Object);

        var destinationBuffer = new byte[reader.GetTelemetryBufferLength()];

        Assert.False(destinationBuffer.All(b => b == bufferFillByte));

        reader.ReadActiveTelemetryBuffer(destinationBuffer);
        Assert.True(destinationBuffer.All(b => b == bufferFillByte));

        // Now read it again with the tickCount out param
        destinationBuffer = new byte[reader.GetTelemetryBufferLength()];
        Assert.False(destinationBuffer.All(b => b == bufferFillByte));

        reader.ReadActiveTelemetryBuffer(destinationBuffer, out var tickCountResult);

        Assert.Equal(tickCount, tickCountResult);
        Assert.True(destinationBuffer.All(b => b == bufferFillByte));
    }

    [Theory]
    [MemberData(nameof(ActiveTelemetryBufferTestData_MaxBuffers))]
    public void ReadActiveTelemetryBuffer_TickCountOutParameterTest(int index, int tickCount, byte bufferFillByte)
    {
        var fakeData = DataFileMemory.Create(b => b.ConfigureVariables(new FakeTelemetryVariableHeaders().Headers));

        var mocks = new MockRepository(MockBehavior.Strict);

        var connectionDataHandle = mocks.Create<IConnectionDataHandle>();
        connectionDataHandle.SetupGet(x => x.Memory).Returns(fakeData.Memory);

        var connection = mocks.Create<ISimulatorConnection>();
        connection.SetupGet(x => x.State).Returns(SimulatorConnectionState.Open);
        connection.Setup(x => x.AcquireDataHandle()).Returns(connectionDataHandle.Object);

        // Configure header to indicate that all four buffers are used
        fakeData.UpdateFileHeader((ref h) => h.SetTelemetryBufferCount(DataFileConstants.MaxTelemetryBuffers));

        // Write a telemetry buffer
        var randomTickSource = new Random();

        fakeData.UpdateTelemetryBufferTickCount(index: 0, tickCount: randomTickSource.Next(minValue: 1, maxValue: tickCount - 1));
        fakeData.UpdateTelemetryBufferTickCount(index: 1, tickCount: randomTickSource.Next(minValue: 1, maxValue: tickCount - 1));
        fakeData.UpdateTelemetryBufferTickCount(index: 2, tickCount: randomTickSource.Next(minValue: 1, maxValue: tickCount - 1));
        fakeData.UpdateTelemetryBufferTickCount(index: 3, tickCount: randomTickSource.Next(minValue: 1, maxValue: tickCount - 1));

        // Configure the active telemetry buffer
        fakeData.UpdateTelemetryBufferTickCount(index, tickCount);
        fakeData.WriteTelemetryBuffer(index, (ref tb) => tb.Span.Fill(bufferFillByte));

        var reader = new SimulatorConnectionDataReader(connection.Object);

        var bufferData = reader.ReadActiveTelemetryBuffer(out var tickCountResult);

        Assert.Equal(tickCount, tickCountResult);
        Assert.True(bufferData.All(b => b == bufferFillByte));
    }

    [Fact]
    public void ReadHeaderTest()
    {
        var mocks = new MockRepository(MockBehavior.Strict);

        var fakeData = DataFileMemory.Create(b => b.ConfigureVariables(new FakeTelemetryVariableHeaders().Headers));

        var connectionDataHandle = mocks.Create<IConnectionDataHandle>();
        connectionDataHandle.SetupGet(x => x.Memory).Returns(fakeData.Memory);

        var connection = mocks.Create<ISimulatorConnection>();
        connection.SetupGet(x => x.State).Returns(SimulatorConnectionState.Open);
        connection.Setup(x => x.AcquireDataHandle()).Returns(connectionDataHandle.Object);

        var reader = new SimulatorConnectionDataReader(connection.Object);

        var header = reader.ReadHeader();

        Assert.Equal(0, header.Status);

        // Update the underlying data and ensure the Status field did not change because we took a by-value copy of it
        fakeData.UpdateFileHeader((ref h) => h.SetStatus(1));

        Assert.Equal(0, header.Status);
    }

    [Fact]
    public void ReadHeader_ThrowIfDisposedTest()
    {
        var mocks = new MockRepository(MockBehavior.Strict);

        var fakeData = DataFileMemory.Create(b => b.ConfigureVariables(new FakeTelemetryVariableHeaders().Headers));

        var connectionDataHandle = mocks.Create<IConnectionDataHandle>();
        connectionDataHandle.SetupGet(x => x.Memory).Returns(fakeData.Memory);
        connectionDataHandle.Setup(x => x.Dispose());

        var connection = mocks.Create<ISimulatorConnection>();
        connection.SetupGet(x => x.State).Returns(SimulatorConnectionState.Open);
        connection.Setup(x => x.AcquireDataHandle()).Returns(connectionDataHandle.Object);

        var reader = new SimulatorConnectionDataReader(connection.Object);

        reader.Dispose();

        Assert.Throws<ObjectDisposedException>(() => reader.ReadHeader());
    }

    [Fact]
    public void ReadSessionInfoStringTest()
    {
        var mocks = new MockRepository(MockBehavior.Strict);

        var fakeData = DataFileMemory.Create(b => b.ConfigureVariables(new FakeTelemetryVariableHeaders().Headers));

        var connectionDataHandle = mocks.Create<IConnectionDataHandle>();
        connectionDataHandle.SetupGet(x => x.Memory).Returns(fakeData.Memory);

        var connection = mocks.Create<ISimulatorConnection>();
        connection.SetupGet(x => x.State).Returns(SimulatorConnectionState.Open);
        connection.Setup(x => x.AcquireDataHandle()).Returns(connectionDataHandle.Object);

        var reader = new SimulatorConnectionDataReader(connection.Object);

        // Update the session info string
        var sessionInfoStr = "Testing123";
        var sessionInfoVersion = 3;
        fakeData.UpdateSessionInfo(sessionInfoStr, sessionInfoVersion);

        var sessionInfoStringResult = reader.ReadSessionInfoString();

        Assert.Equal(sessionInfoStr, sessionInfoStringResult);
    }

    [Fact]
    public void ReadSessionInfoString_ThrowIfDisposedTest()
    {
        var mocks = new MockRepository(MockBehavior.Strict);

        var fakeData = DataFileMemory.Create(b => b.ConfigureVariables(new FakeTelemetryVariableHeaders().Headers));

        var connectionDataHandle = mocks.Create<IConnectionDataHandle>();
        connectionDataHandle.SetupGet(x => x.Memory).Returns(fakeData.Memory);
        connectionDataHandle.Setup(x => x.Dispose());

        var connection = mocks.Create<ISimulatorConnection>();
        connection.SetupGet(x => x.State).Returns(SimulatorConnectionState.Open);
        connection.Setup(x => x.AcquireDataHandle()).Returns(connectionDataHandle.Object);

        var reader = new SimulatorConnectionDataReader(connection.Object);

        reader.Dispose();

        Assert.Throws<ObjectDisposedException>(() => reader.ReadSessionInfoString());
        Assert.Throws<ObjectDisposedException>(() => reader.ReadSessionInfoString(out _));
    }

    [Fact]
    public void ReadSessionInfoString_VersionOutParameterTest()
    {
        var mocks = new MockRepository(MockBehavior.Strict);

        var fakeData = DataFileMemory.Create(b => b.ConfigureVariables(new FakeTelemetryVariableHeaders().Headers));

        var connectionDataHandle = mocks.Create<IConnectionDataHandle>();
        connectionDataHandle.SetupGet(x => x.Memory).Returns(fakeData.Memory);

        var connection = mocks.Create<ISimulatorConnection>();
        connection.SetupGet(x => x.State).Returns(SimulatorConnectionState.Open);
        connection.Setup(x => x.AcquireDataHandle()).Returns(connectionDataHandle.Object);

        var reader = new SimulatorConnectionDataReader(connection.Object);

        // Update the session info string
        var sessionInfoStr = "Testing123";
        var sessionInfoVersion = 3;
        fakeData.UpdateSessionInfo(sessionInfoStr, sessionInfoVersion);

        var sessionInfoStringResult = reader.ReadSessionInfoString(out var sessionInfoVersionResult);

        Assert.Equal(sessionInfoStr, sessionInfoStringResult);
        Assert.Equal(sessionInfoVersion, sessionInfoVersionResult);
    }

    [Fact]
    public void ReadTelemetryVariableHeadersTest()
    {
        var fakeVariableHeaders = new FakeTelemetryVariableHeaders();
        var fakeVariables = new FakeTelemetryVariables(fakeVariableHeaders);
        var fakeData = DataFileMemory.Create(b => b.ConfigureVariables(fakeVariableHeaders.Headers));

        var mocks = new MockRepository(MockBehavior.Strict);

        var connectionDataHandle = mocks.Create<IConnectionDataHandle>();
        connectionDataHandle.SetupGet(x => x.Memory).Returns(fakeData.Memory);

        var connection = mocks.Create<ISimulatorConnection>();
        connection.SetupGet(x => x.State).Returns(SimulatorConnectionState.Open);
        connection.Setup(x => x.AcquireDataHandle()).Returns(connectionDataHandle.Object);

        var reader = new SimulatorConnectionDataReader(connection.Object);

        var result = reader.ReadTelemetryVariableHeaders();

        Assert.Equal(fakeVariableHeaders.Headers.Length, result.Length);
        Assert.True(fakeVariableHeaders.Headers.SequenceEqual(result));
    }

    [Fact]
    public void ReadTelemetryVariableHeaders_NoVariablesTest()
    {
        var fakeData = DataFileMemory.Create(b => { });

        var mocks = new MockRepository(MockBehavior.Strict);

        var connectionDataHandle = mocks.Create<IConnectionDataHandle>();
        connectionDataHandle.SetupGet(x => x.Memory).Returns(fakeData.Memory);

        var connection = mocks.Create<ISimulatorConnection>();
        connection.SetupGet(x => x.State).Returns(SimulatorConnectionState.Open);
        connection.Setup(x => x.AcquireDataHandle()).Returns(connectionDataHandle.Object);

        var reader = new SimulatorConnectionDataReader(connection.Object);

        var result = reader.ReadTelemetryVariableHeaders();

        Assert.NotNull(result);
        Assert.Empty(result);
    }
}
