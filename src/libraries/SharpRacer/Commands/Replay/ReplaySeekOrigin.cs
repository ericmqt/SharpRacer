namespace SharpRacer.Commands.Replay;

/// <summary>
/// Specifies the reference position in a replay to use when seeking.
/// </summary>
/// <remarks>
/// See irsdk_RpyPosMode in the iRacing SDK.
/// </remarks>
public enum ReplaySeekOrigin : ushort
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
