using System.Runtime.InteropServices;

namespace SharpRacer.Interop;

/// <summary>
/// Describes a sub-header used for telemetry written to disk (*.IBT files).
/// </summary>
/// <remarks>
/// See: irsdk_diskSubHeader
/// </remarks>
[StructLayout(LayoutKind.Explicit, Size = Size)]
public readonly struct DiskSubHeader
{
    /// <summary>
    /// The length, in bytes, of an instance of <see cref="DiskSubHeader"/>.
    /// </summary>
    public const int Size = 32;

    public DiskSubHeader()
    {

    }

    public DiskSubHeader(long sessionStartDate, double sessionStartTime, double sessionEndTime, int sessionLapCount, int sessionRecordCount)
    {
        SessionStartDate = sessionStartDate;
        SessionStartTime = sessionStartTime;
        SessionEndTime = sessionEndTime;
        SessionLapCount = sessionLapCount;
        SessionRecordCount = sessionRecordCount;
    }

    /// <summary>
    /// The session start date, described as the number of seconds since the UNIX epoch.
    /// </summary>
    [FieldOffset(FieldOffsets.SessionStartDate)]
    public readonly long SessionStartDate;

    /// <summary>
    /// The session start time, described as the number of seconds since <see cref="SessionStartDate"/>.
    /// </summary>
    [FieldOffset(FieldOffsets.SessionStartTime)]
    public readonly double SessionStartTime;

    /// <summary>
    /// The session end time, described as the number of seconds since <see cref="SessionStartDate"/>.
    /// </summary>
    [FieldOffset(FieldOffsets.SessionEndTime)]
    public readonly double SessionEndTime;

    /// <summary>
    /// The number of laps in the session.
    /// </summary>
    [FieldOffset(FieldOffsets.SessionLapCount)]
    public readonly int SessionLapCount;

    /// <summary>
    /// The number of "frames" of telemetry data buffers in the file.
    /// </summary>
    [FieldOffset(FieldOffsets.SessionRecordCount)]
    public readonly int SessionRecordCount;

    /// <summary>
    /// Gets a <see cref="DateTimeOffset"/> describing the date and time the session ended.
    /// </summary>
    /// <returns></returns>
    public readonly DateTimeOffset GetSessionEndDateTimeOffset()
    {
        return DateTimeOffset.UnixEpoch
            .AddSeconds(SessionStartDate)
            .AddSeconds(SessionEndTime);
    }

    /// <summary>
    /// Gets a <see cref="DateTimeOffset"/> describing the date and time the session started.
    /// </summary>
    /// <returns></returns>
    public readonly DateTimeOffset GetSessionStartDateTimeOffset()
    {
        return DateTimeOffset.UnixEpoch
            .AddSeconds(SessionStartDate)
            .AddSeconds(SessionStartTime);
    }

    /// <summary>
    /// Provides field offsets for a <see cref="DiskSubHeader"/> structure.
    /// </summary>
    public static class FieldOffsets
    {
        /// <summary>
        /// The offset into a <see cref="DiskSubHeader"/> structure where the <see cref="DiskSubHeader.SessionStartDate"/> field is located.
        /// </summary>
        public const int SessionStartDate = 0;

        /// <summary>
        /// The offset into a <see cref="DiskSubHeader"/> structure where the <see cref="DiskSubHeader.SessionStartTime"/> field is located.
        /// </summary>
        public const int SessionStartTime = 8;

        /// <summary>
        /// The offset into a <see cref="DiskSubHeader"/> structure where the <see cref="DiskSubHeader.SessionEndTime"/> field is located.
        /// </summary>
        public const int SessionEndTime = 16;

        /// <summary>
        /// The offset into a <see cref="DiskSubHeader"/> structure where the <see cref="DiskSubHeader.SessionLapCount"/> field is located.
        /// </summary>
        public const int SessionLapCount = 24;

        /// <summary>
        /// The offset into a <see cref="DiskSubHeader"/> structure where the <see cref="DiskSubHeader.SessionRecordCount"/> field is located.
        /// </summary>
        public const int SessionRecordCount = 28;
    }
}