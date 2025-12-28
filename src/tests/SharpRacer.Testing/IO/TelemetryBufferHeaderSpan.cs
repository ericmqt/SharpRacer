using System.Runtime.InteropServices;
using SharpRacer.Interop;

namespace SharpRacer.Testing.IO;

public readonly ref struct TelemetryBufferHeaderSpan
{
    public TelemetryBufferHeaderSpan(int index, Span<byte> span)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(index);
        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(index, DataFileConstants.MaxTelemetryBuffers);

        Index = index;
        Span = span;
        BufferHeader = ref MemoryMarshal.AsRef<TelemetryBufferHeader>(Span);
    }

    public readonly ref readonly TelemetryBufferHeader BufferHeader;

    public readonly int Index { get; }
    public readonly Span<byte> Span { get; }

    public readonly void SetBufferOffset(int fileOffset)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(fileOffset);

        MemoryMarshal.Write(Span[TelemetryBufferHeader.FieldOffsets.BufferOffset..], fileOffset);
    }

    public readonly void SetTickCount(int tickCount)
    {
        MemoryMarshal.Write(Span[TelemetryBufferHeader.FieldOffsets.TickCount..], tickCount);
    }
}
