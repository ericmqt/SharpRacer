using System.Runtime.InteropServices;

namespace SharpRacer.Interop;

/// <summary>
/// Describes the values of a simulator data file header.
/// </summary>
/// <remarks>
/// See: irsdk_header
/// </remarks>
[StructLayout(LayoutKind.Explicit, Size = Size)]
public readonly struct DataFileHeader : IEquatable<DataFileHeader>
{
    /// <summary>
    /// The length, in bytes, of an instance of <see cref="DataFileHeader"/>.
    /// </summary>
    public const int Size = 144;

    /// <summary>
    /// Initializes an instance of <see cref="DataFileHeader"/> with default values.
    /// </summary>
    public DataFileHeader()
    {

    }

    /// <summary>
    /// Initializes an instance of <see cref="DataFileHeader"/> with the specified values.
    /// </summary>
    /// <param name="headerVersion">The value to assign to the <see cref="HeaderVersion"/> field.</param>
    /// <param name="status">The value to assign to the <see cref="Status"/> field.</param>
    /// <param name="tickRate">The value to assign to the <see cref="TickRate"/> field.</param>
    /// <param name="sessionInfoVersion">The value to assign to the <see cref="SessionInfoVersion"/> field.</param>
    /// <param name="sessionInfoLength">The value to assign to the <see cref="SessionInfoLength"/> field.</param>
    /// <param name="sessionInfoOffset">The value to assign to the <see cref="SessionInfoOffset"/> field.</param>
    /// <param name="variableCount">The value to assign to the <see cref="VariableCount"/> field.</param>
    /// <param name="variableHeaderOffset">The value to assign to the <see cref="VariableHeaderOffset"/> field.</param>
    /// <param name="dataBufferCount">The value to assign to the <see cref="DataBufferCount"/> field.</param>
    /// <param name="dataBufferElementLength">The value to assign to the <see cref="DataBufferElementLength"/> field.</param>
    /// <param name="dataBufferHeaders">The value to assign to the <see cref="DataBufferHeaders"/> field.</param>
    /// <param name="diskSubHeader">The value to assign to the <see cref="DiskSubHeader"/> field.</param>
    public DataFileHeader(
        int headerVersion,
        int status,
        int tickRate,
        int sessionInfoVersion,
        int sessionInfoLength,
        int sessionInfoOffset,
        int variableCount,
        int variableHeaderOffset,
        int dataBufferCount,
        int dataBufferElementLength,
        DataBufferHeaderArray dataBufferHeaders,
        DiskSubHeader diskSubHeader)
    {
        HeaderVersion = headerVersion;
        Status = status;
        TickRate = tickRate;

        SessionInfoVersion = sessionInfoVersion;
        SessionInfoLength = sessionInfoLength;
        SessionInfoOffset = sessionInfoOffset;

        VariableCount = variableCount;
        VariableHeaderOffset = variableHeaderOffset;

        DataBufferCount = dataBufferCount;
        DataBufferElementLength = dataBufferElementLength;
        DataBufferHeaders = dataBufferHeaders;

        DiskSubHeader = diskSubHeader;
    }

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
    /// Reinterprets a read-only span of bytes as a read-only reference to a <see cref="DataFileHeader"/> structure.
    /// </summary>
    /// <param name="span">The read-only span of bytes to reinterpret.</param>
    /// <returns>The read-only reference to the <see cref="DataFileHeader"/> structure.</returns>
    /// <remarks>
    /// The length of the span must be at least <see cref="DataFileHeader.Size"/> in length.
    /// </remarks>
    /// <example>
    /// This method must be called with the <c>ref</c> modifier in order to receive a reference to the structure and not a by-value copy.
    /// <code>
    ///     ref readonly var dataFileHeader = ref DataFileHeader.AsRef(dataSpan);
    /// </code>
    /// </example>
    public static ref readonly DataFileHeader AsRef(ReadOnlySpan<byte> span)
    {
        return ref MemoryMarshal.AsRef<DataFileHeader>(span[..Size]);
    }

    /// <summary>
    /// Reads a <see cref="DataFileHeader"/> structure from the specified read-only span of bytes.
    /// </summary>
    /// <param name="span">
    /// A read-only span of bytes at least <see cref="DataFileHeader.Size"/> in length that contains a <see cref="DataFileHeader"/> at
    /// offset zero.
    /// </param>
    /// <returns>The <see cref="DataFileHeader"/> structure read from the read-only span of bytes.</returns>
    public static DataFileHeader Read(ReadOnlySpan<byte> span)
    {
        return MemoryMarshal.Read<DataFileHeader>(span);
    }

    /// <inheritdoc />
    public override bool Equals(object? obj)
    {
        return obj is DataFileHeader header && Equals(header);
    }

    /// <inheritdoc />
    public bool Equals(DataFileHeader other)
    {
        return HeaderVersion == other.HeaderVersion &&
               Status == other.Status &&
               TickRate == other.TickRate &&
               SessionInfoVersion == other.SessionInfoVersion &&
               SessionInfoLength == other.SessionInfoLength &&
               SessionInfoOffset == other.SessionInfoOffset &&
               VariableCount == other.VariableCount &&
               VariableHeaderOffset == other.VariableHeaderOffset &&
               DataBufferCount == other.DataBufferCount &&
               DataBufferElementLength == other.DataBufferElementLength &&
               DataBufferHeaders.Equals(other.DataBufferHeaders) &&
               DiskSubHeader.Equals(other.DiskSubHeader);
    }

    /// <inheritdoc />
    public override int GetHashCode()
    {
        var hash = new HashCode();

        hash.Add(HeaderVersion);
        hash.Add(Status);
        hash.Add(TickRate);
        hash.Add(SessionInfoVersion);
        hash.Add(SessionInfoLength);
        hash.Add(SessionInfoOffset);
        hash.Add(VariableCount);
        hash.Add(VariableHeaderOffset);
        hash.Add(DataBufferCount);
        hash.Add(DataBufferElementLength);
        hash.Add(DataBufferHeaders);
        hash.Add(DiskSubHeader);

        return hash.ToHashCode();
    }

    /// <inheritdoc />
    public static bool operator ==(DataFileHeader left, DataFileHeader right)
    {
        return left.Equals(right);
    }

    /// <inheritdoc />
    public static bool operator !=(DataFileHeader left, DataFileHeader right)
    {
        return !(left == right);
    }

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
