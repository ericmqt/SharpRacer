using System.Runtime.InteropServices;
using SharpRacer.Interop;

namespace SharpRacer.Testing.IO;

public readonly ref struct DiskSubHeaderSpan
{
    public DiskSubHeaderSpan(Span<byte> span)
    {
        // TODO: Verify length
        Span = span;

        Header = ref MemoryMarshal.AsRef<DiskSubHeader>(Span);
    }

    public readonly ref readonly DiskSubHeader Header;
    public readonly Span<byte> Span { get; }

    public readonly void SetSessionEndTime(double value)
    {
        MemoryMarshal.Write(Span[DiskSubHeader.FieldOffsets.SessionEndTime..], value);
    }

    public readonly void SetSessionLapCount(int value)
    {
        MemoryMarshal.Write(Span[DiskSubHeader.FieldOffsets.SessionLapCount..], value);
    }

    public readonly void SetSessionRecordCount(int value)
    {
        MemoryMarshal.Write(Span[DiskSubHeader.FieldOffsets.SessionRecordCount..], value);
    }

    public readonly void SetSessionStartDate(long value)
    {
        MemoryMarshal.Write(Span[DiskSubHeader.FieldOffsets.SessionStartDate..], value);
    }

    public readonly void SetSessionStartTime(double value)
    {
        MemoryMarshal.Write(Span[DiskSubHeader.FieldOffsets.SessionStartTime..], value);
    }
}
