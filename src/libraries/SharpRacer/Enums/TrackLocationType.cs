namespace SharpRacer;

/// <summary>
/// Defines track location types.
/// </summary>
/// <remarks>
/// See irsdk_TrkLoc in the iRacing SDK.
/// </remarks>
public enum TrackLocationType : int
{
    /// <summary>
    /// Not in the world.
    /// </summary>
    NotInWorld = -1,

    /// <summary>
    /// Off-track.
    /// </summary>
    OffTrack = 0,

    /// <summary>
    /// Pit stall.
    /// </summary>
    PitStall = 1,

    /// <summary>
    /// Pit approach.
    /// </summary>
    PitApproach = 2,

    /// <summary>
    /// On-track.
    /// </summary>
    OnTrack = 3
}
