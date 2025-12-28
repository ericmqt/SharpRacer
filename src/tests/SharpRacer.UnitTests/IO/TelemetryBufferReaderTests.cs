using Moq;
using SharpRacer.Interop;
using SharpRacer.IO.Internal;
using SharpRacer.Testing.IO;
using SharpRacer.Testing.Telemetry;

namespace SharpRacer.IO;

public class TelemetryBufferReaderTests
{
    public static TheoryData<int> TelemetryBufferIndices { get; } = [0, 1, 2, 3];

    [Fact]
    public void Ctor_ThrowIfConnectionIsNullTest()
    {
        Assert.Throws<ArgumentNullException>(() => new TelemetryBufferReader(connection: null!));
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
            () => new TelemetryBufferReader(ConnectionDataSpanHandle.Ownerless(zeroLength), isHandleOwnedByReader: true));

        Assert.Throws<ArgumentException>(
            () => new TelemetryBufferReader(ConnectionDataSpanHandle.Ownerless(oneLength), isHandleOwnedByReader: true));

        Assert.Throws<ArgumentException>(
            () => new TelemetryBufferReader(ConnectionDataSpanHandle.Ownerless(oneMinusLength), isHandleOwnedByReader: true));

        var validReader = new TelemetryBufferReader(ConnectionDataSpanHandle.Ownerless(validLength), isHandleOwnedByReader: true);
    }

    [Fact]
    public void CopyActiveBufferTest()
    {
        const int activeBufferTickCount = 3;
        const int activeBufferIndex = 2;

        var fakeVariableHeaders = new FakeTelemetryVariableHeaders();
        var fakeVariables = new FakeTelemetryVariables(fakeVariableHeaders);
        var fakeData = DataFileMemory.Create(b => b.ConfigureVariables(fakeVariableHeaders.Headers));

        // Write a telemetry buffer
        fakeData.UpdateTelemetryBufferTickCount(index: 0, tickCount: 1);
        fakeData.UpdateTelemetryBufferTickCount(index: 1, tickCount: 2);
        fakeData.UpdateTelemetryBufferTickCount(index: 2, tickCount: activeBufferTickCount);
        fakeData.UpdateTelemetryBufferTickCount(index: 3, tickCount: 0);

        fakeData.WriteTelemetryBuffer(activeBufferIndex, (ref tb) => tb.Span.Fill(0xFF));

        using var reader = new TelemetryBufferReader(fakeData.Memory.Span);

        var activeBufferData = new byte[reader.BufferLength];
        reader.CopyActiveBuffer(activeBufferData, out int tickCount);

        Assert.Equal(activeBufferTickCount, tickCount);
        Assert.True(activeBufferData.All(b => b == 0xFF));
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

        var reader = new TelemetryBufferReader(spanHandle, isHandleOwnedByReader: true);

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

        var reader = new TelemetryBufferReader(spanHandle, isHandleOwnedByReader: false);

        Assert.False(reader.IsHandleOwnedByReader);

        reader.Dispose();

        void onOwnerReturned(ConnectionDataSpanHandleToken ownerToken)
        {
            Assert.Fail("ConnectionDataSpanHandleToken released when it was not expected.");
        }
    }

    [Fact]
    public void GetActiveBufferIndexTest()
    {
        int activeBufferTickCount = 3;

        var fakeVariableHeaders = new FakeTelemetryVariableHeaders();
        var fakeVariables = new FakeTelemetryVariables(fakeVariableHeaders);
        var fakeData = DataFileMemory.Create(b => b.ConfigureVariables(fakeVariableHeaders.Headers));

        // Write a telemetry buffer
        fakeData.UpdateTelemetryBufferTickCount(index: 0, tickCount: 1);
        fakeData.UpdateTelemetryBufferTickCount(index: 1, tickCount: 2);
        fakeData.UpdateTelemetryBufferTickCount(index: 2, tickCount: activeBufferTickCount);
        fakeData.UpdateTelemetryBufferTickCount(index: 3, tickCount: 0);

        using var reader = new TelemetryBufferReader(fakeData.Memory.Span);

        Assert.Equal(2, reader.GetActiveBufferIndex());

        // Change the active buffer to index 1
        fakeData.UpdateTelemetryBufferTickCount(index: 1, tickCount: activeBufferTickCount + 1);
        Assert.Equal(1, reader.GetActiveBufferIndex());
    }

    [Theory]
    [MemberData(nameof(TelemetryBufferIndices))]
    public void GetBufferHeaderRefTest(int index)
    {
        var fakeVariableHeaders = new FakeTelemetryVariableHeaders();
        var fakeVariables = new FakeTelemetryVariables(fakeVariableHeaders);
        var fakeData = DataFileMemory.Create(b => b.ConfigureVariables(fakeVariableHeaders.Headers));

        var tickCounts = new int[] { 1, 2, 3, 0 };
        var bufferFill = new byte[] { 0x01, 0x02, 0x03, 0x04 };

        for (int i = 0; i < DataFileConstants.MaxTelemetryBuffers; i++)
        {
            fakeData.UpdateTelemetryBufferTickCount(i, tickCounts[i]);
            fakeData.WriteTelemetryBuffer(i, (ref tb) => tb.Span.Fill(bufferFill[i]));
        }

        using var reader = new TelemetryBufferReader(fakeData.Memory.Span);

        ref readonly var bufferHeader = ref reader.GetBufferHeaderRef(index);

        // Assert data, mutate the underlying data, then repeat, all against the same header ref
        for (int i = 0; i < 10; i++)
        {
            Assert.Equal(tickCounts[index], bufferHeader.TickCount);

            var bufferSpan = fakeData.Memory.Span.Slice(bufferHeader.BufferOffset, fakeVariables.BufferSize);
            var bufferSpanArray = bufferSpan.ToArray();

            Assert.True(bufferSpanArray.All(b => b == bufferFill[index]));

            for (int j = 0; j < DataFileConstants.MaxTelemetryBuffers; j++)
            {
                tickCounts[j] = tickCounts[j] + tickCounts.Length;
                bufferFill[j] = (byte)(bufferFill[j] + bufferFill.Length);

                fakeData.UpdateTelemetryBufferTickCount(j, tickCounts[j]);
                fakeData.WriteTelemetryBuffer(j, (ref tb) => tb.Span.Fill(bufferFill[j]));
            }
        }
    }

    [Theory]
    [MemberData(nameof(TelemetryBufferIndices))]
    public void TryCopyBufferTest(int index)
    {
        var fakeVariableHeaders = new FakeTelemetryVariableHeaders();
        var fakeVariables = new FakeTelemetryVariables(fakeVariableHeaders);
        var fakeData = DataFileMemory.Create(b => b.ConfigureVariables(fakeVariableHeaders.Headers));

        var tickCounts = new int[] { 1, 2, 3, 0 };
        var bufferFill = new byte[] { 0x01, 0x02, 0x03, 0x04 };

        for (int i = 0; i < DataFileConstants.MaxTelemetryBuffers; i++)
        {
            fakeData.UpdateTelemetryBufferTickCount(i, tickCounts[i]);
            fakeData.WriteTelemetryBuffer(i, (ref tb) => tb.Span.Fill(bufferFill[i]));
        }

        using var reader = new TelemetryBufferReader(fakeData.Memory.Span);

        ref readonly var bufferHeader = ref reader.GetBufferHeaderRef(index);
        var buffer = new byte[reader.BufferLength];

        Assert.True(reader.TryCopyBuffer(in bufferHeader, buffer, out var tickCount));
        Assert.True(buffer.All(b => b == bufferFill[index]));
        Assert.Equal(tickCounts[index], tickCount);
    }

    [Fact]
    public void TryCopyActiveBufferTest()
    {
        const int activeBufferTickCount = 3;
        const int activeBufferIndex = 2;

        var fakeVariableHeaders = new FakeTelemetryVariableHeaders();
        var fakeVariables = new FakeTelemetryVariables(fakeVariableHeaders);
        var fakeData = DataFileMemory.Create(b => b.ConfigureVariables(fakeVariableHeaders.Headers));

        // Write a telemetry buffer
        fakeData.UpdateTelemetryBufferTickCount(index: 0, tickCount: 1);
        fakeData.UpdateTelemetryBufferTickCount(index: 1, tickCount: 2);
        fakeData.UpdateTelemetryBufferTickCount(index: 2, tickCount: activeBufferTickCount);
        fakeData.UpdateTelemetryBufferTickCount(index: 3, tickCount: 0);

        fakeData.WriteTelemetryBuffer(activeBufferIndex, (ref tb) => tb.Span.Fill(0xFF));

        using var reader = new TelemetryBufferReader(fakeData.Memory.Span);

        var activeBufferData = new byte[reader.BufferLength];

        Assert.True(reader.TryCopyActiveBuffer(activeBufferData, out int tickCount));

        Assert.Equal(activeBufferTickCount, tickCount);
        Assert.True(activeBufferData.All(b => b == 0xFF));
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
