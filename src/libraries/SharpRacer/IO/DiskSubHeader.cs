using System.Runtime.InteropServices;

namespace SharpRacer.IO;

/// <summary>
/// Describes a sub-header used for telemetry written to disk (*.IBT files).
/// </summary>
/// <remarks>
/// See: irsdk_diskSubHeader
/// </remarks>
[StructLayout(LayoutKind.Explicit, Size = DataFileConstants.DiskSubHeaderLength)]
public struct DiskSubHeader
{
    /// <summary>
    /// The session start date, described as the number of seconds since the UNIX epoch.
    /// </summary>
    [FieldOffset(DiskSubHeaderOffsets.SessionStartDate)]
    public long SessionStartDate;

    /// <summary>
    /// The session start time, described as the number of seconds since <see cref="SessionStartDate"/>.
    /// </summary>
    [FieldOffset(DiskSubHeaderOffsets.SessionStartTime)]
    public double SessionStartTime;

    /// <summary>
    /// The session end time, described as the number of seconds since <see cref="SessionStartDate"/>.
    /// </summary>
    [FieldOffset(DiskSubHeaderOffsets.SessionEndTime)]
    public double SessionEndTime;

    /// <summary>
    /// The number of laps in the session.
    /// </summary>
    [FieldOffset(DiskSubHeaderOffsets.SessionLapCount)]
    public int SessionLapCount;

    /// <summary>
    /// The number of "frames" of telemetry data buffers in the file.
    /// </summary>
    [FieldOffset(DiskSubHeaderOffsets.SessionRecordCount)]
    public int SessionRecordCount;
}