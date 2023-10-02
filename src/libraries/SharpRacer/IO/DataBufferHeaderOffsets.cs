namespace SharpRacer.IO;

/// <summary>
/// Describes offsets into a <see cref="DataBufferHeader"/> structure.
/// </summary>
public static class DataBufferHeaderOffsets
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
    /// The offset into a <see cref="DataBufferHeader"/> structure where the <see cref="DataBufferHeader.Padding"/> field is located.
    /// </summary>
    public const int Padding = 8;
}
