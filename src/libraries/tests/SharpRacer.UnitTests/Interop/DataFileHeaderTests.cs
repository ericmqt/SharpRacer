using System.Runtime.InteropServices;

namespace SharpRacer.Interop;
public class DataFileHeaderTests
{
    [Fact]
    public void Ctor_Test()
    {
        int headerVersion = 1;
        int status = 1;
        int tickRate = 60;
        int sessionInfoVersion = 42;
        int sessionInfoLength = 1024;
        int sessionInfoOffset = 4096;
        int variableCount = 360;
        int variableHeaderOffset = 144;
        int dataBufferCount = 3;
        int dataBufferElementLength = 256;

        var dataBufferHeaders = new DataBufferHeader[] { new(25, 5120), new(52, 5376), new(1, 5632), new(17, 5888) };
        var dataBufferHeadersArray = DataBufferHeaderArray.FromArray(dataBufferHeaders);

        var diskSubHeader = new DiskSubHeader(23402342, 25.1d, 209.4d, 11, 4096);

        var header = new DataFileHeader(
            headerVersion,
            status,
            tickRate,
            sessionInfoVersion,
            sessionInfoLength,
            sessionInfoOffset,
            variableCount,
            variableHeaderOffset,
            dataBufferCount,
            dataBufferElementLength,
            dataBufferHeadersArray,
            diskSubHeader);

        Assert.Equal(headerVersion, header.HeaderVersion);
        Assert.Equal(status, header.Status);
        Assert.Equal(tickRate, header.TickRate);
        Assert.Equal(sessionInfoVersion, header.SessionInfoVersion);
        Assert.Equal(sessionInfoLength, header.SessionInfoLength);
        Assert.Equal(sessionInfoOffset, header.SessionInfoOffset);
        Assert.Equal(variableCount, header.VariableCount);
        Assert.Equal(variableHeaderOffset, header.VariableHeaderOffset);
        Assert.Equal(dataBufferCount, header.DataBufferCount);
        Assert.Equal(dataBufferElementLength, header.DataBufferElementLength);

        Assert.Equal(dataBufferHeadersArray, header.DataBufferHeaders);
        Assert.Equal(dataBufferHeaders[0], header.DataBufferHeaders[0]);
        Assert.Equal(dataBufferHeaders[1], header.DataBufferHeaders[1]);
        Assert.Equal(dataBufferHeaders[2], header.DataBufferHeaders[2]);
        Assert.Equal(dataBufferHeaders[3], header.DataBufferHeaders[3]);

        Assert.Equal(diskSubHeader, header.DiskSubHeader);
    }

    [Fact]
    public void StructLayout_Test()
    {
        var blob = new Span<byte>(new byte[DataFileHeader.Size]);

        int headerVersion = 1;
        int status = 1;
        int tickRate = 60;
        int sessionInfoVersion = 42;
        int sessionInfoLength = 1024;
        int sessionInfoOffset = 4096;
        int variableCount = 360;
        int variableHeaderOffset = 144;
        int dataBufferCount = 3;
        int dataBufferElementLength = 256;

        var dataBufferHeaders = new DataBufferHeader[] { new(25, 5120), new(52, 5376), new(1, 5632), new(17, 5888) };

        var diskSubHeader = new DiskSubHeader(23402342, 25.1d, 209.4d, 11, 4096);

        MemoryMarshal.Write(blob, headerVersion);
        MemoryMarshal.Write(blob[DataFileHeader.FieldOffsets.Status..], status);
        MemoryMarshal.Write(blob[DataFileHeader.FieldOffsets.TickRate..], tickRate);
        MemoryMarshal.Write(blob[DataFileHeader.FieldOffsets.SessionInfoVersion..], sessionInfoVersion);
        MemoryMarshal.Write(blob[DataFileHeader.FieldOffsets.SessionInfoLength..], sessionInfoLength);
        MemoryMarshal.Write(blob[DataFileHeader.FieldOffsets.SessionInfoOffset..], sessionInfoOffset);
        MemoryMarshal.Write(blob[DataFileHeader.FieldOffsets.VariableCount..], variableCount);
        MemoryMarshal.Write(blob[DataFileHeader.FieldOffsets.VariableHeaderOffset..], variableHeaderOffset);
        MemoryMarshal.Write(blob[DataFileHeader.FieldOffsets.DataBufferCount..], dataBufferCount);
        MemoryMarshal.Write(blob[DataFileHeader.FieldOffsets.DataBufferElementLength..], dataBufferElementLength);

        MemoryMarshal.AsBytes<DataBufferHeader>(dataBufferHeaders)
            .CopyTo(blob[DataFileHeader.FieldOffsets.DataBufferHeaderArray..]);

        MemoryMarshal.Write(blob[DataFileHeader.FieldOffsets.DiskSubHeader..], diskSubHeader);

        var header = MemoryMarshal.Read<DataFileHeader>(blob);

        Assert.Equal(headerVersion, header.HeaderVersion);
        Assert.Equal(status, header.Status);
        Assert.Equal(tickRate, header.TickRate);
        Assert.Equal(sessionInfoVersion, header.SessionInfoVersion);
        Assert.Equal(sessionInfoLength, header.SessionInfoLength);
        Assert.Equal(sessionInfoOffset, header.SessionInfoOffset);
        Assert.Equal(variableCount, header.VariableCount);
        Assert.Equal(variableHeaderOffset, header.VariableHeaderOffset);
        Assert.Equal(dataBufferCount, header.DataBufferCount);
        Assert.Equal(dataBufferElementLength, header.DataBufferElementLength);

        Assert.Equal(dataBufferHeaders[0], header.DataBufferHeaders[0]);
        Assert.Equal(dataBufferHeaders[1], header.DataBufferHeaders[1]);
        Assert.Equal(dataBufferHeaders[2], header.DataBufferHeaders[2]);
        Assert.Equal(dataBufferHeaders[3], header.DataBufferHeaders[3]);

        Assert.Equal(diskSubHeader, header.DiskSubHeader);
    }
}
