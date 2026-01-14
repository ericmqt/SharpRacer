using System.Runtime.InteropServices;
using SharpRacer.Interop;

namespace SharpRacer.Testing.IO;

public readonly ref struct TelemetryBufferHeaderArraySpan
{
    public TelemetryBufferHeaderArraySpan(Span<byte> span)
    {
        // TODO: Validate length
        Span = span;

        HeaderArray = ref MemoryMarshal.AsRef<TelemetryBufferHeaderArray>(Span);
    }

    public readonly ref readonly TelemetryBufferHeaderArray HeaderArray;

    public readonly Span<byte> Span { get; }

    public readonly void Clear()
    {
        Span.Clear();
    }

    public readonly void SetBufferHeader(int index, TelemetryBufferHeaderSpanAction spanAction)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(index);
        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(index, DataFileConstants.MaxTelemetryBuffers);

        var headerSpan = new TelemetryBufferHeaderSpan(index, Span.Slice(TelemetryBufferHeader.Size * index, TelemetryBufferHeader.Size));

        spanAction(ref headerSpan);
    }
}
