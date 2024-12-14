namespace SharpRacer.Commands;

/// <summary>
/// Defines replay event search modes.
/// </summary>
/// <remarks>
/// See irsdk_RpySrchMode in the iRacing SDK.
/// </remarks>
public enum ReplaySearchMode : ushort
{
    /// <summary>
    /// Seek to the start.
    /// </summary>
    Start = 0,

    /// <summary>
    /// Seek to the end.
    /// </summary>
    End = 1,

    /// <summary>
    /// Seek to the previous session.
    /// </summary>
    PreviousSession = 2,

    /// <summary>
    /// Seek to the next session.
    /// </summary>
    NextSession = 3,

    /// <summary>
    /// Seek to the previous lap.
    /// </summary>
    PreviousLap = 4,

    /// <summary>
    /// Seek to the next lap.
    /// </summary>
    NextLap = 5,

    /// <summary>
    /// Seek to the previous frame.
    /// </summary>
    PreviousFrame = 6,

    /// <summary>
    /// Seek to the next frame.
    /// </summary>
    NextFrame = 7,

    /// <summary>
    /// Seek to the previous incident.
    /// </summary>
    PreviousIncident = 8,

    /// <summary>
    /// Seek to the next incident.
    /// </summary>
    NextIncident = 9
}
