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
            telemetryBufferElementLength: 1024,
            telemetryaBufferFrameCount: 4096);

        Assert.True(TelemetryFile.ValidateHeader(header));
    }

    [Fact]
    public void TelemetryFile_ValidateHeader_InvalidTelemetryBufferCountTest()
    {
        var validHeader = CreateValidHeader(
            variableCount: 42,
            sessionInfoLength: 256,
            telemetryBufferElementLength: 1024,
            telemetryaBufferFrameCount: 4096);

        Assert.False(
            TelemetryFile.ValidateHeader(
                CopyHeader(validHeader, telemetryBufferCount: -4)));

        Assert.False(
            TelemetryFile.ValidateHeader(
                CopyHeader(validHeader, telemetryBufferCount: DataFileConstants.MaxTelemetryBuffers + 1)));
    }

    [Fact]
    public void TelemetryFile_ValidateHeader_InvalidTelemetryBufferHeaderBufferOffsetTest()
    {
        var fileHeader = CreateValidHeader(
            variableCount: 42,
            sessionInfoLength: 256,
            telemetryBufferElementLength: 1024,
            telemetryaBufferFrameCount: 4096);

        var bufferHeaderArray = TelemetryBufferHeaderArray.FromArray(
            [new TelemetryBufferHeader(43, TelemetryFileHeader.Size - 3), default, default, default]);

        fileHeader = fileHeader.WithTelemetryBufferHeaders(bufferHeaderArray);

        Assert.False(
            TelemetryFile.ValidateHeader(fileHeader));
    }

    [Fact]
    public void TelemetryFile_ValidateHeader_InvalidTelemetryBufferElementLengthTest()
    {
        var validHeader = CreateValidHeader(
            variableCount: 42,
            sessionInfoLength: 256,
            telemetryBufferElementLength: 1024,
            telemetryaBufferFrameCount: 4096);

        Assert.False(
            TelemetryFile.ValidateHeader(
                CopyHeader(validHeader, telemetryBufferElementLength: 0)));
    }

    [Fact]
    public void TelemetryFile_ValidateHeader_InvalidHeaderVersionTest()
    {
        var validHeader = CreateValidHeader(
            variableCount: 42,
            sessionInfoLength: 256,
            telemetryBufferElementLength: 1024,
            telemetryaBufferFrameCount: 4096);

        var testHeader = CopyHeader(validHeader, headerVersion: 12345);

        Assert.False(TelemetryFile.ValidateHeader(testHeader));
    }

    [Fact]
    public void TelemetryFile_ValidateHeader_InvalidSessionInfoLengthTest()
    {
        var validHeader = CreateValidHeader(
            variableCount: 42,
            sessionInfoLength: 256,
            telemetryBufferElementLength: 1024,
            telemetryaBufferFrameCount: 4096);

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
            telemetryBufferElementLength: 1024,
            telemetryaBufferFrameCount: 4096);

        Assert.False(
            TelemetryFile.ValidateHeader(
                CopyHeader(validHeader, sessionInfoOffset: -4)));

        Assert.False(
            TelemetryFile.ValidateHeader(
                CopyHeader(validHeader, sessionInfoOffset: TelemetryFileHeader.Size - 1)));
    }

    [Fact]
    public void TelemetryFile_ValidateHeader_InvalidSessionRecordCountTest()
    {
        var validHeader = CreateValidHeader(
            variableCount: 42,
            sessionInfoLength: 256,
            telemetryBufferElementLength: 1024,
            telemetryaBufferFrameCount: 4096);

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
            telemetryBufferElementLength: 1024,
            telemetryaBufferFrameCount: 4096);

        Assert.False(
            TelemetryFile.ValidateHeader(
                CopyHeader(validHeader, telemetryVariableCount: -4)));
    }

    [Fact]
    public void TelemetryFile_ValidateHeader_InvalidVariableHeaderOffsetTest()
    {
        var validHeader = CreateValidHeader(
            variableCount: 42,
            sessionInfoLength: 256,
            telemetryBufferElementLength: 1024,
            telemetryaBufferFrameCount: 4096);

        Assert.False(
            TelemetryFile.ValidateHeader(
                CopyHeader(validHeader, telemetryVariableHeaderOffset: -4)));

        Assert.False(
            TelemetryFile.ValidateHeader(
                CopyHeader(validHeader, telemetryVariableHeaderOffset: TelemetryFileHeader.Size - 1)));
    }

    private static TelemetryFileHeader CreateValidHeader(
        int variableCount,
        int sessionInfoLength,
        int telemetryBufferElementLength,
        int telemetryaBufferFrameCount)
    {
        var variableHeaderBufferOffset = TelemetryFileHeader.Size;
        var variableHeaderBufferLength = variableCount * TelemetryVariableHeader.Size;

        var sessionInfoOffset = variableHeaderBufferOffset + variableHeaderBufferLength;

        var telemetryBufferHeader = new TelemetryBufferHeader(123, sessionInfoOffset + sessionInfoLength);

        var telemetryBufferHeaders = TelemetryBufferHeaderArray.FromArray([telemetryBufferHeader, default, default, default]);
        var diskSubHeader = new DiskSubHeader(123, 456, 789, 20, telemetryaBufferFrameCount);

        var header = new TelemetryFileHeader(
            headerVersion: DataFileConstants.HeaderVersion,
            status: 1,
            tickRate: 60,
            sessionInfoVersion: 1,
            sessionInfoLength: sessionInfoLength,
            sessionInfoOffset: sessionInfoOffset,
            telemetryVariableCount: variableCount,
            telemetryVariableHeaderOffset: variableHeaderBufferOffset,
            telemetryBufferCount: 1,
            telemetryBufferElementLength: telemetryBufferElementLength,
            telemetryBufferHeaders: telemetryBufferHeaders,
            diskSubHeader: diskSubHeader);

        return header;
    }

    private static TelemetryFileHeader CopyHeader(in TelemetryFileHeader header,
        int? headerVersion = null,
        int? status = null,
        int? tickRate = null,
        int? sessionInfoVersion = null,
        int? sessionInfoLength = null,
        int? sessionInfoOffset = null,
        int? telemetryVariableCount = null,
        int? telemetryVariableHeaderOffset = null,
        int? telemetryBufferCount = null,
        int? telemetryBufferElementLength = null,
        TelemetryBufferHeaderArray? telemetryBufferHeaders = null,
        DiskSubHeader? diskSubHeader = null)
    {
        return new TelemetryFileHeader(
                headerVersion ?? header.HeaderVersion,
                status ?? header.Status,
                tickRate ?? header.TickRate,
                sessionInfoVersion ?? header.SessionInfoVersion,
                sessionInfoLength ?? header.SessionInfoLength,
                sessionInfoOffset ?? header.SessionInfoOffset,
                telemetryVariableCount ?? header.TelemetryVariableCount,
                telemetryVariableHeaderOffset ?? header.TelemetryVariableHeaderOffset,
                telemetryBufferCount ?? header.TelemetryBufferCount,
                telemetryBufferElementLength ?? header.TelemetryBufferElementLength,
                telemetryBufferHeaders ?? header.TelemetryBufferHeaders,
                diskSubHeader ?? header.DiskSubHeader);
    }
}
