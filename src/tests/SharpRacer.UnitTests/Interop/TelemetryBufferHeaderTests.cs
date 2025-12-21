using System.Runtime.InteropServices;
using SharpRacer.Extensions.Xunit;

namespace SharpRacer.Interop;

public class TelemetryBufferHeaderTests
{
    public static TheoryData<TelemetryBufferHeader, TelemetryBufferHeader> InequalityData => GetInequalityData();

    [Fact]
    public void Ctor_DefaultTest()
    {
        var header = new TelemetryBufferHeader();

        Assert.Equal(default, header.BufferOffset);
        Assert.Equal(default, header.TickCount);
    }

    [Fact]
    public void Ctor_ParameterizedTest()
    {
        int tickCount = 1234;
        int bufferOffset = 98765;

        var header = new TelemetryBufferHeader(tickCount, bufferOffset);

        Assert.Equal(tickCount, header.TickCount);
        Assert.Equal(bufferOffset, header.BufferOffset);
    }

    [Fact]
    public void StructLayout_Test()
    {
        var blob = new Span<byte>(new byte[TelemetryBufferHeader.Size]);

        int tickCount = 1234;
        int bufferOffset = 98765;

        MemoryMarshal.Write(blob.Slice(TelemetryBufferHeader.FieldOffsets.TickCount, sizeof(int)), tickCount);
        MemoryMarshal.Write(blob.Slice(TelemetryBufferHeader.FieldOffsets.BufferOffset, sizeof(int)), bufferOffset);

        var header = MemoryMarshal.Read<TelemetryBufferHeader>(blob);

        Assert.Equal(tickCount, header.TickCount);
        Assert.Equal(bufferOffset, header.BufferOffset);
    }

    [Fact]
    public void Equals_DefaultValueEqualityTest()
    {
        var constructedHeader = new TelemetryBufferHeader();

        EquatableStructAssert.Equal(constructedHeader, default);
    }

    [Fact]
    public void Equals_DefaultValueInequalityTest()
    {
        var constructedHeader = new TelemetryBufferHeader(10, 20);

        EquatableStructAssert.NotEqual(constructedHeader, default);
    }

    [Fact]
    public void Equals_EqualityTest()
    {
        var header1 = new TelemetryBufferHeader(2, 4);
        var header2 = new TelemetryBufferHeader(2, 4);

        EquatableStructAssert.Equal(header1, header2);
    }

    [Theory]
    [MemberData(nameof(InequalityData))]
    public void Equals_InequalityTest(TelemetryBufferHeader header1, TelemetryBufferHeader header2)
    {
        EquatableStructAssert.NotEqual(header1, header2);
    }

    [Fact]
    public void Equals_NullObjectTest()
    {
        var header1 = new TelemetryBufferHeader(2, 4);

        Assert.False(header1.Equals(obj: null));
    }

    private static TheoryData<TelemetryBufferHeader, TelemetryBufferHeader> GetInequalityData()
    {
        return new TheoryData<TelemetryBufferHeader, TelemetryBufferHeader>()
        {
            // Tick count
            {
                new TelemetryBufferHeader(1, 0),
                new TelemetryBufferHeader(2, 0)
            },

            // BufferOffset
            {
                new TelemetryBufferHeader(0, 1),
                new TelemetryBufferHeader(0, 2)
            }
        };
    }
}
