namespace SharpRacer.Commands;

/// <summary>
/// Defines seek modes for replay commands.
/// </summary>
/// <remarks>
/// See irsdk_RpyPosMode in the iRacing SDK.
/// </remarks>
public enum ReplaySeekMode : ushort
{
    /// <summary>
    /// Seek from the beginning of the replay.
    /// </summary>
    Begin = 0,

    /// <summary>
    /// Seek from the current position.
    /// </summary>
    Current = 1,

    /// <summary>
    /// Seek from the end of the replay.
    /// </summary>
    End = 2
}
