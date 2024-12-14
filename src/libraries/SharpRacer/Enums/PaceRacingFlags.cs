namespace SharpRacer;

/// <summary>
/// Defines racing flags under pacing.
/// </summary>
/// <remarks>See irsdk_PaceFlags in the iRacing SDK.</remarks>
public enum PaceRacingFlags : uint
{
    /// <summary>
    /// No flag.
    /// </summary>
    /// <remarks>
    /// This value is not defined in the irsdk_PaceFlags enumeration in the iRacing SDK.
    /// </remarks>
    None = 0x00,

    /// <summary>
    /// End-of-line flag.
    /// </summary>
    EndOfLine = 0x01,

    /// <summary>
    /// Free pass.
    /// </summary>
    FreePass = 0x02,

    /// <summary>
    /// Wave-around.
    /// </summary>
    WavedAround = 0x04
}
