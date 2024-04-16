namespace SharpRacer;

/// <summary>
/// Describes racing flags under pacing.
/// </summary>
/// <remarks>See irsdk_PaceFlags</remarks>
public enum PaceRacingFlags : uint
{
    /// <summary>
    /// No flag.
    /// </summary>
    None = 0x00,

    EndOfLine = 0x01,
    FreePass = 0x02,
    WavedAround = 0x04
}
