namespace SharpRacer.IO;

/// <summary>
/// Describes offsets into a <see cref="DataFileHeader"/> structure.
/// </summary>
public static class DataFileHeaderOffsets
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
    /// The offset into a <see cref="DataFileHeader"/> structure where the <see cref="DataFileHeader.BufferCount"/> field is located.
    /// </summary>
    public const int BufferCount = 32;

    /// <summary>
    /// The offset into a <see cref="DataFileHeader"/> structure where the <see cref="DataFileHeader.BufferLength"/> field is located.
    /// </summary>
    public const int BufferLength = 36;

    /// <summary>
    /// The offset into a <see cref="DataFileHeader"/> structure where the <see cref="DataFileHeader.Padding"/> field is located.
    /// </summary>
    public const int Padding = 40;
}
