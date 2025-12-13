using System.Runtime.InteropServices;
using SharpRacer.Extensions.Xunit;

namespace SharpRacer.Interop;
public class DataFileHeaderTests
{
    public static TheoryData<DataFileHeader, DataFileHeader> InequalityData => GetInequalityData();

    [Fact]
    public void Ctor_DefaultTest()
    {
        var header = new DataFileHeader();

        Assert.Equal(default, header.DataBufferCount);
        Assert.Equal(default, header.DataBufferElementLength);
        Assert.Equal(default, header.DataBufferHeaders);
        Assert.Equal(default, header.DiskSubHeader);
        Assert.Equal(default, header.HeaderVersion);
        Assert.Equal(default, header.SessionInfoLength);
        Assert.Equal(default, header.SessionInfoOffset);
        Assert.Equal(default, header.SessionInfoVersion);
        Assert.Equal(default, header.Status);
        Assert.Equal(default, header.TickRate);
        Assert.Equal(default, header.VariableCount);
        Assert.Equal(default, header.VariableHeaderOffset);
    }

    [Fact]
    public void Ctor_ParameterizedTest()
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

    [Fact]
    public void AsRef_Test()
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

        MemoryMarshal.Write(blob, header);

        // Obtain a readonly reference header over the blob
        ref readonly var headerRef = ref DataFileHeader.AsRef(blob);

        Assert.Equal(tickRate, headerRef.TickRate);
        EquatableStructAssert.Equal(header, headerRef);

        // Mutate header stored in blob
        var mutatedTickRate = 360;
        var mutatedHeader = MutateHeader(header, tickRate: mutatedTickRate);
        MemoryMarshal.Write(blob, mutatedHeader);

