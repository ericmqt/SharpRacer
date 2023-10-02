namespace SharpRacer.IO;

/// <summary>
/// Describes offsets into a <see cref="DiskSubHeader"/> structure.
/// </summary>
public static class DiskSubHeaderOffsets
{
    /// <summary>
    /// The offset into a <see cref="DiskSubHeader"/> structure where the <see cref="DiskSubHeader.SessionStartDate"/> field is located.
    /// </summary>
    public const int SessionStartDate = 0;

    /// <summary>
    /// The offset into a <see cref="DiskSubHeader"/> structure where the <see cref="DiskSubHeader.SessionStartTime"/> field is located.
    /// </summary>
    public const int SessionStartTime = 8;

    /// <summary>
    /// The offset into a <see cref="DiskSubHeader"/> structure where the <see cref="DiskSubHeader.SessionEndTime"/> field is located.
    /// </summary>
    public const int SessionEndTime = 16;

    /// <summary>
    /// The offset into a <see cref="DiskSubHeader"/> structure where the <see cref="DiskSubHeader.SessionLapCount"/> field is located.
    /// </summary>
    public const int SessionLapCount = 24;

    /// <summary>
    /// The offset into a <see cref="DiskSubHeader"/> structure where the <see cref="DiskSubHeader.SessionRecordCount"/> field is located.
    /// </summary>
    public const int SessionRecordCount = 28;
}
