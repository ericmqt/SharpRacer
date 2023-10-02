using System.Runtime.InteropServices;

namespace SharpRacer.IO;

/// <summary>
/// Describes the values of a simulator data file header.
/// </summary>
[StructLayout(LayoutKind.Explicit, Size = DataFileConstants.HeaderLength)]
public struct DataFileHeader
{
    /// <summary>
    /// Header version. See IRSDK_VER.
    /// </summary>
    [FieldOffset(DataFileHeaderOffsets.HeaderVersion)]
    public int HeaderVersion;

    /// <summary>
    /// Indicates if the simulator is active. See irsdk_StatusField.
    /// </summary>
    [FieldOffset(DataFileHeaderOffsets.Status)]
    public int Status;

    /// <summary>
    /// Ticks per second.
    /// </summary>
    [FieldOffset(DataFileHeaderOffsets.TickRate)]
    public int TickRate;

    /// <summary>
    /// Incremented when session info changes.
    /// </summary>
    [FieldOffset(DataFileHeaderOffsets.SessionInfoVersion)]
    public int SessionInfoVersion;

    /// <summary>
    /// The length of the session info string.
    /// </summary>
    [FieldOffset(DataFileHeaderOffsets.SessionInfoLength)]
    public int SessionInfoLength;

    /// <summary>
    /// The offset from the beginning of the data file where the session info string is located.
    /// </summary>
    [FieldOffset(DataFileHeaderOffsets.SessionInfoOffset)]
    public int SessionInfoOffset;

    /// <summary>
    /// The length of the array of <see cref="DataVariableHeader"/> entries pointed to by <see cref="VariableHeaderOffset"/>.
    /// </summary>
    [FieldOffset(DataFileHeaderOffsets.VariableCount)]
    public int VariableCount;

    /// <summary>
    /// The offset from the beginning of the data file where the array of <see cref="DataVariableHeader"/> entries is located.
    /// </summary>
    [FieldOffset(DataFileHeaderOffsets.VariableHeaderOffset)]
    public int VariableHeaderOffset;

    /// <summary>
    /// The number of data buffers.
    /// </summary>
    [FieldOffset(DataFileHeaderOffsets.BufferCount)]
    public int BufferCount;

    /// <summary>
    /// The length, in bytes, of a single data buffer.
    /// </summary>
    [FieldOffset(DataFileHeaderOffsets.BufferLength)]
    public int BufferLength;

    /// <summary>
    /// Padding.
    /// </summary>
    [FieldOffset(DataFileHeaderOffsets.Padding)]
    public ulong Padding;
}