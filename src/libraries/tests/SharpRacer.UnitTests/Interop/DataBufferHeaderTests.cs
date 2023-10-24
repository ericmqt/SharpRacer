using System.Runtime.InteropServices;

namespace SharpRacer.Interop;
public class DataBufferHeaderTests
{
    [Fact]
    public void StructLayout_test()
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
}
