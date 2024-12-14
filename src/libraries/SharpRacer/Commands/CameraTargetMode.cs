namespace SharpRacer.Commands;

/// <summary>
/// Defines camera targeting modes.
/// </summary>
/// <remarks>
/// See irsdk_csMode in the iRacing SDK.
/// </remarks>
public enum CameraTargetMode : short
{
    /// <summary>
    /// Incident.
    /// </summary>
    Incident = -3,

    /// <summary>
    /// Leader.
    /// </summary>
    Leader = -2,

    /// <summary>
    /// Exiting.
    /// </summary>
    Exiting = -1,

    /// <summary>
    /// Driver.
    /// </summary>
    Driver = 0
}
