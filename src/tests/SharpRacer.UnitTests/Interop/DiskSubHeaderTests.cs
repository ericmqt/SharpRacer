using System.Runtime.InteropServices;

namespace SharpRacer.Interop;
public class DiskSubHeaderTests
{
    [Fact]
    public void Ctor_DefaultTest()
    {
        var header = new DiskSubHeader();

        Assert.Equal(default, header.SessionEndTime);
        Assert.Equal(default, header.SessionLapCount);
        Assert.Equal(default, header.SessionRecordCount);
        Assert.Equal(default, header.SessionStartDate);
        Assert.Equal(default, header.SessionStartTime);
    }

    [Fact]
    public void Ctor_ParameterizedTest()
    {
        long sessionStartDate = 1234;
        double sessionStartTime = 42.7;
        double sessionEndTime = 98.6;
        int sessionLapCount = 11;
        int sessionRecordCount = 13200;

        var header = new DiskSubHeader(
            sessionStartDate,
            sessionStartTime,
            sessionEndTime,
            sessionLapCount,
            sessionRecordCount);

        Assert.Equal(sessionStartDate, header.SessionStartDate);
        Assert.Equal(sessionStartTime, header.SessionStartTime);
        Assert.Equal(sessionEndTime, header.SessionEndTime);
        Assert.Equal(sessionLapCount, header.SessionLapCount);
        Assert.Equal(sessionRecordCount, header.SessionRecordCount);
    }

    [Fact]
    public void StructLayout_Test()
    {
        var blob = new Span<byte>(new byte[DiskSubHeader.Size]);

        long sessionStartDate = 1234;
        double sessionStartTime = 42.7;
        double sessionEndTime = 98.6;
        int sessionLapCount = 11;
        int sessionRecordCount = 13200;

        MemoryMarshal.Write(blob.Slice(DiskSubHeader.FieldOffsets.SessionStartDate, sizeof(long)), sessionStartDate);
        MemoryMarshal.Write(blob.Slice(DiskSubHeader.FieldOffsets.SessionStartTime, sizeof(double)), sessionStartTime);
        MemoryMarshal.Write(blob.Slice(DiskSubHeader.FieldOffsets.SessionEndTime, sizeof(double)), sessionEndTime);
        MemoryMarshal.Write(blob.Slice(DiskSubHeader.FieldOffsets.SessionLapCount, sizeof(int)), sessionLapCount);
        MemoryMarshal.Write(blob.Slice(DiskSubHeader.FieldOffsets.SessionRecordCount, sizeof(int)), sessionRecordCount);

        var header = MemoryMarshal.Read<DiskSubHeader>(blob);

        Assert.Equal(sessionStartDate, header.SessionStartDate);
        Assert.Equal(sessionStartTime, header.SessionStartTime);
        Assert.Equal(sessionEndTime, header.SessionEndTime);
        Assert.Equal(sessionLapCount, header.SessionLapCount);
        Assert.Equal(sessionRecordCount, header.SessionRecordCount);
    }

    [Fact]
    public void GetSessionEndDateTimeOffset_Test()
    {
        var sessionStartDate = new DateOnly(2023, 9, 22);
        var sessionStartTime = new TimeOnly(14, 05, 0);
        var sessionEndTime = new TimeOnly(14, 33, 0);

        var sessionStartDate_DateTimeOffset = new DateTimeOffset(sessionStartDate, TimeOnly.MinValue, TimeSpan.FromHours(0));
        var sessionEndDateTimeOffset = new DateTimeOffset(sessionStartDate, sessionEndTime, TimeSpan.FromHours(0));

        var unixSessionStartDate = (long)sessionStartDate_DateTimeOffset.Subtract(DateTimeOffset.UnixEpoch).TotalSeconds;

        var header = new DiskSubHeader(
            unixSessionStartDate,
            sessionStartTime.ToTimeSpan().TotalSeconds,
            sessionEndTime.ToTimeSpan().TotalSeconds,
            5,
            1024);

        Assert.Equal(sessionEndDateTimeOffset, header.GetSessionEndDateTimeOffset());
    }

    [Fact]
    public void GetSessionStartDateTimeOffset_Test()
    {
        var sessionStartDate = new DateOnly(2023, 9, 22);
        var sessionStartTime = new TimeOnly(14, 05, 0);
        var sessionEndTime = new TimeOnly(14, 33, 0);

        var sessionStartDate_DateTimeOffset = new DateTimeOffset(sessionStartDate, TimeOnly.MinValue, TimeSpan.FromHours(0));
        var sessionStartDateTimeOffset = new DateTimeOffset(sessionStartDate, sessionStartTime, TimeSpan.FromHours(0));

        var unixSessionStartDate = (long)sessionStartDate_DateTimeOffset.Subtract(DateTimeOffset.UnixEpoch).TotalSeconds;

        var header = new DiskSubHeader(
            unixSessionStartDate,
            sessionStartTime.ToTimeSpan().TotalSeconds,
            sessionEndTime.ToTimeSpan().TotalSeconds,
            5,
            1024);

        Assert.Equal(sessionStartDateTimeOffset, header.GetSessionStartDateTimeOffset());
    }
}
