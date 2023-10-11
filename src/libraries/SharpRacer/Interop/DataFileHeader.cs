using System.Runtime.InteropServices;

namespace SharpRacer.Interop;

/// <summary>
/// Describes the values of a simulator data file header.
/// </summary>
/// <remarks>
/// See: irsdk_header
/// </remarks>
[StructLayout(LayoutKind.Explicit, Size = Size)]
public readonly struct DataFileHeader
{
    /// <summary>
    /// The length, in bytes, of an instance of <see cref="DataFileHeader"/>.
    /// </summary>
    public const int Size = 144;

    /// <summary>
    /// Header version. See IRSDK_VER.
    /// </summary>
    [FieldOffset(FieldOffsets.HeaderVersion)]
    public readonly int HeaderVersion;

    /// <summary>
    /// Indicates if the simulator is active. See irsdk_StatusField.
    /// </summary>
    [FieldOffset(FieldOffsets.Status)]
    public readonly int Status;

    /// <summary>
    /// Ticks per second.
    /// </summary>
    [FieldOffset(FieldOffsets.TickRate)]
    public readonly int TickRate;

    /// <summary>
    /// Incremented when session info changes.
    /// </summary>
    [FieldOffset(FieldOffsets.SessionInfoVersion)]
    public readonly int SessionInfoVersion;

    /// <summary>
    /// The length of the session info string.
    /// </summary>
    [FieldOffset(FieldOffsets.SessionInfoLength)]
    public readonly int SessionInfoLength;

    /// <summary>
    /// The offset from the beginning of the data file where the session info string is located.
    /// </summary>
    [FieldOffset(FieldOffsets.SessionInfoOffset)]
    public readonly int SessionInfoOffset;

    /// <summary>
    /// The length of the array of <see cref="DataVariableHeader"/> entries pointed to by <see cref="VariableHeaderOffset"/>.
    /// </summary>
    [FieldOffset(FieldOffsets.VariableCount)]
    public readonly int VariableCount;

    /// <summary>
    /// The offset from the beginning of the data file where the array of <see cref="DataVariableHeader"/> entries is located.
    /// </summary>
    [FieldOffset(FieldOffsets.VariableHeaderOffset)]
    public readonly int VariableHeaderOffset;

    /// <summary>
    /// The number of data buffers.
    /// </summary>
    [FieldOffset(FieldOffsets.DataBufferCount)]
    public readonly int DataBufferCount;

    /// <summary>
    /// The length, in bytes, of a single data buffer.
    /// </summary>
    [FieldOffset(FieldOffsets.DataBufferElementLength)]
    public readonly int DataBufferElementLength;

    /// <summary>
    /// An inline array of <see cref="DataBufferHeader"/> elements.
    /// </summary>
    [FieldOffset(FieldOffsets.DataBufferHeaderArray)]
    public readonly DataBufferHeaderArray DataBufferHeaders;

    /// <summary>
    /// The sub-header used by telemetry files.
    /// </summary>
    [FieldOffset(FieldOffsets.DiskSubHeader)]
    public readonly DiskSubHeader DiskSubHeader;

    /// <summary>
    /// Provides field offsets for a <see cref="DataFileHeader"/> structure.
    /// </summary>
    public static class FieldOffsets
    {
        /// <summary>
        /// The offset into a <see cref="DataFileHeader"/> structure where the <see cref="DataFileHeader.HeaderVersion"/> field is located.
        /// </summary>
        public const int HeaderVersion = 0;

        /// <summary>
        /// The offset into a <see cref="DataFileHeader"/> structure where the <see cref="DataFileHeader.Status"/> field is located.
        /// </summary>
        public const int Status = 4;

        /// <summary>
        /// The offset into a <see cref="DataFileHeader"/> structure where the <see cref="DataFileHeader.TickRate"/> field is located.
        /// </summary>
        public const int TickRate = 8;

        /// <summary>
        /// The offset into a <see cref="DataFileHeader"/> structure where the <see cref="DataFileHeader.SessionInfoVersion"/> field is located.
        /// </summary>
        public const int SessionInfoVersion = 12;

        /// <summary>
        /// The offset into a <see cref="DataFileHeader"/> structure where the <see cref="DataFileHeader.SessionInfoLength"/> field is located.
        /// </summary>
        public const int SessionInfoLength = 16;

        /// <summary>
        /// The offset into a <see cref="DataFileHeader"/> structure where the <see cref="DataFileHeader.SessionInfoOffset"/> field is located.
        /// </summary>
        public const int SessionInfoOffset = 20;

        /// <summary>
        /// The offset into a <see cref="DataFileHeader"/> structure where the <see cref="DataFileHeader.VariableCount"/> field is located.
        /// </summary>
        public const int VariableCount = 24;

        /// <summary>
        /// The offset into a <see cref="DataFileHeader"/> structure where the <see cref="DataFileHeader.VariableHeaderOffset"/> field is located.
        /// </summary>
        public const int VariableHeaderOffset = 28;

        /// <summary>
        /// The offset into a <see cref="DataFileHeader"/> structure where the <see cref="DataFileHeader.DataBufferCount"/> field is located.
        /// </summary>
        public const int DataBufferCount = 32;

        /// <summary>
        /// The offset into a <see cref="DataFileHeader"/> structure where the <see cref="DataFileHeader.DataBufferElementLength"/> field is located.
        /// </summary>
        public const int DataBufferElementLength = 36;

        /// <summary>
        /// The offset into a <see cref="DataFileHeader"/> structure where the padding field is located.
        /// </summary>
        public const int Padding = 40;

        /// <summary>
        /// The offset into a <see cref="DataFileHeader"/> structure where the <see cref="DataBufferHeader"/> array field is located.
        /// </summary>
        public const int DataBufferHeaderArray = 48;

        /// <summary>
        /// The offset into a <see cref="DataFileHeader"/> structure where the <see cref="DataFileHeader.DiskSubHeader"/> field is located.
        /// </summary>
        public const int DiskSubHeader = 112;
    }
}