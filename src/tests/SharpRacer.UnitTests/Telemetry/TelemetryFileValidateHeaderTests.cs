using SharpRacer.Interop;
using SharpRacer.IO;

namespace SharpRacer.Telemetry;
public class TelemetryFileValidateHeaderTests
{
    [Fact]
    public void TelemetryFile_ValidateHeader_ValidHeaderTest()
    {
        var header = CreateValidHeader(
            variableCount: 42,
            sessionInfoLength: 256,
            dataBufferElementLength: 1024,
            dataBufferFrameCount: 4096);

        Assert.True(TelemetryFile.ValidateHeader(header));
    }

    [Fact]
    public void TelemetryFile_ValidateHeader_InvalidDataBufferCountTest()
    {
        var validHeader = CreateValidHeader(
            variableCount: 42,
            sessionInfoLength: 256,
            dataBufferElementLength: 1024,
            dataBufferFrameCount: 4096);

        Assert.False(
            TelemetryFile.ValidateHeader(
                CopyHeader(validHeader, dataBufferCount: -4)));

        Assert.False(
            TelemetryFile.ValidateHeader(
                CopyHeader(validHeader, dataBufferCount: DataFileConstants.MaxDataVariableBuffers + 1)));
    }

    [Fact]
    public void TelemetryFile_ValidateHeader_InvalidDataBufferHeaderBufferOffsetTest()
    {
        var fileHeader = CreateValidHeader(
            variableCount: 42,
            sessionInfoLength: 256,
            dataBufferElementLength: 1024,
            dataBufferFrameCount: 4096);

        var bufferHeaderArray = DataBufferHeaderArray.FromArray(
            [new DataBufferHeader(43, DataFileHeader.Size - 3), default, default, default]);

        fileHeader = fileHeader.WithDataBufferHeaders(bufferHeaderArray);

        Assert.False(
            TelemetryFile.ValidateHeader(fileHeader));
    }

    [Fact]
    public void TelemetryFile_ValidateHeader_InvalidDataBufferElementLengthTest()
    {
        var validHeader = CreateValidHeader(
            variableCount: 42,
            sessionInfoLength: 256,
            dataBufferElementLength: 1024,
            dataBufferFrameCount: 4096);

        Assert.False(
            TelemetryFile.ValidateHeader(
                CopyHeader(validHeader, dataBufferElementLength: 0)));
    }

    [Fact]
    public void TelemetryFile_ValidateHeader_InvalidHeaderVersionTest()
    {
        var validHeader = CreateValidHeader(
            variableCount: 42,
            sessionInfoLength: 256,
            dataBufferElementLength: 1024,
            dataBufferFrameCount: 4096);

        var testHeader = CopyHeader(validHeader, headerVersion: 12345);

        Assert.False(TelemetryFile.ValidateHeader(testHeader));
    }

    [Fact]
    public void TelemetryFile_ValidateHeader_InvalidSessionInfoLengthTest()
    {
        var validHeader = CreateValidHeader(
            variableCount: 42,
            sessionInfoLength: 256,
            dataBufferElementLength: 1024,
            dataBufferFrameCount: 4096);

        Assert.False(
            TelemetryFile.ValidateHeader(
                CopyHeader(validHeader, sessionInfoLength: -4)));
    }

    [Fact]
    public void TelemetryFile_ValidateHeader_InvalidSessionInfoOffsetTest()
    {
        var validHeader = CreateValidHeader(
            variableCount: 42,
            sessionInfoLength: 256,
            dataBufferElementLength: 1024,
            dataBufferFrameCount: 4096);

        Assert.False(
            TelemetryFile.ValidateHeader(
                CopyHeader(validHeader, sessionInfoOffset: -4)));

        Assert.False(
            TelemetryFile.ValidateHeader(
                CopyHeader(validHeader, sessionInfoOffset: DataFileHeader.Size - 1)));
    }

    [Fact]
    public void TelemetryFile_ValidateHeader_InvalidSessionRecordCountTest()
    {
        var validHeader = CreateValidHeader(
            variableCount: 42,
            sessionInfoLength: 256,
            dataBufferElementLength: 1024,
            dataBufferFrameCount: 4096);

        Assert.False(
            TelemetryFile.ValidateHeader(
                CopyHeader(validHeader, diskSubHeader: new DiskSubHeader(123, 456, 789, 20, -1))));
    }

    [Fact]
    public void TelemetryFile_ValidateHeader_InvalidVariableCountTest()
    {
        var validHeader = CreateValidHeader(
            variableCount: 42,
            sessionInfoLength: 256,
            dataBufferElementLength: 1024,
            dataBufferFrameCount: 4096);

        Assert.False(
            TelemetryFile.ValidateHeader(
                CopyHeader(validHeader, variableCount: -4)));
    }

    [Fact]
    public void TelemetryFile_ValidateHeader_InvalidVariableHeaderOffsetTest()
    {
        var validHeader = CreateValidHeader(
            variableCount: 42,
            sessionInfoLength: 256,
            dataBufferElementLength: 1024,
            dataBufferFrameCount: 4096);

        Assert.False(
            TelemetryFile.ValidateHeader(
                CopyHeader(validHeader, variableHeaderOffset: -4)));

        Assert.False(
            TelemetryFile.ValidateHeader(
                CopyHeader(validHeader, variableHeaderOffset: DataFileHeader.Size - 1)));
    }

    private static DataFileHeader CreateValidHeader(
        int variableCount,
        int sessionInfoLength,
        int dataBufferElementLength,
        int dataBufferFrameCount)
    {
        var variableHeaderBufferOffset = DataFileHeader.Size;
        var variableHeaderBufferLength = variableCount * DataVariableHeader.Size;

        var sessionInfoOffset = variableHeaderBufferOffset + variableHeaderBufferLength;

        var dataBufferHeader = new DataBufferHeader(123, sessionInfoOffset + sessionInfoLength);

        var dataBufferHeaders = DataBufferHeaderArray.FromArray([dataBufferHeader, default, default, default]);
        var diskSubHeader = new DiskSubHeader(123, 456, 789, 20, dataBufferFrameCount);

        var header = new DataFileHeader(
            headerVersion: DataFileConstants.HeaderVersion,
            status: 1,
            tickRate: 60,
            sessionInfoVersion: 1,
            sessionInfoLength: sessionInfoLength,
            sessionInfoOffset: sessionInfoOffset,
            variableCount: variableCount,
            variableHeaderOffset: variableHeaderBufferOffset,
            dataBufferCount: 1,
            dataBufferElementLength: dataBufferElementLength,
            dataBufferHeaders: dataBufferHeaders,
            diskSubHeader: diskSubHeader);

        return header;
    }

    private static DataFileHeader CopyHeader(in DataFileHeader header,
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
