using System.Runtime.InteropServices;

namespace SharpRacer.Interop;

/// <summary>
/// Describes a telemetry data buffer in a simulator data file.
/// </summary>
/// <remarks>
/// See: irsdk_varBuf
/// </remarks>
[StructLayout(LayoutKind.Explicit, Size = Size)]
public readonly struct DataBufferHeader
{
    /// <summary>
    /// The length, in bytes, of an instance of <see cref="DataBufferHeader"/>.
    /// </summary>
    public const int Size = 16;

    /// <summary>
    /// Initializes an instance of <see cref="DataBufferHeader"/> to default values.
    /// </summary>
    public DataBufferHeader()
    {

    }

    /// <summary>
    /// Initializes an instance of <see cref="DataBufferHeader"/> with the specified values.
    /// </summary>
    /// <param name="tickCount">The tick count.</param>
    /// <param name="bufferOffset">The offset to the location of the buffer from the beginning of the <see cref="DataFileHeader"/>.</param>
    public DataBufferHeader(int tickCount, int bufferOffset)
    {
        TickCount = tickCount;
        BufferOffset = bufferOffset;
    }

    /// <summary>
    /// The session 'tick' when the buffer described by this header was updated.
    /// </summary>
    [FieldOffset(FieldOffsets.TickCount)]
    public readonly int TickCount;

    /// <summary>
    /// The offset to the location of the buffer from the beginning of the <see cref="DataFileHeader"/>.
    /// </summary>
    [FieldOffset(FieldOffsets.BufferOffset)]
    public readonly int BufferOffset;

    /// <summary>
    /// Provides field offsets for a <see cref="DataBufferHeader"/> structure.
    /// </summary>
    public static class FieldOffsets
    {
        /// <summary>
        /// The offset into a <see cref="DataBufferHeader"/> structure where the <see cref="DataBufferHeader.TickCount"/> field is located.
        /// </summary>
        public const int TickCount = 0;

        /// <summary>
        /// The offset into a <see cref="DataBufferHeader"/> structure where the <see cref="DataBufferHeader.BufferOffset"/> field is located.
        /// </summary>
        public const int BufferOffset = 4;

        /// <summary>
        /// The offset into a <see cref="DataBufferHeader"/> structure where the padding field is located.
        /// </summary>
        public const int Padding = 8;
    }
}
