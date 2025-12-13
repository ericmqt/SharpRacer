using System.Runtime.InteropServices;
using SharpRacer.Extensions.Xunit;

namespace SharpRacer.Interop;
public class DataBufferHeaderTests
{
    public static TheoryData<DataBufferHeader, DataBufferHeader> InequalityData => GetInequalityData();

    [Fact]
    public void Ctor_DefaultTest()
    {
        var header = new DataBufferHeader();

        Assert.Equal(default, header.BufferOffset);
        Assert.Equal(default, header.TickCount);
    }

    [Fact]
    public void Ctor_ParameterizedTest()
    {
        int tickCount = 1234;
        int bufferOffset = 98765;

        var header = new DataBufferHeader(tickCount, bufferOffset);

        Assert.Equal(tickCount, header.TickCount);
        Assert.Equal(bufferOffset, header.BufferOffset);
    }

    [Fact]
    public void StructLayout_Test()
    {
        var blob = new Span<byte>(new byte[DataBufferHeader.Size]);

        int tickCount = 1234;
        int bufferOffset = 98765;

        MemoryMarshal.Write(blob.Slice(DataBufferHeader.FieldOffsets.TickCount, sizeof(int)), tickCount);
        MemoryMarshal.Write(blob.Slice(DataBufferHeader.FieldOffsets.BufferOffset, sizeof(int)), bufferOffset);

        var header = MemoryMarshal.Read<DataBufferHeader>(blob);

        Assert.Equal(tickCount, header.TickCount);
        Assert.Equal(bufferOffset, header.BufferOffset);
    }

    [Fact]
    public void Equals_DefaultValueEqualityTest()
    {
        var constructedHeader = new DataBufferHeader();

        EquatableStructAssert.Equal(constructedHeader, default);
    }

    [Fact]
    public void Equals_DefaultValueInequalityTest()
    {
        var constructedHeader = new DataBufferHeader(10, 20);

        EquatableStructAssert.NotEqual(constructedHeader, default);
    }

    [Fact]
    public void Equals_EqualityTest()
    {
        var header1 = new DataBufferHeader(2, 4);
        var header2 = new DataBufferHeader(2, 4);

        EquatableStructAssert.Equal(header1, header2);
    }

    [Theory]
    [MemberData(nameof(InequalityData))]
    public void Equals_InequalityTest(DataBufferHeader header1, DataBufferHeader header2)
    {
        EquatableStructAssert.NotEqual(header1, header2);
    }

    [Fact]
    public void Equals_NullObjectTest()
    {
        var header1 = new DataBufferHeader(2, 4);

        Assert.False(header1.Equals(obj: null));
    }

    private static TheoryData<DataBufferHeader, DataBufferHeader> GetInequalityData()
    {
        return new TheoryData<DataBufferHeader, DataBufferHeader>()
        {
            // Tick count
            {
                new DataBufferHeader(1, 0),
                new DataBufferHeader(2, 0)
            },

            // BufferOffset
            {
                new DataBufferHeader(0, 1),
                new DataBufferHeader(0, 2)
            }
        };
    }
}
