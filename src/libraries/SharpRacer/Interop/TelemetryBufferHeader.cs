using System.Runtime.InteropServices;

namespace SharpRacer.Interop;

/// <summary>
/// Describes a telemetry data buffer in a simulator data file.
/// </summary>
/// <remarks>
/// See: irsdk_varBuf
/// </remarks>
[StructLayout(LayoutKind.Explicit, Size = Size)]
public readonly struct TelemetryBufferHeader : IEquatable<TelemetryBufferHeader>
{
    /// <summary>
    /// The length, in bytes, of an instance of <see cref="TelemetryBufferHeader"/>.
    /// </summary>
    public const int Size = 16;

    /// <summary>
    /// Initializes an instance of <see cref="TelemetryBufferHeader"/> to default values.
    /// </summary>
    public TelemetryBufferHeader()
    {

    }

    /// <summary>
    /// Initializes an instance of <see cref="TelemetryBufferHeader"/> with the specified values.
    /// </summary>
    /// <param name="tickCount">The tick count.</param>
    /// <param name="bufferOffset">The offset to the location of the buffer from the beginning of the <see cref="DataFileHeader"/>.</param>
    public TelemetryBufferHeader(int tickCount, int bufferOffset)
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

    /// <inheritdoc />
    public override bool Equals(object? obj)
    {
        return obj is TelemetryBufferHeader header && Equals(header);
    }

    /// <inheritdoc />
    public bool Equals(TelemetryBufferHeader other)
    {
        return TickCount == other.TickCount &&
               BufferOffset == other.BufferOffset;
    }

    /// <inheritdoc />
    public override int GetHashCode()
    {
        return HashCode.Combine(TickCount, BufferOffset);
    }

    /// <inheritdoc />
    public static bool operator ==(TelemetryBufferHeader left, TelemetryBufferHeader right)
    {
        return left.Equals(right);
    }

    /// <inheritdoc />
    public static bool operator !=(TelemetryBufferHeader left, TelemetryBufferHeader right)
    {
        return !(left == right);
    }

    /// <summary>
    /// Provides field offsets for a <see cref="TelemetryBufferHeader"/> structure.
    /// </summary>
    public static class FieldOffsets
    {
        /// <summary>
        /// The offset into a <see cref="TelemetryBufferHeader"/> structure where the <see cref="TelemetryBufferHeader.TickCount"/> field is located.
        /// </summary>
        public const int TickCount = 0;

        /// <summary>
        /// The offset into a <see cref="TelemetryBufferHeader"/> structure where the <see cref="TelemetryBufferHeader.BufferOffset"/> field is located.
        /// </summary>
        public const int BufferOffset = 4;

        /// <summary>
        /// The offset into a <see cref="TelemetryBufferHeader"/> structure where the padding field is located.
        /// </summary>
        public const int Padding = 8;
    }
}
