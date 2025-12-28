using System.Runtime.InteropServices;
using SharpRacer.Interop;

namespace SharpRacer.Testing.IO;

public readonly ref struct DataFileHeaderSpan
{
    public DataFileHeaderSpan(Span<byte> span)
    {
        Span = span;
        Header = ref DataFileHeader.AsRef(Span);
    }

    public readonly ref readonly DataFileHeader Header;
    public readonly Span<byte> Span { get; }

    public readonly void SetTelemetryBufferCount(int value)
    {
        MemoryMarshal.Write(Span[DataFileHeader.FieldOffsets.TelemetryBufferCount..], value);
    }

    public readonly void SetTelemetryBufferElementLength(int value)
    {
        MemoryMarshal.Write(Span[DataFileHeader.FieldOffsets.TelemetryBufferElementLength..], value);
    }

    public readonly void SetTelemetryBufferHeader(int index, TelemetryBufferHeaderSpanAction spanAction)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(index);
        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(index, DataFileConstants.MaxTelemetryBuffers);

        var headerFileOffset = DataFileHeader.FieldOffsets.TelemetryBufferHeaderArray + (TelemetryBufferHeader.Size * index);

        var headerSpan = new TelemetryBufferHeaderSpan(index, Span.Slice(headerFileOffset, TelemetryBufferHeaderArray.Size));

        spanAction(ref headerSpan);
    }

    public readonly void SetTelemetryBufferHeaders(TelemetryBufferHeaderArray value)
    {
        MemoryMarshal.Write(Span[DataFileHeader.FieldOffsets.TelemetryBufferHeaderArray..], value);
    }

    public readonly void SetTelemetryBufferHeaders(TelemetryBufferHeaderArraySpanAction arraySpanAction)
    {
        var span = Span.Slice(DataFileHeader.FieldOffsets.TelemetryBufferHeaderArray, TelemetryBufferHeaderArray.Size);

        var headerArraySpan = new TelemetryBufferHeaderArraySpan(span);

        arraySpanAction(ref headerArraySpan);
    }

    public readonly void SetHeaderVersion(int value)
    {
        MemoryMarshal.Write(Span[DataFileHeader.FieldOffsets.HeaderVersion..], value);
    }

    public readonly void SetSessionInfoLength(int value)
    {
        MemoryMarshal.Write(Span[DataFileHeader.FieldOffsets.SessionInfoLength..], value);
    }

    public readonly void SetSessionInfoOffset(int value)
    {
        MemoryMarshal.Write(Span[DataFileHeader.FieldOffsets.SessionInfoOffset..], value);
    }

    public readonly void SetSessionInfoVersion(int value)
    {
        MemoryMarshal.Write(Span[DataFileHeader.FieldOffsets.SessionInfoVersion..], value);
    }

    public readonly void SetStatus(int value)
    {
        MemoryMarshal.Write(Span[DataFileHeader.FieldOffsets.Status..], value);
    }

    public readonly void SetTickRate(int value)
    {
        MemoryMarshal.Write(Span[DataFileHeader.FieldOffsets.TickRate..], value);
    }

    public readonly void SetTelemetryVariableCount(int value)
    {
        MemoryMarshal.Write(Span[DataFileHeader.FieldOffsets.TelemetryVariableCount..], value);
    }

    public readonly void SetTelemetryVariableHeaderOffset(int value)
    {
        MemoryMarshal.Write(Span[DataFileHeader.FieldOffsets.TelemetryVariableHeaderOffset..], value);
    }
}
