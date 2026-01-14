using SharpRacer.Interop;

namespace SharpRacer;

internal static class TelemetryFileHeaderExtensions
{
    public static TelemetryFileHeader With(
        this in TelemetryFileHeader fileHeader,
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

    public static TelemetryFileHeader WithDiskSubHeader(this in TelemetryFileHeader fileHeader, DiskSubHeader diskSubHeader)
    {
        return fileHeader.With(diskSubHeader: diskSubHeader);
    }

    public static TelemetryFileHeader WithTelemetryBufferCount(this in TelemetryFileHeader fileHeader, int telemetryBufferCount)
    {
        return fileHeader.With(telemetryBufferCount: telemetryBufferCount);
    }

    public static TelemetryFileHeader WithTelemetryBufferHeaders(
        this in TelemetryFileHeader fileHeader,
        TelemetryBufferHeaderArray telemetryBufferHeaders)
    {
        return fileHeader.With(telemetryBufferHeaders: telemetryBufferHeaders);
    }

    public static TelemetryFileHeader WithTelemetryBufferElementLength(this in TelemetryFileHeader fileHeader, int telemetryBufferElementLength)
    {
        return fileHeader.With(telemetryBufferElementLength: telemetryBufferElementLength);
    }

    public static TelemetryFileHeader WithHeaderVersion(this in TelemetryFileHeader fileHeader, int headerVersion)
    {
        return fileHeader.With(headerVersion: headerVersion);
    }

    public static TelemetryFileHeader WithSessionInfoLength(this in TelemetryFileHeader fileHeader, int sessionInfoLength)
    {
        return fileHeader.With(sessionInfoLength: sessionInfoLength);
    }

    public static TelemetryFileHeader WithSessionInfoOffset(this in TelemetryFileHeader fileHeader, int sessionInfoOffset)
    {
        return fileHeader.With(sessionInfoOffset: sessionInfoOffset);
    }

    public static TelemetryFileHeader WithSessionInfoVersion(this in TelemetryFileHeader fileHeader, int sessionInfoVersion)
    {
        return fileHeader.With(sessionInfoVersion: sessionInfoVersion);
    }

    public static TelemetryFileHeader WithStatus(this in TelemetryFileHeader fileHeader, int status)
    {
        return fileHeader.With(status: status);
    }

    public static TelemetryFileHeader WithTickRate(this in TelemetryFileHeader fileHeader, int tickRate)
    {
        return fileHeader.With(tickRate: tickRate);
    }

    public static TelemetryFileHeader WithTelemetryVariableCount(this in TelemetryFileHeader fileHeader, int variableCount)
    {
        return fileHeader.With(telemetryVariableCount: variableCount);
    }

    public static TelemetryFileHeader WithTelemetryVariableHeaderOffset(this in TelemetryFileHeader fileHeader, int variableHeaderOffset)
    {
        return fileHeader.With(telemetryVariableHeaderOffset: variableHeaderOffset);
    }
}
