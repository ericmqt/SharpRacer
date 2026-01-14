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

        Assert.Equal(default, header.TelemetryBufferCount);
        Assert.Equal(default, header.TelemetryBufferElementLength);
        Assert.Equal(default, header.TelemetryBufferHeaders);
        Assert.Equal(default, header.HeaderVersion);
        Assert.Equal(default, header.SessionInfoLength);
        Assert.Equal(default, header.SessionInfoOffset);
        Assert.Equal(default, header.SessionInfoVersion);
        Assert.Equal(default, header.Status);
        Assert.Equal(default, header.TickRate);
        Assert.Equal(default, header.TelemetryVariableCount);
        Assert.Equal(default, header.TelemetryVariableHeaderOffset);
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
        int telemetryBufferCount = 3;
        int telemetryBufferElementLength = 256;

        var telemetryBufferHeaders = new TelemetryBufferHeader[] { new(25, 5120), new(52, 5376), new(1, 5632), new(17, 5888) };
        var telemetryBufferHeadersArray = TelemetryBufferHeaderArray.FromArray(telemetryBufferHeaders);

        var header = new DataFileHeader(
            headerVersion,
            status,
            tickRate,
            sessionInfoVersion,
            sessionInfoLength,
            sessionInfoOffset,
            variableCount,
            variableHeaderOffset,
            telemetryBufferCount,
            telemetryBufferElementLength,
            telemetryBufferHeadersArray);

        Assert.Equal(headerVersion, header.HeaderVersion);
        Assert.Equal(status, header.Status);
        Assert.Equal(tickRate, header.TickRate);
        Assert.Equal(sessionInfoVersion, header.SessionInfoVersion);
        Assert.Equal(sessionInfoLength, header.SessionInfoLength);
        Assert.Equal(sessionInfoOffset, header.SessionInfoOffset);
        Assert.Equal(variableCount, header.TelemetryVariableCount);
        Assert.Equal(variableHeaderOffset, header.TelemetryVariableHeaderOffset);
        Assert.Equal(telemetryBufferCount, header.TelemetryBufferCount);
        Assert.Equal(telemetryBufferElementLength, header.TelemetryBufferElementLength);

        Assert.Equal(telemetryBufferHeadersArray, header.TelemetryBufferHeaders);
        Assert.Equal(telemetryBufferHeaders[0], header.TelemetryBufferHeaders[0]);
        Assert.Equal(telemetryBufferHeaders[1], header.TelemetryBufferHeaders[1]);
        Assert.Equal(telemetryBufferHeaders[2], header.TelemetryBufferHeaders[2]);
        Assert.Equal(telemetryBufferHeaders[3], header.TelemetryBufferHeaders[3]);
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
        int telemetryBufferCount = 3;
        int telemetryBufferElementLength = 256;

        var telemetryBufferHeaders = new TelemetryBufferHeader[] { new(25, 5120), new(52, 5376), new(1, 5632), new(17, 5888) };

        MemoryMarshal.Write(blob, headerVersion);
        MemoryMarshal.Write(blob[DataFileHeader.FieldOffsets.Status..], status);
        MemoryMarshal.Write(blob[DataFileHeader.FieldOffsets.TickRate..], tickRate);
        MemoryMarshal.Write(blob[DataFileHeader.FieldOffsets.SessionInfoVersion..], sessionInfoVersion);
        MemoryMarshal.Write(blob[DataFileHeader.FieldOffsets.SessionInfoLength..], sessionInfoLength);
        MemoryMarshal.Write(blob[DataFileHeader.FieldOffsets.SessionInfoOffset..], sessionInfoOffset);
        MemoryMarshal.Write(blob[DataFileHeader.FieldOffsets.TelemetryVariableCount..], variableCount);
        MemoryMarshal.Write(blob[DataFileHeader.FieldOffsets.TelemetryVariableHeaderOffset..], variableHeaderOffset);
        MemoryMarshal.Write(blob[DataFileHeader.FieldOffsets.TelemetryBufferCount..], telemetryBufferCount);
        MemoryMarshal.Write(blob[DataFileHeader.FieldOffsets.TelemetryBufferElementLength..], telemetryBufferElementLength);

        MemoryMarshal.AsBytes<TelemetryBufferHeader>(telemetryBufferHeaders)
            .CopyTo(blob[DataFileHeader.FieldOffsets.TelemetryBufferHeaderArray..]);

        var header = MemoryMarshal.Read<DataFileHeader>(blob);

        Assert.Equal(headerVersion, header.HeaderVersion);
        Assert.Equal(status, header.Status);
        Assert.Equal(tickRate, header.TickRate);
        Assert.Equal(sessionInfoVersion, header.SessionInfoVersion);
        Assert.Equal(sessionInfoLength, header.SessionInfoLength);
        Assert.Equal(sessionInfoOffset, header.SessionInfoOffset);
        Assert.Equal(variableCount, header.TelemetryVariableCount);
        Assert.Equal(variableHeaderOffset, header.TelemetryVariableHeaderOffset);
        Assert.Equal(telemetryBufferCount, header.TelemetryBufferCount);
        Assert.Equal(telemetryBufferElementLength, header.TelemetryBufferElementLength);

        Assert.Equal(telemetryBufferHeaders[0], header.TelemetryBufferHeaders[0]);
        Assert.Equal(telemetryBufferHeaders[1], header.TelemetryBufferHeaders[1]);
        Assert.Equal(telemetryBufferHeaders[2], header.TelemetryBufferHeaders[2]);
        Assert.Equal(telemetryBufferHeaders[3], header.TelemetryBufferHeaders[3]);
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
        int telemetryBufferCount = 3;
        int telemetryBufferElementLength = 256;

        var telemetryBufferHeaders = new TelemetryBufferHeader[] { new(25, 5120), new(52, 5376), new(1, 5632), new(17, 5888) };
        var telemetryBufferHeadersArray = TelemetryBufferHeaderArray.FromArray(telemetryBufferHeaders);

        var header = new DataFileHeader(
            headerVersion,
            status,
            tickRate,
            sessionInfoVersion,
            sessionInfoLength,
            sessionInfoOffset,
            variableCount,
            variableHeaderOffset,
            telemetryBufferCount,
            telemetryBufferElementLength,
            telemetryBufferHeadersArray);

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
        var telemetryBufferHeadersArray = TelemetryBufferHeaderArray.FromArray(
            [
                new(25, 5120),
                new(52, 5376),
                new(1, 5632),
                new(17, 5888)
            ]);

        var constructedHeader = new DataFileHeader(1, 1, 60, 1, 32, 64, 12, 144, 4, 256, telemetryBufferHeadersArray);

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
        const int telemetryBufferCount = 3;
        const int telemetryBufferElementLength = 256;

        var telemetryBufferHeadersArray = TelemetryBufferHeaderArray.FromArray(
            [
                new(25, 5120),
                new(52, 5376),
                new(1, 5632),
                new(17, 5888)
            ]);

        var header1 = new DataFileHeader(
            headerVersion,
            status,
            tickRate,
            sessionInfoVersion,
            sessionInfoLength,
            sessionInfoOffset,
            variableCount,
            variableHeaderOffset,
            telemetryBufferCount,
            telemetryBufferElementLength,
            telemetryBufferHeadersArray);

        var header2 = new DataFileHeader(
            headerVersion,
            status,
            tickRate,
            sessionInfoVersion,
            sessionInfoLength,
            sessionInfoOffset,
            variableCount,
            variableHeaderOffset,
            telemetryBufferCount,
            telemetryBufferElementLength,
            telemetryBufferHeadersArray);

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
        const int telemetryBufferCount = 3;
        const int telemetryBufferElementLength = 256;

        var telemetryBufferHeadersArray = TelemetryBufferHeaderArray.FromArray(
            [
                new(25, 5120),
                new(52, 5376),
                new(1, 5632),
                new(17, 5888)
            ]);

        var header1 = new DataFileHeader(
            headerVersion,
            status,
            tickRate,
            sessionInfoVersion,
            sessionInfoLength,
            sessionInfoOffset,
            variableCount,
            variableHeaderOffset,
            telemetryBufferCount,
            telemetryBufferElementLength,
            telemetryBufferHeadersArray);

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
        int telemetryBufferCount = 3;
        int telemetryBufferElementLength = 256;

        var telemetryBufferHeaders = new TelemetryBufferHeader[] { new(25, 5120), new(52, 5376), new(1, 5632), new(17, 5888) };

        MemoryMarshal.Write(blob, headerVersion);
        MemoryMarshal.Write(blob[DataFileHeader.FieldOffsets.Status..], status);
        MemoryMarshal.Write(blob[DataFileHeader.FieldOffsets.TickRate..], tickRate);
        MemoryMarshal.Write(blob[DataFileHeader.FieldOffsets.SessionInfoVersion..], sessionInfoVersion);
        MemoryMarshal.Write(blob[DataFileHeader.FieldOffsets.SessionInfoLength..], sessionInfoLength);
        MemoryMarshal.Write(blob[DataFileHeader.FieldOffsets.SessionInfoOffset..], sessionInfoOffset);
        MemoryMarshal.Write(blob[DataFileHeader.FieldOffsets.TelemetryVariableCount..], variableCount);
        MemoryMarshal.Write(blob[DataFileHeader.FieldOffsets.TelemetryVariableHeaderOffset..], variableHeaderOffset);
        MemoryMarshal.Write(blob[DataFileHeader.FieldOffsets.TelemetryBufferCount..], telemetryBufferCount);
        MemoryMarshal.Write(blob[DataFileHeader.FieldOffsets.TelemetryBufferElementLength..], telemetryBufferElementLength);

        MemoryMarshal.AsBytes<TelemetryBufferHeader>(telemetryBufferHeaders)
            .CopyTo(blob[DataFileHeader.FieldOffsets.TelemetryBufferHeaderArray..]);

        var header = DataFileHeader.Read(blob);

        Assert.Equal(headerVersion, header.HeaderVersion);
        Assert.Equal(status, header.Status);
        Assert.Equal(tickRate, header.TickRate);
        Assert.Equal(sessionInfoVersion, header.SessionInfoVersion);
        Assert.Equal(sessionInfoLength, header.SessionInfoLength);
        Assert.Equal(sessionInfoOffset, header.SessionInfoOffset);
        Assert.Equal(variableCount, header.TelemetryVariableCount);
        Assert.Equal(variableHeaderOffset, header.TelemetryVariableHeaderOffset);
        Assert.Equal(telemetryBufferCount, header.TelemetryBufferCount);
        Assert.Equal(telemetryBufferElementLength, header.TelemetryBufferElementLength);

        Assert.Equal(telemetryBufferHeaders[0], header.TelemetryBufferHeaders[0]);
        Assert.Equal(telemetryBufferHeaders[1], header.TelemetryBufferHeaders[1]);
        Assert.Equal(telemetryBufferHeaders[2], header.TelemetryBufferHeaders[2]);
        Assert.Equal(telemetryBufferHeaders[3], header.TelemetryBufferHeaders[3]);
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
        const int telemetryBufferCount = 3;
        const int telemetryBufferElementLength = 256;

        var telemetryBufferHeadersArray = TelemetryBufferHeaderArray.FromArray(
            [
                new(25, 5120),
                new(52, 5376),
                new(1, 5632),
                new(17, 5888)
            ]);

        var header = new DataFileHeader(
            headerVersion,
            status,
            tickRate,
            sessionInfoVersion,
            sessionInfoLength,
            sessionInfoOffset,
            variableCount,
            variableHeaderOffset,
            telemetryBufferCount,
            telemetryBufferElementLength,
            telemetryBufferHeadersArray);

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

            // TelemetryBufferCount
            { header, MutateHeader(header, telemetryBufferCount: 4) },

            // TelemetryBufferElementLength
            { header, MutateHeader(header, telemetryBufferElementLength: 1024) },

            // TelemetryBufferHeaders
            {
                header,
                MutateHeader(header, telemetryBufferHeaders:
                    TelemetryBufferHeaderArray.FromArray(
                    [
                        new(1, 2),
                        new(2, 4),
                        new(3, 8),
                        new(4, 12)
                    ]))
            }
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
        int? telemetryBufferCount = null,
        int? telemetryBufferElementLength = null,
        TelemetryBufferHeaderArray? telemetryBufferHeaders = null)
    {
        return new DataFileHeader(
                headerVersion ?? header.HeaderVersion,
                status ?? header.Status,
                tickRate ?? header.TickRate,
                sessionInfoVersion ?? header.SessionInfoVersion,
                sessionInfoLength ?? header.SessionInfoLength,
                sessionInfoOffset ?? header.SessionInfoOffset,
                variableCount ?? header.TelemetryVariableCount,
                variableHeaderOffset ?? header.TelemetryVariableHeaderOffset,
                telemetryBufferCount ?? header.TelemetryBufferCount,
                telemetryBufferElementLength ?? header.TelemetryBufferElementLength,
                telemetryBufferHeaders ?? header.TelemetryBufferHeaders);
    }
}
