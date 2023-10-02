using System.Runtime.InteropServices;

namespace SharpRacer.IO;

/// <summary>
/// Describes a telemetry data buffer in a simulator data file.
/// </summary>
[StructLayout(LayoutKind.Explicit, Size = DataFileConstants.DataBufferHeaderLength)]
public struct DataBufferHeader
{
    /// <summary>
    /// The session 'tick' when the buffer described by this header was updated.
    /// </summary>
    [FieldOffset(DataBufferHeaderOffsets.TickCount)]
    public int TickCount;

    /// <summary>
    /// The offset from the beginning of the <see cref="DataFileHeader"/> to the location of the buffer.
    /// </summary>
    [FieldOffset(DataBufferHeaderOffsets.BufferOffset)]
    public int BufferOffset;

    /// <summary>
    /// Padding.
    /// </summary>
    [FieldOffset(DataBufferHeaderOffsets.Padding)]
    public ulong Padding;
}
