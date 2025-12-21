using SharpRacer.Interop;

namespace SharpRacer;
internal static class DataFileHeaderExtensions
{
    public static DataFileHeader With(
        this in DataFileHeader fileHeader,
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
        TelemetryBufferHeaderArray? dataBufferHeaders = null,
        DiskSubHeader? diskSubHeader = null)
    {
        return new DataFileHeader(
                headerVersion ?? fileHeader.HeaderVersion,
                status ?? fileHeader.Status,
                tickRate ?? fileHeader.TickRate,
                sessionInfoVersion ?? fileHeader.SessionInfoVersion,
                sessionInfoLength ?? fileHeader.SessionInfoLength,
                sessionInfoOffset ?? fileHeader.SessionInfoOffset,
                variableCount ?? fileHeader.VariableCount,
                variableHeaderOffset ?? fileHeader.VariableHeaderOffset,
                dataBufferCount ?? fileHeader.DataBufferCount,
                dataBufferElementLength ?? fileHeader.DataBufferElementLength,
                dataBufferHeaders ?? fileHeader.DataBufferHeaders,
                diskSubHeader ?? fileHeader.DiskSubHeader);
    }

    public static DataFileHeader WithDataBufferCount(this in DataFileHeader fileHeader, int dataBufferCount)
    {
        return fileHeader.With(dataBufferCount: dataBufferCount);
    }

    public static DataFileHeader WithDataBufferHeaders(this in DataFileHeader fileHeader, TelemetryBufferHeaderArray dataBufferHeaders)
    {
        return fileHeader.With(dataBufferHeaders: dataBufferHeaders);
    }

    public static DataFileHeader WithDataBufferElementLength(this in DataFileHeader fileHeader, int dataBufferElementLength)
    {
        return fileHeader.With(dataBufferElementLength: dataBufferElementLength);
    }

    public static DataFileHeader WithDiskSubHeader(this in DataFileHeader fileHeader, DiskSubHeader diskSubHeader)
    {
        return fileHeader.With(diskSubHeader: diskSubHeader);
    }

    public static DataFileHeader WithHeaderVersion(this in DataFileHeader fileHeader, int headerVersion)
    {
        return fileHeader.With(headerVersion: headerVersion);
    }

    public static DataFileHeader WithSessionInfoLength(this in DataFileHeader fileHeader, int sessionInfoLength)
    {
        return fileHeader.With(sessionInfoLength: sessionInfoLength);
    }

    public static DataFileHeader WithSessionInfoOffset(this in DataFileHeader fileHeader, int sessionInfoOffset)
    {
        return fileHeader.With(sessionInfoOffset: sessionInfoOffset);
    }

    public static DataFileHeader WithSessionInfoVersion(this in DataFileHeader fileHeader, int sessionInfoVersion)
    {
        return fileHeader.With(sessionInfoVersion: sessionInfoVersion);
    }

    public static DataFileHeader WithStatus(this in DataFileHeader fileHeader, int status)
    {
        return fileHeader.With(status: status);
    }

    public static DataFileHeader WithTickRate(this in DataFileHeader fileHeader, int tickRate)
    {
        return fileHeader.With(tickRate: tickRate);
    }

    public static DataFileHeader WithVariableCount(this in DataFileHeader fileHeader, int variableCount)
    {
        return fileHeader.With(variableCount: variableCount);
    }

    public static DataFileHeader WithVariableHeaderOffset(this in DataFileHeader fileHeader, int variableHeaderOffset)
    {
        return fileHeader.With(variableHeaderOffset: variableHeaderOffset);
    }
}
