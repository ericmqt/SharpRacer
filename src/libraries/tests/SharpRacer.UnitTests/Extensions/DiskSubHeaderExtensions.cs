﻿using SharpRacer.Interop;

namespace SharpRacer;
internal static class DiskSubHeaderExtensions
{
    public static DiskSubHeader With(
        this in DiskSubHeader header,
        int? sessionRecordCount = null,
        int? sessionLapCount = null,
        long? sessionStartDate = null,
        double? sessionStartTime = null,
        double? sessionEndTime = null)
    {
        return new DiskSubHeader(
            sessionStartDate ?? header.SessionStartDate,
            sessionStartTime ?? header.SessionStartTime,
            sessionEndTime ?? header.SessionEndTime,
            sessionLapCount ?? header.SessionLapCount,
            sessionRecordCount ?? header.SessionRecordCount);
    }

    public static DiskSubHeader WithSessionRecordCount(this in DiskSubHeader header, int sessionRecordCount)
    {
        return header.With(sessionRecordCount: sessionRecordCount);
    }
}
