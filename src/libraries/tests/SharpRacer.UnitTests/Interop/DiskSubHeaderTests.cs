using System.Runtime.InteropServices;

namespace SharpRacer.Interop;
public class DiskSubHeaderTests
{
    [Fact]
    public void Ctor_Test()
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
}
