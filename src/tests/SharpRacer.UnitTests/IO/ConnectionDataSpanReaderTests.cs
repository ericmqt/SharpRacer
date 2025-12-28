using Moq;
using SharpRacer.Interop;
using SharpRacer.IO.Internal;
using SharpRacer.Testing.IO;
using SharpRacer.Testing.Telemetry;

namespace SharpRacer.IO;

public class ConnectionDataSpanReaderTests
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
    public void Ctor_SpanTest()
    {
        var fakeVariableHeaders = new FakeTelemetryVariableHeaders();
        var fakeVariables = new FakeTelemetryVariables(fakeVariableHeaders);
        var fakeData = DataFileMemory.Create(b => b.ConfigureVariables(fakeVariableHeaders.Headers));

        var reader = new ConnectionDataSpanReader(fakeData.Memory.Span);

        Assert.False(reader.IsHandleOwnedByReader);
    }

    [Fact]
    public void Ctor_SpanHandleTest()
    {
        var fakeVariableHeaders = new FakeTelemetryVariableHeaders();
        var fakeVariables = new FakeTelemetryVariables(fakeVariableHeaders);
        var fakeData = DataFileMemory.Create(b => b.ConfigureVariables(fakeVariableHeaders.Headers));

        var handleOwnerReader = new ConnectionDataSpanReader(
            ConnectionDataSpanHandle.Ownerless(fakeData.Memory.Span), isHandleOwnedByReader: true);

        Assert.True(handleOwnerReader.IsHandleOwnedByReader);

        var ownerlessHandleReader = new ConnectionDataSpanReader(
            ConnectionDataSpanHandle.Ownerless(fakeData.Memory.Span), isHandleOwnedByReader: false);

        Assert.False(ownerlessHandleReader.IsHandleOwnedByReader);
    }

    [Fact]
    public void Ctor_ThrowIfConnectionIsNullTest()
    {
        Assert.Throws<ArgumentNullException>(() => new ConnectionDataSpanReader(connection: null!));
    }

    [Fact]
    public void Ctor_ThrowIfConnectionIsNotOpenTest()
    {
        var mocks = new MockRepository(MockBehavior.Strict);

        var connection = mocks.Create<ISimulatorConnection>();

        // SimulatorConnectionState.None
        connection.SetupGet(x => x.State).Returns(SimulatorConnectionState.None);
        Assert.Throws<ArgumentException>(() => new ConnectionDataSpanReader(connection.Object));

        // SimulatorConnectionState.Connecting
        connection.SetupGet(x => x.State).Returns(SimulatorConnectionState.Connecting);
        Assert.Throws<ArgumentException>(() => new ConnectionDataSpanReader(connection.Object));

        // SimulatorConnectionState.Closed
        connection.SetupGet(x => x.State).Returns(SimulatorConnectionState.Closed);
        Assert.Throws<ArgumentException>(() => new ConnectionDataSpanReader(connection.Object));
    }

    [Fact]
    public void Ctor_ThrowIfSpanLengthBelowMinimumTest()
    {
        var zeroLength = Array.Empty<byte>();
        var oneLength = new byte[1] { 0x01 };
        var oneMinusLength = new byte[DataFileHeader.Size - 1];
        var validLength = new byte[DataFileHeader.Size];

        Array.Fill<byte>(oneMinusLength, 0x01);
        Array.Fill<byte>(validLength, 0x01);

        Assert.Throws<ArgumentException>(
            () => new ConnectionDataSpanReader(ConnectionDataSpanHandle.Ownerless(zeroLength), isHandleOwnedByReader: true));

        Assert.Throws<ArgumentException>(
            () => new ConnectionDataSpanReader(ConnectionDataSpanHandle.Ownerless(oneLength), isHandleOwnedByReader: true));

        Assert.Throws<ArgumentException>(
            () => new ConnectionDataSpanReader(ConnectionDataSpanHandle.Ownerless(oneMinusLength), isHandleOwnedByReader: true));

        var validReader = new ConnectionDataSpanReader(ConnectionDataSpanHandle.Ownerless(validLength), isHandleOwnedByReader: true);
    }

    [Fact]
    public void Dispose_Test()
    {
        var fakeVariableHeaders = new FakeTelemetryVariableHeaders();
        var fakeVariables = new FakeTelemetryVariables(fakeVariableHeaders);
        var fakeData = DataFileMemory.Create(b => b.ConfigureVariables(fakeVariableHeaders.Headers));

        int ownerReturnCount = 0;
        ulong expectedTokenId = 2;

        var connectionSpanOwner = new FakeConnectionDataSpanOwner(expectedTokenId, fakeData.Memory, onOwnerReturned);

        var spanHandle = connectionSpanOwner.AcquireSpanHandle();

        var reader = new ConnectionDataSpanReader(spanHandle, isHandleOwnedByReader: true);

        Assert.True(reader.IsHandleOwnedByReader);
        Assert.Equal(0, ownerReturnCount);

        reader.Dispose();

        Assert.Equal(1, ownerReturnCount);

        void onOwnerReturned(ConnectionDataSpanHandleToken ownerToken)
        {
            Assert.Equal(expectedTokenId, ownerToken.Id);
            ownerReturnCount++;
        }
    }

    [Fact]
    public void Dispose_OwnerlessHandleTest()
    {
        var fakeVariableHeaders = new FakeTelemetryVariableHeaders();
        var fakeVariables = new FakeTelemetryVariables(fakeVariableHeaders);
        var fakeData = DataFileMemory.Create(b => b.ConfigureVariables(fakeVariableHeaders.Headers));

        ulong expectedTokenId = 2;

        var connectionSpanOwner = new FakeConnectionDataSpanOwner(expectedTokenId, fakeData.Memory, onOwnerReturned);

        var spanHandle = connectionSpanOwner.AcquireSpanHandle();

        var reader = new ConnectionDataSpanReader(spanHandle, isHandleOwnedByReader: false);

        Assert.False(reader.IsHandleOwnedByReader);

        reader.Dispose();

        void onOwnerReturned(ConnectionDataSpanHandleToken ownerToken)
        {
            Assert.Fail("ConnectionDataSpanHandleToken released when it was not expected.");
        }
    }

    [Fact]
    public void GetSessionInfoStringSpanTest()
    {
        var fakeVariableHeaders = new FakeTelemetryVariableHeaders();
        var fakeVariables = new FakeTelemetryVariables(fakeVariableHeaders);
        var fakeData = DataFileMemory.Create(b => b.ConfigureVariables(fakeVariableHeaders.Headers));

        var reader = new ConnectionDataSpanReader(fakeData.Memory.Span);

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
        var fakeVariableHeaders = new FakeTelemetryVariableHeaders();
        var fakeVariables = new FakeTelemetryVariables(fakeVariableHeaders);
        var fakeData = DataFileMemory.Create(b => b.ConfigureVariables(fakeVariableHeaders.Headers));

        var reader = new ConnectionDataSpanReader(fakeData.Memory.Span);

        // Session info string should be initialized to zero length if we don't update it first
        var sessionInfoSpan = reader.GetSessionInfoStringSpan();

        Assert.Equal(0, sessionInfoSpan.Length);
    }

    [Fact]
    public void GetTelemetryBufferLengthTest()
    {
        var fakeVariableHeaders = new FakeTelemetryVariableHeaders();
        var fakeVariables = new FakeTelemetryVariables(fakeVariableHeaders);
        var fakeData = DataFileMemory.Create(b => b.ConfigureVariables(fakeVariableHeaders.Headers));

        var reader = new ConnectionDataSpanReader(ConnectionDataSpanHandle.Ownerless(fakeData.Memory.Span), isHandleOwnedByReader: true);

        // Set TelemetryBufferElementLength to 1
        fakeData.UpdateFileHeader((ref h) => h.SetTelemetryBufferElementLength(1));

        Assert.Equal(1, reader.GetTelemetryBufferLength());

        // Change the value in the data file to ensure the reader is returning a live value

        // Set TelemetryBufferElementLength to 100
        fakeData.UpdateFileHeader((ref h) => h.SetTelemetryBufferElementLength(100));

        Assert.Equal(100, reader.GetTelemetryBufferLength());
    }

    [Fact]
    public void GetHeaderRefTest()
    {
        var fakeVariableHeaders = new FakeTelemetryVariableHeaders();
        var fakeVariables = new FakeTelemetryVariables(fakeVariableHeaders);
        var fakeData = DataFileMemory.Create(b => b.ConfigureVariables(fakeVariableHeaders.Headers));

        var reader = new ConnectionDataSpanReader(ConnectionDataSpanHandle.Ownerless(fakeData.Memory.Span), isHandleOwnedByReader: true);
        ref readonly var headerRef = ref reader.GetHeaderRef();

        fakeData.UpdateFileHeader((ref h) => h.SetStatus(0));

        Assert.Equal(0, headerRef.Status);

        // Update the underlying data and check that the value of the read-only field changed. This ensures we did not copy the header by
        // value somehow.

        fakeData.UpdateFileHeader((ref h) => h.SetStatus(1));

        Assert.Equal(1, headerRef.Status);
    }

    [Theory]
    [MemberData(nameof(ActiveTelemetryBufferTestData_MaxBuffers))]
    public void ReadActiveTelemetryBufferTest(int index, int tickCount, byte bufferFillByte)
    {
        var fakeVariableHeaders = new FakeTelemetryVariableHeaders();
        var fakeVariables = new FakeTelemetryVariables(fakeVariableHeaders);
        var fakeData = DataFileMemory.Create(b => b.ConfigureVariables(fakeVariableHeaders.Headers));

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

        var reader = new ConnectionDataSpanReader(fakeData.Memory.Span);

        var bufferData = reader.ReadActiveTelemetryBuffer();

        Assert.True(bufferData.All(b => b == bufferFillByte));
    }

    [Theory]
    [MemberData(nameof(ActiveTelemetryBufferTestData_ThreeBuffers))]
    public void ReadActiveTelemetryBuffer_OnlyThreeUsedBuffersTest(int index, int tickCount, byte bufferFillByte)
    {
        // The simulator doesn't use all four available buffers, so we should ensure that a fourth buffer is not read when the data file
        // is configured to use three, even if the fourth buffer has the highest tick count which ordinarily would indicate it is active.

        var fakeVariableHeaders = new FakeTelemetryVariableHeaders();
        var fakeVariables = new FakeTelemetryVariables(fakeVariableHeaders);
        var fakeData = DataFileMemory.Create(b => b.ConfigureVariables(fakeVariableHeaders.Headers));

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

        var reader = new ConnectionDataSpanReader(fakeData.Memory.Span);

        var bufferData = reader.ReadActiveTelemetryBuffer(out var tickCountResult);

        Assert.Equal(tickCount, tickCountResult);
        Assert.True(bufferData.All(b => b == bufferFillByte));
    }

    [Theory]
    [MemberData(nameof(ActiveTelemetryBufferTestData_MaxBuffers))]
    public void ReadActiveTelemetryBuffer_SpanDestinationTest(int index, int tickCount, byte bufferFillByte)
    {
        var fakeVariableHeaders = new FakeTelemetryVariableHeaders();
        var fakeVariables = new FakeTelemetryVariables(fakeVariableHeaders);
        var fakeData = DataFileMemory.Create(b => b.ConfigureVariables(fakeVariableHeaders.Headers));

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

        var reader = new ConnectionDataSpanReader(fakeData.Memory.Span);

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
        var fakeVariableHeaders = new FakeTelemetryVariableHeaders();
        var fakeVariables = new FakeTelemetryVariables(fakeVariableHeaders);
        var fakeData = DataFileMemory.Create(b => b.ConfigureVariables(fakeVariableHeaders.Headers));

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

        var reader = new ConnectionDataSpanReader(fakeData.Memory.Span);

        var bufferData = reader.ReadActiveTelemetryBuffer(out var tickCountResult);

        Assert.Equal(tickCount, tickCountResult);
        Assert.True(bufferData.All(b => b == bufferFillByte));
    }

    [Fact]
    public void ReadHeaderTest()
    {
        var fakeVariableHeaders = new FakeTelemetryVariableHeaders();
        var fakeVariables = new FakeTelemetryVariables(fakeVariableHeaders);
        var fakeData = DataFileMemory.Create(b => b.ConfigureVariables(fakeVariableHeaders.Headers));

        var reader = new ConnectionDataSpanReader(ConnectionDataSpanHandle.Ownerless(fakeData.Memory.Span), isHandleOwnedByReader: true);

        fakeData.UpdateFileHeader((ref h) => h.SetStatus(1));

        var header = reader.ReadHeader();

        Assert.Equal(1, header.Status);

        // Update the underlying data and ensure the Status field did not change because we took a by-value copy of it

        fakeData.UpdateFileHeader((ref h) => h.SetStatus(0));

        Assert.Equal(1, header.Status);
    }

    [Fact]
    public void ReadSessionInfoStringTest()
    {
        var fakeVariableHeaders = new FakeTelemetryVariableHeaders();
        var fakeVariables = new FakeTelemetryVariables(fakeVariableHeaders);
        var fakeData = DataFileMemory.Create(b => b.ConfigureVariables(fakeVariableHeaders.Headers));

        var reader = new ConnectionDataSpanReader(fakeData.Memory.Span);

        // Update the session info string
        var sessionInfoStr = "Testing123";
        var sessionInfoVersion = 3;
        fakeData.UpdateSessionInfo(sessionInfoStr, sessionInfoVersion);

        var sessionInfoStringResult = reader.ReadSessionInfoString();

        Assert.Equal(sessionInfoStr, sessionInfoStringResult);
    }

    [Fact]
    public void ReadSessionInfoString_VersionOutParameterTest()
    {
        var fakeVariableHeaders = new FakeTelemetryVariableHeaders();
        var fakeVariables = new FakeTelemetryVariables(fakeVariableHeaders);
        var fakeData = DataFileMemory.Create(b => b.ConfigureVariables(fakeVariableHeaders.Headers));

        var reader = new ConnectionDataSpanReader(fakeData.Memory.Span);

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

        var reader = new ConnectionDataSpanReader(fakeData.Memory.Span);

        var result = reader.ReadTelemetryVariableHeaders();

        Assert.Equal(fakeVariableHeaders.Headers.Length, result.Length);
        Assert.True(fakeVariableHeaders.Headers.SequenceEqual(result));
    }

    [Fact]
    public void ReadTelemetryVariableHeaders_NoVariablesTest()
    {
        var fakeData = DataFileMemory.Create(b => { });

        var reader = new ConnectionDataSpanReader(fakeData.Memory.Span);

        var result = reader.ReadTelemetryVariableHeaders();

        Assert.NotNull(result);
        Assert.Empty(result);
    }

    private class FakeConnectionDataSpanOwner : IConnectionDataSpanOwner
    {
        private ulong _tokenId;
        private readonly Memory<byte> _memory;
        private readonly Action<ConnectionDataSpanHandleToken> _onReturn;

        public FakeConnectionDataSpanOwner(ulong tokenId, Memory<byte> memory, Action<ConnectionDataSpanHandleToken> onReturn)
        {
            _tokenId = tokenId;
            _memory = memory;
            _onReturn = onReturn ?? throw new ArgumentNullException(nameof(onReturn));
        }

        public bool IsClosed { get; }
        public bool IsDisposed { get; }

        public void Close()
        {

        }

        public void Dispose()
        {

        }

        public ConnectionDataSpanHandle AcquireSpanHandle()
        {
            return new ConnectionDataSpanHandle(this, new ConnectionDataSpanHandleToken(_tokenId), _memory.Span);
        }

        public void ReleaseSpanHandle(ref readonly ConnectionDataSpanHandle owner)
        {
            _onReturn(owner.Token);
        }
    }
}