        // Assert the value changed in the ref header and the ref header is no longer equal to the original, unmutated header
        Assert.Equal(mutatedTickRate, headerRef.TickRate);
        EquatableStructAssert.NotEqual(header, headerRef);
    }

    [Fact]
    public void Equals_DefaultValueEqualityTest()
    {
        var constructedHeader = new DataFileHeader();

        EquatableStructAssert.Equal(constructedHeader, default);
    }

    [Fact]
    public void Equals_DefaultValueInequalityTest()
    {
        var dataBufferHeadersArray = DataBufferHeaderArray.FromArray(
            [
                new(25, 5120),
                new(52, 5376),
                new(1, 5632),
                new(17, 5888)
            ]);

        var diskSubHeader = new DiskSubHeader(23402342, 25.1d, 209.4d, 11, 4096);

        var constructedHeader = new DataFileHeader(1, 1, 60, 1, 32, 64, 12, 144, 4, 256, dataBufferHeadersArray, diskSubHeader);

        EquatableStructAssert.NotEqual(constructedHeader, default);
    }

    [Fact]
    public void Equals_EqualityTest()
    {
        const int headerVersion = 1;
        const int status = 1;
        const int tickRate = 60;
        const int sessionInfoVersion = 42;
        const int sessionInfoLength = 1024;
        const int sessionInfoOffset = 4096;
        const int variableCount = 360;
        const int variableHeaderOffset = 144;
        const int dataBufferCount = 3;
        const int dataBufferElementLength = 256;

        var dataBufferHeadersArray = DataBufferHeaderArray.FromArray(
            [
                new(25, 5120),
                new(52, 5376),
                new(1, 5632),
                new(17, 5888)
            ]);

        var diskSubHeader = new DiskSubHeader(23402342, 25.1d, 209.4d, 11, 4096);

        var header1 = new DataFileHeader(
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

        var header2 = new DataFileHeader(
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

        EquatableStructAssert.Equal(header1, header2);
    }

    [Theory]
    [MemberData(nameof(InequalityData))]
    public void Equals_InequalityTest(DataFileHeader header1, DataFileHeader header2)
    {
        EquatableStructAssert.NotEqual(header1, header2);
    }

    [Fact]
    public void Equals_NullObjectTest()
    {
        const int headerVersion = 1;
        const int status = 1;
        const int tickRate = 60;
        const int sessionInfoVersion = 42;
        const int sessionInfoLength = 1024;
        const int sessionInfoOffset = 4096;
        const int variableCount = 360;
        const int variableHeaderOffset = 144;
        const int dataBufferCount = 3;
        const int dataBufferElementLength = 256;

        var dataBufferHeadersArray = DataBufferHeaderArray.FromArray(
            [
                new(25, 5120),
                new(52, 5376),
                new(1, 5632),
                new(17, 5888)
            ]);

        var diskSubHeader = new DiskSubHeader(23402342, 25.1d, 209.4d, 11, 4096);

        var header1 = new DataFileHeader(
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

        Assert.False(header1.Equals(obj: null));
    }

    [Fact]
    public void Read_Test()
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

        var header = DataFileHeader.Read(blob);

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

    private static TheoryData<DataFileHeader, DataFileHeader> GetInequalityData()
    {
        const int headerVersion = 1;
        const int status = 1;
        const int tickRate = 60;
        const int sessionInfoVersion = 42;
        const int sessionInfoLength = 1024;
        const int sessionInfoOffset = 4096;
        const int variableCount = 360;
        const int variableHeaderOffset = 144;
        const int dataBufferCount = 3;
        const int dataBufferElementLength = 256;

        var dataBufferHeadersArray = DataBufferHeaderArray.FromArray(
            [
                new(25, 5120),
                new(52, 5376),
                new(1, 5632),
                new(17, 5888)
            ]);

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

        return new TheoryData<DataFileHeader, DataFileHeader>()
        {
            // HeaderVersion
            { header, MutateHeader(header, headerVersion: 2) },

            // Status
            { header, MutateHeader(header, status: 0) },

            // TickRate
            { header, MutateHeader(header, tickRate: 360) },

            // SessionInfoVersion
            { header, MutateHeader(header, sessionInfoVersion: 128) },

            // SessionInfoLength
            { header, MutateHeader(header, sessionInfoLength: 2048) },

            // SessionInfoOffset
            { header, MutateHeader(header, sessionInfoOffset: 256) },

            // VariableCount
            { header, MutateHeader(header, variableCount: 12) },

            // VariableHeaderOffset
            { header, MutateHeader(header, variableHeaderOffset: 123) },

            // DataBufferCount
            { header, MutateHeader(header, dataBufferCount: 4) },

            // DataBufferElementLength
            { header, MutateHeader(header, dataBufferElementLength: 1024) },

            // DataBufferHeaders
            {
                header,
                MutateHeader(header, dataBufferHeaders:
                    DataBufferHeaderArray.FromArray(
                    [
                        new(1, 2),
                        new(2, 4),
                        new(3, 8),
                        new(4, 12)
                    ]))
            },

            // DiskSubHeader
            { header, MutateHeader(header, diskSubHeader: new DiskSubHeader(123, 456, 789, 987, 654)) },
        };
    }

    private static DataFileHeader MutateHeader(in DataFileHeader header,
        int? headerVersion = null,
        int? status = null,
        int? tickRate = null,
        int? sessionInfoVersion = null,
        int? sessionInfoLength = null,
        int? sessionInfoOffset = null,
        int? variableCount = null,
        int? variableHeaderOffset = null,
        int? dataBufferCount = null,
        int? dataBufferElementLength = null,
        DataBufferHeaderArray? dataBufferHeaders = null,
        DiskSubHeader? diskSubHeader = null)
    {
        return new DataFileHeader(
                headerVersion ?? header.HeaderVersion,
                status ?? header.Status,
                tickRate ?? header.TickRate,
                sessionInfoVersion ?? header.SessionInfoVersion,
                sessionInfoLength ?? header.SessionInfoLength,
                sessionInfoOffset ?? header.SessionInfoOffset,
                variableCount ?? header.VariableCount,
                variableHeaderOffset ?? header.VariableHeaderOffset,
                dataBufferCount ?? header.DataBufferCount,
                dataBufferElementLength ?? header.DataBufferElementLength,
                dataBufferHeaders ?? header.DataBufferHeaders,
                diskSubHeader ?? header.DiskSubHeader);
    }
}
