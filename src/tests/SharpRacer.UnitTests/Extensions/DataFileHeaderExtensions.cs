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
        int? telemetryVariableCount = null,
        int? telemetryVariableHeaderOffset = null,
        int? telemetryBufferCount = null,
        int? telemetryBufferElementLength = null,
        TelemetryBufferHeaderArray? telemetryBufferHeaders = null,
        DiskSubHeader? diskSubHeader = null)
    {
        return new DataFileHeader(
                headerVersion ?? fileHeader.HeaderVersion,
                status ?? fileHeader.Status,
                tickRate ?? fileHeader.TickRate,
                sessionInfoVersion ?? fileHeader.SessionInfoVersion,
                sessionInfoLength ?? fileHeader.SessionInfoLength,
                sessionInfoOffset ?? fileHeader.SessionInfoOffset,
                telemetryVariableCount ?? fileHeader.TelemetryVariableCount,
                telemetryVariableHeaderOffset ?? fileHeader.TelemetryVariableHeaderOffset,
                telemetryBufferCount ?? fileHeader.TelemetryBufferCount,
                telemetryBufferElementLength ?? fileHeader.TelemetryBufferElementLength,
                telemetryBufferHeaders ?? fileHeader.TelemetryBufferHeaders,
                diskSubHeader ?? fileHeader.DiskSubHeader);
    }

    public static DataFileHeader WithTelemetryBufferCount(this in DataFileHeader fileHeader, int telemetryBufferCount)
    {
        return fileHeader.With(telemetryBufferCount: telemetryBufferCount);
    }

    public static DataFileHeader WithTelemetryBufferHeaders(
        this in DataFileHeader fileHeader,
        TelemetryBufferHeaderArray telemetryBufferHeaders)
    {
        return fileHeader.With(telemetryBufferHeaders: telemetryBufferHeaders);
    }

    public static DataFileHeader WithTelemetryBufferElementLength(this in DataFileHeader fileHeader, int telemetryBufferElementLength)
    {
        return fileHeader.With(telemetryBufferElementLength: telemetryBufferElementLength);
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

    public static DataFileHeader WithTelemetryVariableCount(this in DataFileHeader fileHeader, int variableCount)
    {
        return fileHeader.With(telemetryVariableCount: variableCount);
    }

    public static DataFileHeader WithTelemetryVariableHeaderOffset(this in DataFileHeader fileHeader, int variableHeaderOffset)
    {
        return fileHeader.With(telemetryVariableHeaderOffset: variableHeaderOffset);
    }
}
